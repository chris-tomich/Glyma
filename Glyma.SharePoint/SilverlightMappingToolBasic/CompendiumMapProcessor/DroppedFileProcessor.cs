using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SimpleIoC;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class DroppedFileProcessor : IFileProcessor
    {
        private readonly Point _location;
        private Stream _droppedFileStream;
        private List<LargeFileUpload> _fileUploaders;

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<FileProcessorCompletedEventArgs> ProgressCompleted;

        private FileInfo DroppedFile
        {
            get;
            set;
        }

        private Stream DroppedFileStream
        {
            get
            {
                if (_droppedFileStream == null)
                {
                    _droppedFileStream = DroppedFile.OpenRead();
                }

                return _droppedFileStream;
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

        public DroppedFileProcessor(FileInfo droppedFile, Point location)
        {
            _location = location;
            DroppedFile = droppedFile;
        }

        private void OnUploadProgressCompleted(object sender, EventArgs e)
        {
            var fileUploader = sender as LargeFileUpload;
            if (fileUploader != null)
            {
                FileUploaders.Remove(fileUploader);

                fileUploader.FileStream.Close();
                fileUploader.FileStream.Dispose();
            }

            var metadata = new Dictionary<string, string>
            {
                {"Name", DroppedFile.Name},
                {
                    "Link",
                    string.Format("{0}/{1}/{2}", App.Params.SiteUrl, App.Params.FileUploadLibrary, DroppedFile.Name)
                }
            };

            IoCContainer.GetInjectionInstance().GetInstance<ISuperGraphNodeFactory>().AddNote(_location, metadata);

            if (ProgressCompleted != null)
            {
                ProgressCompleted(sender, new FileProcessorCompletedEventArgs{NeedRefresh = false});
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
            var fileUploader = new LargeFileUpload(DroppedFile.Name, DroppedFileStream);
            fileUploader.UploadProgressCompleted += OnUploadProgressCompleted;
            fileUploader.UploadProgressChanged += OnUploadProgressChanged;
            fileUploader.UploadAddress = string.Format("{0}?filename={1}", App.Params.FileUploaderUrl, DroppedFile.Name);

            FileUploaders.Add(fileUploader);

            fileUploader.UploadFile();
        }

        public void Dispose()
        {
            if (_droppedFileStream != null)
            {
                _droppedFileStream.Close();
                _droppedFileStream.Dispose();
            }
        }
    }
}
