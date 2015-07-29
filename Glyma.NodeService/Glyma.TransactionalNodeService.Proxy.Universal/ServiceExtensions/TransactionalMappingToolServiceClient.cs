using System;
using System.IO;
using System.Runtime.Serialization;
using Ionic.Zlib;

namespace TransactionalNodeService.Proxy.Universal.Service
{
    public partial class TransactionalMappingToolServiceClient
    {
        public void SubmitBulkOperationsAsync(string callingUrl, Guid sessionId, OPS operations)
        {
            SubmitBulkOperationsAsync(callingUrl, sessionId, operations, null);
        }

        public void SubmitBulkOperationsAsync(string callingUrl, Guid sessionId, OPS operations, object userState)
        {
            MemoryStream operationsXml = null;
            MemoryStream compressedOperationsStream = null;
            ZlibStream zipStream = null;

            try
            {
                operationsXml = new MemoryStream();

                DataContractSerializer serializer = new DataContractSerializer(typeof(OPS));

                serializer.WriteObject(operationsXml, operations);

                byte[] operationsXmlBytes = operationsXml.ToArray();
                compressedOperationsStream = new MemoryStream();

                using (zipStream = new ZlibStream(compressedOperationsStream, CompressionMode.Compress))
                {
                    zipStream.Write(operationsXmlBytes, 0, operationsXmlBytes.Length);
                }

                byte[] compressedOperationsXmlBytes = compressedOperationsStream.ToArray();

                string compressedOperations = Convert.ToBase64String(compressedOperationsXmlBytes);

                BOCAsync(callingUrl, sessionId, compressedOperations, userState);
            }
            finally
            {
                if (operationsXml != null)
                {
                    operationsXml.Dispose();
                    operationsXml = null;
                }
            }
        }
    }
}
