using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell.SharePoint
{
    [Cmdlet(VerbsCommon.Set, "SPGLWebServiceMaximum")]
    public class Set_SPGLWebServiceMaximum : PSCmdlet
    {
        // This is 10MB
        private const int READER_QUOTAS_MAX_STRING_CONTENT_LENGTH_DEFAULT = 10485760;
        // This is 2MB
        private const int READER_QUOTAS_MAX_ARRAY_LENGTH_DEFAULT = 2097152;
        private const int READER_QUOTAS_MAX_BYTES_PER_READ_DEFAULT = 10485760;
        private const int MAX_RECEIVED_MESSAGE_SIZE_DEFAULT = 10485760;

        private int _readerQuotasMaxStringContentLength = -1;
        private int _readerQuotasMaxArrayLength = -1;
        private int _readerQuotasMaxBytesPerRead = -1;
        private int _maxReceivedMessageSize = -1;
        private string _serviceName = null;

        [Parameter(Position = 0, Mandatory = false)]
        public string ServiceName
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceName))
                {
                    _serviceName = "transactionalmappingtoolservice.svc";
                }

                return _serviceName;
            }
            set
            {
                _serviceName = value;
            }
        }

        [Parameter(Position = 1, Mandatory = false)]
        public int ReaderQuotasMaxStringContentLength
        {
            get
            {
                if (_readerQuotasMaxStringContentLength == -1)
                {
                    _readerQuotasMaxStringContentLength = READER_QUOTAS_MAX_STRING_CONTENT_LENGTH_DEFAULT;
                }

                return _readerQuotasMaxStringContentLength;
            }
            set
            {
                _readerQuotasMaxStringContentLength = value;
            }
        }

        [Parameter(Position = 2, Mandatory = false)]
        public int ReaderQuotasMaxArrayLength
        {
            get
            {
                if (_readerQuotasMaxArrayLength == -1)
                {
                    _readerQuotasMaxArrayLength = READER_QUOTAS_MAX_ARRAY_LENGTH_DEFAULT;
                }

                return _readerQuotasMaxArrayLength;
            }
            set
            {
                _readerQuotasMaxArrayLength = value;
            }
        }

        [Parameter(Position = 3, Mandatory = false)]
        public int ReaderQuotasMaxBytesPerRead
        {
            get
            {
                if (_readerQuotasMaxBytesPerRead == -1)
                {
                    _readerQuotasMaxBytesPerRead = READER_QUOTAS_MAX_BYTES_PER_READ_DEFAULT;
                }

                return _readerQuotasMaxBytesPerRead;
            }
            set
            {
                _readerQuotasMaxBytesPerRead = value;
            }
        }

        [Parameter(Position = 4, Mandatory = false)]
        public int MaxReceivedMessageSize
        {
            get
            {
                if (_maxReceivedMessageSize == -1)
                {
                    _maxReceivedMessageSize = MAX_RECEIVED_MESSAGE_SIZE_DEFAULT;
                }

                return _maxReceivedMessageSize;
            }
            set
            {
                _maxReceivedMessageSize = value;
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            SPWebService contentService = SPWebService.ContentService;

            SPWcfServiceSettings wcfServiceSettings = new SPWcfServiceSettings();
            wcfServiceSettings.ReaderQuotasMaxStringContentLength = ReaderQuotasMaxStringContentLength;
            wcfServiceSettings.ReaderQuotasMaxArrayLength = ReaderQuotasMaxArrayLength;
            wcfServiceSettings.ReaderQuotasMaxBytesPerRead = ReaderQuotasMaxBytesPerRead;
            wcfServiceSettings.MaxReceivedMessageSize = MaxReceivedMessageSize;
            contentService.WcfServiceSettings[ServiceName] = wcfServiceSettings;

            contentService.Update();
        }
    }
}
