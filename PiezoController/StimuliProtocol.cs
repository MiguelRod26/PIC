namespace PiezoController
{
    public class StimuliProtocol
    {
        public List<StimulusOptions> StimulusOptionsList { get; }
        public double FirstWaitingInterval { get; }
        public double ISI { get; }

        public StimuliProtocol(List<StimulusOptions> stimOptions, double fwi, double isi)
        {
            StimulusOptionsList = stimOptions;
            FirstWaitingInterval = fwi;
            ISI = isi;
        }

    }
}
