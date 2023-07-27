using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TMStudio.Dialogs;

public partial class DialogView : UserControl, INotifyPropertyChanged
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

    public EventHandler<bool> onReturn;

    public static DialogView? Instance { get; private set; }
    public bool Response { get; private set; }

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

    string _message;
    public string Message
    {
        get { return _message; }
        set
        {
            _message = value;
            OnPropertyChanged("Message");
        }
    }

    string _accept;
    public string Accept
    {
        get { return _accept; }
        set
        {
            _accept = value;
            OnPropertyChanged("Accept");
        }
    }

    string _cancel;
    public string Cancel
    {
        get { return _cancel; }
        set
        {
            _cancel = value;
            OnPropertyChanged("Cancel");
        }
    }

    bool _waitForResponse;
    public bool WaitForResponse
    {
        get { return _waitForResponse; }
        set
        {
            _waitForResponse = value;
            OnPropertyChanged("WaitForResponse");
        }
    }

    bool _isSingle;
    public bool IsSingle
    {
        get { return _isSingle; }
        set
        {
            _isSingle = value;
            OnPropertyChanged("IsSingle");
        }
    }

    bool _isQuestion;
    public bool IsQuestion
    {
        get { return _isQuestion; }
        set
        {
            _isQuestion = value;
            OnPropertyChanged("IsQuestion");
        }
    }

    public DialogView()
    {
        InitializeComponent();
        IsVisible = false;
        Instance = this;
        DataContext = this;
    }

    public async Task Show(string title, string message, string accept, string cancel = "")
    {
        Title = title;
        Message = message;
        Accept = accept;
        Cancel = cancel;

        if (!string.IsNullOrEmpty(Cancel))
        {
            IsQuestion = true;
            IsSingle = false;
        }
        else
        {
            IsQuestion = false;
            IsSingle = true;
        }

        IsVisible = true;
        WaitForResponse = true;

        await Task.Run(async () =>
        {
            while (WaitForResponse)
            {
                await Task.Delay(1);
            }
        });
    }

    public void Close()
    {
        IsVisible = false;
        WaitForResponse = false;
    }

    public void onOK()
    {
        if (IsSingle && !IsQuestion)
        {
            Response = true;
            Close();
        }
    }

    public void onAccept()
    {
        if (IsQuestion && !IsSingle)
        {
            Response = true;
            Close();
        }
    }

    public void onCancel()
    {
        Response = false;
        Close();
    }
}