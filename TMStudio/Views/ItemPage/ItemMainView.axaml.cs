using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMStudio.Views;
using TMFormat.Formats;
using TMStudio.Models;
using TMStudio.Utils;
using TMStudio.Views.MainPage;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Linq;
using TMFormat.Framework.Creatures;
using System.IO;
using Avalonia.Media;
using TMStudio.Engine.Enums;
using TMStudio.Engine;
using TMStudio.Enums;
using System.Reflection;
using TMFormat.Helpers;
using TMStudio.Helpers;
using System.Diagnostics;
using TMFormat.Models;
using TMFormat.Framework.Enums;
using TMFormat.Enums;
using System.Data;

namespace TMStudio.Views.ItemPage;
public partial class ItemMainView : UserControl, INotifyPropertyChanged
{
    #region Propiedades

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    ObservableCollection<TMLook> _sprites;

    public ObservableCollection<TMLook> Sprites
    {
        get { return _sprites; }
        set
        {
            _sprites = value;
            OnPropertyChanged("Sprites");
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

    List<TMItem> _items;
    public List<TMItem> Items
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

            Items = TMItem.Load(FileItem);

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
        bool result = TMItem.SaveFile(Items, FileItem);
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


    void onSelectSpriteChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            if(lstSprites.SelectedIndex < 0)
            {
                return;
            }

            onItemSelect(Items[lstSprites.SelectedIndex]);
        }
    }

    async void onItemSelect(TMItem item)
    {
        await DialogManager.Show();
        Properties.Clear();

        FieldInfo[] fi = typeof(TMItem).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (FieldInfo info in fi)
        {
            if (!info.isItemOmite() && !info.isNotReader() && !info.isHideField())
            {
                if (info.isItemGroup() == ((ItemType)item.Type))
                {
                    string _name = info.Name.GetTextBetweenAngleBrackets().FirstOrDefault();

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

                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Value = info.GetValue(item), Items = _lights }); //Arrays
                            continue;
                        }
                    }

                    if ((ItemType)item.Type == ItemType.Item)
                    {
                        var _slots = EnumConvert.EquipSlotTypeToList();

                        if (_name == "EquipSlot")
                        {
                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Value = info.GetValue(item), Items = _slots }); //Arrays
                            continue;
                        }
                    }

                    if ((ItemType)item.Type == ItemType.Item)
                    {
                        var _slots = EnumConvert.EquipSlotTypeToList();

                        if (_name == "LightColor")
                        {
                            Properties.Add(new ItemPropertiesModel() { Type = 2, Name = _name, Value = info.GetValue(item), Items = _slots }); //Arrays
                            continue;
                        }
                    }
                }
            }
        }

        Item = item;

        cmbType.SelectedIndex = Item.Type;
        onLoadTexture();

        await DialogManager.Close();
    }


    void onLoadTexture()
    {
        texture1.Source = Item.Textures[0].Texture1.ToImage();
        texture2.Source = Item.Textures[0].Texture2.ToImage();
        texture3.Source = Item.Textures[0].Texture3.ToImage();
        texture4.Source = Item.Textures[0].Texture4.ToImage();
    }

    int GetFileType(Type type)
    {
        int _ret = 0;

        if(type  == typeof(string))
        {
            _ret = 0;
        }else if(type == typeof(bool))
        {
            _ret = 1;
        }else if (type == typeof(Array))
        {
            _ret = 2;
        }else if(type == typeof(ItemColor))
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
            int index = int.Parse(control.Tag.ToString());

            switch (index)
            {
                case 0:
                   // onImportTextures(texture1, SlootEnum.Texture, index);
                    break;
                case 1:
                   //onImportTextures(texture2, SlootEnum.Texture, index);
                    break;
                case 2:
                   // onImportTextures(texture3, SlootEnum.Texture, index);
                    break;
                case 3:
                   // onImportTextures(texture4, SlootEnum.Texture, index);
                    break;
            }

        }
    }

    public async void onItemSave()
    {
        if (Item == null)
        {
            await DialogManager.Display("Requerido", "Debes seleccionar un item de la lista.", "OK");
            return;
        }

    }
}