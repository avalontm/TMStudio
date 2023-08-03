using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMFormat;
using TMFormat.Enums;
using TMFormat.Formats;
using TMFormat.Framework.Creatures;
using TMFormat.Framework.Enums;
using TMFormat.Framework.Loaders;
using TMFormat.Helpers;
using TMFormat.Models;
using TMStudio.Enums;
using TMStudio.Helpers;
using TMStudio.Models;
using TMStudio.Utils;
using TMStudio.Views.MainPage;

namespace TMStudio.Views.ItemPage;
public partial class ItemMainView : UserControl, INotifyPropertyChanged
{
    #region Propiedades

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    ObservableCollection<Bitmap> _animations;
    public ObservableCollection<Bitmap> Animations
    {
        get { return _animations; }
        set
        {
            _animations = value;
            OnPropertyChanged("Animations");
        }
    }
    


    ObservableCollection<TMLoot> _loots;
    public ObservableCollection<TMLoot> Loots
    {
        get { return _loots; }
        set
        {
            _loots = value;
            OnPropertyChanged("Loots");
        }
    }

    ObservableCollection<ItemPropertiesModel> _properties;
    public ObservableCollection<ItemPropertiesModel> Properties
    {
        get { return _properties; }
        set
        {
            _properties = value;
            OnPropertyChanged("Properties");
        }
    }

    ObservableCollection<string> _types;
    public ObservableCollection<string> Types
    {
        get { return _types; }
        set
        {
            _types = value;
            OnPropertyChanged("Types");
        }
    }

    ObservableCollection<TMItem> _items;
    public ObservableCollection<TMItem> Items
    {
        get { return _items; }
        set
        {
            _items = value;
            OnPropertyChanged("Items");
        }
    }

    TMItem _item;
    public TMItem Item
    {
        get { return _item; }
        set
        {
            _item = value;
            OnPropertyChanged("Item");
        }
    }

    int _dirIndex;
    public int DirIndex
    {
        get { return _dirIndex; }
        set
        {
            _dirIndex = value;
            OnPropertyChanged("DirIndex");
        }
    }

    int _spriteIndex;
    public int SpriteIndex
    {
        get { return _spriteIndex; }
        set
        {
            _spriteIndex = value;
            OnPropertyChanged("SpriteIndex");
        }
    }

    string _fileItem;
    public string FileItem
    {
        get { return _fileItem; }
        set
        {
            _fileItem = value;
            OnPropertyChanged("FileItem");
        }
    }

    public static ItemMainView Instance { private set; get; }

    #endregion

    public ItemMainView()
    {
        InitializeComponent();
        Instance = this;
        Properties = new ObservableCollection<ItemPropertiesModel>();
        Animations = new ObservableCollection<Bitmap>();
        Types = new ObservableCollection<string>(EnumConvert.TypeItemTypesToList());
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Instance = this;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Instance = null;
    }

    public void Toolbar_Tapped(object obj)
    {
        string parameter = obj as string;
        ToggleButton button = null;

        switch (parameter)
        {
            case "new":
                onNew();
                break;
            case "open":
                onOpen();
                break;
            case "save":
                onSave();
                break;
            case "close":
                onClose();
                break;

            default:

                break;
        }

        //Seleccionamos el toolbar
        StackPanel stackPanel = this.FindNameScope().Find<StackPanel>("panelMenu");

        for (int i = 0; i < stackPanel.Children.Count; i++)
        {
            if (stackPanel.Children[i] is ToggleButton)
            {
                ToggleButton _button = (stackPanel.Children[i] as ToggleButton);

                if (_button.CommandParameter == obj)
                {
                    button = _button;
                }
                else
                {
                    _button.IsChecked = false;
                }
            }
        }

        button.IsChecked = true;

    }

    async void onClose()
    {
        bool response = await DialogManager.Display("Confirmar", "¿Esta seguro que deseas cerrar el editor?", "SI", "NO");

        if (!response)
        {
            return;
        }

        if (MainView.Instance != null)
        {
            MainView.Instance.ToPage(new MainControlView());
        }
    }

    void onNew()
    {

    }

