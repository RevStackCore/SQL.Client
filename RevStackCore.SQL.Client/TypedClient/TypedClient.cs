using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;
using RevStackCore.Pattern;
using RevStackCore.Extensions.SQL;

namespace RevStackCore.SQL.Client
{
    public class TypedClient<TEntity,TConnection,TKey> where TEntity : class, IEntity<TKey>
        where TConnection : class, IDbConnection, new()
    {
        private readonly string _connectionString;
        private readonly string _type;
        private Type _tKeyType;
        private SQLLanguageType _languageType;
        public TypedClient(string connectionString, SQLLanguageType languageType)
        {
            _connectionString = connectionString;
            _type = typeof(TEntity).Name;
            _tKeyType = typeof(TKey);
            _languageType = languageType;
        }

        public IDbConnection Db
        {
            get
            {
                var connection = new TConnection();
                connection.ConnectionString = _connectionString;
                IDbConnection db = connection;
                return db;
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                string sql = "Select * From " + _type;
                var query = db.Query<TEntity>(sql).ToList();
                return query;
            }
        }

        public TEntity GetById(TKey id)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Get<TEntity>(id);
                return query;
            }
        }

        public TEntity Insert(TEntity entity)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                //Inserts may entail Id as auto-identity field
                //If so, set the Dapper Contrib identity return value to the entity Id property
                bool setIdFromResult = false;
                var type = entity.GetType();
                var idProperty = type.GetProperty("Id");
                if(idProperty==null & (_tKeyType == typeof(int) || _tKeyType == typeof(long)))
                {
                    setIdFromResult = true;
                }
                db.Open();
                var r=db.Insert(entity);
                if(setIdFromResult && r!=default(long))
                {
                    if(_tKeyType == typeof(int))
                    {
                        int intId = Convert.ToInt32(r);
                        idProperty.SetValue(entity, intId);
                    }
                    else
                    {
                        idProperty.SetValue(entity, r);
                    }
                }
                return entity;
            }
        }

        public TEntity Update(TEntity entity)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                db.Open();
                db.Update(entity);
                return entity;
            }
        }

        public void Delete(TEntity entity)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                db.Open();
                db.Delete(entity);
            }
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            var db = new TConnection();
            db.ConnectionString = _connectionString;
            return db.Find(predicate, _languageType);
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>(string sp_procedure, object param) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(sp_procedure, param, commandType: CommandType.StoredProcedure);
                return query;
            }
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>(string sp_procedure) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(sp_procedure, commandType: CommandType.StoredProcedure);
                return query;
            }
        }

        public TResult ExecuteProcedureSingle<TResult>(string sp_procedure, object param) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query= db.Query<TResult>(sp_procedure, param, commandType: CommandType.StoredProcedure);
                if(query.Any())
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public TResult ExecuteProcedureSingle<TResult>(string sp_procedure) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(sp_procedure, commandType: CommandType.StoredProcedure);
                if (query.Any())
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public TValue ExecuteScalar<TValue>(string s_function, object param) where TValue : struct
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.ExecuteScalar<TValue>(s_function,param);
                return query;
            }
        }

        public TValue ExecuteScalar<TValue>(string s_function) where TValue : struct
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.ExecuteScalar<TValue>(s_function);
                return query;
            }
        }

        public TResult ExecuteFunction<TResult>(string s_function, object param) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(s_function, param);
                if(query.Any())
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public TResult ExecuteFunction<TResult>(string s_function) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(s_function);
                if (query.Any())
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<TResult> ExecuteFunctionWithResults<TResult>(string s_function) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(s_function);
                return query;
            }
        }

        public IEnumerable<TResult> ExecuteFunctionWithResults<TResult>(string s_function, object param) where TResult : class
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var query = db.Query<TResult>(s_function,param);
                return query;
            }
        }

        public void Execute(string sp_procedure, object param)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                db.Execute(sp_procedure, param, commandType: CommandType.StoredProcedure);
            }
        }

        public void Execute(string sp_procedure)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                db.Execute(sp_procedure, commandType: CommandType.StoredProcedure);
            }
        }

        public DynamicParameters Execute(string sp_procedure,DynamicParameters param)
        {
            var connection = new TConnection();
            connection.ConnectionString = _connectionString;
            using (IDbConnection db = connection)
            {
                var result = db.Execute(sp_procedure, param, commandType: CommandType.StoredProcedure);
                return param;
            }
        }
    }
}
