using GetAllDevicesStatusSingalR.Model.Device;
using GetAllDevicesStatusSingalR.Model.Global;

namespace GetAllDevicesStatusSingalR.Interface.Device
{
    public interface IGetUdiBreathing
    {
        public Task<DyanmicModel<UdiBreathingModel>> GetUdiBreathing(string connectionStr, string DBName);
    }
}
