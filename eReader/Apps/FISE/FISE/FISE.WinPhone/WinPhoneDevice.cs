using FISE;
using FISE.WinPhone;
using System;
using Windows.System.Profile;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(WinPhoneDevice))]
namespace FISE.WinPhone
{
    public class WinPhoneDevice : IDevice
    {
        public string GetIdentifier()
        {
            //NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            ////for each j you can get the MAC 
            //PhysicalAddress address = nics[0].GetPhysicalAddress();
            //byte[] bytes = address.GetAddressBytes();
            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    // Display the physical address in hexadecimal. 
            //    Console.Write("{0}", bytes[i].ToString("X2"));
            //    // Insert a hyphen after each byte, unless we are at the end of the 
            //    // address. 
            //    if (i != bytes.Length - 1)
            //    {
            //        Console.Write("-");
            //    }
            //}
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);
            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);
            var st = BitConverter.ToString(bytes);
            // byte[] myDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");

            //    byte[] myDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            //byte[] myDeviceId = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return st;
        }
    }
}
