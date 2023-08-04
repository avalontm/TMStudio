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
using ReactiveUI;

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
            Items.Add(new PropertiesModel() { Name = "Tipo", Value = Model.Tile.Type, Items = typeitems, Type = 2 });
            Items.Add(new PropertiesModel() { Name = "X", Value = Model.X, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Y", Value = Model.Y, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Z", Value = Model.Z, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Usable", Value = Model.Tile.Use, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Bloqueable", Value = Model.Tile.Block, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Movible", Value = Model.Tile.Moveable, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Proteccion", Value = isPZ, Type = 1 });

            //Items
            if (Model.Items != null)
            {
                int index = 1;

                foreach (var item in Model.Items)
                {
                    Items.Add(new PropertiesModel() { Name = "Id", Value = item.Id, Type = 0 });
                    Items.Add(new PropertiesModel() { Name = "Nombre", Value = item.Name, Type = 0 });
                    Items.Add(new PropertiesModel() { Name = "Tipo", Value = item.Type, Items = typeitems, Type = 2 });
                    Items.Add(new PropertiesModel() { Name = "Campo", Value = item.Field, Items = fielditems, Type = 2 });
                    Items.Add(new PropertiesModel() { Name = "Usable", Value = item.Use, Type = 1 });
                    Items.Add(new PropertiesModel() { Name = "Bloqueable", Value = item.Block, Type = 1 });
                    Items.Add(new PropertiesModel() { Name = "Movible", Value = item.Moveable, Type = 1 });

                    if ((TypeField)item.Field == TypeField.Teleport)
                    {
                        Items.Add(new PropertiesModel() { Id = $"field_{index}_x", Name = "Destino X:", Value = item.Destine.X, Type = 0, IsEnabled = true });
                        Items.Add(new PropertiesModel() { Id = $"field_{index}_y", Name = "Destino Y:", Value = item.Destine.Y, Type = 0, IsEnabled = true });
                        Items.Add(new PropertiesModel() { Id = $"field_{index}_z", Name = "Destino Z:", Value = item.Destine.Z, Type = 0, IsEnabled = true });
                        Items.Add(new PropertiesModel() { Name = "Accion", Value = "Teleport", Type = 4, Action = ReactiveCommand.Create<ItemMapModel>(onTeleport), Bind = new ItemMapModel() {  Index = index, Item  = item} });
                    }

                    index++;
                }
            }
        }
    }

    async void onTeleport(ItemMapModel model)
    {
        var itemX = Items.Where(x => x.Id == $"field_{model.Index}_x").FirstOrDefault();
        var itemY = Items.Where(x => x.Id == $"field_{model.Index}_y").FirstOrDefault();
        var itemZ = Items.Where(x => x.Id == $"field_{model.Index}_z").FirstOrDefault();

        if (itemX != null && itemY != null && itemZ != null)
        {
            try
            {
                model.Item.Destine = new TMFormat.Models.ItemVector3(int.Parse(itemX.Value.ToString()), int.Parse(itemY.Value.ToString()), int.Parse(itemZ.Value.ToString()));
            }
            catch (Exception ex)
            {
                await DialogManager.Display($"Error", $"{ex.Message}", "OK");
            }
        }
    }
}
