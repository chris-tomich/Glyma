using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class FileProcessorFactory
    {
        private static FileProcessorFactory instance = null;
        private static readonly object padlock = new object();

        private FileProcessorFactory()
        {
        }

        public static FileProcessorFactory Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new FileProcessorFactory();
                    }

                    return instance;
                }
            }
        }

        public IFileProcessor CreateFileProcessor(FileInfo file, Proxy.IMapManager mapManager, Proxy.INode map, Point location)
        {
            if (file.Name.EndsWith(".zip"))
            {
                using (FileStream fileStream = file.OpenRead())
                {
                    bool isCompendiumExport = false;
                    bool hasExportsFolderAndXml = false;
                    bool hasLinkedFilesFolder = false;

                    ZipFile zipFile = new ZipFile(fileStream);

                    foreach (string filename in zipFile.FileNamesInZip)
                    {
                        if (filename.StartsWith("Exports/"))
                        {
                            if (filename.EndsWith(".xml"))
                            {
                                isCompendiumExport = true;
                                //hasExportsFolderAndXml = true;

                                break;
                            }
                        }

                        //if (filename.StartsWith("Linked Files/"))
                        //{
                        //    hasLinkedFilesFolder = true;
                        //}

                        //if (hasExportsFolderAndXml && hasLinkedFilesFolder)
                        //{
                        //    isCompendiumExport = true;

                        //    break;
                        //}
                    }

                    if (isCompendiumExport)
                    {
                        var compendiumFileProcessor = new CompendiumArchiveProcessor(file,
                            mapManager, map);

                        return compendiumFileProcessor;
                    }
                }
            }
            else if (file.Name.EndsWith(".xml"))
            {
                using (var stream = file.OpenRead())
                {
                    var compendiumXmlProcessor = new CompendiumXmlFileProcessor(stream, "", "");
                    compendiumXmlProcessor.MapManager = mapManager;
                    compendiumXmlProcessor.Map = map;
                    if (compendiumXmlProcessor.Nodes.Any())
                    {
                        return compendiumXmlProcessor;
                    }
                    
                }
            }

            return new DroppedFileProcessor(file, location);
        }
    }
}
