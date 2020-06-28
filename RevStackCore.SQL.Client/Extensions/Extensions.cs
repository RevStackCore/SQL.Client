using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using RevStackCore.Extensions.SQL;
using RevStackCore.DataAnnotations;
using System.Reflection;

namespace RevStackCore.SQL.Client
{
    public static class SQLExtensions
    {
        public static void SetTableNameMapper(bool mapPropertiesToUnderscore=false)
        {
            if (mapPropertiesToUnderscore)
            {
                DefaultTypeMap.MatchNamesWithUnderscores = true;
            }

            SqlMapperExtensions.TableNameMapper = (type) => {
                var tableAttribute = type.GetCustomAttribute<TableAttribute>(true);
                if (tableAttribute != null && !string.IsNullOrEmpty(tableAttribute.Name))
                {
                    return tableAttribute.Name;
                }
                else
                {
                    return type.Name;
                }
               
            };
        }

    }


}
