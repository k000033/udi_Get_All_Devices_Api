namespace GetAllDevicesStatusSingalR.Model.Device
{
    public class DeviceControlModel
    {
        public string DEVICE_ID { get; set; }
        public int? ORDER_ID { get; set; }
        public byte? STATE { get; set; }

        public int RTN_CODE { get; set; }

        public string STATE_DESCRIPTION { get; set; }
        public string TASK_ID { get; set; }


        public byte? BREATHING_ORDER { get; set; }

        public string BREATHING_LIGHT { get; set; }
        public string BREATHING_ALARM { get; set; }

        public int WO_ASSIGN { get; set; }
        public int WO_RESULT { get; set; }
        public string WO_RATE { get; set; }
        public int QTY_ASSIGN { get; set; }

        public int QTY_RESULT { get; set; }
        public string QTY_RATE { get; set; }

        public string? B_TIME { get; set; }
        public string? E_TIME { get; set; }

        public string DESCRIPTION { get; set; }
    }
}
