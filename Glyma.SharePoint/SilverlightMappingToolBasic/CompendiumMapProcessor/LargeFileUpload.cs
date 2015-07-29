using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Browser;
using System.IO;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class LargeFileUpload
    {
        private readonly string _filename;
        private const int _10MB = 10485760;
        private const int _20MB = 20971520;
        private const int _100MB = 104857600;

        private int _bufferLength = -1;
        private Stream _fileStream;
        private WebClient _uploadWebClient;

        public event EventHandler UploadProgressCompleted;
        public event EventHandler<ProgressChangedEventArgs> UploadProgressChanged; 

        public LargeFileUpload(string filename, Stream fileStream)
        {
            _filename = filename;
            FileStream = fileStream;
        }

        private int BufferLength
        {
            get
            {
                if (_bufferLength == -1)
                {
                    long fileStreamLength = FileStream.Length;

                    if (fileStreamLength < _20MB)
                    {
                        _bufferLength = _10MB / 100;
                    }
                    else if (fileStreamLength < _100MB)
                    {
                        _bufferLength = _100MB / 200;
                    }
                    else
                    {
                        _bufferLength = _100MB / 100;
                    }
                }

                return _bufferLength;
            }
        }

        private WebClient UploadClient
        {
            get
            {
                if (_uploadWebClient == null)
                {
                    _uploadWebClient = new WebClient();

                    _uploadWebClient.OpenWriteCompleted += OnWriteReady;
                    _uploadWebClient.WriteStreamClosed += OnWriteCompleted;
                }

                return _uploadWebClient;
            }
        }

        public Stream FileStream
        {
            get
            {
                return _fileStream;
            }
            private set
            {
                _fileStream = value;
            }
        }

        public string UploadAddress
        {
            get;
            set;
        }

        public FilePackage LastFilePackage
        {
            get;
            set;
        }

        public string FileName
        {
            get
            {
                return _filename;
            }
        }

        private byte[] GetDataArray(Stream fileStream)
        {
            byte[] filePackageData = new byte[BufferLength];

            int lastNumberOfBytesSent = FileStream.Read(filePackageData, 0, BufferLength);

            if (lastNumberOfBytesSent != BufferLength)
            {
                byte[] truncatedChunk = new byte[lastNumberOfBytesSent];

                Array.Copy(filePackageData, truncatedChunk, lastNumberOfBytesSent);

                filePackageData = truncatedChunk;
            }

            return filePackageData;
        }

        private void OnWriteReady(object sender, OpenWriteCompletedEventArgs e)
        {
            byte[] fileChunk = e.UserState as byte[];

            if (fileChunk != null)
            {
                Stream httpStream = e.Result;
                httpStream.Write(fileChunk, 0, fileChunk.Length);
                httpStream.Close();
            }
        }

        private void OnWriteCompleted(object sender, WriteStreamClosedEventArgs e)
        {
            if (FileStream.Position >= (FileStream.Length - 1))
            {
                if (UploadProgressCompleted != null)
                {
                    UploadProgressCompleted(this, new EventArgs());
                }
                return;
            }

            if (UploadProgressChanged != null)
            {
                var percentage = FileStream.Position * 100 / (FileStream.Length - 1);
                UploadProgressChanged(this, new ProgressChangedEventArgs((int)percentage, string.Format("Currently uploading the following file - '{0}'", _filename)));
            }

            UploadFile(e.Error != null);
        }

        public void UploadFile()
        {
            UploadFile(false);
            if (UploadProgressChanged != null)
            {
                UploadProgressChanged(this, new ProgressChangedEventArgs(0, string.Format("Currently uploading the following file - '{0}'", _filename)));
            }
        }

        public void UploadFile(bool isResent)
        {
            byte[] dataToSend;

            if (isResent)
            {
                dataToSend = LastFilePackage.GetCompletePackage();
            }
            else
            {
                byte[] filePackageData = GetDataArray(FileStream);
                LastFilePackage = new FilePackage(filePackageData);
                dataToSend = LastFilePackage.GetCompletePackage();
            }

            string uploadAddress = UploadAddress + "&hash=" + LastFilePackage.SHA1CheckSumSize;

            uploadAddress += "&site=" + HttpUtility.UrlEncode(App.Params.SiteUrl);
            uploadAddress += "&fileUploadLibrary=" + HttpUtility.UrlEncode(App.Params.FileUploadLibrary);

            if (FileStream.Position >= (FileStream.Length - 1))
            {
                uploadAddress += "&position=last";
            }

            Uri uploadAddressUri = new Uri(uploadAddress);

            UploadClient.OpenWriteAsync(uploadAddressUri, null, dataToSend);
        }
    }
}
