using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using TMapEditor.Engine;
using TMapEditor.Engine.Enums;
using TMapEditor.Models;
using TMapEditor.Utils;
using TMFormat.Formats;

namespace TMapEditor.Views.MainPage;

public partial class MainViewControl : UserControl, INotifyPropertyChanged
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

    public Game CurrentGame { get; set; } = new MapEngine();
    public static MainViewControl? Instance { get; private set; }

    public MainViewControl()
    {
        InitializeComponent();
        Instance = this;
        Toolbar = new ObservableCollection<ToolbarModel>();
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
        Toolbar_Tapped("draw");
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

    public void onSelectSpriteChanged(object? sender, SelectionChangedEventArgs e)
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
    }

    void onScrollHorizontalChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        MapManager.Instance.Camera.ToMove((int)hScroll.Value, (int)vScroll.Value);
    }

    void onScrollVerticalChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        MapManager.Instance.Camera.ToMove((int)hScroll.Value, (int)vScroll.Value);
    }

    public void onOpen()
    {
        MapManager.Instance.Open();
    }
}