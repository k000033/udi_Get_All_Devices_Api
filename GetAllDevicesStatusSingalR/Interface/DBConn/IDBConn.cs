namespace GetAllDevicesStatusSingalR.Interface.DBConn
{
    public interface IDBConn
    {
        public string ConnectionStr(string DBName);
        public List<string> getUdiConnectionKey();
    }
}
