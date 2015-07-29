using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Service
{
    public partial class QueryResponse
    {
        public void DecompressResponse()
        {
            if (string.IsNullOrEmpty(this.ZNO))
            {
                return;
            }

            MemoryStream compressMetadataCollection = null;
            MemoryStream decompressedMetadataCollection = null;
            ZlibStream decompressStream = null;

            try
            {
                byte[] compressedMetadataCollectionBytes = Convert.FromBase64String(this.ZNO);
                compressMetadataCollection = new MemoryStream(compressedMetadataCollectionBytes);
                byte[] buffer = new byte[1024];
                int numBytesRead = 0;
                bool start = true;

                decompressedMetadataCollection = new MemoryStream();

                using (decompressStream = new ZlibStream(compressMetadataCollection, CompressionMode.Decompress))
                {
                    while (start || numBytesRead > 0)
                    {
                        numBytesRead = decompressStream.Read(buffer, 0, buffer.Length);

                        if (numBytesRead > 0)
                        {
                            decompressedMetadataCollection.Write(buffer, 0, numBytesRead);
                        }

                        start = false;
                    }
                }

                decompressedMetadataCollection.Position = 0;

                DataContractSerializer deserializer = new DataContractSerializer(typeof(CompressedResponseTuple));

                CompressedResponseTuple responseTuple = deserializer.ReadObject(decompressedMetadataCollection) as CompressedResponseTuple;

                Nodes = responseTuple.Nodes;
                Relationships = responseTuple.Relationships;
            }
            finally
            {
                if (decompressedMetadataCollection != null)
                {
                    decompressedMetadataCollection.Dispose();
                    decompressedMetadataCollection = null;
                }
            }
        }
    }
}
