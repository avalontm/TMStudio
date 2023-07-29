using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TMapEditor.Utils;
using TMFormat.Enums;
using TMFormat.Formats;
using TMFormat.Framework.Enums;
using TMStudio.Models;
using TMStudio.Utils;

namespace TMStudio.Views.MainPage.Properties;

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

    public static MapTilePropertieView Instence { private set; get; }
    #endregion

    public MapTilePropertieView()
    {
        InitializeComponent();
        Instence = this;
        Items = new ObservableCollection<PropertiesModel>();
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Instence = this;
        onLoadProperties();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Instence = null;
    }

    void onLoadProperties()
    {
        if (MapManager.Instance.isLoaded)
        {
            var typeitems = EnumConvert.TypeItemToList();

            Items.Add(new PropertiesModel() { Name = "Id", Value = Model.Tile.Id, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Nombre", Value = Model.Tile.Name, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Tipo", Value= Model.Tile.Type, Items = typeitems, Type = 2 });
            Items.Add(new PropertiesModel() { Name = "Usable", Value = Model.Tile.Use, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Bloqueable", Value = Model.Tile.Block, Type = 1 });
        }
    }
}