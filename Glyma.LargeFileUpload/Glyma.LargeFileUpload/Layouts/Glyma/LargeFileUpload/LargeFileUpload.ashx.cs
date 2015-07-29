using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.SessionState;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;

namespace Glyma.LargeFileUpload
{
    /// <summary>
    /// Summary description for LargeFileUpload
    /// </summary>
    public class LargeFileUpload : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                var hashLength = int.Parse(context.Request.QueryString["hash"]);
                var filename = context.Request.QueryString["filename"];
                var fileUploadLibrary = HttpUtility.UrlDecode(context.Request.QueryString["fileUploadLibrary"]);
                var position = context.Request.QueryString["position"];
                var serverUrl = HttpUtility.UrlDecode(context.Request.QueryString["site"]);//string.Format("http://{0}/", context.Request.Url.Authority);
                context.Response.Expires = -1;
                context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
                context.Response.AddHeader("cache-control", "private");
                context.Response.CacheControl = "no-cache";

                string tempDirectory = Environment.GetEnvironmentVariable("tmp");

                string filePath = tempDirectory + @"\" + filename;

                FileInfo fileInfo = new FileInfo(filePath);

                int bufferLength = (int)context.Request.InputStream.Length;
                byte[] receivedData = new byte[bufferLength];

                int bytesRead = context.Request.InputStream.Read(receivedData, 0, bufferLength);

                if (bytesRead != bufferLength)
                {
                    byte[] truncatedChunk = new byte[bytesRead];

                    Array.Copy(receivedData, truncatedChunk, bytesRead);

                    receivedData = truncatedChunk;
                }

                ReceivedPackage receivedPackage = new ReceivedPackage(hashLength, receivedData);

                if (!receivedPackage.IsCorrupted())
                {
                    using (FileStream fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write))
                    {
                        fileStream.Write(receivedPackage.FileDataReceived, 0, receivedPackage.FileDataSize);
                    }
                }
                else
                {
                    throw new CorruptedPackageException();
                }

                if (position == "last")
                {
                    using (var fileStream = fileInfo.OpenRead())
                    {
                        using (var site = new SPSite(serverUrl))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                SPList list;
                                try
                                {
                                    list = web.GetList(serverUrl + "/" + fileUploadLibrary);
                                }
                                catch (Exception ex)
                                {
                                    context.AddError(ex);
                                    return;
                                }
                                list.RootFolder.Files.Add(serverUrl + "/" + fileUploadLibrary + "/" + filename, fileStream);
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    }

                    fileInfo.Delete();
                }
            });
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}