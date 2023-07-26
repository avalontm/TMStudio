using AvaloniaInside.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMapEditor.Engine
{
    public class MapEngine : Game
    {
        GraphicsDeviceManager GraphicsDeviceManager { get; }

        private Matrix _world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private readonly Matrix _view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private readonly Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        private SpriteBatch _spriteBatch;
        private ResolutionRenderer _res;

        private int _lastWidth, _lastHeight;

        public Vector3 DiffuseColor { get; set; } = new(1f, 0.2f, 0.2f);
        public Vector3 SpecularColor { get; set; } = new(0, 1, 0);
        public Vector3 AmbientLightColor { get; set; } = new(0.2f, 0.2f, 0.2f);
        public Vector3 EmissiveColor { get; set; } = new(1, 0, 0);

        public MapEngine()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _lastWidth = GraphicsDevice.Viewport.Width;
            _lastHeight = GraphicsDevice.Viewport.Height;

            _res = new ResolutionRenderer(new Point(GraphicsDevice.Adapter.CurrentDisplayMode.Width, GraphicsDevice.Adapter.CurrentDisplayMode.Height), GraphicsDevice)
            {
                ScreenResolution = new Point(_lastWidth, _lastHeight),
                Method = ResizeMethod.Fill
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _world = Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds);

            if (_lastWidth != GraphicsDevice.Viewport.Width ||
                _lastHeight != GraphicsDevice.Viewport.Height)
            {
                _lastWidth = GraphicsDevice.Viewport.Width;
                _lastHeight = GraphicsDevice.Viewport.Height;

                _res.ScreenResolution = new Point(_lastWidth, _lastHeight);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _res.Begin();
            _spriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.End();
            _res.End();

            base.Draw(gameTime);
        }
    }
}
