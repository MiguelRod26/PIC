namespace PiezoController
{
    //Receives a protocol file in txt and has available the ISI and the List of stimuli to be ran by the execute 
    public static class ProtocolReader
    {
        public static StimuliProtocol ReadFile(string filename)
        {
            List<StimulusOptions> stimulusOptionsList = new();
            double fwi = 0;
            double isi = 0;

            using TextReader reader = File.OpenText($"{filename}.txt");

            string? line;
            int lineCounter = 0;
            ExecutionMode mode;
            double amplitude; double freq; double timeDuration;

            while (true)
            {
                line = reader.ReadLine();
                
                if (line is null)
                    break;

                string[] bits = line.Split(',');//Separator for parameters
                

                if (lineCounter == 0)
                {
                    fwi = double.Parse(bits[0]);
                    isi = double.Parse(bits[1]);
                }
                else
                {
                    mode = bits[0].ToExecutionMode();
                    amplitude = double.Parse(bits[1]);
                    freq = double.Parse(bits[2]);
                    timeDuration = double.Parse(bits[3]);

                    switch (mode)
                    {
                        case ExecutionMode.SquareWave:
                            stimulusOptionsList.Add(new StimulusOptions(mode, amplitude, freq, timeDuration, 
                                dutyCycle: double.Parse(bits[4])));
                            break;
                        case ExecutionMode.SineWave:
                            stimulusOptionsList.Add(new StimulusOptions(mode, amplitude, freq, timeDuration));
                            break;
                        case ExecutionMode.Pulse:
                            stimulusOptionsList.Add(new StimulusOptions(mode, amplitude, freq, timeDuration));
                            break;
                    }
                }

                lineCounter++;
            }

            return new StimuliProtocol(stimulusOptionsList, fwi, isi);
        }

    }
}
