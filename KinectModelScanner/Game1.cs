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
        ModelMapper _modelMapper;
        int _vertexCountInLine;
        float _vertexStep;
        Model _model;

        Matrix _view, _projection;
        Vector3 _cameraPos;
        Vector3 _cameraTarget;

        KeyboardState _oldKeyboardState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _kinect = new Kinect();
            _vertexCountInLine = 20;
            _vertexStep = 20f;
            _cameraPos = new Vector3(-600, 0, 0);
            _cameraTarget = Vector3.Zero;
            _view = Matrix.CreateLookAt(_cameraPos, _cameraTarget, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Window.ClientBounds.Width / (float)Window.ClientBounds.Height, 1, 1500);
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
            _modelMapper = new ModelMapper(_kinect, ref _model);
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
        }

        private void SetTrianglesToDraw()
        {
            var triangles = new List<Vector3[]>();
            var marchingCubes = new MarchingCubes();
            foreach (var cube in _model.GetCubes())
            {
                triangles.AddRange(marchingCubes.MarchCube(cube));
            }
            int i = 0;
            _toDraw.Clear();
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
                _model.Rotate(right: false);
            }
            else if(keyboardState.IsKeyDown(Keys.Right) && _oldKeyboardState.IsKeyUp(Keys.Right))
            {
                _model.Rotate(right: true);
            }
            else if(keyboardState.IsKeyDown(Keys.S) && _oldKeyboardState.IsKeyUp(Keys.S))
            {
                _modelMapper.Map();
            }
            _oldKeyboardState = keyboardState;
            SetTrianglesToDraw();
            base.Update(gameTime);
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
                World = _model.World,
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
    }
}
