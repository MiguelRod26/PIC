using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiezoController
{
    public interface ICamera
    {
        public event EventHandler<CameraFrameEventArgs> OnCameraFrame;
    }

    public class CameraFrameEventArgs : EventArgs
    {
        public int FrameNumber { get; }

        public CameraFrameEventArgs(int frameNumber)
        {
            FrameNumber = frameNumber;
        }
    }
}
