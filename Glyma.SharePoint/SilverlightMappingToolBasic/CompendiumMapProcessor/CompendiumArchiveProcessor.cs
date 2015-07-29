using System;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class CompendiumArchiveProcessor : IFileProcessor
    {
        private Stream _compendiumFileStream;
        private List<LargeFileUpload> _fileUploaders;
        private readonly int _fileCount;
        private int _processedFile;
        private readonly ZipFile _zipFile;
        private List<string> _fileNames; 

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<FileProcessorCompletedEventArgs> ProgressCompleted;

        private FileInfo CompendiumFile
        {
            get;
            set;
        }

        private Stream CompendiumFileStream
        {
            get
            {
                if (_compendiumFileStream == null)
                {
                    _compendiumFileStream = CompendiumFile.OpenRead();
                }
                
                return _compendiumFileStream;
            }
        }

        private List<LargeFileUpload> FileUploaders
        {
            get
            {
                if (_fileUploaders == null)
                {
                    _fileUploaders = new List<LargeFileUpload>();
                }

                return _fileUploaders;
            }
        }

        public Proxy.IMapManager MapManager
        {
            get;
            private set;
        }

        public Proxy.INode Map
        {
            get;
            private set;
        }

        public CompendiumArchiveProcessor(FileInfo compendiumFile, Proxy.IMapManager mapManager, Proxy.INode map)
        {
            CompendiumFile = compendiumFile;
            _zipFile = new ZipFile(CompendiumFileStream);
            _fileCount = _zipFile.FileNamesInZip.Count();
            _fileNames = _zipFile.FileNamesInZip.ToList();
            MapManager = mapManager;
            Map = map;
        }

        private void OnProgressCompleted(object sender, FileProcessorCompletedEventArgs e)
        {
            _processedFile++;
            if (_processedFile == _fileCount)
            {
                if (ProgressCompleted != null)
                {
                    ProgressCompleted(sender, e);
                }
            }
            else
            {
                OnProgressChanged(this, new ProgressChangedEventArgs(100, CompendiumFile));
                ProcessFile();
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(sender, e);
            }
        }

        private void OnUploadCompleted(object sender, EventArgs e)
        {
            LargeFileUpload fileUploader = sender as LargeFileUpload;

            if (fileUploader != null)
            {
                FileUploaders.Remove(fileUploader);

                fileUploader.FileStream.Close();
                fileUploader.FileStream.Dispose();

                OnProgressCompleted(sender, new FileProcessorCompletedEventArgs{NeedRefresh = true});
            }
        }

        private void UploadFiles(string fileName, Stream fileStream)
        {
            if (fileStream != null)
            {
                var uploadAddress =
                    string.Format("{0}?filename={1}", App.Params.FileUploaderUrl, fileName);

                var fileUploader = new LargeFileUpload(fileName, fileStream);
                fileUploader.UploadProgressCompleted += OnUploadCompleted;
                fileUploader.UploadAddress = uploadAddress;
                fileUploader.UploadProgressChanged += OnUploadProgressChanged;
                FileUploaders.Add(fileUploader);

                fileUploader.UploadFile();
            }
        }

        private void OnUploadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(sender, e);
            }
        }

        public void ProcessFile()
        {
            if (_fileNames.Count > 0)
            {
                var fileName = _fileNames[_processedFile];
                var uncompressedFilename = fileName.Substring(fileName.LastIndexOf('/') + 1);
                using (var uncompressedFileStream = _zipFile.GetFileStream(fileName))
                {
                    if (fileName.StartsWith("Exports/"))
                    {
                        if (fileName.EndsWith(".xml"))
                        {
                            var linkedFilesUrl = App.Params.SiteUrl + "/" + App.Params.FileUploadLibrary + "/";
                            var compendiumXmlFileProcess = new CompendiumXmlFileProcessor(uncompressedFileStream, linkedFilesUrl, fileName);
                            compendiumXmlFileProcess.Map = Map;
                            compendiumXmlFileProcess.MapManager = MapManager;
                            compendiumXmlFileProcess.ProgressChanged += OnProgressChanged;
                            compendiumXmlFileProcess.ProgressCompleted += OnProgressCompleted;
                            compendiumXmlFileProcess.ProcessFile();
                        }
                    }
                    else
                    {
                        UploadFiles(uncompressedFilename, uncompressedFileStream);
                    }
                }
            }
            else
            {
                
            }
        }

        public void Dispose()
        {
            if (_compendiumFileStream != null)
            {
                _compendiumFileStream.Close();
                _compendiumFileStream.Dispose();
            }
        }
    }
}
