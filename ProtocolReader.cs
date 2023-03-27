namespace PIC
{
    //Receives a protocol file in txt and has available the ISI and the List of stimuli to be ran by the execute 
    internal class ProtocolReader
    {
        private string[][]? Lines;
        private List<StimuliOptions> stimuli = new List<StimuliOptions>();
        private double[] isi;

       
        public List<StimuliOptions> Stimuli { get { return stimuli; } set => stimuli = value; }
        public double[] ISI { get => isi; set => isi = value; }
        
        
        public ProtocolReader(string FileName)
        {
            
            using TextReader reader = File.OpenText($"{FileName}.txt");
            string linha;
            int counterline = 0;

            while ((linha = reader.ReadLine()) != null)
            {
                int counteroption = 0;
                string[] bits = linha.Split(',');//Separator for parameters
                foreach (string bit in bits)
                {

                   
                    Lines[counterline][counteroption] = bit;
                   
                    counteroption++;
                }

                if (counterline == 0)
                {
                    ISI[0] = double.Parse(Lines[0][0]);
                    ISI[1] = double.Parse(Lines[0][1]);
                }
                else
                {
                    Stimuli.Add(new StimuliOptions());
                    Stimuli[counterline - 1].ModeToExecute = Lines[counterline][0];
                    Stimuli[counterline - 1].AmplitudeV = double.Parse(Lines[counterline][1]);
                    Stimuli[counterline - 1].FreqHZ = double.Parse(Lines[counterline][2]);
                    Stimuli[counterline - 1].Time_DurationS = double.Parse(Lines[counterline][3]);
                    if (Lines[counterline][0] == "Square Wave")
                    {
                        Stimuli[counterline - 1].Dutycycle = double.Parse(Lines[counterline][4]);
                    }

                }
                counterline++;
            }

        }

        public void ReadNewFile(string NewFile)
        {
            Lines = null;
            Stimuli.Clear();
            using TextReader reader = File.OpenText($"{NewFile}.txt");
            string linha;
            int counterline = 0;

            while ((linha = reader.ReadLine()) != null)
            {
                int counteroption = 0;
                string[] bits = linha.Split(',');//Separator for parameters
                foreach (string bit in bits)
                {


                    Lines[counterline][counteroption] = bit;

                    counteroption++;
                }

                if (counterline == 0)
                {
                    ISI[0] = double.Parse(Lines[0][0]);
                    ISI[1] = double.Parse(Lines[0][1]);
                }
                else
                {
                    Stimuli.Add(new StimuliOptions());
                    Stimuli[counterline - 1].ModeToExecute = Lines[counterline][0];
                    Stimuli[counterline - 1].AmplitudeV = double.Parse(Lines[counterline][1]);
                    Stimuli[counterline - 1].FreqHZ = double.Parse(Lines[counterline][2]);
                    Stimuli[counterline - 1].Time_DurationS = double.Parse(Lines[counterline][3]);
                    if (Lines[counterline][0] == "Square Wave")
                    {
                        Stimuli[counterline - 1].Dutycycle = double.Parse(Lines[counterline][4]);
                    }

                }
                counterline++;
            }
        }

    }
}
