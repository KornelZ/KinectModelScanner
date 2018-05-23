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
        const int KINECT_DISTANCE = -600;

        public ModelMapper(Kinect kinect, ref Model model)
        {
            _kinect = kinect;
            _model = model;
        }


        public void Map()
        {
            var depthStream = _kinect.KinectSensor.DepthStream;
            _view = Matrix.CreateLookAt(new Vector3(KINECT_DISTANCE, 0, 0), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(depthStream.NominalVerticalFieldOfView), depthStream.FrameWidth / (float)depthStream.FrameHeight, 1, depthStream.TooFarDepth);
            DepthImagePixel[] depth = _kinect.Depth;
            foreach (MarkableVertex v in _model.Vertices)
            {
                Microsoft.Xna.Framework.Vector4 extended = new Microsoft.Xna.Framework.Vector4(v.Position, 0);
                Matrix tempMatrix = _projection * _view * _model.World;
                Microsoft.Xna.Framework.Vector4 ver = Microsoft.Xna.Framework.Vector4.Transform(extended, tempMatrix);
                ver = Microsoft.Xna.Framework.Vector4.Multiply(ver, (1 / ver.W));
                float currDistance = Math.Abs(Vector3.Transform(v.Position, _model.World).X - KINECT_DISTANCE);
                try
                {
                    var newDistance = depth[calculateIndex(ver.X, ver.Y)].Depth;
                    if (newDistance > 0)
                    {
                        if (currDistance < newDistance)
                        {
                            v.Marked = true;
                        }
                    }

                }
                catch (Exception e) {
                    int i = 0;
                }
            }
        }

        private int calculateIndex(float x, float y)
        {
            x = ((x + 1) / 2) * _kinect.KinectSensor.DepthStream.FrameWidth;
            y = ((y + 1) / 2) * _kinect.KinectSensor.DepthStream.FrameHeight;
            return Convert.ToInt32(y) * _kinect.KinectSensor.DepthStream.FrameWidth + Convert.ToInt32(x);
        }
    }
}
