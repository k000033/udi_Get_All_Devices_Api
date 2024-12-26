using GetAllDevicesStatusSingalR.Hubs;
using GetAllDevicesStatusSingalR.Interface.Device;
using GetAllDevicesStatusSingalR.Model.Device;
using GetAllDevicesStatusSingalR.Model.Global;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace GetAllDevicesStatusSingalR.Service.Device
{
    public class GetUdIBreathing: IGetUdiBreathing
    {

        private readonly IHubContext<SignalRServer> _context;
        private static Dictionary<string, SqlDependency> _dependencies = new();
        private string _connectionStr = "";
        private string _dbName = "";

        public GetUdIBreathing(IHubContext<SignalRServer> context)
        {
            _context = context;
        }


        public async Task<DyanmicModel<UdiBreathingModel>> GetUdiBreathing(string connectionStr, string DBName)
        {
            _connectionStr = connectionStr;
            _dbName = DBName;
            _dependencies[DBName] = null;

            DyanmicModel<UdiBreathingModel> dyanmicModel = new DyanmicModel<UdiBreathingModel>();
            UdiBreathingModel udiBreathingModel = new UdiBreathingModel();


            string sqlStr = @"SELECT BREATHING FROM [dbo].[UDI_PROFILE] ";
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionStr))
                using (SqlCommand sqlCommand = new SqlCommand(sqlStr, sqlConnection))
                {
                    sqlConnection.Open();
                    SqlDependency.Start(_connectionStr);

                    if (_dependencies[DBName] == null)
                    {
                        SqlDependency dependency = new SqlDependency(sqlCommand);
                        dependency.OnChange -= dbChangeNotification;
                        dependency.OnChange += new OnChangeEventHandler(dbChangeNotification);
                        _dependencies[DBName] = dependency;
                    }


                    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            udiBreathingModel.SITE = DBName;
                            udiBreathingModel.BREATHING = reader["BREATHING"].ToString();

                        }
                    }
                }
                dyanmicModel.DATA = udiBreathingModel;
            }
            catch (Exception ex)
            {
                RtnMsg rtnMsg = new RtnMsg();
                rtnMsg.RTN_CODE = 1;
                rtnMsg.RTN_MSG = ex.Message;

                dyanmicModel.RTM_MSG = rtnMsg;

            }

            return dyanmicModel;
        }


        public void dbChangeNotification(object sender, SqlNotificationEventArgs e)
        {
            if (_dependencies[_dbName] != null)
            {
                _dependencies[_dbName].OnChange -= dbChangeNotification;
                _dependencies[_dbName].OnChange -= new OnChangeEventHandler(dbChangeNotification);
                _dependencies[_dbName] = null;
            }



            if (e.Type == SqlNotificationType.Change)
            {
                Task.Run(async () =>
                {
                    _dependencies[_dbName] = null;
                    DyanmicModel<UdiBreathingModel> dyanmicModel = await GetUdiBreathing(_connectionStr, _dbName).ConfigureAwait(false);
                    await _context.Clients.All.SendAsync($"refreshBreathing_{_dbName}", JsonConvert.SerializeObject(dyanmicModel.DATA)).ConfigureAwait(false);
                });
            }
        }
    }
}
