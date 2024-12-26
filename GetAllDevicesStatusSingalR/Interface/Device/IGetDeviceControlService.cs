using GetAllDevicesStatusSingalR.Model.Device;
using GetAllDevicesStatusSingalR.Model.Global;

namespace GetAllDevicesStatusSingalR.Interface.Device
{
    public interface IGetDeviceControlService
    {
        public Task<DyanmicModel<List<DeviceControlModel>>> GetDeciveControl(string connectionStr, string DBName);
    }
}
