using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

//using HamamatsuCamera;

using PiezoController;

namespace PiezoController
{
    public class ProtocolManager
    {
        private StimuliExecuter Executer { get; set; }
        private ICamera Camera { get; set; }
        public event EventHandler? OnThreadFinish;
        public AutoResetEvent cameraFrameTriggerEvent = new(false);

        Thread? protocolThread;
        CancellationTokenSource? tokenSource;

        public int CurrentFrame { get; private set; } = 0;

        private int NextCameraFrameForTrigger { get; set; } = -1;

        public ProtocolManager(StimuliExecuter executer)
        {
            Executer = executer;
           // Camera = camera;
            //Camera.OnCameraFrame += Camera_OnCameraFrame;
        }

        private void Camera_OnCameraFrame(object? sender, CameraFrameEventArgs e)
        {
            CurrentFrame = e.FrameNumber;


            if (e.FrameNumber == NextCameraFrameForTrigger)
                cameraFrameTriggerEvent.Set();
        }
        

        public void StartProtocol(StimuliProtocol protocol)
        {
            CurrentFrame = 0;

            tokenSource = new CancellationTokenSource();

            protocolThread = new Thread(new ParameterizedThreadStart(
                (ct) =>
                {
                    CancellationToken token = (CancellationToken)ct!;
                    RunProtocol(protocol, token);
                }
                ));
                
            protocolThread.Start(tokenSource.Token);
        }

        public void StopProtocol()
        {
            tokenSource?.Cancel();
        }

        readonly System.Timers.Timer timer = new();

        private void RunProtocol(StimuliProtocol protocol, CancellationToken token)
        {
            NextCameraFrameForTrigger = protocol.FirstWaitingInterval;


            WaitHandle.WaitAny(new WaitHandle[2] { cameraFrameTriggerEvent, token.WaitHandle});

            

            if (token.IsCancellationRequested)
            {
                OnThreadFinish?.Invoke(this, new EventArgs());
                return;
            }

            


            foreach (StimulusOptions stim_ in protocol.StimulusOptionsList)
            {
                NextCameraFrameForTrigger += protocol.ISI;
                
                Executer.StartStim(stim_);
                
                WaitHandle.WaitAny(new WaitHandle[2] { cameraFrameTriggerEvent, token.WaitHandle});
                
                if (token.IsCancellationRequested)
                {
                    Executer.StopStim();
                    OnThreadFinish?.Invoke(this, new EventArgs());
                    return;
                }
            }

            OnThreadFinish?.Invoke(this, new EventArgs());
        }
    }
}
