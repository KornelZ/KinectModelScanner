using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectModelScanner
{
    public class ModelMapper
    {
        Kinect _kinect;
        Model _model;
        Matrix _view, _projection;
        const int DistanceFromKinect = -500;

        public ModelMapper(Kinect kinect, ref Model model)
        {
            _kinect = kinect;
            _model = model;
        }


        public void Map()
        {
            var depthStream = _kinect.KinectSensor.DepthStream;
            _view = Matrix.CreateLookAt(new Vector3(DistanceFromKinect, 0, 0), Vector3.Zero, Vector3.Up);
            var fov = MathHelper.ToRadians(depthStream.NominalDiagonalFieldOfView);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(depthStream.NominalVerticalFieldOfView), 
                depthStream.FrameWidth / (float)depthStream.FrameHeight, 0.0001f, depthStream.TooFarDepth);
            DepthImagePixel[] depth = _kinect.Depth;
            foreach (MarkableVertex v in _model.Vertices)
            {
                Matrix tempMatrix = _projection * _view * _model.World;
                var position = Microsoft.Xna.Framework.Vector4.Transform(new Microsoft.Xna.Framework.Vector4(v.Position, 1), tempMatrix);
                float camDepth = Vector3.Transform(v.Position, _model.World).X - DistanceFromKinect;
                var kinectDepth = depth[CalculateIndex(position, depthStream)];
                //Trace.WriteLineIf(kinectDepth.IsKnownDepth, string.Format("Kin:{0}  Cam:{1}, vec: {2}", kinectDepth.Depth, camDepth, v.Position));
                if (kinectDepth.IsKnownDepth && camDepth < kinectDepth.Depth)
                {
                    if(!v.Marked)
                    {
                        Trace.WriteLine("Marked");
                    }
                    v.Marked = true;
                }
            }
            Trace.WriteLine("Scanned");
        }

        private int CalculateIndex(Microsoft.Xna.Framework.Vector4 position, DepthImageStream depth)
        {
            float x = -position.X / position.W, y = -position.Y / position.W;
            x = (x + 1) * depth.FrameWidth / 2;
            y = (y + 1) * depth.FrameHeight / 2;

            int xCoord = (int)MathHelper.Clamp(x, 0, depth.FrameWidth - 1);
            int yCoord = (int)MathHelper.Clamp(y, 0, depth.FrameHeight - 1);

            return yCoord * depth.FrameHeight + xCoord;
        }
    }
}
