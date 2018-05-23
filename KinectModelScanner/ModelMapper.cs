using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

        public ModelMapper(Kinect kinect, ref Model model)
        {
            _kinect = kinect;
            _model = model;
        }


        public void Map()
        {
            var depthStream = _kinect.KinectSensor.DepthStream;
            _view = Matrix.CreateLookAt(new Vector3(-10, 0, 0), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(depthStream.NominalVerticalFieldOfView), depthStream.FrameWidth / (float)depthStream.FrameHeight, 1, depthStream.TooFarDepth);
            DepthImagePixel[] depth = _kinect.Depth;
            foreach (MarkableVertex v in _model.Vertices)
            {
                Matrix tempMatrix = _projection * _view * _model.World;
                var ver = Vector3.Transform(v.Position, tempMatrix);
                float currDistance = Math.Abs(Vector3.Transform(v.Position, _model.World).X + 10);
                try
                {
                    var newDistance = Math.Abs(depth[calculateIndex(ver.X, ver.Y)].Depth);
                    if (currDistance < newDistance)
                    {
                        v.Marked = true;
                    }

                }
                catch (Exception e) {
                    int i = 0;
                }
            }
        }

        private int calculateIndex(float x, float y)
        {
            return Convert.ToInt32(y) * _kinect.KinectSensor.DepthStream.FrameWidth + Convert.ToInt32(x);
        }
    }
}
