using Avalonia.Controls;
using Avalonia.Interactivity;
using TMStudio.Engine;
using TMStudio.Utils;
using TMStudio.Views.MapPage;

namespace TMStudio.Views;

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
        if (MapMainView.Instance != null)
        {
            if (MapEngine.Instance != null)
            {
                MapEngine.Instance.SizeChanged((int)MapMainView.Instance.monoGame.Bounds.Width, (int)MapMainView.Instance.monoGame.Bounds.Height);
               
                if (MapManager.Instance.isLoaded)
                {
                    MapManager.Instance.Camera.ToMove((int)MapMainView.Instance.hScroll.Value, (int)MapMainView.Instance.vScroll.Value);
                }
            }
        }
    }
}
