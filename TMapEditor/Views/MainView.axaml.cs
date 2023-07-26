using Avalonia.Controls;
using Microsoft.Xna.Framework;
using TMapEditor.Engine;

namespace TMapEditor.Views;

public partial class MainView : UserControl
{
    public Game CurrentGame { get; set; } = new MapEngine();

    public MainView()
    {
        InitializeComponent();
        DataContext = this;
    }
}
