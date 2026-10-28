using Microsoft.Data.SqlClient;

namespace manaslearning.Business.Interfaces
{
    public interface IApplicationConnection
    {
        SqlConnection GetConnection();
    }
}
