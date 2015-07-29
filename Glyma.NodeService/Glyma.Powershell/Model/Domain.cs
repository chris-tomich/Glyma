using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Powershell.Model
{
    public class Domain : IDatabaseInfo
    {
        public Guid DomainId
        {
            get;
            set;
        }

        public Guid NodeId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        string IDatabaseInfo.DatabaseServer
        {
            get;
            set;
        }

        string IDatabaseInfo.DatabaseName
        {
            get;
            set;
        }

        string IDatabaseInfo.ConnectionString
        {
            get
            {
                IDatabaseInfo dbInfo = this;

                return "Data Source=" + dbInfo.DatabaseServer + ";Initial Catalog=" + dbInfo.DatabaseName + ";Integrated Security=True;Connection Timeout=180;";
            }
        }

        /// <summary>
        /// Returns true if valid, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool CheckIsValid()
        {
            IDatabaseInfo databaseInfo = this;

            /// Check DomainId and NodeId.
            if (DomainId == Guid.Empty || NodeId == Guid.Empty)
            {
                return false;
            }

            /// Check that DatabaseServer and DatabaseName have been provided.
            if (string.IsNullOrEmpty(databaseInfo.DatabaseServer) || string.IsNullOrEmpty(databaseInfo.DatabaseName))
            {
                return false;
            }

            return true;
        }
    }
}
