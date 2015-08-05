using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Glyma.SharePoint.Search
{
    public static class SqlReaderExtensions
    {
        public static T GetValue<T>(this SqlDataReader dbReader, string columnName)
        {
            T result = default(T);
            if (!dbReader.IsDBNull(dbReader.GetOrdinal(columnName)))
            {
                result = (T) dbReader[columnName];
            }
            return result;
        }
    }
}
