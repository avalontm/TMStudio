using Avalonia.Controls;
using AvaloniaInside.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Engine.Enums;
using TMapEditor.Utils;
using TMapEditor.Views;
using TMFormat;
using TMFormat.Enums;
using TMFormat.Formats;
using TMFormat.Framework.Inputs;

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
        ResolutionRenderer _res;

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

        Vector2 _globalpos;
        public Vector2 GlobalPos
        {
            get { return _globalpos; }
            set
            {
                _globalpos = value;
                OnPropertyChanged("GlobalPos");
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

        bool _isFocus;
        public bool IsFocus
        {
            get { return _isFocus; }
            set
            {
                _isFocus = value;
                OnPropertyChanged("IsFocus");
            }
        }

        int xOffset = 0;
        int yOffset = 0;

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

            _res = new ResolutionRenderer(new Point(800, 480), GraphicsDevice)
            {
                ScreenResolution = new Point(ActualWidth, ActualHeight),
                Method = ResizeMethod.Fill
            };
  
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TMInstance.GraphicsDevice = GraphicsDevice;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();

            if (ActualWidth != GraphicsDevice.Viewport.Width || ActualHeight != GraphicsDevice.Viewport.Height)
            {
                ActualWidth = GraphicsDevice.Viewport.Width;
                ActualHeight = GraphicsDevice.Viewport.Height;

                _res.ScreenResolution = new Point(ActualWidth, ActualHeight);
            }

            if (IsFocus)
            {
                if (MapManager.Instance.MapBase != null)
                {
                    GlobalPos = new Vector2(((MouseState.X) / TMBaseMap.TileSize) + MapManager.Instance.Camera.Scroll.X, ((MouseState.Y) / TMBaseMap.TileSize) + MapManager.Instance.Camera.Scroll.Y);
                    ScreenPos = new Vector2(((MouseState.X) / TMBaseMap.TileSize), ((MouseState.Y) / TMBaseMap.TileSize));

                    OnInput();
                }
            }

            MapManager.Instance.Update(gameTime);
            base.Update(gameTime);

            _previousState = KeyboardState;
            _lastMouseState = MouseState;
        }

        protected override void Draw(GameTime gameTime)
        {
            _res.Begin();

            _spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);

            MapManager.Instance.Draw(gameTime);

            DrawTextureSelect(ScreenPos);
            DrawRectangle(ScreenPos, Color.Red, 2);

            _spriteBatch.End();
            _res.End();

            base.Draw(gameTime);

        }

        public void KeyDown(Avalonia.Input.Key key)
        {
            if (key == Avalonia.Input.Key.OemPlus || key == Avalonia.Input.Key.Add)
            {
                if (MapManager.Instance.FloorCurrent < (MapManager.Instance.MapBase.Floors.Count - 1))
                {
                    MapManager.Instance.FloorCurrent++;
                }
            }

            if (key == Avalonia.Input.Key.OemMinus || key == Avalonia.Input.Key.Subtract)
            {
                if (MapManager.Instance.FloorCurrent > 0)
                {
                    MapManager.Instance.FloorCurrent--;
                }
            }
        }

        void OnInput()
        {
            

            if (MouseState.LeftButton == ButtonState.Pressed)
            {
                switch (Pincel)
                {
                    case PincelStatus.Draw:
                        onPincel();
                        break;
                    case PincelStatus.Erase:
                        onErase();
                        break;
                    case PincelStatus.Protection:
                        onProtectionZone();
                        break;
                }

            }
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

        void onErase()
        {
            if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item == null)
            {
                return;
            }

            if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items?.Count == 0) //Si no hay mas item borramos el tile.
            {
                MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].isPZ = false;
                MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item = null;
                MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = null;
            }

            if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items != null) // Items
            {
                var item = MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.LastOrDefault();

                if (item != null)
                {
                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Remove(item);
                }
            }
        }

        void onProtectionZone()
        {
            if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item == null)
            {
                return;
            }

            MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].isPZ = true;
        }

        void onPincel()
        {
            if (ItemsManager.Instance.ItemSelect == null)
            {
                return;
            }

            TMSprite Item = new TMSprite();
            Item.Copy(ItemsManager.Instance.ItemSelect);

            switch ((TypeItem)ItemsManager.Instance.ItemSelect.Type)
            {
                case TypeItem.Tile:

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item = Item;

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    break;
                case TypeItem.Border:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item == null)
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item = MapManager.Instance.Items[1]; //Item Transparente
                    }

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Insert(0, Item); //Borde en el principio.

                    break;
                case TypeItem.Field:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Add(Item); //Agregamos hasta final

                    break;
                case TypeItem.Item:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Add(Item); //Agregamos hasta final

                    break;
                case TypeItem.Tree:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Add(Item); //Agregamos hasta final

                    break;
                case TypeItem.Door:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Add(Item); //Agregamos hasta final
                    break;
                case TypeItem.Wall:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Add(Item); //Agregamos hasta final

                    break;
                case TypeItem.Stair:

                    if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items == null) // Items
                    {
                        MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items = new List<TMSprite>();
                    }

                    MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items.Add(Item); //Agregamos hasta final

                    break;
            }

        }

        void onField(TMSprite item)
        {
            switch ((TypeField)item.Field)
            {
                case TypeField.None:

                    break;
                case TypeField.Fire:

                    break;
                case TypeField.Teleport:
                    /*
                    if (TeleportWindow.Instance == null)
                    {
                        TeleportWindow frm = new TeleportWindow(item);
                        frm.Owner = MainWindow.Instance;
                        frm.ShowDialog();
                    }*/
                    break;
            }
        }
    }
}
