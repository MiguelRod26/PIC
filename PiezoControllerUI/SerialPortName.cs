namespace PiezoControllerUI
{
    public class SerialPortName
    {
        public int PortID { get; }
        public string ComName => $"COM{PortID}";
        public string DeviceName { get; }

        public SerialPortName(int portID, string deviceName)
        {
            PortID = portID;
            DeviceName = deviceName;
        }


        public override string ToString()
        {
            return $"{DeviceName} ({ComName})";
        }
    }
}
