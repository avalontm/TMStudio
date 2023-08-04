using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using TMFormat.Formats;
using TMFormat.Framework.Enums;
using TMStudio.Models;
using TMStudio.Utils;

namespace TMStudio.Views.CreaturePage.Panels;

public partial class ItemSearchDialog : UserControl, INotifyPropertyChanged
{
    #region Propiedades

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public EventHandler<TMLoot> onReturn;

    ObservableCollection<TMItem> _items;
    public ObservableCollection<TMItem> items
    {
        get { return _items; }
        set
        {
            _items = value;
            OnPropertyChanged("items");
        }
    }

    string _itemSearch;
    public string itemSearch
    {
        get { return _itemSearch; }
        set
        {
            _itemSearch = value;
            OnPropertyChanged("itemSearch");
        }
    }

    int _units;
    public int units
    {
        get { return _units; }
        set
        {
            _units = value;
            OnPropertyChanged("units");
        }
    }

    double _rate;
    public double rate
    {
        get { return _rate; }
        set
        {
            _rate = value;
            OnPropertyChanged("rate");
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

    #endregion

    public ItemSearchDialog()
    {
        InitializeComponent();
        items = new ObservableCollection<TMItem>();
        this.IsVisible = false;
        DataContext = this;
    }

    public void Show()
    {
        this.IsVisible = true;
    }

    void GridBackground_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Close();
    }

    public void Close()
    {
        this.IsVisible = false;
        items.Clear();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
    }


    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
    }

    void TxtSearch_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(itemSearch))
        {
            items.Clear();
            return;
        }

        var _filter = ItemsManager.Instance.Items.Where(x => (ItemType)x.Type == ItemType.Item && x.Moveable && x.Name.Contains(itemSearch)).ToList();

        items = new ObservableCollection<TMItem>(_filter);

    }

    void LstItems_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
       if(IsLoaded)
        {
            if(lstItems.SelectedIndex < 0)
            {
                return;
            }

            item = items[lstItems.SelectedIndex];
            units = 1;
            rate = 10;
        }
    }

    public async void onLootAdd()
    {
        if(item == null)
        {
            await DialogManager.Display("Requerido", "Debes seleccionar un item.", "OK");
            return;
        }

        TMLoot loot = new TMLoot()
        {
            id = item.Id,
            units = units,
            rate = rate,
        };
        
        onReturn?.Invoke(this, loot);
    }
}