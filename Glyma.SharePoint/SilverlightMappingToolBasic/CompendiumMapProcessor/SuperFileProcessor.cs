using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class SuperFileProcessor
    {
        private readonly FileInfo[] _files;
        private readonly IMapManager _mapManager;
        private readonly INode _map;
        private readonly Point _location;
        private readonly int _fileCount;
        private int _processedFile;
        private bool _needRefresh;
        
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<FileProcessorCompletedEventArgs> ProgressCompleted;

        public SuperFileProcessor(FileInfo[] files, IMapManager mapManager, INode map, Point location)
        {
            _files = files;
            _mapManager = mapManager;
            _map = map;
            _location = location;
            _fileCount = _files == null ? 0 : _files.Count();
            _processedFile = 0;
        }

        


        private void OnProgressCompleted(object sender, FileProcessorCompletedEventArgs e)
        {
            if (e.NeedRefresh)
            {
                _needRefresh = e.NeedRefresh;
            }

            _processedFile ++;

            if (_processedFile == _fileCount)
            {
                if (ProgressCompleted != null)
                {
                    ProgressCompleted(sender, e);
                }
            }
            else
            {
                if (ProgressChanged != null)
                {
                    ProgressChanged(sender, new ProgressChangedEventArgs(100, null));
                }
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                if (e.UserState == null || e.UserState.ToString().Length <= 0)
                {
                    ProgressChanged(sender, new ProgressChangedEventArgs(e.ProgressPercentage, string.Format("Currently processing the following file - '{0}'", _files[_processedFile])));
                }
                else
                {
                    ProgressChanged(sender, e);
                }
                
            }
        }


        public void ProcessFiles ()
        {
            foreach (var file in _files)
            {
                var fileProcessor = FileProcessorFactory.Instance.CreateFileProcessor(file, _mapManager, _map, _location);
                fileProcessor.ProgressChanged += OnProgressChanged;
                fileProcessor.ProgressCompleted += OnProgressCompleted;
                fileProcessor.ProcessFile();
            }
        }
    }
}
