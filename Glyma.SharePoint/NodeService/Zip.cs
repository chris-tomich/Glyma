using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace NodeService
{

    /// <summary>
    /// Provides functionality to zip and folder/Files into a *.zip file or memorystream.
    /// </summary>
    public static class Zip
    {
        /// <summary>
        /// Stream copy buffer size.
        /// </summary>
        private const long BufferSize = 4096;

        /// <summary>
        /// Returns a package that can be traversed
        /// </summary>
        /// <param name="fileStream">the memorystream containing the zip file.</param>
        /// <returns>The package</returns>
        public static ZipFile ExtractZipFileFromZipStream(Stream fileStream)
        {
            Stream packageStream = GetSeekableStream(fileStream);
            ZipFile zipFile = new ZipFile(packageStream);
            return zipFile;
        }

        public static Stream GetPartStream(ZipFile zipFile, ZipEntry zipEntry)
        {
            MemoryStream partStream = new MemoryStream();
            Stream inputStream = zipFile.GetInputStream(zipEntry);
            Stream seekableStream = GetSeekableStream(inputStream);
            CopyStream(seekableStream, partStream);
            return partStream;
        }

        private static Stream GetSeekableStream(Stream sourceStream)
        {
            Stream retStream = null;
            if (sourceStream.CanSeek)
            {
                retStream = sourceStream;
            }
            else
            {
                retStream = new MemoryStream();
                CopyNonSeekableStream(sourceStream, retStream);
            }
            return retStream;
        }

        /// <summary>
        /// Copies one filestream to another.
        /// </summary>
        /// <param name="sourceStream">Source file stream.</param>
        /// <param name="outputStream">Destination file stream.</param>
        private static void CopyStream(System.IO.Stream sourceStream, System.IO.Stream outputStream)
        {
            long bufferSize = sourceStream.Length < BufferSize ? sourceStream.Length : BufferSize;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            sourceStream.Position = 0; //rewind the input stream
            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
            outputStream.Position = 0; //rewind the output stream
        }

        private static void CopyNonSeekableStream(Stream source, Stream outputStream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                byte[] actualData = new byte[bytesRead];
                Array.Copy(buffer, 0, actualData, 0, bytesRead);
                outputStream.Write(actualData, 0, bytesRead);
                bytesWritten += bytesRead;
            }
        }
    }
}
