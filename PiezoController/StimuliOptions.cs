namespace PiezoController
{
    public class StimulusOptions
    {
        public double AmplitudeV { get;  }
        public double FreqHZ { get;  }
        public int NumberRepetitions { get;  }
        public double Dutycycle { get; }
        public ExecutionMode ModeToExecute { get; }

        public StimulusOptions(ExecutionMode modeToExecute, double amplitude, double freq, int numberofrepetitions, double dutyCycle = 50)
        {
            AmplitudeV = amplitude;
            FreqHZ = freq;
            NumberRepetitions = numberofrepetitions;
            Dutycycle = dutyCycle;
            ModeToExecute = modeToExecute;
        }
    }   
}
