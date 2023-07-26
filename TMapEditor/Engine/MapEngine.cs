using Avalonia.Controls;
using AvaloniaInside.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Engine.Enums;
using TMapEditor.Utils;
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

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        private ResolutionRenderer _res;
        public ResolutionRenderer Resolution
        {
            get { return _res; }
        }

        int _actualWidth;
        public int ActualWidth
        {
            get { return _actualWidth; }
            set
            {
                _actualWidth = value;
                OnPropertyChanged("ActualWidth");
            }
        }


        int _actualHeight;
        public int ActualHeight
        {
            get { return _actualHeight; }
            set
            {
                _actualHeight = value;
                OnPropertyChanged("ActualHeight");
            }
        }

        public static List<TMSprite> Items;
        Texture2D _pointTexture;

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

        Vector2 _screenpos;
        public Vector2 ScreenPos
        {
            get { return _screenpos; }
            set
            {
                _screenpos = value;
                OnPropertyChanged("ScreenPos");
            }
        }

        MouseState _mouseState;
        public MouseState MouseState
        {
            get { return _mouseState; }
            set
            {
                _mouseState = value;
                OnPropertyChanged("Mouse");
            }
        }

        KeyboardState _previousState;
        MouseState _lastMouseState;

        KeyboardState _keyboardState;
        public KeyboardState KeyboardState
        {
            get { return _keyboardState; }
            set
            {
                _keyboardState = value;
                OnPropertyChanged("Keyboard");
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
            ActualWidth = GraphicsDevice.Viewport.Width;
            ActualHeight = GraphicsDevice.Viewport.Height;

            _res = new ResolutionRenderer(new Point(GraphicsDevice.Adapter.CurrentDisplayMode.Width, GraphicsDevice.Adapter.CurrentDisplayMode.Height), GraphicsDevice)
            {
                ScreenResolution = new Point(ActualWidth, ActualHeight),
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
            //MouseState = _mouse.GetState();
            //KeyboardState = _keyboard.GetState();

            if (ActualWidth != GraphicsDevice.Viewport.Width || ActualHeight != GraphicsDevice.Viewport.Height)
            {
                ActualWidth = GraphicsDevice.Viewport.Width;
                ActualHeight = GraphicsDevice.Viewport.Height;

                _res.ScreenResolution = new Point(ActualWidth, ActualHeight);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

             //_res.Begin();
            _spriteBatch.Begin();

            MapManager.Instance.Draw(gameTime);

            DrawTextureSelect(ScreenPos);
            DrawRectangle(ScreenPos, Color.Red, 2);

            _spriteBatch.End();
            //_res.End();

            base.Draw(gameTime);
        }

        void DrawTextureSelect(Vector2 pos)
        {
            if (ItemsManager.Instance.ItemSelect != null)
            {
                Rectangle rectangle = new Rectangle((int)pos.X * TMBaseMap.TileSize, (int)pos.Y * TMBaseMap.TileSize, TMBaseMap.TileSize, TMBaseMap.TileSize);
                _spriteBatch.Draw(ItemsManager.Instance.ItemSelect.Sprites[0].Sprite1, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), Color.White * 0.75f);
            }
        }

        void DrawRectangle(Vector2 pos, Color color, int lineWidth)
        {
            Rectangle rectangle = new Rectangle((int)pos.X * TMBaseMap.TileSize, (int)pos.Y * TMBaseMap.TileSize, TMBaseMap.TileSize, TMBaseMap.TileSize);
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData<Color>(new Color[] { Color.White });
            }

            _spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            _spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            _spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            _spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }
    }
}
