using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Threading;
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
using TMStudio.Models;

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

        int _actualX;
        public int ActualX
        {
            get { return _actualX; }
            set
            {
                _actualX = value;
                OnPropertyChanged("ActualX");
            }
        }

        int _actualY;
        public int ActualY
        {
            get { return _actualY; }
            set
            {
                _actualY = value;
                OnPropertyChanged("ActualY");
            }
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

        Vector3 _lastPosition = Vector3.Zero;

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

        bool _mousePressed;
        public bool MousePressed
        {
            get { return _mousePressed; }
            set
            {
                _mousePressed = value;
                OnPropertyChanged("MousePressed");
            }
        }

        public EventHandler<TileModel> onSelectionReturn;
        float gamePositionX;
        float gamePositionY;

        #endregion

        public MapEngine()
        {
            Instance = this;
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            
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

            // Crea un nuevo SpriteBatch, que se puede usar para dibujar texturas.
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
            if (IsFocus)
            {
                if (MapManager.Instance.MapBase != null)
                {
                    OnInput();
                }
            }

            MapManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
           // _res.Begin();

            _spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);

            if (MapManager.Instance.MapBase != null)
            {
                MapManager.Instance.Draw(gameTime);

                DrawTextureSelect(ScreenPos);
                DrawRectangle(ScreenPos, Color.Red, 2);
            }

            _spriteBatch.End();
           // _res.End();

            base.Draw(gameTime);

        }

        public void SizeChanged(int width, int height)
        {
            // Obtener el tamaño actual
            ActualWidth = width;
            ActualHeight = height;

            // Actualizar la posición del juego en la ventana cuando esta cambie de tamaño
            gamePositionX = (float)GraphicsDeviceManager.PreferredBackBufferWidth / width;
            gamePositionY = (float)GraphicsDeviceManager.PreferredBackBufferHeight / height;

            _res.ScreenResolution = new Point(ActualWidth, ActualHeight);
            ScreenResolution();
        }

        public void ScreenResolution()
        {
            // Establecer la nueva resolución del juego
            GraphicsDeviceManager.PreferredBackBufferWidth = ActualWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ActualHeight;

            // Actualizar la ventana del juego para aplicar los cambios
            GraphicsDeviceManager.ApplyChanges();
        }

        public void MouseMove(MouseModel CurrentMouse)
        {
            if (MapManager.Instance.MapBase != null)
            {
                // Obtener las coordenadas del mouse en base a la resolución del juego
                ScreenPos = new Vector2((int)(CurrentMouse.X * gamePositionX / TMBaseMap.TileSize) , (int)(CurrentMouse.Y * gamePositionY / TMBaseMap.TileSize));
                GlobalPos = new Vector2(((CurrentMouse.X / TMBaseMap.TileSize) + MapManager.Instance.Camera.Scroll.X), ((CurrentMouse.Y / TMBaseMap.TileSize) + MapManager.Instance.Camera.Scroll.Y));
            }
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

        bool IsTileDrawing()
        {
            if((int)GlobalPos.X == _lastPosition.X && (int)GlobalPos.Y == _lastPosition.Y  && MapManager.Instance.FloorCurrent == _lastPosition.Z)
            {
                return true;
            }

            return false;
        }

        public void Pressed()
        {
            MapEngine.Instance.MousePressed = true;
        }

        public void Released()
        {
            MapEngine.Instance.MousePressed = false;
            _lastPosition = Vector3.Zero;
        }

        void OnInput()
        {
            if (MousePressed)
            {
                if(IsTileDrawing())
                {
                    return;
                }

                switch (Pincel)
                {
                    case PincelStatus.Selection:
                        onSelection();
                        break;
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
                _lastPosition = new Vector3((int)GlobalPos.X, (int)GlobalPos.Y, MapManager.Instance.FloorCurrent);
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

        bool isRegion()
        {
            if (GlobalPos.X >=0 && GlobalPos.X < MapManager.Instance.MapBase.mapInfo.Size.X && GlobalPos.Y >= 0 && GlobalPos.Y < MapManager.Instance.MapBase.mapInfo.Size.Y)
            {
                return true;
            }
            return false;
        }

        void onErase()
        {
            if(!isRegion())
            {
                return;
            }

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
            if (!isRegion())
            {
                return;
            }
            if (MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item == null)
            {
                return;
            }

            MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].isPZ = true;
        }

        void onPincel()
        {
            if (!isRegion())
            {
                return;
            }
            if (ItemsManager.Instance.ItemSelect == null)
            {
                return;
            }

            TMSprite Item = new TMSprite();
            Item.Copy(ItemsManager.Instance.ItemSelect);

            switch ((TypeItem)ItemsManager.Instance.ItemSelect.Type)
            {
                case TypeItem.Ground:

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

        void onSelection()
        {
            TMSprite item = MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item;

            if (item != null)
            {
                TileModel model = new TileModel() 
                { 
                    X = (int)GlobalPos.X, 
                    Y = (int)GlobalPos.Y, 
                    Z = MapManager.Instance.FloorCurrent, 
                    Tile = MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].item, 
                    Items = MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)GlobalPos.X, (int)GlobalPos.Y].items 
                };

                Dispatcher.UIThread.Post(() =>
                {
                      onSelectionReturn?.Invoke(this, model);
                });
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
