using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TMapEditor.Engine;
using TMapEditor.Models;

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

    }
}