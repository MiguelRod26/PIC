using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

using Microsoft.Win32;

using PiezoController;

namespace PiezoControllerUI
{
    /*Next steps :
     -Null warnings; 
     -Plot the stimuli type;
     -If the button is running cant change stats;
 
     -Parametros: deadtime(trial interval ISI - transitar para frames da camara); dutycycle (square wave uptime%) DONE
     -Protocol Running;
    */

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double amplitude = 5;
        private double frequency = 2;
        private double duration = 5;
        private double dutyCycle = 50;
        private ExecutionMode mode = ExecutionMode.SquareWave;
        private bool isRunning;
        private bool automaticMode;

        Piezo? piezo;
        public StimulusOptions stimuli;
        public StimuliExecuter StimulusExecuter;
        public ProtocolManager Protocol;
        private Thread stimuliThread;
        private string filename;
        private SerialPortName selectedComPort;

        public event PropertyChangedEventHandler? PropertyChanged;


        // Amp in V
        // Freq in Hz
        // Duration in s
        // DutyCycle in %
        public double Amplitude
        {
            get => amplitude;
            set
            {
                amplitude = value;
                OnPropertyChanged();
            }
        }

        public string Filename
        {
            get => filename; set
            {
                filename = value;
                OnPropertyChanged();
            }
        }

        public double Frequency
        {
            get => frequency; set
            {
                frequency = value;
                OnPropertyChanged();
            }
        }
        public double Duration
        {
            get => duration; set
            {
                duration = value;
                OnPropertyChanged();
            }
        }
        public double DutyCycle
        {
            get => dutyCycle; set
            {
                if (value <= 0)
                {
                    value = 0;
                }
                if (value >= 100)
                {
                    value = 100;
                }

                dutyCycle = value;
                OnPropertyChanged();
            }
        }

        public ExecutionMode Mode
        {
            get => mode; set
            {
                mode = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get => isRunning; set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }

        public bool AutomaticMode
        {
            get => automaticMode;
            set
            {
                automaticMode = value;
                OnPropertyChanged();
            }
        }

        public List<SerialPortName> SerialPortNames { get; private set; } = new List<SerialPortName>();
        public List<ExecutionMode> ExecutionModes { get; } = new List<ExecutionMode>(Enum.GetValues(typeof(ExecutionMode)).Cast<ExecutionMode>());

        public SerialPortName SelectedComPort
        {
            get => selectedComPort; set
            {
                selectedComPort = value;
                OnPropertyChanged();
            }
        }



        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            PopulateUsbPorts();

            //stimuli = new StimulusOptions(0, 0, 0);
            //StimulusExecuter = new StimuliExecuter(stimuli);
            //StimulusExecuter.OnStimuliFinished += OnStimThreadFinished;

        }

        private void PopulateUsbPorts()
        {
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                int id = int.Parse(Regex.Match(portName, "[0-9]+").Value);
                SerialPortNames.Add(new SerialPortName(id, ""));
            }
        }


        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (piezo is not null)
                piezo.Dispose();

            piezo = new(SelectedComPort.PortID);
            StimulusExecuter = new StimuliExecuter(piezo);
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            //If the run protocol option is selected we wait for an event to happen from the protocol manager to change the button to stop
            if (Protocol != null)
            {
                if (!AutomaticMode)
                {
                    if (!IsRunning)
                    {
                        stimuliThread = new Thread(() => StimulusExecuter.RunStim(stimuli));
                        stimuliThread.Start();

                        IsRunning = true;
                    }
                    else
                    {

                        StimulusExecuter.StopStim();
                        stimuliThread.Join(); // Wait for thread to exit
                        IsRunning = false;
                    }
                }
                else
                {
                    if (!IsRunning)
                    {
                        stimuliThread = new Thread(() => Protocol.RunProtocol());
                        stimuliThread.Start();

                        IsRunning = true;
                    }
                    else
                    {

                        StimulusExecuter.StopStim();
                        stimuliThread.Join(); // wait for thread to exit
                        IsRunning = false;
                    }
                }
            }
        }


        private void OnStimThreadFinished(object? sender, EventArgs e)
        {
            IsRunning = false;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                // Set filter for file extension and default file extension
                DefaultExt = ".txt",
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            // Display OpenFileDialog by calling ShowDialog method
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                Filename = openFileDialog.FileName;

                //Protocol = new(fileName);

            }
        }





        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
