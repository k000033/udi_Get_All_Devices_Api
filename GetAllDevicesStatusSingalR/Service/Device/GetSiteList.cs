using GetAllDevicesStatusSingalR.Interface.Device;
using GetAllDevicesStatusSingalR.Model.Device;
using GetAllDevicesStatusSingalR.Model.Global;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GetAllDevicesStatusSingalR.Service.Device
{
 
    public class GetSiteList: IGetSiteList
    {
        private readonly IConfiguration _configuration;

        public GetSiteList(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 取得各廠區的 店號，AP_IP DB_IP
        public async Task<DyanmicModel<List<SiteInfoModel>>> GetSiteInfoList(string connectionStr)
        {

            DyanmicModel<List<SiteInfoModel>> dyanmicModel = new DyanmicModel<List<SiteInfoModel>>();
            List<SiteInfoModel> siteInfoModels = new List<SiteInfoModel>();

            string sql = @"  ;with AP AS ( SELECT SITE_ID,HOST_IP FROM [DRP].[system].[DRP_SITE] WHERE RIGHT([HOST_NAME],3)='AP1'),
                                      DB AS ( SELECT SITE_ID,HOST_IP FROM [DRP].[system].[DRP_SITE] WHERE RIGHT([HOST_NAME],2)='DB' )
                        
                            SELECT DISTINCT(S.SITE_ID) AS SITE_ID
                                  ,AP.HOST_IP AS AP_IP
		                          ,DB.HOST_IP AS DB_IP
                              FROM [DRP].[system].[DRP_SITE] S
                             INNER JOIN AP
                                ON AP.SITE_ID = S.SITE_ID
                             INNER JOIN DB 
                                ON DB.SITE_ID = S.SITE_ID";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionStr))
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlConnection.Open();

                  

                    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            SiteInfoModel siteInfoModel = new SiteInfoModel();
                            siteInfoModel.SITE_ID = reader["SITE_ID"].ToString();
                            siteInfoModel.AP_IP = reader["AP_IP"].ToString();
                            siteInfoModel.DB_IP = reader["DB_IP"].ToString();

                            siteInfoModels.Add(siteInfoModel);
                        }
                    }   
                }
                dyanmicModel.DATA = siteInfoModels;

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


        // 取得各廠區的 OrderType
        public async Task<DyanmicModel<List<GetOrderTypeModel>>> GetSiteOrderType()
        {
            DyanmicModel<List<GetOrderTypeModel>> dyanmicModel = new DyanmicModel<List<GetOrderTypeModel>>();
            List<GetOrderTypeModel> getOrderTypeModels = new List<GetOrderTypeModel>();
      

            var keyStr = _configuration.GetSection("KEY").GetSection("UDI").Value;

            string sql = @"SELECT TYPE
                          ,NAME 
                      FROM [UDI].[dbo].[UDI_ORDER_TYPE]";
            try
            {

                foreach (var key in keyStr.Split(','))
                {
                    var connectionStr = _configuration.GetSection("ConnectionStrings").GetSection(key).Value;

                    using (SqlConnection sqlConnection = new SqlConnection(connectionStr))
                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        sqlConnection.Open();

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {
                                GetOrderTypeModel getOrderTypeModel = new GetOrderTypeModel();

                                getOrderTypeModel.ID = key;
                                getOrderTypeModel.TYPE = reader["TYPE"].ToString();
                                getOrderTypeModel.NAME = reader["NAME"].ToString();
                                getOrderTypeModels.Add(getOrderTypeModel);
                            }
                        }
                    }

                }

                dyanmicModel.DATA = getOrderTypeModels;
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
    }
}
