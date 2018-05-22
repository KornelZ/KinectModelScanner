using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace KinectModelScanner
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<VertexPositionColor> _toDraw = new List<VertexPositionColor>();
        Kinect _kinect;
        int _vertexCountInLine;
        float _vertexStep;
        Model _model;

        Matrix _world, _view, _projection;
        Vector3 _cameraPos;
        Vector3 _cameraTarget;
        Vector3 _modelTranslation;
        float _rotation = MathHelper.PiOver4 / 2;


        KeyboardState _oldKeyboardState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _kinect = new Kinect();
            _vertexCountInLine = 4;
            _vertexStep = 1f;
            float translation = -(_vertexCountInLine * _vertexStep - 1) / 2f;
            _modelTranslation = new Vector3(translation, translation, translation);
            Matrix.CreateTranslation(ref _modelTranslation, out _world);
            _cameraPos = new Vector3(-10, 0, 0);
            _cameraTarget = Vector3.Zero;
            _view = Matrix.CreateLookAt(_cameraPos, _cameraTarget, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4 / 2, Window.ClientBounds.Width / (float)Window.ClientBounds.Height, 1, 100);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InitModel();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        private void InitModel()
        { 
            _model = new Model(_vertexCountInLine, _vertexStep);
            _model[1, 2, 1].Marked = _model[1, 2, 2].Marked =
                _model[1, 1, 1].Marked = _model[1, 1, 2].Marked = _model[2, 2, 2].Marked = true;
        }

        private void SetTrianglesToDraw()
        {
            var triangles = new List<Vector3[]>();
            foreach (var cube in _model.GetCubes())
            {
                triangles.AddRange(Model.GetTrianglesFromCube(cube));
            }
            int i = 0;
            foreach (var t in triangles)
            {
                foreach (var p in t)
                {
                    Color c = Color.Red;
                    if (i % 3 == 0)
                    {
                        c = Color.Blue;
                    }
                    else if (i % 3 == 1)
                    {
                        c = Color.Green;
                    }
                    _toDraw.Add(new VertexPositionColor()
                    {
                        Position = p,
                        Color = c
                    });
                    i++;
                }
            }
        }
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if(keyboardState.IsKeyDown(Keys.Left) && _oldKeyboardState.IsKeyUp(Keys.Left))
            {
                RotateCamera(left: true);
            }
            else if(keyboardState.IsKeyDown(Keys.Right) && _oldKeyboardState.IsKeyUp(Keys.Right))
            {
                RotateCamera(left: false);
            }
            _oldKeyboardState = keyboardState;
            SetTrianglesToDraw();
            base.Update(gameTime);
        }
        private void RotateCamera(bool left)
        {
            float angle = _rotation;
            if(left)
            {
                angle = -angle;
            }
            _cameraPos = Vector3.Transform(_cameraPos - _cameraTarget, 
                Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), angle));
            _view = Matrix.CreateLookAt(_cameraPos, _cameraTarget, new Vector3(0, 1, 0));
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            BasicEffect e = new BasicEffect(GraphicsDevice)
            {
                World = _world,
                View = _view,
                Projection = _projection,
                VertexColorEnabled = true,
                LightingEnabled = false
            };
            var rasterizer = new RasterizerState();
            rasterizer.FillMode = FillMode.Solid;
            rasterizer.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizer;
            foreach(var pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (_toDraw.Count != 0)
                {
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        _toDraw.ToArray(), 0, _toDraw.Count / 3);
                }
            }
            base.Draw(gameTime);
        }

        private Matrix GetWorldViewProjection()
        {
            return _projection * _view * _world;
        }
    }
}
