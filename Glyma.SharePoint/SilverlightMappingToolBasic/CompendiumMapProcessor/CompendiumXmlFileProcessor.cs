using System;
using System.ComponentModel;
using System.Xml;
using System.IO;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel;
using Proxy = TransactionalNodeService.Proxy;
using System.Windows.Threading;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public partial class CompendiumXmlFileProcessor : IFileProcessor
    {
        private XmlReader _compendiumFileReader;
        private CompendiumStorage _compendiumXmlParserStore;
        private TransactionBuilder _transactionBuilder;
        private DispatcherTimer _transactionBuilderTimer;
        private readonly string _fileName;
        private INode[] _nodes;
        

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<FileProcessorCompletedEventArgs> ProgressCompleted;

        private string LinkedFilesLocationUrl
        {
            get;
            set;
        }

        private Stream CompendiumFileStream
        {
            get;
            set;
        }

        private XmlReader CompendiumFileReader
        {
            get
            {
                if (_compendiumFileReader == null)
                {
                    // The Compendium XML files always have DTD declarations at the top of the page so we need to ignore these otherwise the .NET XML parsers cry cause they can't find what they refer to.
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Ignore;
                    settings.XmlResolver = null;

                    _compendiumFileReader = XmlReader.Create(CompendiumFileStream, settings);

                    // Traverse the XML tree till we find the model element. This marks the start of the Compendium data and this is where the CompendiumStorage class expects to load from.
                    while (!(_compendiumFileReader.Name == "model" && _compendiumFileReader.NodeType == XmlNodeType.Element) && _compendiumFileReader.Read())
                    {
                    }
                }

                return _compendiumFileReader;
            }
        }

        private CompendiumStorage CompendiumXmlParserStore
        {
            get
            {
                try
                {
                    if (_compendiumXmlParserStore == null)
                    {
                        _compendiumXmlParserStore = new XmlModel.CompendiumStorage();
                        _compendiumXmlParserStore.Load(CompendiumFileReader, LinkedFilesLocationUrl);
                    }
                    return _compendiumXmlParserStore;
                }
                catch (Exception ex)
                {
                    return null;
                }
                

                
            }
        }

        public INode[] Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    if (CompendiumXmlParserStore == null)
                    {
                        _nodes = new INode[] {};
                    }
                    else
                    {
                        _nodes = CompendiumXmlParserStore.GetAllNodes();
                    }
                    
                }
                return _nodes;
            }
        }

        private TransactionBuilder Transactions
        {
            get
            {
                if (_transactionBuilder == null)
                {
                    _transactionBuilder = new TransactionBuilder(Nodes, CompendiumXmlParserStore.RootViewRelationships);
                    _transactionBuilder.Map = Map;
                    _transactionBuilder.MapManager = MapManager;
                    _transactionBuilder.InitialiseTransactions();
                    _transactionBuilder.ProgressChanged += TransactionBuilderOnProgressChanged;
                    _transactionBuilder.ProgressCompleted += TransactionBuilderOnProgressCompleted;
                }

                return _transactionBuilder;
            }
        }

        public Proxy.INode Map
        {
            get;
            set;
        }

        public Proxy.IMapManager MapManager
        {
            get;
            set;
        }

        public CompendiumXmlFileProcessor(Stream compendiumFileStream, string linkedFilesLocationUrl, string fileName)
        {
            LinkedFilesLocationUrl = linkedFilesLocationUrl;
            CompendiumFileStream = compendiumFileStream;
            _fileName = fileName;
        }

        private void TransactionBuilderOnProgressCompleted(object sender, FileProcessorCompletedEventArgs e)
        {
            if (ProgressCompleted != null)
            {
                ProgressCompleted(sender, e);
            }
        }

        private void TransactionBuilderOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(sender, new ProgressChangedEventArgs(e.ProgressPercentage, "We're now working on importing the map into Glyma..."));
            }
        }

        private void transactionBuilderTimer_Tick(object sender, EventArgs e)
        {
            ProcessFile();
        }

        public void ProcessFile()
        {
            if (_transactionBuilderTimer != null)
            {
                _transactionBuilderTimer.Stop();
                _transactionBuilderTimer = null;
            }

            if (Transactions.ExecuteTransactions(10))
            {
                _transactionBuilderTimer = new DispatcherTimer();
                _transactionBuilderTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                _transactionBuilderTimer.Tick += transactionBuilderTimer_Tick;
                _transactionBuilderTimer.Start();
            }
        }

        public void Dispose()
        {
        }
    }
}
