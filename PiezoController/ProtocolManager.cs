using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using PiezoController;

namespace PiezoController
{
    public class ProtocolManager
    {
        //private ProtocolReader Protocol;
        private StimuliExecuter Piezo;
        private StimulusOptions _; //pode ser desnecessário
        public EventHandler OnThreadFinish;

        public ProtocolManager(string _Protocol)
        {
            //Protocol = new(_Protocol);
           // Piezo = new StimuliExecuter(_);
        }
        public void RunProtocol()
        {
           // Thread.Sleep((int) (Protocol.ISI[0] * 1000));
           // foreach (StimulusOptions stim_ in Protocol.Stimuli)
            {
         //       Piezo.RunStim(stim_);
          //      Thread.Sleep((int) (Protocol.ISI[1]* 1000));
            }
            OnThreadFinish?.Invoke(this, new EventArgs());
        }
        
    }
}
