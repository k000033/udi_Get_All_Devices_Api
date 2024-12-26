using GetAllDevicesStatusSingalR.Hubs;
using GetAllDevicesStatusSingalR.Interface.Device;
using GetAllDevicesStatusSingalR.Model.Device;
using GetAllDevicesStatusSingalR.Model.Global;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Xml.Linq;

namespace GetAllDevicesStatusSingalR.Service.Device
{
    public class GetDeviceControlService: IGetDeviceControlService
    {
        private readonly IHubContext<SignalRServer> _context;
        private static  Dictionary<string, SqlDependency> _dependencies = new();
        private string _connectionStr = "";
        private string _dbName = "";

        public GetDeviceControlService(IHubContext<SignalRServer> context)
        {
            _context = context;
        }



        public async Task<DyanmicModel<List<DeviceControlModel>>> GetDeciveControl(string connectionStr,string DBName)
        {
            _connectionStr = connectionStr;
            _dbName = DBName;
            _dependencies[DBName] = null;

            DyanmicModel<List<DeviceControlModel>> dyanmicModel = new DyanmicModel<List<DeviceControlModel>>();
            List<DeviceControlModel> deviceControlList = new List<DeviceControlModel>();

            string sqlStr = @"SELECT DEVICE_ID
                                 ,ORDER_ID    
                                 ,STATE
                                 ,DESCRIPTION
                                 ,B_TIME
                                 ,E_TIME
                                 ,STATE_DESCRIPTION
                                 ,TASK_ID
                                 ,BREATHING_ORDER
                                 ,BREATHING_LIGHT
                                 ,BREATHING_ALARM
                                 ,WO_ASSIGN
                                 ,WO_RESULT
                                 ,WO_RATE
                                 ,QTY_ASSIGN
                                 ,QTY_RESULT
                                 ,QTY_RATE
                             FROM dev.UDI_CONTROL";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionStr))
                using (SqlCommand sqlCommand = new SqlCommand(sqlStr, sqlConnection))
                {
                    sqlConnection.Open();
                    SqlDependency.Start(_connectionStr);

                    if (_dependencies[DBName]==null)
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

                            DeviceControlModel deviceControlModel = new DeviceControlModel
                            {
                                DEVICE_ID = reader["DEVICE_ID"].ToString(),
                                ORDER_ID = reader.IsDBNull(reader.GetOrdinal("ORDER_ID")) ? null : (int)reader["ORDER_ID"],
                                STATE = reader.IsDBNull(reader.GetOrdinal("STATE")) ? null : (byte)reader["STATE"],
                                DESCRIPTION = reader["DESCRIPTION"].ToString(),
                                STATE_DESCRIPTION = reader["STATE_DESCRIPTION"].ToString(),
                                TASK_ID = reader["TASK_ID"].ToString(),
                                BREATHING_ORDER = reader.IsDBNull(reader.GetOrdinal("BREATHING_ORDER")) ? null : (byte)reader["BREATHING_ORDER"],
                                BREATHING_LIGHT = reader["BREATHING_LIGHT"].ToString(),
                                BREATHING_ALARM = reader["BREATHING_ALARM"].ToString(),
                                WO_ASSIGN = (int)reader["WO_ASSIGN"],
                                WO_RESULT = (int)reader["WO_RESULT"],
                                WO_RATE = reader["WO_RATE"].ToString(),
                                QTY_ASSIGN = (int)reader["QTY_ASSIGN"],
                                QTY_RESULT = (int)reader["QTY_RESULT"],
                                QTY_RATE = reader["QTY_RATE"].ToString(),
                                B_TIME = reader["B_TIME"].ToString(),
                                E_TIME = reader["E_TIME"].ToString(),
                                RTN_CODE = 0
                            };

                            deviceControlList.Add(deviceControlModel);
                        }

                    }
                }
                dyanmicModel.DATA = deviceControlList;
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
            if (_dependencies[_dbName] !=null)
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
                    DyanmicModel<List<DeviceControlModel>> dyanmicModel = await GetDeciveControl(_connectionStr, _dbName).ConfigureAwait(false);
                    await _context.Clients.All.SendAsync($"refreshDeviceControl_{_dbName}", JsonConvert.SerializeObject(dyanmicModel.DATA)).ConfigureAwait(false);
                });
            }
        }

    }
}
