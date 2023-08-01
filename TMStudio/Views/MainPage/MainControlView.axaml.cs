using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMStudio.Engine;
using TMStudio.Utils;
using TMStudio.Views;
using TMStudio.Views.MapPage;
using TMStudio.Views.CreaturePage;
using TMStudio.Views.ItemPage;

namespace TMStudio.Views.MainPage;

public partial class MainControlView : UserControl, INotifyPropertyChanged
{
    #region Propiedades

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    string _message = "";
    public string Message
    {
        get { return _message; }
        set
        {
            _message = value;
            OnPropertyChanged("Message");
        }
    }

    bool _isLoading;
    public bool IsLoading
    {
        get { return _isLoading; }
        set
        {
            _isLoading = value;
            OnPropertyChanged("IsLoading");
        }
    }

    public static MainControlView Instance { get; private set; }

    #endregion

    public MainControlView()
    {
        InitializeComponent();
        Instance = this;
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

    async Task SetMessage(string message)
    {
        Message = message;
        await Task.Delay(100);
    }

    public async void OptionCommand(object obj)
    {
        if(IsLoading)
        {
            return;
        }

        int index = int.Parse(obj.ToString());

        switch (index)
        {
            case 0:
                MainView.Instance.ToPage(new ItemMainView());
                break;
            case 1:
                MainView.Instance.ToPage(new CreatureMainView());
                break;
            case 2:
                onMapEditor();
                break;
            default:
                break;
        }
    }

    async Task onMapEditor()
    {
        IsLoading = true;

        ItemsManager.Instance.Progress = 0;

        await SetMessage("Cargando Items...");

        bool status = await ItemsManager.Instance.Load();

        await SetMessage($"Se han cargado [{MapEngine.Items.Count}] items");

        await Task.Delay(100);

        await SetMessage($"Iniciando");

        await Task.Delay(100);

        MainView.Instance.ToPage(new MapMainView());
    }
}