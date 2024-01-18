using MySqlConnector;

namespace PhiThanh.DataAccess.Contexts
{
    public class CoreDapperContext : BaseDapperContext
    {
        public CoreDapperContext(string connection) : base(connection)
        {
            Connection = new MySqlConnection(connection);
        }
    }
}
