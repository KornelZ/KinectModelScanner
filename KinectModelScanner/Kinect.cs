using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectModelScanner
{
    public class Kinect
    {
        private KinectSensor _kinectSensor;
        public DepthImagePixel[] Depth { get; set; }
        public Kinect()
        {
            foreach(var sensor in KinectSensor.KinectSensors)
            {
                if(sensor.Status == KinectStatus.Connected)
                {
                    _kinectSensor = sensor;
                    break;
                }
            }
            if(_kinectSensor != null)
            {
                _kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                Depth = new DepthImagePixel[_kinectSensor.DepthStream.FramePixelDataLength];
                _kinectSensor.DepthFrameReady += OnDepthImageReady;
                _kinectSensor.Start();
            }
        }

        private void OnDepthImageReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var frame = e.OpenDepthImageFrame())
            {
                frame?.CopyDepthImagePixelDataTo(Depth);
            }
        }
    }
}
