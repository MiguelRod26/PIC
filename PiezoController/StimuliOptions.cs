namespace PiezoController
{
    public class StimulusOptions
    {
        public double AmplitudeV { get;  }
        public double FreqHZ { get;  }
        public double TimeDurationSeconds { get;  }
        public double Dutycycle { get; }
        public ExecutionMode ModeToExecute { get; }

        public StimulusOptions(ExecutionMode modeToExecute, double amplitude, double freq, double timeDuration, double dutyCycle = 50)
        {
            AmplitudeV = amplitude;
            FreqHZ = freq;
            TimeDurationSeconds = timeDuration;
            Dutycycle = dutyCycle;
            ModeToExecute = modeToExecute;
        }
    }   
}
