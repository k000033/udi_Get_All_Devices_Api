using GetAllDevicesStatusSingalR.Interface.Device;
using GetAllDevicesStatusSingalR.Model.Global;
using GetAllDevicesStatusSingalR.Service.Device;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;

namespace GetAllDevicesStatusSingalR.Hubs
{
    public class SignalRServer: Hub
    {
        private static readonly ConcurrentDictionary<string, bool> _connectedClients = new ConcurrentDictionary<string, bool>();
        //private static readonly ConcurrentDictionary<string, ClientInfo> _connectedClientInfo = new ConcurrentDictionary<string, ClientInfo>();
        //private readonly IGetDeviceControlService _getDeviceControlService;
        private IConfiguration _configuration;

        public SignalRServer(IGetDeviceControlService getDeviceControlService, IConfiguration configuration)
        {
            //_getDeviceControlService = getDeviceControlService;
            _configuration = configuration; 
        }



        /// 連線事件
        public override async Task OnConnectedAsync()
        {
            _connectedClients.TryAdd(Context.ConnectionId, true);

            var keyStr = _configuration.GetSection("KEY").GetSection("UDI").Value;


            // 发送用户数更新
            await Clients.All.SendAsync("UpdateUserCount", _connectedClients.Count);
            await base.OnConnectedAsync();
        }

        /// 離線事件
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedClients.TryRemove(Context.ConnectionId, out _);

            stopSqlDependency();

             // 发送用户数更新
             await Clients.All.SendAsync("UpdateUserCount", _connectedClients.Count);
            await base.OnDisconnectedAsync(exception);
        }

        public void stopSqlDependency()
        {
            var keys = _configuration.GetSection("Key").GetSection("UDI").Value;
        
            foreach(var key in keys.Split(","))
            {
                var connectionStr = _configuration.GetSection("ConnectionStrings").GetSection(key).Value;
                SqlDependency.Stop(connectionStr);
            }
        }

        //// 根據 ConnectionId 獲取連接字串
        //public string GetConnectionStringById(string connectionId)
        //{
        //    if (_connectedClientInfo.TryGetValue(connectionId, out var clientInfo))
        //    {
        //        return clientInfo.ConnectionString;
        //    }
        //    return "未找到此連接 ID 的連接字串";
        //}

    }


}
