using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

using Microsoft.Win32;

using PiezoController;

namespace PiezoControllerUI
{
    /*Next steps :
     -Plot the stimuli type; 
     -Parametros: deadtime(trial interval ISI - transitar para frames da camara)
     -Protocol Running;
    */

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double amplitude = 5;
        private double frequency = 2;
        private int repetitions = 2;
        private double dutyCycle = 50;
        private ExecutionMode mode = ExecutionMode.SquareWave;
        private bool isRunning;
        private bool automaticMode;
        private Piezo? Piezo { get; set; }
        public StimuliExecuter? StimulusExecuter { get; private set; }
        public ProtocolManager? ProtocolManager { get; private set; }
        public StimuliProtocol? Protocol { get; set; }

        private string filename = string.Empty;
        private SerialPortName? selectedComPort;

        public event PropertyChangedEventHandler? PropertyChanged;


        // Amp in V
        // Freq in Hz
        // N of Repetitions of the stim type
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
        public int Repetitions
        {
            get => repetitions; set
            {
                repetitions = value;
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

        public SerialPortName? SelectedComPort
        {
            get => selectedComPort; set
            {
                selectedComPort = value;
                OnPropertyChanged();
            }
        }



        public HamamatsuCamera.HamamatsuCamera Camera { get; }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            PopulateUsbPorts();
            Camera = new HamamatsuCamera.HamamatsuCamera();
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
            if (SelectedComPort is null)
            {
                MessageBox.Show("Choose a COM Port");
                return;
            }


            if (Piezo is not null)
                Piezo.Dispose();

            Piezo = new(SelectedComPort.PortID);
            StimulusExecuter = new StimuliExecuter(Piezo);
            StimulusExecuter.OnStimulusFinished += StimulusExecuter_OnStimulusFinished;
            ProtocolManager = new ProtocolManager(StimulusExecuter, Camera);
            ProtocolManager.OnThreadFinish += ProtocolManager_OnThreadFinish;
        }

        private void ProtocolManager_OnThreadFinish(object? sender, EventArgs e)
        {
            IsRunning = false;
        }

        private void StimulusExecuter_OnStimulusFinished(object? sender, EventArgs e)
        {
            if (!AutomaticMode)
                IsRunning = false;
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            //If the run protocol option is selected we wait for an event to happen from the protocol manager to change the button to stop
            if (!AutomaticMode) // manual
            {
                if (!IsRunning)
                {
                    if (StimulusExecuter is not null)
                    {
                        StimulusExecuter.StartStim(new StimulusOptions(Mode, Amplitude, Frequency, Repetitions, DutyCycle));
                        IsRunning = true;
                    }
                }
                else
                {
                    StimulusExecuter?.StopStim();
                }
            }
            else // automatic
            {
                if (!IsRunning)
                {
                    if (Protocol is not null && ProtocolManager is not null)
                    {
                        ProtocolManager.StartProtocol(Protocol);
                        IsRunning = true;
                    }
                }
                else
                {
                    ProtocolManager?.StopProtocol();
                }
            }
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
                Protocol = ProtocolReader.ReadFile(Filename);
            }
        }





        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
