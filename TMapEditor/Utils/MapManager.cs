using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Engine;
using TMapEditor.Views;
using TMapEditor.Views.MainPage;
using TMFormat.Formats;
using TMFormat.Framework.Maps;

namespace TMapEditor.Utils
{
    public class MapManager : INotifyPropertyChanged
    {
        #region Propiedades
        public new event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        ScrollBar _hScroll;
        public ScrollBar hScroll
        {
            get { return _hScroll; }
            set
            {
                _hScroll = value;
                OnPropertyChanged("hScroll");
            }
        }

        ScrollBar _vScroll;
        public ScrollBar vScroll
        {
            get { return _vScroll; }
            set
            {
                _vScroll = value;
                OnPropertyChanged("vScroll");
            }
        }

        public static readonly int FloorDefault = 7;

        public int FloorCurrent = FloorDefault;

        public TMBaseMap MapBase;
        public List<TMSprite> Items;
        public CameraManager Camera;

        public bool UseDebug = false;
        public bool UseAnimtaion = true;

        MapTile mapTile;
        int TimeItem = 250;

        public readonly static MapManager? Instance = new MapManager();
        public readonly static Game CurrentGame = new MapEngine();

        #endregion

        public MapManager() 
        {
         
        }

        public void SetScrolls(ScrollBar hScroll, ScrollBar vScroll)
        {
            this.hScroll = hScroll;
            this.vScroll = vScroll;
        }

        public async void Open()
        {
            var dialog = new Avalonia.Controls.OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter(){Name = "TMap files", Extensions = new List<string> { "tmap" } });
            dialog.AllowMultiple = false;

            // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
            var parent = (Window)MainView.Instance.GetVisualRoot();

            string[] result = await dialog.ShowAsync(parent);

            if (result != null && result.Any())
            {
                string FilePath = result.First();
                MapBase = new TMBaseMap(MapEngine.Items);
                bool isMapLoaded = MapBase.Load(FilePath);

                if(!isMapLoaded)
                {
                    Debug.WriteLine($"No se pudo cargar el mapa.");
                    return;
                }

                //Iniciamos la camara
                Camera = new CameraManager();
                Camera.ToMove(0, 0);

                MainViewControl.Instance.Title = $"{MapBase.mapInfo.Name} - [{FilePath}]";

                onLoadScrolls();

                Camera.ToMove((int)hScroll.Value, (int)vScroll.Value);

                //Cargamos el mapa
                mapTile = new MapTile(MapBase, MapEngine.Instance.SpriteBatch);
            }
        }

        void onLoadScrolls(bool _isnew = true)
        {
            if (MapBase != null)
            {
                hScroll.Minimum = 0;

                if (_isnew)
                {
                    hScroll.Value = 0;
                }

                hScroll.Maximum = MapBase.mapInfo.Size.X - (MapEngine.Instance.ActualWidth / TMBaseMap.TileSize);

                vScroll.Minimum = 0;

                if (_isnew)
                {
                    vScroll.Value = 0;
                }

                vScroll.Maximum = MapBase.mapInfo.Size.Y - (MapEngine.Instance.ActualWidth / TMBaseMap.TileSize);
            }
        }

        public void Update(GameTime time)
        {
            if (MapBase != null && MapBase.Floors.Count > 0)
            {
                if (UseAnimtaion)
                {
                    onAnimateFloorCurrent(time);
                }
            }
        }

        public void Draw(GameTime time)
        {
            if (MapBase != null && MapBase.Floors.Count > 0)
            {
                if (!isDungeon())
                {
                    for (int z = FloorCurrent; z <= FloorDefault; z++)
                    {
                        onDrawFloor(z);
                    }
                }

                onDrawFloorCurrent();

            }
        }

        bool isDungeon()
        {
            if (FloorCurrent > FloorDefault)
            {
                return true;
            }
            return false;
        }

