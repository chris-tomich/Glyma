using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace TransactionalNodeService
{
    public interface IPersistableSessionObject
    {
        bool IsNew { get; }
        bool IsDirty { get; }
        void LoadSessionObject(IDataRecord record);
        void PersistSessionObject(IDbConnectionAbstraction connection);
    }
}