    async void onOpen()
    {
        var dialog = new Avalonia.Controls.OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "TMI files", Extensions = new List<string> { "tmi" } });
        dialog.Directory = PathManager.Data;
        dialog.AllowMultiple = false;

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string[] result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            FileItem = result.FirstOrDefault();

            if (!File.Exists(FileItem))
            {
                return;
            }

            Items = new ObservableCollection<TMItem>(TMItem.Load(FileItem));

            if (Items == null)
            {
                await DialogManager.Display("Error", "No se pudo cargar el archivo.\nFormato desconocido.", "OK");
                return;
            }

        }
    }

    async void onSave()
    {
        if (!string.IsNullOrEmpty(FileItem))
        {
            onSaveFile();
            return;
        }

        var dialog = new Avalonia.Controls.SaveFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "TMI files", Extensions = new List<string> { "tmi" } });
        dialog.Directory = PathManager.Data;

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            FileItem = result;
            onSaveFile();
        }
    }

    async void onSaveFile()
    {
        await DialogManager.Show("Guardando creatura");
        bool result = TMItem.SaveFile(Items.ToList(), FileItem);
        await DialogManager.Close();

        if (result)
        {
            await DialogManager.Display("Guardado", "El archivo se a guardado correctamente.", "OK");
        }
        else
        {
            await DialogManager.Display("Error", "El archivo no se ha podido guardar.", "OK");
        }
    }


    void onSelectItemsChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            if (lstItems.SelectedIndex < 0)
            {
                return;
            }

            onItemSelect(Items[lstItems.SelectedIndex]);
        }
    }

    void onSelectAnimationChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.IsLoaded)
        {

        }
    }

    async void onItemSelect(TMItem item)
    {
        await DialogManager.Show("Cargando item");
        Properties.Clear();

        PropertyInfo[] fi = typeof(TMItem).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (PropertyInfo info in fi)
        {
            if (!info.isItemOmite() && !info.isNotReader() && !info.isHideField())
            {
                string _name = info.Name;

                if (info.isItemGroup() != null && info.isItemGroup().Value == ((ItemType)item.Type))
                {
                    if ((ItemType)item.Type == ItemType.Field)
                    {

                        if (_name == "Field")
                        {
                            var _fields = EnumConvert.TypeItemFieldToList();

                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Value = info.GetValue(item), Items = _fields }); //Arrays
                            continue;
                        }

                        if (_name == "LightColor")
                        {
                            var _lights = EnumConvert.TypeFieldColorToList();

                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Index = (int)info.GetValue(item), Value = info.GetValue(item), Items = _lights }); //Arrays
                            continue;
                        }
                    }

                    if ((ItemType)item.Type == ItemType.Item)
                    {
                        var _slots = EnumConvert.EquipSlotTypeToList();

                        if (_name == "EquipSlot")
                        {
                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Index = (int)info.GetValue(item),  Value = info.GetValue(item), Items = _slots }); //Arrays
                            continue;
                        }
                    }

                    if ((ItemType)item.Type == ItemType.Item)
                    {
                        var _slots = EnumConvert.TypeFieldColorToList();

                        if (_name == "LightColor")
                        {
                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Index = (int)info.GetValue(item), Value = (ItemColor)info.GetValue(item), Items = _slots }); //Arrays
                            continue;
                        }
                    }


                    if ((ItemType)item.Type == ItemType.Stair)
                    {
                        var _dirs = EnumConvert.TypeDirToList();

                        if (_name == "Dir")
                        {
                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Value = info.GetValue(item), Items = _dirs }); //Arrays
                            continue;
                        }
                    }

                    Properties.Add(new ItemPropertiesModel() { Type = GetFileType(info.PropertyType), Name = _name, Value = info.GetValue(item) }); //Default
                }
                else if(info.isItemGroup() == null)
                {

                    Properties.Add(new ItemPropertiesModel() { Type = GetFileType(info.PropertyType), Name = _name, Value = info.GetValue(item) }); //Default
                }
            }
        }

        Item = item;

        cmbType.SelectedIndex = Item.Type;
        onLoadTexture();

        await Task.Delay(1);
        await DialogManager.Close();
    }


    void onLoadTexture()
    {
        if (Item != null)
        {
            if (Item.Textures.Count== 0)
            {
                texture1.Source = null;
                texture2.Source = null;
                texture3.Source = null;
                texture4.Source = null;
            }
            else
            {
                texture1.Source = Item.Textures[0].Texture1.ToImage();
                texture2.Source = Item.Textures[0].Texture2.ToImage();
                texture3.Source = Item.Textures[0].Texture3.ToImage();
                texture4.Source = Item.Textures[0].Texture4.ToImage();
            }
            //Animaciones
            Animations.Clear();

            foreach (var tex in Item.Textures)
            {
                Animations.Add(tex.Texture1?.ToImage());
            }
        }
    }

    int GetFileType(Type type)
    {
        int _ret = 0;

        if (type == typeof(string))
        {
            _ret = 0;
        }
        else if (type == typeof(bool))
        {
            _ret = 1;
        }
        else if (type == typeof(Array))
        {
            _ret = 2;
        }
        else if (type == typeof(ItemColor))
        {
            _ret = 3;
        }
        return _ret;
    }

    void onItemTypeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
        {
            if (cmbType.SelectedIndex < 0)
            {
                return;
            }

            if (Item != null)
            {
                Item.Type = cmbType.SelectedIndex;
                onItemSelect(Item);
            }
        }
    }

    void onImportTexture(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Border control = sender as Border;

        if (control != null)
        {
            bool useBlock = false;
            if (e.KeyModifiers == Avalonia.Input.KeyModifiers.Control)
            {
                useBlock = true;
            }

            int index = int.Parse(control.Tag.ToString());

            switch (index)
            {
                case 0:
                    if (useBlock)
                    {
                        Item.Block = !Item.Block;
                        return;
                    }
                    onImportTextures(texture1, SlootEnum.Texture, index);
                    break;
                case 1:
                    if (useBlock)
                    {
                        Item.Block2 = !Item.Block2;
                        return;
                    }
                    onImportTextures(texture2, SlootEnum.Texture, index);
                    break;
                case 2:
                    if (useBlock)
                    {
                        Item.Block3 = !Item.Block3;
                        return;
                    }
                    onImportTextures(texture3, SlootEnum.Texture, index);
                    break;
                case 3:
                    if (useBlock)
                    {
                        Item.Block4 = !Item.Block4;
                        return;
                    }
                    onImportTextures(texture4, SlootEnum.Texture, index);
                    break;
            }

        }
    }


    async void onImportTextures(Image source, SlootEnum sloot, int index)
    {
        if(Item == null)
        {
            return;
        }

        var dialog = new Avalonia.Controls.OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "images files (*.png, *.bmp)|*.png; *.bmp;", Extensions = new List<string> { "png", "bmp" } });
        dialog.AllowMultiple = false;

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string[] result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            string _fileName = result.FirstOrDefault();

            if (File.Exists(_fileName))
            {
                onLoadTextureFromFile(source, sloot, index, _fileName);
            }
        }
    }

    void onLoadTextureFromFile(Image source, SlootEnum sloot, int index, string file)
    {
        byte[] _bytes = TMImageHelper.FromFile(file, true);

        switch (index)
        {
            case 0:
                Item.Textures[0].Texture1 = _bytes;
                source.Source = Item.Textures[0].Texture1.ToImage();
                break;
            case 1:
                Item.Textures[0].Texture2 = _bytes;
                source.Source = Item.Textures[0].Texture2.ToImage();
                break;
            case 2:
                Item.Textures[0].Texture3 = _bytes;
                source.Source = Item.Textures[0].Texture3.ToImage();
                break;
            case 3:
                Item.Textures[0].Texture4 = _bytes;
                source.Source = Item.Textures[0].Texture4.ToImage();
                break;
        }
       
    }

    public async void onItemSave()
    {
        if (Item == null)
        {
            await DialogManager.Display("Requerido", "Debes seleccionar un item de la lista.", "OK");
            return;
        }

        PropertyInfo[] fi = typeof(TMItem).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var prop in Properties)
        {
            foreach (var info in fi)
            {
                if (!info.isItemOmite() && !info.isNotReader() && !info.isHideField())
                {
                    string _name = info.Name;

                    if (_name == prop.Name)
                    {
                        if (info.PropertyType == typeof(string))
                        {
                            info.SetValue(Item, prop.Value.ToString());
                        }
                        else if (info.PropertyType == typeof(bool))
                        {
                            info.SetValue(Item, bool.Parse(prop.Value.ToString()));
                        }
                        else if (info.PropertyType == typeof(double))
                        {
                            info.SetValue(Item, double.Parse(prop.Value.ToString()));
                        }
                        else if (info.PropertyType == typeof(int))
                        {
                            info.SetValue(Item, int.Parse(prop.Value.ToString()));
                        }
                    }
                }
            }
        }

        var _exist = Items.Where(x => x.Id == Item.Id).FirstOrDefault();

        if (_exist != null)
        {
            _exist = Item;

            await DialogManager.Display("Guardado", "Se ha modificado este item.", "OK");
        }
        else
        {
            Items.Add(Item);
            await DialogManager.Display("Guardado", "Se ha agregado este item.", "OK");
        }
    }

    public void onNewItem()
    {
        int _id = (Items.LastOrDefault().Id + 1);

        Item = new TMItem()
        {
            Id = _id,
            Textures = new List<TMItemTexture> { new TMItemTexture() }
        };

        onItemSelect(Item);
    }
}