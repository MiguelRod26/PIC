using System.Diagnostics;
using System.Timers;

namespace PiezoController
{
    public class PiezoStim
    {
        private StimuliOptions stimuli;
        private System.Timers.Timer? timer;
        private System.Timers.Timer? totaltimer;
        public COM Com;
        //Event handlers for executable
        public double counter_dutycycle;
        public EventHandler OnThreadFinish;
        private ManualResetEvent waitHandleTD = new ManualResetEvent(false);
        private ManualResetEvent waitHandleStop = new ManualResetEvent(false);
        public StimuliOptions Stimuli { get => stimuli; set => stimuli = value; }

        public PiezoStim (StimuliOptions _)
        {
            Stimuli = _;
        }
        public void RunStim(StimuliOptions _stimuli)
        {
            Stimuli = _stimuli;
            counter_dutycycle = 0;
            double TPeriod = 0;
            switch (Stimuli.ModeToExecute)
            {
                case "Square Wave":
                    if (Stimuli.Dutycycle > 50)
                    {
                        TPeriod = (1 - Stimuli.Dutycycle * 0.01) / Stimuli.FreqHZ;
                    }
                    else
                    {
                        TPeriod = Stimuli.Dutycycle * 0.01 / (Stimuli.FreqHZ);
                    }
                    break;
                case "Pulse":
                    TPeriod = 1 / Stimuli.FreqHZ;
                    break;
                default:
                    throw new Exception($"Erro modo inválido Runstimuli: {Stimuli.ModeToExecute}");

            }

            timer = new System.Timers.Timer(TPeriod * 1000); // Frequency of secondS
            totaltimer = new System.Timers.Timer(Stimuli.Time_DurationS * 1000); //Time Duration in seconds
            timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            totaltimer.Elapsed += new ElapsedEventHandler(OnTotalTimerElapsed);

            timer.Start();
            totaltimer.Start();

            WaitHandle.WaitAny(new WaitHandle[] { waitHandleTD, waitHandleStop });


            timer.Stop();
            totaltimer.Stop();
            waitHandleStop.Reset();
            waitHandleTD.Reset();
            Com.Write(0);
            OnThreadFinish?.Invoke(this, new EventArgs());
        }

        private void OnTotalTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            waitHandleTD.Set();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            switch (Stimuli.ModeToExecute)
            {
                case "Square Wave":

                    if (Stimuli.Dutycycle > 50)
                    {
                        if (counter_dutycycle == (1 - Stimuli.Dutycycle * 0.01) / Stimuli.FreqHZ)
                        {
                            counter_dutycycle = 0;
                            Com.Write(0);
                            Debug.WriteLine(0);

                        }
                        else
                        {
                            counter_dutycycle += Stimuli.Dutycycle * 0.01 / Stimuli.FreqHZ;
                            Com.Write(Stimuli.AmplitudeV);
                            Debug.WriteLine(Stimuli.AmplitudeV);

                        }
                    }
                    else
                    {
                        if (counter_dutycycle == (1 - Stimuli.Dutycycle * 0.01) / Stimuli.FreqHZ)
                        {
                            counter_dutycycle = 0;
                            Com.Write(Stimuli.AmplitudeV);
                            Debug.WriteLine(Stimuli.AmplitudeV);

                        }
                        else
                        {
                            counter_dutycycle += Stimuli.Dutycycle * 0.01 / Stimuli.FreqHZ;
                            Com.Write(0);
                            Debug.WriteLine(0);

                        }
                    }
                    break;
                case "Sine Wave":
                    //Debug.Write("SineW\n");

                    break;
                case "Pulse":
                    Com.Write(Stimuli.AmplitudeV);
                    Com.Write(0);
                    break;
                default:
                    break;
            }
        }

        public void StopStim()
        {
            waitHandleStop.Set();
        }





    }
}
