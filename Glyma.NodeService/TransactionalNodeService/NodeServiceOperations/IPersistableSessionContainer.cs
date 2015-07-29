using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public interface IPersistableSessionContainer
    {
        void PersistSessionObject();
    }
}