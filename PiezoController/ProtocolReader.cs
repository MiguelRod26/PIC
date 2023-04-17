namespace PiezoController
{
    //Receives a protocol file in txt and has available the ISI and the List of stimuli to be ran by the execute 
    //
    // PROTOCOL TEMPLATE
    // fwi,isi
    // # Comments
    // mode,amplitude,freq,numberofrepetitions,(...)
    // ""
    //

    public static class ProtocolReader
    {
        public static StimuliProtocol ReadFile(string filename)
        {
            List<StimulusOptions> stimulusOptionsList = new();
            int fwi = 0;
            int isi = 0;

            using TextReader reader = File.OpenText(filename);

            string? line;
            int lineCounter = 0;
            ExecutionMode mode;
            double amplitude; double freq; int numberofrepetitions;

            while (true)
            {
                line = reader.ReadLine();
                
                if (line is null)
                    break;

                if (line[0] == '#')
                    continue;

                string[] bits = line.Split(',');//Separator for parameters
                

                if (lineCounter == 0)
                {
                    fwi = int.Parse(bits[0]);
                    isi = int.Parse(bits[1]);
                }
                else
                {
                    mode = bits[0].ToExecutionMode();
                    amplitude = double.Parse(bits[1]);
                    freq = double.Parse(bits[2]);
                    numberofrepetitions = int.Parse(bits[3]);

                    switch (mode)
                    {
                        case ExecutionMode.SquareWave:
                            stimulusOptionsList.Add(new StimulusOptions(mode, amplitude, freq, numberofrepetitions, 
                                dutyCycle: double.Parse(bits[4])));
                            break;
                        case ExecutionMode.SineWave:
                            stimulusOptionsList.Add(new StimulusOptions(mode, amplitude, freq, numberofrepetitions));
                            break;
                        case ExecutionMode.Pulse:
                            stimulusOptionsList.Add(new StimulusOptions(mode, amplitude, freq, numberofrepetitions));
                            break;
                    }
                }

                lineCounter++;
            }

            return new StimuliProtocol(stimulusOptionsList, fwi, isi);
        }

    }
}
