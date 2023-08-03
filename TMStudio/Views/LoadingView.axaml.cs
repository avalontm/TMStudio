using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using TMStudio.Engine;
using TMStudio.Utils;
using TMStudio.Views.MapPage;
using TMStudio.Views.MainPage;
using System.IO;

namespace TMStudio.Views;

public partial class LoadingView : UserControl, INotifyPropertyChanged
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

    bool isAnimation;

    public Game CurrentGame { get; set; } = MapManager.CurrentGame;

    public LoadingView()
    {
        InitializeComponent();
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        onLoading();
    }

    async Task onAnimation()
    {
        bool isReverse = false;

        while (isAnimation)
        {
            if (!isReverse)
            {
                imgLogo.Height += 2;
                imgLogo.Width += 2;

                await Task.Delay(50);

                if (imgLogo.Height > 220 || imgLogo.Width > 220)
                {
                    imgLogo.Height = 220;
                    imgLogo.Width = 220;
                    isReverse = true;
                }
            }
            else if (isReverse)
            {
                imgLogo.Height -= 2;
                imgLogo.Width -= 2;

                await Task.Delay(50);

                if (imgLogo.Height < 200 || imgLogo.Width < 200)
                {
                    imgLogo.Height = 200;
                    imgLogo.Width = 200;
                    isReverse = false;
                }
            }
        }
    }

    async Task SetMessage(string message)
    {
        Message = message;
        await Task.Delay(100);
    }

    async Task onLoading()
    {
        await Task.Delay(100);

        onAnimation();

        await WaitForDevice();

        TMFormat.TMInstance.InitAvalonia(CurrentGame.GraphicsDevice);
   
        await SetMessage("Cargando configuracion");

        string dataDir = PathManager.Data;

        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        string mapDir = Path.Combine(PathManager.Data, "maps");

        if (!Directory.Exists(mapDir))
        {
            Directory.CreateDirectory(mapDir);
        }

        await Task.Delay(100);

        await SetMessage($"Iniciando");

        await Task.Delay(500);

        MainView.Instance.ToPage(new MainControlView());
    }

    async  Task WaitForDevice()
    {
        while (CurrentGame.GraphicsDevice == null)
        {
            await Task.Delay(1);
        }
    }
}