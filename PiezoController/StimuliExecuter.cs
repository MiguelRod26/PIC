using System.Diagnostics;
using System.Timers;

namespace PiezoController
{
    public class StimuliExecuter
    {
        private readonly ManualResetEvent waitHandleTD = new(false);
        private readonly ManualResetEvent waitHandleStop = new(false);
        private readonly Piezo Com;

        private StimulusOptions? stimuli;
        private System.Timers.Timer? timer;
        private System.Timers.Timer? totaltimer;
        private double counterDutyCycle;

        public EventHandler? OnStimuliFinished;

        public StimuliExecuter (Piezo piezo)
        {
            Com = piezo;
        }


        public void RunStim(StimulusOptions stimOptions)
        {
            stimuli = stimOptions;
            counterDutyCycle = 0;
            double TPeriod = 0;
            switch (stimuli.ModeToExecute)
            {
                case ExecutionMode.SquareWave:
                    if (stimuli.Dutycycle > 50)
                    {
                        TPeriod = (1 - stimuli.Dutycycle * 0.01) / stimuli.FreqHZ;
                    }
                    else
                    {
                        TPeriod = stimuli.Dutycycle * 0.01 / (stimuli.FreqHZ);
                    }
                    break;
                case ExecutionMode.Pulse:
                    TPeriod = 1 / stimuli.FreqHZ;
                    break;
                default:
                    throw new Exception($"Erro modo inválido Runstimuli: {stimuli.ModeToExecute}");

            }
            

            timer = new System.Timers.Timer(TPeriod * 1000); // Frequency of secondS
            totaltimer = new System.Timers.Timer(stimuli.TimeDurationSeconds * 1000); //Time Duration in seconds
            timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            totaltimer.Elapsed += new ElapsedEventHandler(OnTotalTimerElapsed);

            timer.Start();
            totaltimer.Start();

            WaitHandle.WaitAny(new WaitHandle[] { waitHandleTD, waitHandleStop });


            timer.Stop();
            totaltimer.Stop();
            waitHandleStop.Reset();
            waitHandleTD.Reset();
            Com.SetVoltage(0);
            OnStimuliFinished?.Invoke(this, new EventArgs());
        }

        private void OnTotalTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            waitHandleTD.Set();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            switch (stimuli.ModeToExecute)
            {
                case ExecutionMode.SquareWave:
                    if (stimuli.Dutycycle > 50)
                    {
                        if (counterDutyCycle == (1 - stimuli.Dutycycle * 0.01) / stimuli.FreqHZ)
                        {
                            counterDutyCycle = 0;
                            Com.SetVoltage(0);
                            Debug.WriteLine(0);

                        }
                        else
                        {
                            counterDutyCycle += stimuli.Dutycycle * 0.01 / stimuli.FreqHZ;
                            Com.SetVoltage(stimuli.AmplitudeV);
                            Debug.WriteLine(stimuli.AmplitudeV);

                        }
                    }
                    else
                    {
                        if (counterDutyCycle == (1 - stimuli.Dutycycle * 0.01) / stimuli.FreqHZ)
                        {
                            counterDutyCycle = 0;
                            Com.SetVoltage(stimuli.AmplitudeV);
                            Debug.WriteLine(stimuli.AmplitudeV);

                        }
                        else
                        {
                            counterDutyCycle += stimuli.Dutycycle * 0.01 / stimuli.FreqHZ;
                            Com.SetVoltage(0);
                            Debug.WriteLine(0);

                        }
                    }
                    break;
                case ExecutionMode.SineWave:
                    break;
                case ExecutionMode.Pulse:
                    Com.SetVoltage(stimuli.AmplitudeV);
                    Com.SetVoltage(0);
                    break;
            }





        }

        public void StopStim()
        {
            waitHandleStop.Set();
        }





    }
}
