using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

using System.Diagnostics;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.Controls
{
    [TemplatePart(Name = NodeControl.ImagePartName, Type = typeof(Rectangle))]
    [TemplatePart(Name = NodeControl.TextBlockPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = NodeControl.TextBoxPartName, Type = typeof(TextBox))]
    [TemplatePart(Name = NodeControl.OkButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = NodeControl.CancelButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = NodeControl.TranscludionsCountTextBlockPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = NodeControl.ChildCountTextBlockPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = NodeControl.NoteTextBlockPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = NodeControl.TextBlockBorderName, Type = typeof(Border))]
    [TemplatePart(Name = NodeControl.PlayMediaImagePartName, Type = typeof(Rectangle))]
    [TemplatePart(Name = NodeControl.PauseMediaImagePartName, Type = typeof(Rectangle))]
    [TemplateVisualState(Name = NodeControl.MouseOverStateName, GroupName = NodeControl.CommonStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.NormalStateName, GroupName = NodeControl.CommonStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.SelectedStateName, GroupName = NodeControl.FocusStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.NotSelectedStateName, GroupName = NodeControl.FocusStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.EdittingStateName, GroupName = NodeControl.EditStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.NotEdittingStateName, GroupName = NodeControl.EditStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.PlayingStateName, GroupName = NodeControl.MediaPlaybackStatesGroupName)]
    [TemplateVisualState(Name = NodeControl.IdleStateName, GroupName = NodeControl.MediaPlaybackStatesGroupName)]
    public class NodeControl : Control
    {
        //Template parts
        public const string ImagePartName = "NodeImage";
        public const string TextBlockPartName = "NodeTextBlock";
        public const string TextBoxPartName = "NodeTextBox";
        public const string OkButtonPartName = "OkButton";
        public const string CancelButtonPartName = "CancelButton";
        public const string TranscludionsCountTextBlockPartName = "TransclusionCountTextBlock";
        public const string ChildCountTextBlockPartName = "ChildCountTextBlock";
        public const string NoteTextBlockPartName = "NoteTextBlock";
        public const string TextBlockBorderName = "TextBlockBorder";
        public const string PlayMediaImagePartName = "PlayMediaImage";
        public const string PauseMediaImagePartName = "PauseMediaImage";

        //Visual states
        public const string CommonStatesGroupName = "CommonStates";
        public const string EditStatesGroupName = "EditStates";
        public const string FocusStatesGroupName = "FocusStates";
        public const string MediaPlaybackStatesGroupName = "MediaPlaybackStates";
        public const string MouseOverStateName = "MouseOver";
        public const string NormalStateName = "Normal";
        public const string EdittingStateName = "Editting";
        public const string NotEdittingStateName = "NotEditting";
        public const string DefaultStateName = "Default";
        public const string SelectedStateName = "Selected";
        public const string NotSelectedStateName = "NotSelected";
        public const string PlayingStateName = "Playing";
        public const string IdleStateName = "Idle";

        private TextBlock textBlockPart;
        private TextBox textBoxPart;
        private Rectangle imagePart;
        private Button okButtonPart;
        private Button cancelButtonPart;
        private TextBlock transCntTextBlockPart;
        private TextBlock childCntTextBlockPart;
        private TextBlock noteTextBlockPart;
        private Border textBlockBorderPart;

        private Rectangle playMediaImagePart;
        private Rectangle pauseMediaImagePart;

        private INodeProxy nodeProxy;

        private bool isTemplateApplied;
        private bool isSelected = false;

        public event RoutedEventHandler PlayMediaIconClicked;
        public event RoutedEventHandler PauseMediaIconClicked;
        public event RoutedEventHandler Selected;
        public event RoutedEventHandler Unselected;
        public event RoutedEventHandler NodeTextBlockClicked;

        private double textBlockHeight = double.NaN;
        private double textBlockWidth = double.NaN;

        //the static cache of brushes will save on image loading times
        private static Dictionary<string, ImageBrush> nodeImageCache = new Dictionary<string, ImageBrush>();

        public NodeControl()
        {
            this.isTemplateApplied = false;
            InEditState = false;
            IsPlayingMedia = false;

            this.DefaultStyleKey = typeof(NodeControl);

            this.MouseEnter += new MouseEventHandler(OnMouseEnter);
            this.MouseLeave += new MouseEventHandler(OnMouseLeave);
            this.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseRightButtonDown);
            this.MouseRightButtonUp += new MouseButtonEventHandler(OnMouseRightButtonUp);

            this.Loaded += new RoutedEventHandler(NodeControl_Loaded);
        }

        public double ActualTextHeight
        {
            get
            {
                if (this.textBlockPart != null)
                {
                    if (textBlockHeight != double.NaN)
                    {
                        return textBlockHeight;
                    }
                    else
                    {
                        return this.textBlockPart.ActualHeight;
                    }
                }
                else
                {
                    return this.TextHeight;
                }
            }
        }

        public double ActualTextWidth
        {
            get
            {
                if (this.textBlockPart != null)
                {
                    if (textBlockWidth != double.NaN)
                    {
                        return textBlockWidth;
                    }
                    else
                    {
                        return this.textBlockPart.ActualWidth;
                    }
                }
                else
                {
                    return this.TextWidth;
                }
            }
        }

        public new double ActualHeight
        {
            get
            {
                double result = ActualTextHeight;
                result += NodeSkinHeight;
                result += (TextMarginY - NodeSkinHeight);
                return result;
            }
        }

        public new double ActualWidth
        {
            get
            {
                if (ActualTextWidth > NodeSkinWidth)
                {
                    return ActualTextWidth;
                }
                return NodeSkinWidth;
            }
        }

        public bool IsPlayingMedia
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                if (value)
                {
                    GoToState(NodeControl.SelectedStateName, true);
                    if (Selected != null)
                    {
                        Selected.Invoke(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    GoToState(NodeControl.NotSelectedStateName, true);
                    if (Unselected != null)
                    {
                        Unselected.Invoke(this, new RoutedEventArgs());
                    }
                }
            }
        }

        private void NodeControl_Loaded(object sender, RoutedEventArgs e)
        {
           // if (childCntTextBlockPart != null)
           // {
            //    childCntTextBlockPart.GetBindingExpression(TextBlock.TextProperty).UpdateSource();
           // }

            //if (transCntTextBlockPart != null)
           // {
            //    transCntTextBlockPart.GetBindingExpression(TextBlock.TextProperty).UpdateSource();
           // }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.isTemplateApplied) return;

            this.nodeProxy = DataContext as INodeProxy;
            if (nodeProxy == null) 
            {
                throw new Exception("The DataContext was not a valid NodeProxy");
            }

            this.imagePart = GetTemplateChild(NodeControl.ImagePartName) as Rectangle;
            if (this.imagePart != null)
            {
                this.imagePart.SetValue(Canvas.LeftProperty, 0.0);
                this.imagePart.SetValue(Canvas.TopProperty, 0.0);
                ImageBrush nodeImageBrush = this.imagePart.Fill as ImageBrush;
                if (nodeImageBrush != null)
                {
                    nodeImageBrush.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(nodeImageBrush_ImageFailed);
                }
            }

            this.playMediaImagePart = GetTemplateChild(NodeControl.PlayMediaImagePartName) as Rectangle;
            if (this.playMediaImagePart != null)
            {
                this.playMediaImagePart.MouseLeftButtonDown += new MouseButtonEventHandler(playMediaImagePart_MouseLeftButtonUp);
                MetadataContext videoSourceKey = new MetadataContext()
                {
                    MetadataName = "Video.Source",
                    NodeUid = nodeProxy.Id
                };
                if (nodeProxy.HasMetadata(videoSourceKey) && !string.IsNullOrEmpty(nodeProxy.GetNodeMetadata(videoSourceKey).MetadataValue))
                {
                    this.playMediaImagePart.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.playMediaImagePart.Visibility = System.Windows.Visibility.Collapsed;
                }
                ImageBrush brush = playMediaImagePart.Fill as ImageBrush;
                if (brush != null)
                {
                    brush.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(brush_ImageFailed);
                }
            }
            this.pauseMediaImagePart = GetTemplateChild(NodeControl.PauseMediaImagePartName) as Rectangle;
            if (this.pauseMediaImagePart != null)
            {
                this.pauseMediaImagePart.MouseLeftButtonDown += new MouseButtonEventHandler(pauseMediaImagePart_MouseLeftButtonUp);
                this.pauseMediaImagePart.Visibility = System.Windows.Visibility.Collapsed;
                ImageBrush brush = pauseMediaImagePart.Fill as ImageBrush;
                if (brush != null)
                {
                    brush.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(brush_ImageFailed);
                }
            }


            this.textBlockPart = GetTemplateChild(NodeControl.TextBlockPartName) as TextBlock;
            this.textBlockBorderPart = GetTemplateChild(NodeControl.TextBlockBorderName) as Border;
            this.textBoxPart = GetTemplateChild(NodeControl.TextBoxPartName) as TextBox;
            this.transCntTextBlockPart = GetTemplateChild(NodeControl.TranscludionsCountTextBlockPartName) as TextBlock;
            this.childCntTextBlockPart = GetTemplateChild(NodeControl.ChildCountTextBlockPartName) as TextBlock;
            this.noteTextBlockPart = GetTemplateChild(NodeControl.NoteTextBlockPartName) as TextBlock;

            Rect textLocation = GetTextLocation();
            if (this.textBlockBorderPart != null)
            {
                this.textBlockBorderPart.SetValue(Canvas.LeftProperty, textLocation.X);
                this.textBlockBorderPart.SetValue(Canvas.TopProperty, textLocation.Y);
                this.textBlockBorderPart.Width = textLocation.Width;
                this.textBlockBorderPart.Height = textLocation.Height;
            }
            if (this.textBlockPart != null)
            {
                this.textBlockPart.Width = textLocation.Width;
                this.textBlockPart.Height = textLocation.Height;
                if (this.textBlockBorderPart != null)
                {
                    if (textLocation.Height < this.textBlockPart.ActualHeight)
                    {
                        this.textBlockBorderPart.Height = this.textBlockPart.ActualHeight;
                        this.textBlockPart.Height = this.textBlockPart.ActualHeight;
                    }
                    else
                    {
                        if (this.textBlockPart.ActualHeight > 10)
                        {
                            this.textBlockBorderPart.Height = this.textBlockPart.ActualHeight;
                            this.textBlockPart.Height = this.textBlockPart.ActualHeight;
                        }
                    }
                }
                this.textBlockHeight = this.textBlockPart.ActualHeight;
                this.textBlockWidth = this.textBlockPart.ActualWidth;
                this.textBlockPart.MouseLeftButtonDown += new MouseButtonEventHandler(OnNodeTextBlockMouseLeftButtonDown);
            }

            if (this.textBoxPart != null)
            {
                this.textBoxPart.SetValue(Canvas.LeftProperty, textLocation.X);
                this.textBoxPart.SetValue(Canvas.TopProperty, textLocation.Y);
                this.textBoxPart.Width = textLocation.Width + 5;
                this.textBoxPart.Height = textLocation.Height + 5;
                this.textBoxPart.KeyDown += new KeyEventHandler(TextBoxPart_KeyDown);
            }

            if (this.transCntTextBlockPart != null)
            {
                if (this.transCntTextBlockPart.Text == "1")
                {
                    this.transCntTextBlockPart.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.transCntTextBlockPart.Visibility = Visibility.Visible;
                }
            }

            if (this.childCntTextBlockPart != null)
            {
                if (nodeProxy.NodeType.Name == "CompendiumMapNode")
                {
                    this.childCntTextBlockPart.Visibility = Visibility.Visible;
                    this.childCntTextBlockPart.Text = GetMapNodeChildCount(nodeProxy);
                }
                else
                {
                    this.childCntTextBlockPart.Visibility = Visibility.Collapsed;
                }
            }

            if (this.noteTextBlockPart != null)
            {
                MetadataContext noteKey = new MetadataContext() 
                {
                    MetadataName = "Note",
                    NodeUid = nodeProxy.Id
                };
                if (nodeProxy.HasMetadata(noteKey) && !string.IsNullOrEmpty(nodeProxy.GetNodeMetadata(noteKey).MetadataValue))
                {
                    this.noteTextBlockPart.Visibility = Visibility.Visible;
                    ToolTipService.SetToolTip(this.noteTextBlockPart, nodeProxy.GetNodeMetadata(noteKey).MetadataValue);
                }
                else
                {
                    this.noteTextBlockPart.Visibility = Visibility.Collapsed;
                }
            }

            BitmapImage nodeImg = new BitmapImage();
            //for optimal memory usage and speed only create as many ImageBrush objects as there are images.
            if (ImageUrl != null)
            {
                if (!nodeImageCache.ContainsKey(ImageUrl))
                {
                    nodeImg.UriSource = new Uri(ImageUrl);

                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = nodeImg;

                    NodeImage = imageBrush;
                    nodeImageCache.Add(ImageUrl, imageBrush);
                }
                else
                {
                    NodeImage = nodeImageCache[ImageUrl];
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(NodeImageName))
                {
                    
                    switch (NodeImageName)
                    {
                        case "Map":
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/network.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                        case "Pro":
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/plus.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                        case "Con":
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/minus.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                        case "Decision":
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/generic.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                        case "Idea":
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/exclamation.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                        case "Question":
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/question.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                        default:
                            nodeImg.UriSource = new Uri("/SilverlightMappingToolBasic;component/Images/generic.png", UriKind.Relative);
                            NodeImage = new ImageBrush() { ImageSource = nodeImg };
                            break;
                    }
                }
            }

            this.okButtonPart = GetTemplateChild(NodeControl.OkButtonPartName) as Button;
            this.cancelButtonPart = GetTemplateChild(NodeControl.CancelButtonPartName) as Button;

            if (this.textBlockBorderPart != null 
                && this.okButtonPart != null && this.cancelButtonPart != null)
            {
                double left = (double)textBlockBorderPart.GetValue(Canvas.LeftProperty);
                double top = (double)textBlockBorderPart.GetValue(Canvas.TopProperty);

                this.okButtonPart.SetValue(Canvas.LeftProperty, left + this.textBoxPart.Width - 100);
                this.okButtonPart.SetValue(Canvas.TopProperty, top + this.textBoxPart.Height);
                this.okButtonPart.Click += new RoutedEventHandler(OnNodeTextBoxOkClick);

                this.cancelButtonPart.SetValue(Canvas.LeftProperty, left + this.textBoxPart.Width - 50);
                this.cancelButtonPart.SetValue(Canvas.TopProperty, top + this.textBoxPart.Height);
                this.cancelButtonPart.Click += new RoutedEventHandler(OnNodeTextBoxCancelClick);
            }

            if (InEditState)
            {
                GoToState(NodeControl.EdittingStateName, false);
                if (this.textBoxPart != null)
                {
                    this.textBoxPart.Focus();
                }
            }
            else
            {
                this.GoToState(NodeControl.NormalStateName, false);
            }

            this.isTemplateApplied = true;
        }

        private string GetMapNodeChildCount(INodeProxy nodeProxy)
        {
            int count = 0;
            if (nodeProxy != null)
            {
                foreach (IDescriptorProxy descriptor in nodeProxy.Descriptors.GetByDescriptorTypeName("To"))
                {
                    if (descriptor.Relationship.RelationshipType.Name == "MapContainerRelationship")
                    {
                        count++;
                    }
                }
                List<Guid> transcludedNodes = new List<Guid>();
                foreach (IDescriptorProxy descriptor in nodeProxy.Descriptors.GetByDescriptorTypeName("TransclusionMap")) 
                {
                    foreach (IDescriptorProxy transDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                    {
                        if (!transcludedNodes.Contains(transDesc.NodeId) && transDesc.Node != null)
                        {
                            transcludedNodes.Add(transDesc.NodeId);
                            count++;
                        }
                    }
                }
            }
            return count.ToString();
        }

        private void nodeImageBrush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void playMediaImagePart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsPlayingMedia = true;
            GoToState(NodeControl.PlayingStateName, false);
            if (PlayMediaIconClicked != null)
                PlayMediaIconClicked(this, new RoutedEventArgs());
        }

        private void pauseMediaImagePart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsPlayingMedia = false;
            GoToState(NodeControl.IdleStateName, false);
            if (PauseMediaIconClicked != null)
                PauseMediaIconClicked(this, new RoutedEventArgs());
        }

        private void brush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
           
        }

        public void RefreshMetadata(INodeProxy node)
        {
            nodeProxy.Metadata = node.Metadata;
            DataContext = nodeProxy;
            
            if (nodeProxy != null && isTemplateApplied)
            {
                if (transCntTextBlockPart != null)
                {
                    if (this.transCntTextBlockPart.Text == "1")
                    {
                        this.transCntTextBlockPart.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.transCntTextBlockPart.Visibility = Visibility.Visible;
                    }
                }
                if (this.childCntTextBlockPart != null)
                {
                    if (nodeProxy.NodeType.Name == "CompendiumMapNode")
                    {
                        this.childCntTextBlockPart.Visibility = Visibility.Visible;
                        this.childCntTextBlockPart.Text = GetMapNodeChildCount(nodeProxy);
                    }
                    else
                    {
                        this.childCntTextBlockPart.Visibility = Visibility.Collapsed;
                    }
                }
                if (noteTextBlockPart != null)
                {
                    MetadataContext noteKey = new MetadataContext()
                    {
                        MetadataName = "Note",
                        NodeUid = nodeProxy.Id
                    };
                    if (nodeProxy.HasMetadata(noteKey) && !string.IsNullOrEmpty(nodeProxy.GetNodeMetadata(noteKey).MetadataValue))
                    {
                        string note = nodeProxy.GetNodeMetadata(noteKey).MetadataValue;
                        if (!string.IsNullOrEmpty(note))
                        {
                            this.noteTextBlockPart.Visibility = Visibility.Visible;
                            ToolTipService.SetToolTip(this.noteTextBlockPart, note);
                        }
                        else
                        {
                            this.noteTextBlockPart.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        this.noteTextBlockPart.Visibility = Visibility.Collapsed;
                    }
                }

                if (playMediaImagePart != null && pauseMediaImagePart != null)
                {
                    MetadataContext videoSourceKey = new MetadataContext()
                    {
                        MetadataName = "Video.Source",
                        NodeUid = nodeProxy.Id
                    };
                    if (nodeProxy.HasMetadata(videoSourceKey) && !string.IsNullOrEmpty(nodeProxy.GetNodeMetadata(videoSourceKey).MetadataValue))
                    {
                        this.playMediaImagePart.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.playMediaImagePart.Visibility = System.Windows.Visibility.Collapsed;
                        this.pauseMediaImagePart.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        private void TextBoxPart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitNodeName();
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Pass this node to the surface as the start point
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            nrh.FromNode = this;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            nrh.ToNode = this;
        }
        
        public bool InEditState
        {
            get;
            set;
        }

        public void CommitNodeName()
        {
            GoToState(NodeControl.NotEdittingStateName, true);
            this.textBlockPart.Text = this.textBoxPart.Text;

            //adjust the height of the textblock if it's grown
            if (this.textBlockPart.ActualHeight > TextHeight)
            {
                this.textBlockBorderPart.Height = this.textBlockPart.ActualHeight;
                this.textBlockPart.Height = this.textBlockPart.ActualHeight;
                this.textBlockHeight = this.textBlockPart.ActualHeight;
            }
            else
            {
                if (this.textBlockPart.ActualHeight > 10)
                {
                    this.textBlockBorderPart.Height = this.textBlockPart.ActualHeight;
                    this.textBlockPart.Height = this.textBlockPart.ActualHeight;
                    this.textBlockHeight = this.textBlockPart.ActualHeight;
                }
                else
                {
                    //makes sure if all the text is cleared it still has an area to click
                    this.textBlockBorderPart.Height = TextHeight;
                    this.textBlockPart.Height = TextHeight;
                    this.textBlockHeight = TextHeight;
                }
            }
            this.textBlockWidth = this.textBlockPart.ActualWidth;
        }

        public void ResetMediaIcon()
        {
            MetadataContext videoSourceKey = new MetadataContext()
            {
                MetadataName = "Video.Source",
                NodeUid = nodeProxy.Id
            };
            if (nodeProxy.HasMetadata(videoSourceKey) && !string.IsNullOrEmpty(nodeProxy.GetNodeMetadata(videoSourceKey).MetadataValue))
            {
                GoToState(NodeControl.IdleStateName, false);
            }
        }

        public void SetPlayingMediaIcon()
        {
            MetadataContext videoSourceKey = new MetadataContext()
            {
                MetadataName = "Video.Source",
                NodeUid = nodeProxy.Id
            };
            if (nodeProxy.HasMetadata(videoSourceKey) && !string.IsNullOrEmpty(nodeProxy.GetNodeMetadata(videoSourceKey).MetadataValue))
            {
                GoToState(NodeControl.PlayingStateName, false);
            }
        }

        #region Visual State Manager Handlers

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            this.GoToState(NodeControl.MouseOverStateName, true);

            //TODO: If a map node begin the animation to show the preview of the next map depth
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.GoToState(NodeControl.NormalStateName, true);
        }

        private void OnNodeTextBoxCancelClick(object sender, RoutedEventArgs e)
        {
            GoToState(NodeControl.NotEdittingStateName, true);
            this.textBoxPart.Text = this.textBlockPart.Text; //revert the text
        }

        private void OnNodeTextBoxOkClick(object sender, RoutedEventArgs e)
        {
            CommitNodeName();
        }

        private void OnNodeTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; //stop the node navigating when the text block is being clicked
            if (NodeTextBlockClicked != null)
            {
                NodeTextBlockClicked.Invoke(this, new RoutedEventArgs());
            }
            if (e.ClickCount == 2)
            {
                GoToState(NodeControl.EdittingStateName, true);
                if (this.textBoxPart != null)
                {
                    this.textBoxPart.Focus();
                    this.textBoxPart.SelectionStart = this.textBoxPart.Text.Length;
                }
            }
        }

        private void GoToState(string stateName, bool useTransitions)
        {
            VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        #endregion

        #region Registered Control Properties

        public static readonly DependencyProperty NodeImageProperty = DependencyProperty.Register("NodeImage", typeof(ImageBrush), typeof(NodeControl), null);
        public ImageBrush NodeImage
        {
            get { return (ImageBrush)GetValue(NodeImageProperty); }
            set { SetValue(NodeImageProperty, value); }
        }
        
        /* TODO Get rid of this, hardcoded for the demo  */
        public static readonly DependencyProperty NodeImageNameProperty = DependencyProperty.Register("NodeImageName", typeof(string), typeof(NodeControl), null);
        public string NodeImageName
        {
            get { return (string)GetValue(NodeImageNameProperty); }
            set { SetValue(NodeImageNameProperty, value); }
        }

        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register("ImageUrl", typeof(string), typeof(NodeControl), null);
        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public static readonly DependencyProperty TextMarginXProperty = DependencyProperty.Register("TextMarginX", typeof(Double), typeof(NodeControl), new PropertyMetadata(-45.0));
        public double TextMarginX 
        {
            get { return (double)GetValue(TextMarginXProperty); }
            set { SetValue(TextMarginXProperty, value); }
        }

        public static readonly DependencyProperty TextMarginYProperty = DependencyProperty.Register("TextMarginY", typeof(Double), typeof(NodeControl), new PropertyMetadata(60.0));
        public double TextMarginY 
        {
            get { return (double)GetValue(TextMarginYProperty); }
            set { SetValue(TextMarginYProperty, value); }
        }

        public static readonly DependencyProperty TextWidthProperty = DependencyProperty.Register("TextWidth", typeof(Double), typeof(NodeControl), new PropertyMetadata(140.0));
        public double TextWidth 
        {
            get { return (double)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }

        public static readonly DependencyProperty TextHeightProperty = DependencyProperty.Register("TextHeight", typeof(Double), typeof(NodeControl), new PropertyMetadata(50.0));
        public double TextHeight 
        {
            get { return (double)GetValue(TextHeightProperty); }
            set { SetValue(TextHeightProperty, value); }
        }

        public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register("TextFontSize", typeof(Double), typeof(NodeControl), new PropertyMetadata(12.0));
        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TextFontFamilyProperty = DependencyProperty.Register("TextFontFamily", typeof(FontFamily), typeof(NodeControl), new PropertyMetadata(new FontFamily("Arial")));
        public FontFamily TextFontFamily
        {
            get { return (FontFamily)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }

        public static readonly DependencyProperty NodeSkinWidthProperty = DependencyProperty.Register("NodeSkinWidth", typeof(Double), typeof(NodeControl), new PropertyMetadata(50.0));
        public double NodeSkinWidth 
        {
            get { return (double)GetValue(NodeSkinWidthProperty); }
            set { SetValue(NodeSkinWidthProperty, value); }
        }

        public static readonly DependencyProperty NodeSkinHeightProperty = DependencyProperty.Register("NodeSkinHeight", typeof(Double), typeof(NodeControl), new PropertyMetadata(50.0));
        public double NodeSkinHeight
        {
            get { return (double)GetValue(NodeSkinHeightProperty); }
            set { SetValue(NodeSkinHeightProperty, value); }
        }

        public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register("GlowColor", typeof(Color), typeof(NodeControl), new PropertyMetadata(new Color() { A = 255, R = 50, G = 50, B = 255 }));
        public Color GlowColor
        {
            get { return (Color)GetValue(GlowColorProperty); }
            set { SetValue(GlowColorProperty, value); }
        }

        public static readonly DependencyProperty TransclusionCountLeftOffsetProperty = DependencyProperty.Register("TransclusionCountLeftOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(45.0));
        public double TransclusionCountLeftOffset
        {
            get { return (double)GetValue(TransclusionCountLeftOffsetProperty); }
            set { SetValue(TransclusionCountLeftOffsetProperty, value); }
        }

        public static readonly DependencyProperty TransclusionCountTopOffsetProperty = DependencyProperty.Register("TransclusionCountTopOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(40.0));
        public double TransclusionCountTopOffset
        {
            get { return (double)GetValue(TransclusionCountTopOffsetProperty); }
            set { SetValue(TransclusionCountTopOffsetProperty, value); }
        }

        public static readonly DependencyProperty NoteLeftOffsetProperty = DependencyProperty.Register("NoteLeftOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(45.0));
        public double NoteLeftOffset
        {
            get { return (double)GetValue(NoteLeftOffsetProperty); }
            set { SetValue(NoteLeftOffsetProperty, value); }
        }

        public static readonly DependencyProperty NoteTopOffsetProperty = DependencyProperty.Register("NoteTopOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(0.0));
        public double NoteTopOffset
        {
            get { return (double)GetValue(NoteTopOffsetProperty); }
            set { SetValue(NoteTopOffsetProperty, value); }
        }

        public static readonly DependencyProperty ChildCountLeftOffsetProperty = DependencyProperty.Register("ChildCountLeftOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(-12.0));
        public double ChildCountLeftOffset
        {
            get { return (double)GetValue(ChildCountLeftOffsetProperty); }
            set { SetValue(ChildCountLeftOffsetProperty, value); }
        }

        public static readonly DependencyProperty ChildCountTopOffsetProperty = DependencyProperty.Register("ChildCountTopOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(40.0));
        public double ChildCountTopOffset
        {
            get { return (double)GetValue(ChildCountTopOffsetProperty); }
            set { SetValue(ChildCountTopOffsetProperty, value); }
        }

        public static readonly DependencyProperty MediaIconLeftOffsetProperty = DependencyProperty.Register("MediaIconLeftOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(30.0));
        public double MediaIconLeftOffset
        {
            get { return (double)GetValue(MediaIconLeftOffsetProperty); }
            set { SetValue(MediaIconLeftOffsetProperty, value); }
        }

        public static readonly DependencyProperty MediaIconTopOffsetProperty = DependencyProperty.Register("MediaIconTopOffset", typeof(Double), typeof(NodeControl), new PropertyMetadata(35.0));
        public double MediaIconTopOffset
        {
            get { return (double)GetValue(MediaIconTopOffsetProperty); }
            set { SetValue(MediaIconTopOffsetProperty, value); }
        }

        public static readonly DependencyProperty MediaIconHeightProperty = DependencyProperty.Register("MediaIconHeight", typeof(Double), typeof(NodeControl), new PropertyMetadata(15.0));
        public double MediaIconHeight
        {
            get { return (double)GetValue(MediaIconHeightProperty); }
            set { SetValue(MediaIconHeightProperty, value); }
        }

        public static readonly DependencyProperty MediaIconWidthProperty = DependencyProperty.Register("MediaIconWidth", typeof(Double), typeof(NodeControl), new PropertyMetadata(15.0));
        public double MediaIconWidth
        {
            get { return (double)GetValue(MediaIconWidthProperty); }
            set { SetValue(MediaIconWidthProperty, value); }
        }

        #endregion

        private Rect GetTextLocation()
        {
            Rect textLocation = new Rect();
            textLocation.X = TextMarginX;
            textLocation.Y = TextMarginY;
            textLocation.Width = TextWidth;
            textLocation.Height = TextHeight;
            return textLocation;
        }
    }
}
