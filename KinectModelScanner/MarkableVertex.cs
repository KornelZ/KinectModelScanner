using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectModelScanner
{
    public class MarkableVertex
    {
        public Vector3 Position { get; set; }
        public bool Marked { get; set; }
        public float IsoLevel { get; set; }

        public Vector3 ProjectPosition(Matrix worldViewProjection)
        {
            return Vector3.Transform(Position, worldViewProjection);
        }
    }
}
