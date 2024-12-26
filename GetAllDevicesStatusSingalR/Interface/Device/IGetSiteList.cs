using GetAllDevicesStatusSingalR.Model.Device;
using GetAllDevicesStatusSingalR.Model.Global;

namespace GetAllDevicesStatusSingalR.Interface.Device
{
    public interface IGetSiteList
    {
        public  Task<DyanmicModel<List<SiteInfoModel>>> GetSiteInfoList(string connectionStr);
        public Task<DyanmicModel<List<GetOrderTypeModel>>> GetSiteOrderType();
    }
}
