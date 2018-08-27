using System;
using System.Collections.Generic;

namespace FISE_API.Models
{
    public class DeviceDetail
    {
        public string DeviceDetails { get; set; }
        public string DeviceName { get; set; }
        public string DeviceOS { get; set; }
        public string Environment { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? TrashedOn { get; set; }
        public int UserId { get; set; }
        public int DeviceId { get; set; }
        public string Platform { get; set; }
    }

    public class AddDeviceModel
    {        
        public DeviceStatus Status { get; set; }
        public List<DeviceDetail> Devices { get; set; }
        public int DeviceId { get; set; }
    }
}