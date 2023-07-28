using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using TMapEditor.Utils;
using TMStudio.Models;

namespace TMStudio.Views.MainPage.Properties;

public partial class MapPropertieView : UserControl, INotifyPropertyChanged
{
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
        set { _items = value;
            OnPropertyChanged(nameof(Items));
        }

    }

    public MapPropertieView()
    {
        InitializeComponent();
        Items = new ObservableCollection<PropertiesModel>();
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        onLoadProperties();
    }

    void onLoadProperties()
    {
        if (MapManager.Instance.isLoaded)
        {
            Items.Add(new PropertiesModel() { Name = "Nombre", Value = MapManager.Instance.MapBase.mapInfo.Name });
            Items.Add(new PropertiesModel() { Name = "Autor", Value = MapManager.Instance.MapBase.mapInfo.Autor });
            Items.Add(new PropertiesModel() { Name = "Version", Value = MapManager.Instance.MapBase.mapInfo.Version });
            Items.Add(new PropertiesModel() { Name = "Ancho", Value = MapManager.Instance.MapBase.mapInfo.Size.X });
            Items.Add(new PropertiesModel() { Name = "Alto", Value = MapManager.Instance.MapBase.mapInfo.Size.Y });
        }
    }
}