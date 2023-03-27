using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PIC;

namespace WPF__PIC
{
    /*Next steps :
     -Null warnings; 
     -Plot the stimuli type;
     -If the button is running cant change stats;
 
     -Parametros: deadtime(trial interval ISI - transitar para frames da camara); dutycycle (square wave uptime%) DONE
     -Protocol Running;
    */

    public partial class MainWindow : Window
    {
        public StimuliOptions stimuli;
        public PiezoStim Piezo;
        public ProtocolManager Protocol;
        private Thread stimuliThread;
        private bool isRunning = false;
        private bool ProtocolOption = false;
        public MainWindow()
        {
            InitializeComponent();
            PopulateUsbPorts();
            stimuli = new StimuliOptions(0, 0, 0);
            Piezo = new PiezoStim(stimuli);
            Piezo.OnThreadFinish += OnStimThreadFinished;
            //Initial Conditions
            TypeStimuliCB.SelectedIndex = 0; //Mode
            //USBPortsCB.SelectedIndex = 0; //USB Port
            AmpTextBox.Text = "5"; //Amp in V
            FreqTextBox.Text = "2"; //Freq in Hz
            DurationTextBox.Text = "5"; //Duration in s
            DutyCycleTextBox.Text = "50"; //DutyCycle in %
        }

        private void PopulateUsbPorts()
        {
            USBPortsCB.Items.Clear();
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                USBPortsCB.Items.Add(portName);
            }
        }
        private void USBPortsCB_DropDownClosed(object sender, EventArgs e)
        {
            Piezo.Com.Open(USBPortsCB.Text);
        }
     
        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            //If the run protocol option is selected we wait for an event to happen from the protocol manager to change the button to stop
            if (Protocol != null)
            {
                if (!ProtocolOption)
                {
                    if (!isRunning)
                    {
                        stimuliThread = new Thread(() => Piezo.RunStim(stimuli));
                        stimuliThread.Start();

                        isRunning = true;
                        StartStopButton.Content = "Stop";
                    }
                    else
                    {

                        Piezo.StopStim();
                        stimuliThread.Join(); // Wait for thread to exit
                        ChangeStatusToStopped();
                    }
                }
                else
                {
                    if (!isRunning)
                    {
                        stimuliThread = new Thread(() => Protocol.RunProtocol());
                        stimuliThread.Start();

                        isRunning = true;
                        StartStopButton.Content = "Stop";
                    }
                    else
                    {

                        Piezo.StopStim();
                        stimuliThread.Join(); // wait for thread to exit
                        ChangeStatusToStopped();
                    }
                }
            }
        }
       
    
        private void OnStimThreadFinished(object? sender, EventArgs e)
        {
            ChangeStatusToStopped();
        }
        private void ChangeStatusToStopped()
        {
            isRunning = false;
            _ = Dispatcher.BeginInvoke(() => StartStopButton.Content = "Start");
        }


        private void TypeStimuliCB_DropDownClosed(object sender, EventArgs e)
        {
            stimuli.ModeToExecute = TypeStimuliCB.Text;
        }
        private void Duration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(DurationTextBox.Text, out double value))
            {
                stimuli.Time_DurationS = value;
            }
        }

        private void AmpTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(AmpTextBox.Text, out double value))
            {
                stimuli.AmplitudeV = value;
            }
        }

        private void FreqTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            if (double.TryParse(FreqTextBox.Text, out double value))
            {
                stimuli.FreqHZ = value;
            }
        }
        private void DutyCycle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(DutyCycleTextBox.Text, out double value))
            {
                //Sets the value of the dutycycle between 0 and 100
                if (value <= 0)
                {
                    value = 0;
                }
                if (value >= 100)
                {
                    value = 100;
                }
                stimuli.Dutycycle = value;

            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter for file extension and default file extension
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                SelectedFileLabel.Content = fileName;
                Protocol = new(fileName);
                
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == RadioButtonManual)
            {
                // Enable controls for Option 1
                TypeStimuliCB.IsEnabled = true;
                DurationTextBox.IsEnabled = true;
                AmpTextBox.IsEnabled = true;
                FreqTextBox.IsEnabled = true;

                //Disable controls for Option 1
                SelectFileButton.IsEnabled = false;

                ProtocolOption = false;
   
            }
            else if (sender == RadioButtonProtocol)
            {
                // Enable controls for Option 2
                SelectFileButton.IsEnabled = true;

                // Disable controls for Option 1
                TypeStimuliCB.IsEnabled = false;
                DurationTextBox.IsEnabled = false;
                AmpTextBox.IsEnabled = false;
                FreqTextBox.IsEnabled = false;

                ProtocolOption = true;
            }
        }
    }
}
