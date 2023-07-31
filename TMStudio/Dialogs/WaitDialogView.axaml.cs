using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TMStudio.Dialogs;

public partial class WaitDialogView : UserControl, INotifyPropertyChanged
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

    public static WaitDialogView? Instance { get; private set; }

    public WaitDialogView()
    {
        InitializeComponent();
        Instance = this;
        IsVisible = false;
        DataContext = this;
    }

    public async void Show(string message = "")
    {
        Message = message;
        await Task.Delay(1);
        IsVisible = true;
    }

    public void Close()
    {
        IsVisible = false;
        Message = string.Empty;
    }
}