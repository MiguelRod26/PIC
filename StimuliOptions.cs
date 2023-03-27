namespace PIC
{
    public class StimuliOptions 
    {
        //Fields to Stimuli Options
        private double amplitudeV;
        private double freqHZ;
        private double time_durationS;
        private double dutycycle;
        private string modeToExecute;
        //Usb manager

        //Timers for Executable



        //Properties
        public double AmplitudeV
        {
            get => amplitudeV; set
            {
                amplitudeV = value;
                //OnPropertyChanged();
            }
        }
        public double FreqHZ
        {
            get => freqHZ; set
            {
                freqHZ = value;
                //OnPropertyChanged();
            }
        }
        public double Time_DurationS
        {
            get => time_durationS; set
            {
                time_durationS = value;
                //OnPropertyChanged();
            }
        }
        public double Dutycycle
        {
            get => dutycycle; set { dutycycle = value; }
        }
        public string ModeToExecute
        {
            get => modeToExecute; set
            {
                modeToExecute = value;
                //OnPropertyChanged();
            }
        }


        //End of proprieties,
        //Constructor
        public StimuliOptions(double _amplitude = 0, double _freq = 0, double _time_duration = 0)
        {
            AmplitudeV = _amplitude;
            FreqHZ = _freq;
            Time_DurationS = _time_duration;
        }

    }   
        
}
