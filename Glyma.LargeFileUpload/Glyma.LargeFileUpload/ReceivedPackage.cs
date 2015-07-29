using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Glyma.LargeFileUpload
{
    public class ReceivedPackage
    {
        private int _fileDataSize = -1;
        private byte[] _fileDataReceived;
        private string _receivedSha1CheckSum;
        private string _calculatedSha1CheckSum;

        public const int NewChunk = 0;
        public const int ResentChunk = 1;

        public ReceivedPackage(int hashLength, byte[] receivedData)
        {
            HashLength = hashLength;
            ReceivedData = receivedData;
        }

        private int HashLength
        {
            get;
            set;
        }

        private byte[] ReceivedData
        {
            get;
            set;
        }

        public string ReceivedSha1Hash
        {
            get
            {
                if (string.IsNullOrEmpty(_receivedSha1CheckSum))
                {
                    _receivedSha1CheckSum = Encoding.UTF8.GetString(ReceivedData, 0, HashLength);
                }

                return _receivedSha1CheckSum;
            }
        }

        public int FileDataSize
        {
            get
            {
                if (_fileDataSize == -1)
                {
                    _fileDataSize = ReceivedData.Length - HashLength;
                }

                return _fileDataSize;
            }
        }

        public byte[] FileDataReceived
        {
            get
            {
                if (_fileDataReceived == null)
                {
                    _fileDataReceived = new byte[FileDataSize];

                    Array.Copy(ReceivedData, HashLength, _fileDataReceived, 0, FileDataSize);
                }

                return _fileDataReceived;
            }
        }

        public string CalculatedSHA1CheckSum
        {
            get
            {
                if (string.IsNullOrEmpty(_calculatedSha1CheckSum))
                {
                    SHA1 sha1Hasher = new SHA1Managed();
                    byte[] sha1Hash = sha1Hasher.ComputeHash(FileDataReceived);
                    _calculatedSha1CheckSum = Encoding.UTF8.GetString(sha1Hash, 0, sha1Hash.Length);
                }

                return _calculatedSha1CheckSum;
            }
        }

        public bool IsCorrupted()
        {
            return (ReceivedSha1Hash != CalculatedSHA1CheckSum);
        }
    }
}
