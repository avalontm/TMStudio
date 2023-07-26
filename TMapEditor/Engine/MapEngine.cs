using AvaloniaInside.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Engine.Enums;
using TMFormat.Formats;

namespace TMapEditor.Engine
{
    public class MapEngine : Game, INotifyPropertyChanged
    {
        #region Propiedades

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        GraphicsDeviceManager GraphicsDeviceManager { get; }

        private SpriteBatch _spriteBatch;
        private ResolutionRenderer _res;

        private int _lastWidth, _lastHeight;

        public static List<TMSprite> Items;

        PincelStatus _pincel;
        public PincelStatus Pincel
        {
            get { return _pincel; }
            set
            {
                _pincel = value;
                OnPropertyChanged("Pincel");
            }
        }

        public static MapEngine? Instance { private set; get; }

        #endregion

        public MapEngine()
        {
            Instance = this;
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
