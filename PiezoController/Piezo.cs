using System.IO.Ports;

namespace PiezoController
{
    public class Piezo : IDisposable
    {
        SerialPort Com { get; }
        public int ID { get; }

        public Piezo(int portID)
        {
            ID = portID;
            Com = new SerialPort($"COM{portID}", 115200);
            Com.Open();
        }
        public void Dispose()
        {
            Com.Close();
        }

        public void SetVoltage(double value)
        {
            Com.WriteLine($"xvoltage={value}");
        }
    }
}

