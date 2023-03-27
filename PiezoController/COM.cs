using System.IO.Ports;

namespace PIC
{
    public class COM : IDisposable
    {
        private SerialPort? com;

        public SerialPort? Com { get => com; set { com = value; } }
        //Funcs to USB

        public COM()
        {
        }
        public void Open(string PortName)
        {
            if (Com != null)
            {
                Com.Close();
            }
            Com = new SerialPort($"{PortName}", 115200);
            Com.Open();
        }
        public void Dispose()
        {
            Com.Close();
        }
        //Funcs to Execute
        public void Write(double _Amp)
        {
            Com.WriteLine($"xvoltage={_Amp}");
        }
    }
}

