using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectModelScanner
{
    public class Model
    {
        private const float RotationAmount = MathHelper.PiOver4 / 2;
        private float _currentRotation = 0;
        public int VertexCountInLine { get; private set; }
        public List<MarkableVertex> Vertices { get; private set; }
        public List<int> Indices { get; private set; }
        public Vector3 Center { get; private set; }

        private Matrix _translation;
        public Matrix Translation { get { return _translation; } }
        private Matrix _rotation = Matrix.Identity;
        public Matrix Rotation { get { return _rotation; } }

        public Matrix World { get { return _translation * _rotation; } }
        
        public MarkableVertex this[int x, int y, int z]
        {
            get { return Vertices[Index(x, y, z)]; }
            set { Vertices[Index(x, y, z)] = value; }
        }

        private int Index(int x, int y, int z)
        {
            return VertexCountInLine * VertexCountInLine * x + VertexCountInLine * y + z;
        }

        public Model(int vertexCountInLine, float step)
        {
            float translation = (vertexCountInLine * step - 1) / 2f;
            Center = new Vector3(translation, translation, translation);
            var modelTranslation = -Center;
            Matrix.CreateTranslation(ref modelTranslation, out _translation);
            VertexCountInLine = vertexCountInLine;
            Vertices = new List<MarkableVertex>();
            for (int x = 0; x < vertexCountInLine; x++)
            {
                for(int y = 0; y < vertexCountInLine; y++)
                {
                    for(int z = 0; z < vertexCountInLine; z++)
                    {
                        Vertices.Add(new MarkableVertex()
                        {
                            Position = new Vector3(x * step, y * step, z * step),
                        });
                    }
                }
            }
        }

        public List<MarkableVertex[]> GetCubes()
        {
            var cubes = new List<MarkableVertex[]>();
            for (int x = 0; x < VertexCountInLine - 1; x++)
            {
                for (int y = 0; y < VertexCountInLine - 1; y++)
                {
                    for (int z = 0; z < VertexCountInLine - 1; z++)
                    {
                        cubes.Add(new MarkableVertex[8]
                        {
                            this[x, y, z],
                            this[x + 1, y, z],
                            this[x + 1, y, z + 1],
                            this[x, y, z + 1],
                            this[x, y + 1, z],
                            this[x + 1, y + 1, z],
                            this[x + 1, y + 1, z + 1],
                            this[x, y + 1, z + 1]
                        });
                    }
                }
            }
            return cubes;
        }

        public static List<Vector3[]> GetTrianglesFromCube(MarkableVertex[] cube)
        {
            var triangles = new List<Vector3[]>();

            AddTriangle(cube[0], cube[1], cube[2], triangles);
            AddTriangle(cube[0], cube[3], cube[2], triangles);

            AddTriangle(cube[4], cube[5], cube[6], triangles);
            AddTriangle(cube[4], cube[7], cube[6], triangles);

            AddTriangle(cube[1], cube[2], cube[6], triangles);
            AddTriangle(cube[1], cube[5], cube[6], triangles);

            AddTriangle(cube[0], cube[3], cube[7], triangles);
            AddTriangle(cube[0], cube[4], cube[7], triangles);

            AddTriangle(cube[0], cube[1], cube[5], triangles);
            AddTriangle(cube[0], cube[4], cube[5], triangles);

            AddTriangle(cube[2], cube[3], cube[7], triangles);
            AddTriangle(cube[2], cube[6], cube[7], triangles);

            return triangles;
        }

        private static void AddTriangle(MarkableVertex a, MarkableVertex b, MarkableVertex c, List<Vector3[]> triangles)
        {
            if(a.Marked && b.Marked && c.Marked)
            {
                triangles.Add(new Vector3[] { a.Position, b.Position, c.Position });
            }
        }

        public void Rotate(bool right)
        {
            float angle = RotationAmount;
            if (right)
            {
                angle = -angle;
            }
            _currentRotation += angle;
            if(Math.Abs(_currentRotation) > MathHelper.Pi * 2)
            {
                _currentRotation = 0;
            }
            _rotation = Matrix.CreateRotationY(_currentRotation);
        }
    }
}
