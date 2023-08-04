using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using TMStudio.Engine;
using TMStudio.Engine.Enums;
using TMStudio.Helpers;
using TMStudio.Models;
using TMStudio.Utils;
using TMFormat.Formats;
using TMStudio.Models;
using TMStudio.Utils;
using TMStudio.Views.CreaturePage;
using TMStudio.Views.MainPage;
using TMStudio.Views.MapPage.Properties;
using TMStudio.AvaloniaInside.MonoGame;

namespace TMStudio.Views.MapPage;

public partial class MapMainView : UserControl, INotifyPropertyChanged
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

    string _title;
    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged("Title");
        }
    }

    ObservableCollection<ToolbarModel> _toolbar;
    public ObservableCollection<ToolbarModel> Toolbar
    {
        get { return _toolbar; }
        set
        {
            _toolbar = value;
            OnPropertyChanged("Toolbar");
        }
    }

    MouseModel _currentMouse = MouseModel.Zero;
    public MouseModel CurrentMouse
    {
        get { return _currentMouse; }
        set
        {
            _currentMouse = value;
            OnPropertyChanged("CurrentMouse");
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

    public Game CurrentGame { get; set; } = MapManager.CurrentGame;
    public static MapMainView? Instance { get; private set; }

    public MapMainView()
    {
        InitializeComponent();
        Instance = this;
        Toolbar = new ObservableCollection<ToolbarModel>();
        MapEngine.Instance.onSelectionReturn += onItemSelectionReturn;
        DataContext = this;
    }


    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Instance = this;
        onLoadContent();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Instance = null;
    }

    async void onLoadContent()
    {
        MapManager.Instance.SetScrolls(hScroll, vScroll);
        Toolbar_Tapped("select");
        onLoadSoprites();
    }

    void onLoadSoprites()
    {
        ItemsManager.Instance.Sprites = new ObservableCollection<TMSprite>(ItemsManager.Instance.Groups[ItemsManager.Instance.GroupIndex].Items);
        lstSprites.ItemsSource = ItemsManager.Instance.Sprites;
    }

    public void onGroupSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            ItemsManager.Instance.GroupIndex = (sender as ComboBox).SelectedIndex;

            if (ItemsManager.Instance.GroupIndex >= 0)
            {
                onLoadSoprites();
            }
        }
    }

    void MonoGame_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        MapEngine.Instance.Released();
    }

    void MonoGame_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        MapEngine.Instance.Pressed();
    }

    void MonoGame_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        MapEngine.Instance.SizeChanged((int)e.NewSize.Width, (int)e.NewSize.Height);

        if (MapManager.Instance.isLoaded)
        {
            MapManager.Instance.Camera.ToMove((int)hScroll.Value, (int)vScroll.Value);
        }
    }

    void MonoGame_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        Debug.WriteLine($"[KeyDown] {e.Key}");
        MapEngine.Instance.KeyDown(e.Key);
    }

    void MonoGame_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        MapEngine.Instance.MousePressed = false;
        MapEngine.Instance.IsFocus = false;
    }

    void MonoGame_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        MapEngine.Instance.IsFocus = true;
    }

    void MonoGame_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var position = e.GetPosition((sender as MonoGameControl));
        MapEngine.Instance.MouseMove(new MouseModel((int)position.X, (int)position.Y));

        CurrentMouse = new MouseModel((int) MapEngine.Instance.GlobalPos.X, (int)MapEngine.Instance.GlobalPos.Y);
        CurrentFloor = MapManager.Instance.FloorCurrent;
      
    }

    void onSelectSpriteChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            if (MapEngine.Instance.Pincel == PincelStatus.Draw)
            {
                if (lstSprites.SelectedIndex >= 0)
                {
                    ItemsManager.Instance.ItemSelect = ItemsManager.Instance.Sprites[lstSprites.SelectedIndex] as TMSprite;
                }
            }
        }
    }

    public void Toolbar_Tapped(object obj)
    {
        string parameter = obj as string;
        ToggleButton button = null;

        switch (parameter)
        {
            case "select":
                MapEngine.Instance.Pincel = PincelStatus.Selection;
                break;
            case "draw":
                MapEngine.Instance.Pincel = PincelStatus.Draw;
                break;
            case "eraser":
                MapEngine.Instance.Pincel = PincelStatus.Erase;
                break;
            case "pz":
                MapEngine.Instance.Pincel = PincelStatus.Protection;
                break;

            default:
                MapEngine.Instance.Pincel = PincelStatus.None;
                break;
        }

        //Seleccionamos el toolbar
        StackPanel stackPanel =  this.FindNameScope().Find<StackPanel>("panelMenu");

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

        if (MapEngine.Instance.Pincel == PincelStatus.Draw)
        {
            if (lstSprites.SelectedIndex >= 0)
            {
                ItemsManager.Instance.ItemSelect = ItemsManager.Instance.Sprites[lstSprites.SelectedIndex] as TMSprite;
            }
        }
        else
        {
            ItemsManager.Instance.ItemSelect = null;
        }
    }

    void onScrollHorizontalChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        MapManager.Instance.Camera.ToMove((int)hScroll.Value, (int)vScroll.Value);
    }

    void onScrollVerticalChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        MapManager.Instance.Camera.ToMove((int)hScroll.Value, (int)vScroll.Value);
    }

    public void onShowProperties(Control control)
    {
        gridProperties.Children.Clear();
        gridProperties.Children.Add(control);
    }

    public async void onNew()
    {

    }

    public async void onOpen()
    {
        await DialogManager.Show("Cargando mapa...");
        bool? status  = await MapManager.Instance.Open();
        await DialogManager.Close();

        if(status != null && !status.Value)
        {
            await DialogManager.Display("Error", "No se pudo cargar el archivo.\nFormato desconocido.", "OK");
            return;
        }

        MapEngine.Instance.SizeChanged((int)monoGame.Bounds.Width, (int)monoGame.Bounds.Height);
    }

    public async void onSave()
    {
        await DialogManager.Show("Guardando mapa...");
        bool? status = await MapManager.Instance.Save();
        await DialogManager.Close();

        if (status != null && !status.Value)
        {
            await DialogManager.Display("Error", "No se pudo guardar el mapa.", "OK");
            return;
        }
    }

    public async void onSaveAs()
    {
        await DialogManager.Show("Guardando mapa...");
        bool? status = await MapManager.Instance.SaveAs();
        await DialogManager.Close();

        if (status != null && !status.Value)
        {
            await DialogManager.Display("Error", "No se pudo guardar el mapa.", "OK");
            return;
        }

    }

    void onItemSelectionReturn(object? sender, TileModel e)
    {
        if(MapTilePropertieView.Instance != null)
        {
            if(MapTilePropertieView.Instance.Model.X == e.X && MapTilePropertieView.Instance.Model.Y == e.Y && MapTilePropertieView.Instance.Model.Z == e.Z)
            {
                return;
            }
        }

        if (MapTilePropertieView.Instance == null)
        {
            var _view = new MapTilePropertieView();
            _view.onLoadProperties(e);
        }
        else
        {
            MapTilePropertieView.Instance.onLoadProperties(e);
        }

        onShowProperties(MapTilePropertieView.Instance);
    }

    public async void onMapProperties()
    {
        onShowProperties(new MapPropertieView());
    }

    public async void onExit()
    {
        bool response = await DialogManager.Display("Confirmar", "¿Esta seguro que deseas cerrar el editor?", "SI", "NO");

        if (!response)
        {
            return;
        }

        if (MainView.Instance != null)
        {
            MainView.Instance.ToPage(new MainControlView());

            if (ItemsManager.Instance != null)
            {
                ItemsManager.Instance.Items = null;
            }
        }
    }
}