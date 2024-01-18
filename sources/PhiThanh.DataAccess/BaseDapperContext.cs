using Dapper;
using MySqlConnector;
using System.Data;

namespace PhiThanh.DataAccess
{
    public abstract class BaseDapperContext : IDapperContext, IDisposable
    {
        private readonly string _connectionString;

        private IDbConnection _connection;
        protected IDbConnection Connection
        {
            get
            {
                _connection ??= new MySqlConnection(_connectionString);

                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return _connection;
            }
            set { _connection = value; }
        }
        private IDbTransaction _transaction;

        protected BaseDapperContext(string connection)
        {
            _connectionString = connection;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<List<T>> QueryAsync<T>(string sql, object param = null!, IDbTransaction? transaction = null, int timeout = 600)
        {
            return (await Connection.QueryAsync<T>(sql, param, transaction, timeout)).ToList();
        }

        public async Task<T?> QueryFirstOrDefault<T>(string sql, object param = null!, IDbTransaction? transaction = null, int timeout = 600)
        {
            return await Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, timeout);
        }

        public async Task<List<T>> ListAsync<T>(string sql, object param = null!, IDbTransaction? transaction = null, int page = 0, int size = 20, int timeout = 600)
        {
            if (page > 0)
            {
                sql += $" LIMIT {size} OFFSET {(page - 1) * size} ";
            }

            return (await Connection.QueryAsync<T>(sql, param, transaction, timeout)).ToList();
        }

        public async Task ExecuteAsync(string sql, object param = null!, IDbTransaction? transaction = null, int timeout = 600)
        {
            await Connection.ExecuteAsync(sql, param, transaction, timeout);
        }

        public IDbTransaction BeginTransaction()
        {
            _transaction ??= Connection.BeginTransaction();
            return _transaction;
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
        }

        public void RollBackTransaction()
        {
            _transaction.Rollback();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                _connection.Dispose();
                _transaction?.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
