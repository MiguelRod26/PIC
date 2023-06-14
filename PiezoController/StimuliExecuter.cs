using System.Diagnostics;
using System.Timers;

namespace PiezoController
{
    public class StimuliExecuter
    {
        private readonly ManualResetEvent waitHandleStop = new(false);
        private readonly Piezo Com;

        private StimulusOptions? stimulus;
        private System.Timers.Timer? ptimer;
        private System.Timers.Timer? dutycicletimer;
        private int repcounter = 0;

        public event EventHandler? OnStimulusFinished;

        Thread? stimExecutionThread;


        public StimuliExecuter (Piezo piezo)
        {
            Com = piezo;
        }


        private void RunStim(StimulusOptions stimOptions)
        {
            stimulus = stimOptions;
            //repcounter = 0;
            //double TPeriod = 0;
            //TPeriod = 1/stimulus.FreqHZ;


            //ptimer = new System.Timers.Timer(TPeriod * 1000); // Frequency of secondS
            //dutycicletimer = new System.Timers.Timer(stimulus.Dutycycle*0.01*TPeriod*1000); // Frequency of secondS

            //ptimer.Elapsed += new ElapsedEventHandler(OnPTimerElapsed);
            //dutycicletimer.Elapsed += new ElapsedEventHandler(OnDutyCicleTimerElapsed);

            //ptimer.Start();
            //dutycicletimer.Start();

            //Com.SetVoltage(stimulus.AmplitudeV);
            //Debug.WriteLine(stimulus.AmplitudeV);
            switch (stimulus!.ModeToExecute)
            {
                case ExecutionMode.SquareWave:

                    Com.SquareOscilate(stimulus.Dutycycle, stimulus.NumberRepetitions, stimulus.FreqHZ, stimulus.AmplitudeV);
                    StopStim();
                    break;
                case ExecutionMode.SineWave:
                    break;
                case ExecutionMode.Pulse:
                    Com.Pulse(stimulus.AmplitudeV);
                    StopStim();
                    break;
            }
            
            WaitHandle.WaitAny(new WaitHandle[] { waitHandleStop });

            //ptimer.Stop();
            //dutycicletimer.Stop();
            Com.SetVoltage(0);
            waitHandleStop.Reset();
            
            OnStimulusFinished?.Invoke(this, new EventArgs());
        }


        public void StartStim(StimulusOptions stimOptions)
        {
            waitHandleStop.Reset();
            stimExecutionThread = new Thread(() => RunStim(stimOptions));
            stimExecutionThread.Start();
        }

        public void StopStim()
        {
            waitHandleStop.Set();
        }
        //private void OnPTimerElapsed(object? sender, ElapsedEventArgs e)
        //{
        //    if (repcounter != (stimulus.NumberRepetitions - 1))
        //    {
        //        repcounter++;
        //        switch (stimulus!.ModeToExecute)
        //        {
        //            case ExecutionMode.SquareWave:

        //                Com.SetVoltage(stimulus.AmplitudeV);
        //                Debug.WriteLine(stimulus.AmplitudeV);
        //                dutycicletimer.Start();


        //                break;
        //            case ExecutionMode.SineWave:
        //                break;
        //            case ExecutionMode.Pulse:
        //                Com.SetVoltage(stimulus.AmplitudeV);
        //                Com.SetVoltage(0);
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        StopStim();
        //    }
           



        //}
        //private void OnDutyCicleTimerElapsed(object? sender, ElapsedEventArgs e)
        //{
        //    dutycicletimer.Stop();
        //    Com.SetVoltage(0);
        //    Debug.WriteLine(0);
        //}



    }
}
