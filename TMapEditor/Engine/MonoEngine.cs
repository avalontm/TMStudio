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
    public class MonoEngine : Game, IDisposable
    {
        GraphicsDeviceManager GraphicsDeviceManager { get; }

        SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public MonoEngine()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }
    }
}
