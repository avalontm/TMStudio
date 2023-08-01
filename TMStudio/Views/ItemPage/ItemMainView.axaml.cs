using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMStudio.Views;
using TMFormat.Formats;
using TMStudio.Models;
using TMStudio.Utils;
using TMStudio.Views.MainPage;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Linq;
using TMFormat.Framework.Creatures;
using System.IO;

namespace TMStudio.Views.ItemPage;
public partial class ItemMainView : UserControl, INotifyPropertyChanged
{
    #region Propiedades

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    ObservableCollection<TMLook> _sprites;

    public ObservableCollection<TMLook> sprites
    {
        get { return _sprites; }
        set
        {
            _sprites = value;
            OnPropertyChanged("sprites");
        }
    }

    ObservableCollection<TMLoot> _loots;
    public ObservableCollection<TMLoot> loots
    {
        get { return _loots; }
        set
        {
            _loots = value;
            OnPropertyChanged("loots");
        }
    }


    List<TMItem> _items;
    public List<TMItem> items
    {
        get { return _items; }
        set
        {
            _items = value;
            OnPropertyChanged("items");
        }
    }

    TMItem _item;
    public TMItem item
    {
        get { return _item; }
        set
        {
            _item = value;
            OnPropertyChanged("item");
        }
    }

    int _dirIndex;
    public int DirIndex
    {
        get { return _dirIndex; }
        set
        {
            _dirIndex = value;
            OnPropertyChanged("DirIndex");
        }
    }

    int _spriteIndex;
    public int SpriteIndex
    {
        get { return _spriteIndex; }
        set
        {
            _spriteIndex = value;
            OnPropertyChanged("SpriteIndex");
        }
    }

    string _fileItem;
    public string FileItem
    {
        get { return _fileItem; }
        set
        {
            _fileItem = value;
            OnPropertyChanged("FileItem");
        }
    }

    public static ItemMainView Instance { private set; get; }

    #endregion

    public ItemMainView()
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

    public void Toolbar_Tapped(object obj)
    {
        string parameter = obj as string;
        ToggleButton button = null;

        switch (parameter)
        {
            case "new":
                onNew();
                break;
            case "open":
                onOpen();
                break;
            case "save":
                onSave();
                break;
            case "close":
                onClose();
                break;

            default:

                break;
        }

        //Seleccionamos el toolbar
        StackPanel stackPanel = this.FindNameScope().Find<StackPanel>("panelMenu");

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

    async void onClose()
    {
        bool response = await DialogManager.Display("Confirmar", "¿Esta seguro que deseas cerrar el editor?", "SI", "NO");

        if (!response)
        {
            return;
        }

        if (MainView.Instance != null)
        {
            MainView.Instance.ToPage(new MainControlView());
        }
    }

    void onNew()
    {

    }

    async void onOpen()
    {
        var dialog = new Avalonia.Controls.OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "TMI files", Extensions = new List<string> { "tmi" } });
        dialog.AllowMultiple = false;

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string[] result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            FileItem = result.FirstOrDefault();

            if (!File.Exists(FileItem))
            {
                return;
            }

            items = TMItem.Load(FileItem);

            if (items == null)
            {
                await DialogManager.Display("Error", "No se pudo cargar el archivo.\nFormato desconocido.", "OK");
                return;
            }

        }
    }

    async void onSave()
    {
        if (!string.IsNullOrEmpty(FileItem))
        {
            onSaveFile();
            return;
        }

        var dialog = new Avalonia.Controls.SaveFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "TMI files", Extensions = new List<string> { "tmi" } });

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            FileItem = result;
            onSaveFile();
        }
    }

    async void onSaveFile()
    {
        await DialogManager.Show("Guardando creatura");
        bool result = TMItem.SaveFile(items, FileItem);
        await DialogManager.Close();

        if (result)
        {
            await DialogManager.Display("Guardado", "El archivo se a guardado correctamente.", "OK");
        }
        else
        {
            await DialogManager.Display("Error", "El archivo no se ha podido guardar.", "OK");
        }
    }
}