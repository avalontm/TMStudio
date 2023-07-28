using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using TMapEditor.Utils;
using TMStudio.Models;
using TMStudio.Utils;

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

    public async void onSave()
    {
        if (!MapManager.Instance.isLoaded)
        {
            await DialogManager.Display("Requerido", "Debes cargar el mapa primero.", "OK");
            return;
        }

        try
        {
            var _nombre = Items.Where(x => x.Name == "Nombre").FirstOrDefault();
            var _autor = Items.Where(x => x.Name == "Autor").FirstOrDefault();
            var _version = Items.Where(x => x.Name == "Version").FirstOrDefault();
            var _ancho = Items.Where(x => x.Name == "Ancho").FirstOrDefault();
            var _alto = Items.Where(x => x.Name == "Alto").FirstOrDefault();

            MapManager.Instance.MapBase.mapInfo.Name = _nombre.Value.ToString();
            MapManager.Instance.MapBase.mapInfo.Autor = _autor.Value.ToString();
            MapManager.Instance.MapBase.mapInfo.Version = _version.Value.ToString();
            MapManager.Instance.MapBase.mapInfo.Size.X = int.Parse(_ancho.Value.ToString());
            MapManager.Instance.MapBase.mapInfo.Size.Y = int.Parse(_alto.Value.ToString());

            await DialogManager.Display("Correcto", "Se han modificado las propiedades.", "OK");
        }
        catch(Exception ex)
        {
            await DialogManager.Display("Error", ex.ToString(), "OK");
        }
    }
}