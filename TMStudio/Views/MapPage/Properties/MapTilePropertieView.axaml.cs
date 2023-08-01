using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TMStudio.Utils;
using TMFormat.Enums;
using TMFormat.Formats;
using TMFormat.Framework.Enums;
using TMStudio.Models;
using TMStudio.Utils;

namespace TMStudio.Views.MapPage.Properties;

public partial class MapTilePropertieView : UserControl, INotifyPropertyChanged
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

    ObservableCollection<PropertiesModel> _items;
    public ObservableCollection<PropertiesModel> Items
    {
        get { return _items; }
        set
        {
            _items = value;
            OnPropertyChanged(nameof(Items));
        }

    }

    TileModel _model;
    public TileModel Model
    {
        get { return _model; }
        set
        {
            _model = value;
            OnPropertyChanged(nameof(Model));
        }
    }

    public static MapTilePropertieView Instance { private set; get; }
    #endregion

    public MapTilePropertieView()
    {
        InitializeComponent();
        Instance = this;
        Items = new ObservableCollection<PropertiesModel>();
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

    public void onLoadProperties(TileModel model)
    {
        Model = model;

        if (MapManager.Instance.isLoaded)
        {
            Items.Clear();

            var typeitems = EnumConvert.TypeItemToList();
            var fielditems = EnumConvert.TypeItemFieldToList();

            bool isPZ = MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)Model.X, (int)Model.Y].isPZ;
            //Ground
            Items.Add(new PropertiesModel() { Name = "Id", Value = Model.Tile.Id, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Nombre", Value = Model.Tile.Name, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Tipo", Value= Model.Tile.Type, Items = typeitems, Type = 2 });
            Items.Add(new PropertiesModel() { Name = "X", Value = Model.X, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Y", Value = Model.Y, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Z", Value = Model.Z, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Usable", Value = Model.Tile.Use, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Bloqueable", Value = Model.Tile.Block, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Moveable", Value = Model.Tile.Moveable, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Proteccion", Value = isPZ, Type = 1 });
            //Items
            if (Model.Items != null)
            {
                foreach (var item in Model.Items)
                {
                    Items.Add(new PropertiesModel() { Name = "Id", Value = item.Id, Type = 0 });
                    Items.Add(new PropertiesModel() { Name = "Nombre", Value = item.Name, Type = 0 });
                    Items.Add(new PropertiesModel() { Name = "Tipo", Value = item.Type, Items = typeitems, Type = 2 });
                    Items.Add(new PropertiesModel() { Name = "Campo", Value = item.Field, Items = fielditems, Type = 2 });
                    Items.Add(new PropertiesModel() { Name = "Usable", Value = item.Use, Type = 1 });
                    Items.Add(new PropertiesModel() { Name = "Bloqueable", Value = item.Block, Type = 1 });
                    Items.Add(new PropertiesModel() { Name = "Moveable", Value = item.Moveable, Type = 1 });

                    if((TypeField)item.Field == TypeField.Teleport)
                    {
                        Items.Add(new PropertiesModel() { Name = "Destine X:", Value = item.Destine.X, Type = 0 });
                        Items.Add(new PropertiesModel() { Name = "Destine Y:", Value = item.Destine.Y, Type = 0 });
                        Items.Add(new PropertiesModel() { Name = "Destine Z:", Value = item.Destine.Z, Type = 0 });
                    }
                }
            }
        }
    }
}