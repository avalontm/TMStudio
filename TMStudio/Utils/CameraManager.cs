using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Engine;
using TMFormat.Formats;

namespace TMapEditor.Utils
{
    public class CameraManager
    {
        public Rectangle Screen;
        public Vector2 Scroll;

        public CameraManager()
        {
            Scroll = new Vector2(0, 0);
            ToMove(0, 0);
        }

        public void ToMove(int x, int y)
        {
            int _screenWidth = (int)((x + 1) + (MapEngine.Instance.ActualWidth / TMBaseMap.TileSize));
            int _screenHeight = (int)((y + 1) + (MapEngine.Instance.ActualHeight / TMBaseMap.TileSize));

            if (MapManager.Instance.MapBase != null)
            {
                if (_screenWidth > MapManager.Instance.MapBase.mapInfo.Size.X)
                {
                    _screenWidth = (int)MapManager.Instance.MapBase.mapInfo.Size.X;
                }

                if (_screenHeight > MapManager.Instance.MapBase.mapInfo.Size.Y)
                {
                    _screenHeight = (int)MapManager.Instance.MapBase.mapInfo.Size.Y;
                }
            }

            Screen = new Rectangle(x, y, _screenWidth, _screenHeight);
            Scroll = new Vector2(Screen.X, Screen.Y);
        }

        public void Update()
        {
            ToMove(Screen.X, Screen.Y);
        }
    }
}
