using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMStudio.Engine.Enums;
using TMStudio.Engine;
using TMStudio.Utils;
using TMFormat.Formats;
using TMStudio.Models;
using System;
using System.IO;
using TMStudio.Utils;
using TMStudio.Helpers;
using System.Collections.Generic;
using System.Linq;
using TMStudio.Views;
using Avalonia.VisualTree;
using Avalonia.Media;
using System.Diagnostics;
using TMStudio.Views.MapPage;
using TMStudio.Enums;
using TMFormat.Helpers;
using TMStudio.Views.MainPage;

namespace TMStudio.Views.CreaturePage;

public partial class CreatureMainView : UserControl, INotifyPropertyChanged
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

    TMCreature _creature;

    public TMCreature creature
    {
        get { return _creature; }
        set
        {
            _creature = value;
            OnPropertyChanged("creature");
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

    string _fileCreature;

    public string FileCreature
    {
        get { return _fileCreature; }
        set
        {
            _fileCreature = value;
            OnPropertyChanged("FileCreature");
        }
    }

    public static CreatureMainView Instance { private set; get; }
    #endregion

    public CreatureMainView()
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

    async void onNew()
    {

        if (!string.IsNullOrEmpty(FileCreature))
        {
            bool response = await DialogManager.Display("Confirmar", "¿Desea crear una nueva creatura?", "SI", "NO");

            if (!response)
            {
                return;
            }
        }

        await DialogManager.Show("Creando creatura");

        FileCreature = string.Empty;
        creature = new TMCreature();
        creature.name = "creatura";
        //Title = $"{creature.name} - [sin guardar]";
        onLoadCreature();

        await DialogManager.Close();
    }

    async void onOpen()
    {
        var dialog = new Avalonia.Controls.OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "TMC files", Extensions = new List<string> { "tmc" } });
        dialog.AllowMultiple = false;

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string[] result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            FileCreature = result.FirstOrDefault();

            if (!File.Exists(FileCreature))
            {
                return;
            }

            creature = TMCreature.Load(FileCreature);

            if (creature == null)
            {
                await DialogManager.Display("Error", "No se pudo cargar el archivo.\nFormato desconocido.", "OK");
                return;
            }

            //Title = $"{creature.name} - [{FileCreature}]";
            onLoadCreature();
        }
    }

    async void onSave()
    {
        if (!string.IsNullOrEmpty(FileCreature))
        {
            onSaveFile();
            return;
        }

        var dialog = new Avalonia.Controls.SaveFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "TMC files", Extensions = new List<string> { "tmc" } });

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            FileCreature = result;
            onSaveFile();
        }
    }

    async void onSaveFile()
    {
        await DialogManager.Show("Guardando creatura");
        bool result = creature.SaveToFile(FileCreature);
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

    void onLoadCreature()
    {
        DirIndex = 0;
        SpriteIndex = 0;
        onLoadCreatureDir();
        onLoadDirSprites();
        onLoadLoots();
    }

    void onLoadDirSprites()
    {
        sprites = new ObservableCollection<TMLook>();
        int index = 0;
        foreach (var spr in creature.dirs[DirIndex].sprites)
        {
            index++;
            TMLook sprite = new TMLook();
            sprite.title = $"sprite_{index}";
            sprite.image = spr.textures[0].ToImage();

            sprites.Add(sprite);
        }

        //lstSprites.ItemsSource = sprites;
    }

    async void onLoadCreatureDir()
    {
        if (creature == null)
        {
            return;
        }

        await DialogManager.Show("Cargando creatura");

        Debug.WriteLine($"[DirIndex] {DirIndex} | [SpriteIndex] {SpriteIndex}");
        // Texturas //
        texture1.Source = creature.dirs[DirIndex].sprites[SpriteIndex].textures[0].ToImage();
        texture2.Source = creature.dirs[DirIndex].sprites[SpriteIndex].textures[1].ToImage();
        texture3.Source = creature.dirs[DirIndex].sprites[SpriteIndex].textures[2].ToImage();
        texture4.Source = creature.dirs[DirIndex].sprites[SpriteIndex].textures[3].ToImage();

        // Mascaras //
        mask1.Source = creature.dirs[DirIndex].sprites[SpriteIndex].masks[0].ToImage();
        mask2.Source = creature.dirs[DirIndex].sprites[SpriteIndex].masks[1].ToImage();
        mask3.Source = creature.dirs[DirIndex].sprites[SpriteIndex].masks[2].ToImage();
        mask4.Source = creature.dirs[DirIndex].sprites[SpriteIndex].masks[3].ToImage();

        await DialogManager.Close();
    }

    void onLoadLoots()
    {
        if (loots == null)
        {
            loots = new ObservableCollection<TMLoot>();
        }

        loots.Clear();

        foreach (var loot in creature.loots)
        {
            loots.Add(new TMLoot() { id = loot.id, units = loot.count, rate = loot.probability });
        }

        lstLoot.ItemsSource = loots;
    }

    void onImportTexture(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Border control = sender as Border;

        if (control != null)
        {
            int index = int.Parse(control.Tag.ToString());

            switch (index)
            {
                case 0:
                    onImportTextures(texture1, SlootEnum.Texture, index);
                    break;
                case 1:
                    onImportTextures(texture2, SlootEnum.Texture, index);
                    break;
                case 2:
                    onImportTextures(texture3, SlootEnum.Texture, index);
                    break;
                case 3:
                    onImportTextures(texture4, SlootEnum.Texture, index);
                    break;
            }
          
        }
    }

    void onImportMask(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Border control = sender as Border;

        if (control != null)
        {
            int index = int.Parse(control.Tag.ToString());

            switch (index)
            {
                case 0:
                    onImportTextures(mask1, SlootEnum.Mask, index);
                    break;
                case 1:
                    onImportTextures(mask2, SlootEnum.Mask, index);
                    break;
                case 2:
                    onImportTextures(mask3, SlootEnum.Mask, index);
                    break;
                case 3:
                    onImportTextures(mask4, SlootEnum.Mask, index);
                    break;
            }
        }
    }

    void onDirSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            ComboBox cmbBox = sender as ComboBox;
            if (cmbBox.SelectedIndex >= 0)
            {
                DirIndex = cmbBox.SelectedIndex;
                SpriteIndex = 0;
                onFrames();
                onLoadCreatureDir();
            }
        }
    }

    public void onFrame(object obj)
    {
        int frame = int.Parse(obj.ToString());
        SpriteIndex = frame;
        onFrames();
        onLoadCreatureDir();
        
    }

    void onFrames()
    {
        switch (SpriteIndex)
        {
            case 0:
                BtnFrame1.Background = new SolidColorBrush("#333337".ToBrush().Color);
                BtnFrame2.Background = new SolidColorBrush("#252526".ToBrush().Color);
                BtnFrame3.Background = new SolidColorBrush("#252526".ToBrush().Color);
                break;
            case 1:
                BtnFrame1.Background = new SolidColorBrush("#252526".ToBrush().Color);
                BtnFrame2.Background = new SolidColorBrush("#333337".ToBrush().Color);
                BtnFrame3.Background = new SolidColorBrush("#252526".ToBrush().Color);
                break;
            case 2:
                BtnFrame1.Background = new SolidColorBrush("#252526".ToBrush().Color);
                BtnFrame2.Background = new SolidColorBrush("#252526".ToBrush().Color);
                BtnFrame3.Background = new SolidColorBrush("#333337".ToBrush().Color);
                break;
        }
    }

    async void onImportTextures(Image source, SlootEnum sloot, int index)
    {
        if(creature == null)
        {
            return;
        }

        var dialog = new Avalonia.Controls.OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "images files (*.png, *.bmp)|*.png; *.bmp;", Extensions = new List<string> { "png", "bmp"} });
        dialog.AllowMultiple = false;

        // how to get the window from a control: https://stackoverflow.com/questions/56566570/openfiledialog-in-avalonia-error-with-showasync
        var parent = (Window)MainView.Instance.GetVisualRoot();

        string[] result = await dialog.ShowAsync(parent);

        if (result != null && result.Any())
        {
            string _fileName = result.FirstOrDefault();

            if (File.Exists(_fileName))
            {
                onLoadTextureFromFile(source, sloot, index, _fileName);
            }
        }
    }

    void onLoadTextureFromFile(Image source, SlootEnum sloot, int index, string file)
    {
        byte[] _bytes = TMImageHelper.FromFile(file, true);

        switch (sloot)
        {
            case SlootEnum.Texture:
                {
                    creature.dirs[DirIndex].sprites[SpriteIndex].textures[index] = _bytes;
                    if (creature.dirs[DirIndex].sprites[SpriteIndex].textures[index] != null)
                    {
                        source.Source = creature.dirs[DirIndex].sprites[SpriteIndex].textures[index].ToImage();
                    }
                }
                break;
            case SlootEnum.Mask:
                {
                    creature.dirs[DirIndex].sprites[SpriteIndex].masks[index] = _bytes;
                    if (creature.dirs[DirIndex].sprites[SpriteIndex].masks[index] != null)
                    {
                        source.Source = creature.dirs[DirIndex].sprites[SpriteIndex].masks[index].ToImage();
                    }
                }
                break;
        }
    }
}