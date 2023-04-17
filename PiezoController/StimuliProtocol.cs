namespace PiezoController
{
    public class StimuliProtocol
    {
        public List<StimulusOptions> StimulusOptionsList { get; }
        public int FirstWaitingInterval { get; }
        public int ISI { get; }

        public StimuliProtocol(List<StimulusOptions> stimOptions, int fwi, int isi)
        {
            StimulusOptionsList = stimOptions;
            FirstWaitingInterval = fwi;
            ISI = isi;
        }

    }
}
