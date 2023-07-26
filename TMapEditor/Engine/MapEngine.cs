using AvaloniaInside.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMFormat.Formats;

namespace TMapEditor.Engine
{
    public class MapEngine : Game
    {
        GraphicsDeviceManager GraphicsDeviceManager { get; }

        private SpriteBatch _spriteBatch;
        private ResolutionRenderer _res;

        private int _lastWidth, _lastHeight;

        public static List<TMSprite> Items;

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
