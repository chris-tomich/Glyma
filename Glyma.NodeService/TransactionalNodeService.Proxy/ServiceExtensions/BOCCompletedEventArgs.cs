using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Service
{
    public partial class BOCCompletedEventArgs
    {
        private ObservableCollection<BOR> _decompressedResult = null;

        public ObservableCollection<BOR> DecompressedResult
        {
            get
            {
                if (_decompressedResult == null)
                {
                    if (string.IsNullOrEmpty(Result))
                    {
                        return null;
                    }

                    MemoryStream compressedBulkOperationsResponsesStream = null;
                    MemoryStream decompressedBulkOperationsResponsesStream = null;
                    ZlibStream decompressionStream = null;

                    try
                    {
                        byte[] compressedBulkOperationsResponsesBytes = Convert.FromBase64String(Result);
                        compressedBulkOperationsResponsesStream = new MemoryStream(compressedBulkOperationsResponsesBytes);
                        byte[] buffer = new byte[1024];
                        int numBytesRead = 0;
                        bool start = true;

                        decompressedBulkOperationsResponsesStream = new MemoryStream();

                        using (decompressionStream = new ZlibStream(compressedBulkOperationsResponsesStream, CompressionMode.Decompress))
                        {
                            while (start || numBytesRead > 0)
                            {
                                numBytesRead = decompressionStream.Read(buffer, 0, buffer.Length);

                                if (numBytesRead > 0)
                                {
                                    decompressedBulkOperationsResponsesStream.Write(buffer, 0, numBytesRead);
                                }

                                start = false;
                            }
                        }

                        decompressedBulkOperationsResponsesStream.Position = 0;

                        DataContractSerializer deserializer = new DataContractSerializer(typeof(ObservableCollection<BOR>));

                        _decompressedResult = deserializer.ReadObject(decompressedBulkOperationsResponsesStream) as ObservableCollection<BOR>;
                    }
                    finally
                    {
                        if (decompressedBulkOperationsResponsesStream != null)
                        {
                            decompressedBulkOperationsResponsesStream.Dispose();
                            decompressedBulkOperationsResponsesStream = null;
                        }
                    }
                }

                return _decompressedResult;
            }
        }
    }
}