        void onAnimateFloorCurrent(GameTime gameTime)
        {
            // UPDATE TILE LAYER
            for (int y = Camera.Screen.Y; y < Camera.Screen.Height; y++)
            {
                for (int x = Camera.Screen.X; x < Camera.Screen.Width; x++)
                {
                    if (MapBase.Floors[FloorCurrent][x, y].item != null)
                    {
                        var _item = MapBase.Floors[FloorCurrent][x, y].item;

                        if (_item.isAnimation)
                        {
                            _item.TimeAnimation += (float)(gameTime.ElapsedGameTime.TotalMilliseconds * _item.AniSpeed);

                            if (_item.TimeAnimation > TimeItem)//FPS?
                            {
                                _item.TimeAnimation = 0;
                                _item.IndexAnimation++;

                                if (_item.IndexAnimation == _item.Textures.Count)
                                {
                                    _item.IndexAnimation = 0;
                                }
                            }
                        }
                    }

                    if (MapBase.Floors[FloorCurrent][x, y].items != null)
                    {
                        for (var a = 0; a < MapBase.Floors[FloorCurrent][x, y].items.Count; a++)
                        {
                            var _item = MapBase.Floors[FloorCurrent][x, y].items[a];

                            if (_item.isAnimation)
                            {
                                _item.TimeAnimation += (float)(gameTime.ElapsedGameTime.TotalMilliseconds * _item.AniSpeed);

                                if (_item.TimeAnimation > TimeItem) //FPS?
                                {
                                    _item.TimeAnimation = 0;
                                    _item.IndexAnimation++;

                                    if (_item.IndexAnimation == _item.Sprites.Count)
                                    {
                                        _item.IndexAnimation = 0;
                                    }
                                }
                            }
                        }
                    }

                } //Y
            } //X
        }

        void onDrawFloorCurrent()
        {
            // DRAW FLOOR LAYER
            for (int y = Camera.Screen.Y; y < Camera.Screen.Height; y++)
            {
                for (int x = Camera.Screen.X; x < Camera.Screen.Width; x++)
                {
                    //COORDENADAS
                    float tmpX = ((x * TMBaseMap.TileSize) - (Camera.Scroll.X * TMBaseMap.TileSize));
                    float tmpY = ((y * TMBaseMap.TileSize) - (Camera.Scroll.Y * TMBaseMap.TileSize));

                    if (MapBase.Floors[FloorCurrent][x, y].item != null)
                    {
                        mapTile.DrawTileBase(FloorCurrent, x, y, tmpX, tmpY, color: Color.White);
                    }
                }
            }

            // DRAW TOP LAYER
            for (int y = Camera.Screen.Y; y < Camera.Screen.Height; y++)
            {
                for (int x = Camera.Screen.X; x < Camera.Screen.Width; x++)
                {
                    //COORDENADAS
                    float tmpX = ((x * TMBaseMap.TileSize) - (Camera.Scroll.X * TMBaseMap.TileSize));
                    float tmpY = ((y * TMBaseMap.TileSize) - (Camera.Scroll.Y * TMBaseMap.TileSize));

                    if (MapBase.Floors[FloorCurrent][x, y].item != null)
                    {
                        mapTile.DrawTileTop(FloorCurrent, x, y, tmpX, tmpY, color: Color.White);
                    }
                }
            }
        }

        void onDrawFloor(int FloorIndex)
        {
            int _floor = FloorDefault;

            // DRAW FLOOR LAYER
            for (int y = Camera.Screen.Y; y < Camera.Screen.Height; y++)
            {
                for (int x = Camera.Screen.X; x < Camera.Screen.Width; x++)
                {
                    //COORDENADAS
                    float tmpX = ((x * TMBaseMap.TileSize) - (Camera.Scroll.X * TMBaseMap.TileSize));
                    float tmpY = ((y * TMBaseMap.TileSize) - (Camera.Scroll.Y * TMBaseMap.TileSize));

                    tmpX += (TMBaseMap.TileSize * (_floor - FloorIndex));
                    tmpY += (TMBaseMap.TileSize * (_floor - FloorIndex));

                    if (MapBase.Floors[FloorIndex][x, y].item != null)
                    {
                        mapTile.DrawTileBase(FloorIndex, x, y, tmpX, tmpY, color: Color.DarkGray);
                    }
                }
            }

            // DRAW TOP LAYER
            for (int y = Camera.Screen.Y; y < Camera.Screen.Height; y++)
            {
                for (int x = Camera.Screen.X; x < Camera.Screen.Width; x++)
                {
                    //COORDENADAS
                    float tmpX = ((x * TMBaseMap.TileSize) - (Camera.Scroll.X * TMBaseMap.TileSize));
                    float tmpY = ((y * TMBaseMap.TileSize) - (Camera.Scroll.Y * TMBaseMap.TileSize));

                    tmpX += (TMBaseMap.TileSize * (_floor - FloorIndex));
                    tmpY += (TMBaseMap.TileSize * (_floor - FloorIndex));

                    if (MapBase.Floors[FloorIndex][x, y].item != null)
                    {
                        mapTile.DrawTileTop(FloorIndex, x, y, tmpX, tmpY, color: Color.DarkGray);
                    }
                }
            }
        }
    }
}
