using System.Data;

namespace PhiThanh.DataAccess
{
    public interface IDapperContext
    {
        Task<List<T>> QueryAsync<T>(string sql, object param = null!, IDbTransaction? transaction = null, int timeout = 600);
        Task<T?> QueryFirstOrDefault<T>(string sql, object param = null!, IDbTransaction? transaction = null, int timeout = 600);
        Task<List<T>> ListAsync<T>(string sql, object param = null!, IDbTransaction? transaction = null, int page = 0, int size = 20, int timeout = 600);
        Task ExecuteAsync(string sql, object param = null!, IDbTransaction? transaction = null, int timeout = 600);
        IDbConnection CreateConnection();
        IDbTransaction BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();
    }
}
