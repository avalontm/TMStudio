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
using Microsoft.VisualBasic;
using System.Threading.Tasks;

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
            dataGrid.ItemsSource = null;
            Items.Clear();

            var typeitems = EnumConvert.TypeItemToList();
            var fielditems = EnumConvert.TypeItemFieldToList();

            bool isPZ = MapManager.Instance.MapBase.Floors[MapManager.Instance.FloorCurrent][(int)Model.X, (int)Model.Y].isPZ;

            //Ground
            Items.Add(new PropertiesModel() { Name = "Id", Text = Model.Tile.Id.ToString(), Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Nombre", Text = Model.Tile.Name, Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Tipo", Selected = Model.Tile.Type, Items = typeitems, Type = 2 });
            Items.Add(new PropertiesModel() { Name = "X", Text = Model.X.ToString(), Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Y", Text = Model.Y.ToString(), Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Z", Text = Model.Z.ToString(), Type = 0 });
            Items.Add(new PropertiesModel() { Name = "Usable", Checked = Model.Tile.Use, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Bloqueable", Checked = Model.Tile.Block, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Movible", Checked = Model.Tile.Moveable, Type = 1 });
            Items.Add(new PropertiesModel() { Name = "Proteccion", Checked = isPZ, Type = 1 });

            //Items
            if (Model.Items != null)
            {
                int index = 1;

                foreach (var item in Model.Items)
                {
                    Items.Add(new PropertiesModel() { Name = "Id", Text = item.Id.ToString(), Type = 0 });
                    Items.Add(new PropertiesModel() { Name = "Nombre", Text = item.Name, Type = 0 });
                    Items.Add(new PropertiesModel() { Name = "Tipo", Selected = item.Type, Items = typeitems, Type = 2 });
                    Items.Add(new PropertiesModel() { Name = "Campo", Selected = item.Field, Items = fielditems, Type = 2 });
                    Items.Add(new PropertiesModel() { Name = "Usable", Checked = item.Use, Type = 1 });
                    Items.Add(new PropertiesModel() { Name = "Bloqueable", Checked = item.Block, Type = 1 });
                    Items.Add(new PropertiesModel() { Name = "Movible", Checked = item.Moveable, Type = 1 });
                    Items.Add(new PropertiesModel() { Id = $"item_{index}_cid", Name = "UID", Text = item.UID.ToString(), Type = 0 , IsEnabled =true});
                    Items.Add(new PropertiesModel() { Name = "Accion", Text = "Guardar", Type = 4, Action = ReactiveCommand.Create<ItemMapModel>(onScript), Bind = new ItemMapModel() { Index = index, Item = item } });


                    if ((TypeField)item.Field == TypeField.Teleport)
                    {
                        Items.Add(new PropertiesModel() { Id = $"field_{index}_x", Name = "Destino X:", Text = item.Destine.X.ToString(), Type = 0, IsEnabled = true });
                        Items.Add(new PropertiesModel() { Id = $"field_{index}_y", Name = "Destino Y:", Text = item.Destine.Y.ToString(), Type = 0, IsEnabled = true });
                        Items.Add(new PropertiesModel() { Id = $"field_{index}_z", Name = "Destino Z:", Text = item.Destine.Z.ToString(), Type = 0, IsEnabled = true });
                        Items.Add(new PropertiesModel() { Name = "Accion", Text = "Teleport", Type = 4, Action = ReactiveCommand.Create<ItemMapModel>(onTeleport), Bind = new ItemMapModel() {  Index = index, Item  = item} });
                    }

                    index++;
                }
            }

            dataGrid.ItemsSource = Items;
        }
    }

    async void onScript(ItemMapModel model)
    {
        var itemCid = Items.Where(x => x.Id == $"item_{model.Index}_cid").FirstOrDefault();

        if (itemCid != null)
        {
            try
            {
                model.Item.UID = int.Parse(itemCid.Text);
                Debug.WriteLine($"[onScript] {model.Item.UID}");
            }
            catch (Exception ex)
            {
                await DialogManager.Display($"Error", $"{ex.Message}", "OK");
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
                model.Item.Destine = new TMFormat.Models.ItemVector3(int.Parse(itemX.Text), int.Parse(itemY.Text), int.Parse(itemZ.Text));
            }
            catch (Exception ex)
            {
                await DialogManager.Display($"Error", $"{ex.Message}", "OK");
            }
        }
    }
}
