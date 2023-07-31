using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Engine;
using TMapEditor.Helpers;
using TMapEditor.Models;
using TMFormat.Enums;
using TMFormat.Formats;
using TMFormat.Framework.Maps;
using TMFormat.Helpers;

namespace TMapEditor.Utils
{
    public class ItemsManager : INotifyPropertyChanged
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

        string root = System.IO.Directory.GetCurrentDirectory();

        ObservableCollection<TMSprite> _sprites;

        public ObservableCollection<TMSprite> Sprites
        {
            get { return _sprites; }
            set
            {
                _sprites = value;
                OnPropertyChanged("sprites");
            }
        }

        ObservableCollection<GroupSprites> _groups;
        public ObservableCollection<GroupSprites> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged("groups");
            }
        }

        ObservableCollection<string> _itemsCategory;
        public ObservableCollection<string> ItemsCategory
        {
            get { return _itemsCategory; }
            set
            {
                _itemsCategory = value;
                OnPropertyChanged("ItemsCategory");
            }
        }

        int _groupIndex;
        public int GroupIndex
        {
            get { return _groupIndex; }
            set
            {
                _groupIndex = value;
                OnPropertyChanged("GroupIndex");
            }
        }

        string _fileMap;

        public string FileMap
        {
            get { return _fileMap; }
            set
            {
                _fileMap = value;
                OnPropertyChanged("FileMap");
            }
        }

        TMSprite _itemSelect;

        public TMSprite ItemSelect
        {
            get { return _itemSelect; }
            set
            {
                _itemSelect = value;
                OnPropertyChanged("ItemSelect");
            }
        }

        Point _mouse = new Point();

        public Point Mouse
        {
            get { return _mouse; }
            set
            {
                _mouse = value;
                OnPropertyChanged("Mouse");
            }
        }

        int _currentFloor;

        public int CurrentFloor
        {
            get { return _currentFloor; }
            set
            {
                _currentFloor = value;
                OnPropertyChanged("CurrentFloor");
            }
        }

        bool _isLoaded;

        public bool isLoaded
        {
            get { return _isLoaded; }
            set
            {
                _isLoaded = value;
                OnPropertyChanged("isLoaded");
            }
        }

        double _progress;
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }
        #endregion

        public readonly static ItemsManager Instance = new ItemsManager();

        public ItemsManager()
        {
            ItemsCategory = new ObservableCollection<string>();
            ItemsCategory.Add("Grounds");
            ItemsCategory.Add("Borders");
            ItemsCategory.Add("Fields");
            ItemsCategory.Add("Items");
            ItemsCategory.Add("Trees");
            ItemsCategory.Add("Walls");
            ItemsCategory.Add("Stairs");
            ItemsCategory.Add("Doors");
        }

        public async Task<bool> Load()
        {
            string dataDir = Path.Combine(root, "data");

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            MapEngine.Items = TMItem.Load(Path.Combine(root, "data", "items.dat")).ToSprites();

            int index = 0;

            Groups = new ObservableCollection<GroupSprites>();

            for (int i = 0; i < Enum.GetNames(typeof(TypeItem)).Length; i++)
            {
                Groups.Add(new GroupSprites());
            }

            foreach (var item in MapEngine.Items)
            {
                if (item.Textures.Count > 0)
                {
                    Bitmap _image = item.Textures[0].Texture1.ToImage();
                    item.Image = _image;
                }

                foreach (var text in item.Textures)
                {
                    item.Sprites.Add(new TMSpriteTexture() { Sprite1 = text.Texture1.ToTexture2D(), Sprite2 = text.Texture2.ToTexture2D(), Sprite3 = text.Texture3.ToTexture2D(), Sprite4 = text.Texture4.ToTexture2D() });
                }


                switch ((TypeItem)item.Type)
                {
                    case TypeItem.Ground:
                        Groups[0].Items.Add(item);
                        break;

                    case TypeItem.Border:
                        Groups[1].Items.Add(item);
                        break;

                    case TypeItem.Field:
                        Groups[2].Items.Add(item);
                        break;

                    case TypeItem.Item:
                        Groups[3].Items.Add(item);
                        break;

                    case TypeItem.Tree:
                        Groups[4].Items.Add(item);
                        break;

                    case TypeItem.Wall:
                        Groups[5].Items.Add(item);
                        break;

                    case TypeItem.Stair:
                        Groups[6].Items.Add(item);
                        break;

                    case TypeItem.Door:
                        Groups[7].Items.Add(item);
                        break;
                }


                index++;
                Progress = (double)((double)index / (double)MapEngine.Items.Count);
                await Task.Delay(1);
            }

            if (GroupIndex >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
