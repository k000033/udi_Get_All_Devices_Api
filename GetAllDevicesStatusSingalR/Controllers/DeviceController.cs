using GetAllDevicesStatusSingalR.Interface.DBConn;
using GetAllDevicesStatusSingalR.Interface.Device;
using GetAllDevicesStatusSingalR.Service.Device;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GetAllDevicesStatusSingalR.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeviceController : Controller
    {

        private readonly IDBConn _dBConn;
        private readonly IGetDeviceControlService _getDeviceControlService;
        private readonly IGetUdiBreathing _getUdiBreathing;
        private readonly IConfiguration _configuration;
        private readonly IGetSiteList _getSiteList;

        public DeviceController(IDBConn dBConn, IGetDeviceControlService getDeviceControlService, IConfiguration configuration, IGetUdiBreathing getUdiBreathing, IGetSiteList getSiteList)
        {
            _dBConn = dBConn;
            _getDeviceControlService = getDeviceControlService;
            _configuration = configuration;
            _getUdiBreathing = getUdiBreathing;
            _getSiteList = getSiteList;
        }

        // 取得所有SingalR 的 所有連接字串
        [HttpGet]
        public IActionResult GetUdiConnectionKey()
        {
            List<string> udiConnectionKey = _dBConn.getUdiConnectionKey();
            return Ok( JsonConvert.SerializeObject(udiConnectionKey));
        }




        [HttpGet]
        public async Task<IActionResult> GetDeviceControl(string DbName)
        {
            string connectionStr = _configuration[$"ConnectionStrings:{DbName}"];
            var res = await _getDeviceControlService.GetDeciveControl(connectionStr, DbName);

            if (res.RTM_MSG != null)
            {
                return Ok(res.RTM_MSG.RTN_MSG);
            }

            return Ok(JsonConvert.SerializeObject(res.DATA));
        }

        [HttpGet]
        public async Task<IActionResult> GetUdiBreathing(string DbName)
        {
            string connectionStr = _configuration[$"ConnectionStrings:{DbName}"];
            var res = await _getUdiBreathing.GetUdiBreathing(connectionStr, DbName);

            if (res.RTM_MSG != null)
            {
                return Ok(res.RTM_MSG.RTN_MSG);
            }

            return Ok(JsonConvert.SerializeObject(res.DATA));
        }


        // 取得各廠區的 店號，AP_IP DB_IP
        [HttpGet]
        public async Task<IActionResult> GetSiteList()
        {
            string connectionStr = _configuration["ConnectionStrings:85_DRP"];

            var res = await _getSiteList.GetSiteInfoList(connectionStr);


            if (res.RTM_MSG != null)
            {
                return Ok(res.RTM_MSG.RTN_MSG);
            }

            return Ok(JsonConvert.SerializeObject(res.DATA));

        }

        // 取得各廠區的 OrderType
        [HttpGet]
        public async Task<IActionResult> GetSiteOrderType()
        {
            var res = await _getSiteList.GetSiteOrderType();

            if (res.RTM_MSG != null)
            {
                return Ok(res.RTM_MSG.RTN_MSG);
            }

            return Ok(JsonConvert.SerializeObject(res.DATA));
        }
    }
}
