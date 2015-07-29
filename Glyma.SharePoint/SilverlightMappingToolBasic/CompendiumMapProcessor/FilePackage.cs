using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Text;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class FilePackage
    {
        private byte[] _sha1CheckSum;

        public FilePackage(byte[] fileData)
        {
            FileData = fileData;
        }

        private byte[] FileData
        {
            get;
            set;
        }

        public byte[] SHA1CheckSum
        {
            get
            {
                if (_sha1CheckSum == null)
                {
                    SHA1 sha1Hasher = new SHA1Managed();
                    _sha1CheckSum = sha1Hasher.ComputeHash(FileData);
                }

                return _sha1CheckSum;
            }
        }

        public int SHA1CheckSumSize
        {
            get
            {
                return SHA1CheckSum.Length;
            }
        }

        public byte[] GetCompletePackage()
        {
            byte[] completeChunk = new byte[SHA1CheckSumSize + FileData.Length];

            Array.Copy(SHA1CheckSum, 0, completeChunk, 0, SHA1CheckSumSize);
            Array.Copy(FileData, 0, completeChunk, SHA1CheckSumSize, FileData.Length);

            return completeChunk;
        }
    }
}
