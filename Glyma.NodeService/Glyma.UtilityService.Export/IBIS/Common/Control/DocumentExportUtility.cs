using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;
using Telerik.Windows.Documents.FormatProviders.Html;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx.Import;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.Lists;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Model.Styles;
using Telerik.Windows.Documents.UI.TextDecorations.DecorationProviders;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Common.Control
{
    public abstract class DocumentExportUtility: ExportUtility
    {
        #region private
        private List<GlymaMap> _maps;

        private Section _section;

        private RadDocument _document;

        private Assembly _assembly;

        private object _descriptionIconStreamLock = new object();

        private Stream _descriptionIconStream;

        private object _videoIconStreamLock = new object();
        private Stream _videoIconStream;

        private List<Block> _currentBlockCollection;

        private Span[] _bullets;

        #endregion

        protected List<Block> CurrentBlockCollection
        {
            get
            {
                if (_currentBlockCollection == null)
                {
                    _currentBlockCollection = new List<Block>();
                }
                return _currentBlockCollection;
            }
        }

        protected bool ShowImage { get; set; }

        protected bool ShowDescription { get; set; }

        protected bool ShowVideo { get; set; }

        protected bool ShowDescriptionIcon { get; set; }


        protected bool ShowPages { get; set; }

        protected Assembly Assembly
        {
            get
            {
                if (_assembly == null)
                {
                    _assembly = Assembly.GetExecutingAssembly();
                }
                return _assembly;
            }
        }

        protected Stream DescriptionIconStream
        {
            get
            {
                if (_descriptionIconStream == null)
                {
                    lock (_descriptionIconStreamLock)
                    {
                        if (_descriptionIconStream == null)
                        {
                            _descriptionIconStream = Assembly.GetManifestResourceStream("Glyma.UtilityService.Export.Images.RelatedContent.png");
                        }
                    }
                }
                return _descriptionIconStream;
            }
        }

        protected Stream VideoIconStream
        {
            get
            {
                if (_videoIconStream == null)
                {
                    lock (_videoIconStreamLock)
                    {
                        if (_videoIconStream == null)
                        {
                            _videoIconStream = Assembly.GetManifestResourceStream("Glyma.UtilityService.Export.Images.Video.png");
                        }
                    }
                }
                return _videoIconStream;
            }
        }

        public List<GlymaMap> Maps
        {
            get
            {
                if (_maps == null)
                {
                    _maps = new List<GlymaMap>();
                }
                return _maps;
            }
        }

        protected Section Section
        {
            get
            {
                if (_section == null)
                {
                    _section = new Section();
                }
                return _section;
            }
        }

        protected RadDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };
                }
                return _document;
            }
        }

        private Span[] Bullets
        {
            get
            {
                if (_bullets == null)
                {
                    _bullets = BuildBulletSpans();
                }
                return _bullets;
            }
        }

        protected DocumentExportUtility(IMapManager mapmanager)
            : base(mapmanager)
        {
            
        }

        /// <summary>
        /// These character codes were retrieved by copying and pasting the original bullets in a generated PDF to Word and determining the Font details
        /// </summary>
        /// <returns></returns>
        private Span[] BuildBulletSpans()
        {
            Span[] bullets = new Span[3];

            Span blackCircleBullet = new Span("• ");  //Character Code: U+2022
            blackCircleBullet.FontFamily = new FontFamily("Courier New");
            blackCircleBullet.FontSize = 12.0;
            blackCircleBullet.FontStyle = FontStyles.Normal;
            bullets[0] = blackCircleBullet;

            Span whiteCircleBullet = new Span("o ");  //Character Code: (Latin Small Letter O): U+006F
            whiteCircleBullet.FontFamily = new FontFamily("Courier New");
            whiteCircleBullet.FontSize = 12.0;
            whiteCircleBullet.FontStyle = FontStyles.Normal;
            bullets[1] = whiteCircleBullet;

            Span blackSquareBullet = new Span(" "); //U+F0A7
            blackSquareBullet.FontFamily = new FontFamily("Wingdings");
            blackSquareBullet.FontSize = 12.0;
            blackSquareBullet.FontStyle = FontStyles.Normal;
            bullets[2] = blackSquareBullet;

            return bullets;
        }

        private Span GetBulletForDepth(int depth)
        {
            Span selectedBullet = Bullets[depth % 3];
            Span copyOfBullet = selectedBullet.CreateShallowCopy() as Span;
            return copyOfBullet;
        }

        protected override void OnContainerMapLoaded(INode node)
        {
            base.OnContainerMapLoaded(node);

            if (GetExportProperty("ShowDescription").ToLower() == "true")
            {
                ShowDescription = true;
            }

            if (GetExportProperty("ShowImage").ToLower() == "true")
            {
                ShowImage = true;
            }

            if (GetExportProperty("ShowVideo").ToLower() == "true")
            {
                ShowVideo = true;
            }

            if (GetExportProperty("ShowPages").ToLower() == "true")
            {
                ShowPages = true;
            }

            
            var mapQuerier = new GlymaMapQuerier(MapManager, node, SelectedNodes,RootMap);
            mapQuerier.QueryCompleted += MapQuerierOnQueryCompleted;
            mapQuerier.MapAdded += MapQuerierOnMapAdded;
            mapQuerier.Process();
        }

        private void AddPageFooter()
        {
            RadDocument document = new RadDocument();
            Section sectionf = new Section();
            Paragraph paragraphPageField = new Paragraph() { TextAlignment = RadTextAlignment.Right };
            PageField pageField = new PageField() { DisplayMode = FieldDisplayMode.Result };
            FieldRangeStart pageFieldStart = new FieldRangeStart();
            pageFieldStart.Field = pageField;
            FieldRangeEnd pageFieldEnd = new FieldRangeEnd();
            pageFieldEnd.Start = pageFieldStart;

            paragraphPageField.Inlines.Add(pageFieldStart);
            paragraphPageField.Inlines.Add(pageFieldEnd);

            FieldRangeStart numPagesFieldStart = new FieldRangeStart();
            numPagesFieldStart.Field = new NumPagesField() { DisplayMode = FieldDisplayMode.Result };
            FieldRangeEnd numPagesFieldEnd = new FieldRangeEnd();
            numPagesFieldEnd.Start = numPagesFieldStart;

            paragraphPageField.Inlines.Add(new Span("/"));
            paragraphPageField.Inlines.Add(numPagesFieldStart);
            paragraphPageField.Inlines.Add(numPagesFieldEnd);

            sectionf.Blocks.Add(paragraphPageField);
            document.Sections.Add(sectionf);

            Document.Sections.First.Footers.Default.Body = document;

            //Document.Sections.Last.Blocks.AddAfter(Document.Sections.Last.Blocks.Last, paragraphPageField);
            //Document.Sections.Last.Footers.Default.Body.InsertFragment(new DocumentFragment(document));
        }

        private void CreateDocument()
        {
            Document.LineSpacing = 1;

            var titleparagraph = new Paragraph();
            titleparagraph.StyleName = string.Format("Heading{0}", 1);

            if (ShowImage)
            {
                CreateNodeIcon(RootMap, ref titleparagraph);
            }

            var titlespan = new Span(RootMap.Name);// {FontSize = 22, ForeColor = Colors.DarkSlateBlue};
            titleparagraph.Inlines.Add(titlespan);
            Section.Blocks.Add(titleparagraph);

            var documentList = new DocumentList(DefaultListStyles.Bulleted, Document);
            documentList.Style.Levels[8].LevelText = "";

            foreach (var node in RootMap.ChildNodes.OrderBy(q => q.YPosition).ThenBy(q => q.XPosition))
            {
                AddNodeToDocument(node, documentList.ID);
            }

            foreach (var map in Maps)
            {
                if (map.Id != RootMap.Id)
                {
                    var subMapDocumentList = new DocumentList(DefaultListStyles.Bulleted, Document);
                    subMapDocumentList.Style.Levels[8].LevelText = "";

                    AddSubTitle(map, map.Id.ToLongString());
                    foreach (var node in map.ChildNodes.OrderBy(q => q.YPosition).ThenBy(q => q.XPosition))
                    {
                        AddNodeToDocument(node, subMapDocumentList.ID);
                    }
                }
            }

            Document.Sections.Add(Section);
            if (ShowPages)
            {
                AddPageFooter();
            }
        }

        private int GetDepth()
        {
            int depth = 0;
            foreach (var map in Maps)
            {
                if (map.Depth > depth)
                {
                    depth = map.Depth;
                }
                if (map.ChildNodes.Count > 0)
                {
                    var sumDepth = map.GetAllChildNodes().Max(q => q.Depth);
                    if (depth < sumDepth)
                    {
                        depth = sumDepth;
                    }
                }
            }
            return depth;
        }

        protected abstract void WriteToFile();

        protected override bool CreateFile()
        {
            bool fileCreated = false;
            var thread = new Thread(() =>
            {
                var isDocumentCreated = false;
                try
                {
                    CreateDocument();
                    isDocumentCreated = true;
                }
                catch (Exception ex)
                {
                    OnExceptionRaised(this, "Error occurred when creating export document", ex);
                }

                if (isDocumentCreated)
                {
                    try
                    {
                        WriteToFile();
                        fileCreated = true;
                    }
                    catch (Exception ex)
                    {
                        OnExceptionRaised(this, "Error occurred when writing document to file", ex);
                    }
                }
                
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return fileCreated;
        }

        private void CreateNodeIcon(IGlymaNode node, ref Paragraph paragraph)
        {
            var image = NodeIconManager.GetImage(node.NodeType.GetImageName());

            if (image != null)
            {
                paragraph.Inlines.Add(image);
                paragraph.Inlines.Add(new Span("  "));
            }
        }

        private void CreateNodeTitle(IGlymaNode node, ref Paragraph paragraph)
        {
            if (node.NodeType.Equals(MapManager.NodeTypes["CompendiumMapNode"]))
            {
                var text = new Span(string.IsNullOrEmpty(node.Name) ? "(No Name)" : node.Name)
                {
                    //ForeColor = Colors.DodgerBlue,
                    UnderlineDecoration = UnderlineTypes.Line
                };

                var hyperlinkStart = new HyperlinkRangeStart();
                var hyperlinkEnd = new HyperlinkRangeEnd();
                hyperlinkEnd.PairWithStart(hyperlinkStart);
                var hyperlinkInfo = new HyperlinkInfo
                {
                    NavigateUri = node.Id.ToLongString(),
                    Target = HyperlinkTargets.Self,
                    IsAnchor = true
                };
                hyperlinkStart.HyperlinkInfo = hyperlinkInfo;

                paragraph.Inlines.Add(hyperlinkStart);
                paragraph.Inlines.Add(text);
                paragraph.Inlines.Add(hyperlinkEnd);
            }
            else
            {
                var text = new Span(node.Name);
                paragraph.Inlines.Add(text);
            }
        }

        private void CreateDescription(IGlymaNode node, ref Paragraph paragraph)
        {
            //paragraph.Inlines.Add(new Span(FormattingSymbolLayoutBox.LINE_BREAK));

            if (node.NodeDescription.Type == "Iframe")
            {
                if (ShowDescriptionIcon)
                {
                    var descriptionIcon = new ImageInline(DescriptionIconStream, new Size(20, 15), "png");
                    paragraph.Inlines.Add(descriptionIcon);
                    paragraph.Inlines.Add(new Span("  "));
                }
                else
                {
                    paragraph.Inlines.Add(new Span("Link: ") { FontSize = 12 });
                }

                CreateLink(node.NodeDescription.Link, ref paragraph);
            }
            else
            {
                if (ShowDescriptionIcon)
                {
                    var descriptionIcon = new ImageInline(DescriptionIconStream, new Size(20, 15), "png");
                    paragraph.Inlines.Add(descriptionIcon);
                    paragraph.Inlines.Add(new Span("  "));
                }
                else
                {
                    paragraph.Inlines.Add(new Span("Description: ") { FontSize = 12 });
                }

                var htmlFormatProvider = new HtmlFormatProvider();
                var description = htmlFormatProvider.Import(node.NodeDescription.Description);
                foreach (var item in description.Sections)
                {
                    var section = item.CreateDeepCopy() as Section;
                    if (section != null)
                    {
                        foreach (var block in section.Blocks)
                        {
                            var para = block as Paragraph;
                            if (para != null)
                            {
                                foreach (Span span in para.EnumerateChildrenOfType<Span>())
                                {
                                    span.FontSize = 12;
                                }

                                foreach (var inline in para.Inlines)
                                {
                                    var theNewInLineItem = inline.CreateDeepCopy() as Inline;
                                    paragraph.Inlines.Add(theNewInLineItem);
                                }
                            }
                            else
                            {
                                CurrentBlockCollection.Add(block.CreateDeepCopy() as Block);
                            }
                        }
                    }
                }
            }
        }

        private void CreateLink(string link, ref Paragraph paragraph)
        {
            var hyperlinkStart = new HyperlinkRangeStart();
            var hyperlinkEnd = new HyperlinkRangeEnd();
            hyperlinkEnd.PairWithStart(hyperlinkStart);
            link = link.Replace(" ", "20%");
            Uri uri;
            if (Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                var hyperlinkInfo = new HyperlinkInfo
                {
                    NavigateUri = link,
                    Target = HyperlinkTargets.Blank
                };
                hyperlinkStart.HyperlinkInfo = hyperlinkInfo;

                paragraph.Inlines.Add(hyperlinkStart);
                var spanLink = new Span(link)
                {
                    FontSize = 12,
                    ForeColor = Colors.DodgerBlue,
                    UnderlineDecoration = UnderlineTypes.Line
                };
                paragraph.Inlines.Add(spanLink);
                paragraph.Inlines.Add(hyperlinkEnd);
            }
            else
            {
                var spanLink = new Span(link)
                {
                    FontSize = 12,
                    ForeColor = Colors.DodgerBlue,
                    UnderlineDecoration = UnderlineTypes.Line
                };
                paragraph.Inlines.Add(spanLink);
            }
        }

        private void CreateVideo(IGlymaNode node, ref Paragraph paragraph)
        {
            //paragraph.Inlines.Add(new Span(FormattingSymbolLayoutBox.LINE_BREAK));

            if (ShowDescriptionIcon)
            {
                var videoIcon = new ImageInline(VideoIconStream, new Size(20, 15), "png");

                paragraph.Inlines.Add(videoIcon);
                paragraph.Inlines.Add(new Span("  "));
            }
            else
            {
                paragraph.Inlines.Add(new Span("Video:") { FontSize = 12 });
            }

            CreateLink(node.NodeVideo.Link, ref paragraph);

            if (node.NodeVideo.StartPosition.HasValue || node.NodeVideo.EndPosition.HasValue)
            {
                paragraph.Inlines.Add(new Span(FormattingSymbolLayoutBox.LINE_BREAK));
                paragraph.Inlines.Add(new Span("Start Time:") { FontSize = 12 });
                if (node.NodeVideo.StartPosition.HasValue)
                {
                    paragraph.Inlines.Add(new Span(string.Format("{0:00}:{1:00}:{2:00}", node.NodeVideo.StartPosition.Value.TotalHours, node.NodeVideo.StartPosition.Value.Minutes, node.NodeVideo.StartPosition.Value.Seconds)) { FontSize = 12 });
                    paragraph.Inlines.Add(new Span("  ") { FontSize = 12 });
                }
                else
                {
                    paragraph.Inlines.Add(new Span("00:00:00") { FontSize = 12 });
                    paragraph.Inlines.Add(new Span("  ") { FontSize = 12 });
                }

                if (node.NodeVideo.EndPosition.HasValue)
                {
                    paragraph.Inlines.Add(new Span("End Time:") { FontSize = 12 });
                    paragraph.Inlines.Add(new Span(string.Format("{0:00}:{1:00}:{2:00}", node.NodeVideo.EndPosition.Value.TotalHours, node.NodeVideo.EndPosition.Value.Minutes, node.NodeVideo.EndPosition.Value.Seconds)) { FontSize = 12 });
                }
            }
        }

        private void AddNodeToDocument(IGlymaNode node, int documentListId)
        {
            var depth = node.Depth > 8 ? 8 : node.Depth;

            var paragraph = new Paragraph {ListId = documentListId, ListLevel = depth};

            int headingLevel = ((depth + 2) > 8) ? 9 : (depth + 2); //maximum of Heading level of 9
            paragraph.StyleName = string.Format("Heading{0}", headingLevel);

            if (node.Depth == 8)
            {
                paragraph.Inlines.Add(GetBulletForDepth(8));
            }

            //check if list level is more than 8, it needs to set to 8 because telerik only support maximum of 9 levels.
            if (node.Depth > 8)
            {
                var prefix = string.Empty;
                for (int i = 8; i < node.Depth; i++)
                {
                    prefix = "    " + prefix;
                }
                Span bulletSpan = GetBulletForDepth(node.Depth);
                paragraph.Inlines.Add(new Span(prefix));
                paragraph.Inlines.Add(bulletSpan);
            }

            if (ShowImage)
            {
                CreateNodeIcon(node, ref paragraph);
            }
            CreateNodeTitle(node, ref paragraph);

            Section.Blocks.Add(paragraph);

            if (ShowDescription || ShowVideo)
            {
                if (node.NodeVideo.HasVideo && ShowVideo)
                {
                    var videoParagraph = new Paragraph() { ListId = documentListId, ListLevel = depth  };
                    CreateVideo(node, ref videoParagraph);
                    Section.Blocks.Add(videoParagraph);
                }

                if (node.NodeDescription.HasDescription && ShowDescription)
                {
                    var descriptionParagraph = new Paragraph() { ListId = documentListId, ListLevel = depth };
                    CreateDescription(node, ref descriptionParagraph);
                    Section.Blocks.Add(descriptionParagraph);
                }
            }
            
            if (CurrentBlockCollection.Count > 0)
            {
                foreach (var block in CurrentBlockCollection)
                {
                    foreach (Span span in block.EnumerateChildrenOfType<Span>())
                    {
                        span.FontSize = 12;
                    }
                    Section.Blocks.Add(block);
                }
                _currentBlockCollection = null;
                Section.Blocks.Add(new Paragraph());
            }

            foreach (var child in node.ChildNodes.OrderBy(q => q.YPosition).ThenBy(q => q.XPosition))
            {
                AddNodeToDocument(child, documentListId);
            }
        }

        private void AddSubTitle(IGlymaNode map, string bookMarkId)
        {
            var paragraph = new Paragraph();
            paragraph.StyleName = string.Format("Heading{0}", 1);
            if (ShowImage)
            {
                CreateNodeIcon(map, ref paragraph);
            }
            var text = new Span(string.IsNullOrEmpty(map.Name) ? "(No Name)" : map.Name);
            //{
            //    FontSize = 18,
            //    ForeColor = Colors.DarkSlateBlue
            //};

            var bookMarkStart = new BookmarkRangeStart(bookMarkId);
            var bookMarkEnd = new BookmarkRangeEnd();
            bookMarkEnd.PairWithStart(bookMarkStart);
            paragraph.Inlines.Add(bookMarkStart);
            paragraph.Inlines.Add(text);
            paragraph.Inlines.Add(bookMarkEnd);
            Section.Blocks.Add(paragraph);          
        }

        protected override void ReadNextMap(INode nextMap)
        {
            var map = new GlymaMap(nextMap);
            Maps.Add(map);
            var mapQuerier = new GlymaMapQuerier(MapManager, nextMap, null, map);
            mapQuerier.QueryCompleted += MapQuerierOnQueryCompleted;
            mapQuerier.MapAdded += MapQuerierOnMapAdded;
            mapQuerier.Process();
        }
    }
}
