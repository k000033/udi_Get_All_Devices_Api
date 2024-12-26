using GetAllDevicesStatusSingalR.Interface.DBConn;
using System.Data.SqlClient;

namespace GetAllDevicesStatusSingalR.Service.DBConn
{
    public class DBConn:IDBConn
    {
        private readonly IConfiguration _configuration;

        public DBConn(IConfiguration configuration)
        {
            _configuration= configuration;  
        }

        public string ConnectionStr(string DBName)
        {
            string connectionStr = _configuration[$"ConnectionStrings:{DBName}"].ToString();
            return  connectionStr;
        }

        // 取得所有SingalR 的 所有連接字串
        public List<string> getUdiConnectionKey()
        {
            List<string> singalRConnectionKey = new List<string>();

            var keyStr = _configuration.GetSection("KEY").GetSection("UDI").Value;

            foreach(var key in keyStr.Split(','))
            {
                singalRConnectionKey.Add(key);
            }

            return singalRConnectionKey;
        }
    }
}
