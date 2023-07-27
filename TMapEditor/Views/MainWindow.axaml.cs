using Avalonia.Controls;
using Avalonia.Interactivity;
using TMapEditor.Engine;
using TMapEditor.Utils;
using TMapEditor.Views.MainPage;

namespace TMapEditor.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
    }

    void MainWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (MainViewControl.Instance != null)
        {
            if (MapEngine.Instance != null)
            {
                MapEngine.Instance.SizeChanged((int)MainViewControl.Instance.monoGame.Bounds.Width, (int)MainViewControl.Instance.monoGame.Bounds.Height);
               
                if (MapManager.Instance.Camera != null)
                {
                    MapManager.Instance.Camera.ToMove((int)MainViewControl.Instance.hScroll.Value, (int)MainViewControl.Instance.vScroll.Value);
                }
            }
        }
    }
}
