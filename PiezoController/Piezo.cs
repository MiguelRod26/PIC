using Sardine.Devices.NI.DAQ;
using Sardine.Utils.Waveforms;

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
            //Blabla(dutycycle,numberofrepetitions,waveformrate,highvalue);
        }

        private void SquareOscilate(double dutycycle, double numberofrepetitions, double waveformrate, double highvalue )
        {
            DAQBoard board = DAQBoard.GetDAQInfo();

            DaqPhysicalChannelID piezoChannel = board.IO.AnalogOut[3];

            DAQControlledAODevice piezoDevice = new(board, piezoChannel, 0, 10);

            WaveformSupplier waveformSupplier = new(piezoDevice);

            SquareWaveGeneratorOptions options = new()
            {
                DutyCycle = dutycycle,
                NumberOfRepetitions = numberofrepetitions,  
                WaveformRate = waveformrate, //stim freq
                HighValue = highvalue,   //amp
            };
            waveformSupplier.SampleRate = options.WaveformRate*10;


            //ConstantValueWaveOptions options = new() { Value = 5 };

            waveformSupplier.GenerateAndSupplyWaveform(options);

            piezoDevice.OnExecutionStatusChanged += PiezoDevice_OnExecutionStatusChanged;
            piezoDevice.Start();


        }
        private void Pulse(double value)
        {
            DAQBoard board = DAQBoard.GetDAQInfo();

            DaqPhysicalChannelID piezoChannel = board.IO.AnalogOut[3];

            DAQControlledAODevice piezoDevice = new(board, piezoChannel, 0, 10);

            WaveformSupplier waveformSupplier = new(piezoDevice);

            ConstantValueWaveOptions options = new() { Value = value };

            waveformSupplier.GenerateAndSupplyWaveform(options);

            piezoDevice.OnExecutionStatusChanged += PiezoDevice_OnExecutionStatusChanged;
            piezoDevice.Start();


        }
        private static void PiezoDevice_OnExecutionStatusChanged(object? sender, Sardine.Constructs.ExecutionStatusEventArgs e) => Console.WriteLine(e.IsExecuting);
    }
}

