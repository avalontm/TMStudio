using Avalonia.Controls;
using Avalonia.Interactivity;
using System.ComponentModel;

namespace TMStudio.Views;

public partial class MainView : UserControl, INotifyPropertyChanged
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

    public static MainView? Instance { get; private set; }

    public MainView()
    {
        InitializeComponent();
        Instance = this;
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ToPage(new LoadingView());
    }

    public void ToPage(UserControl control)
    {
        gridMain.Children.Clear();
        gridMain.Children.Add(control);
    }
}
