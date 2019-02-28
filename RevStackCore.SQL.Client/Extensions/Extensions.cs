using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using RevStackCore.Extensions.SQL;

namespace RevStackCore.SQL.Client
{
    public static class SQLExtensions
    {
        public static void SetTableNameMapper()
        {
            SqlMapperExtensions.TableNameMapper = (type) => {
                return type.Name;
            };
        }

    }


}
