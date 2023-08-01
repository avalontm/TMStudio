using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMStudio.Models;
using TMStudio.Views.MapPage;

namespace TMapEditor.Controls
{
    public partial class PageControl : UserControl, INotifyPropertyChanged
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

        string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (MapMainView.Instance != null)
                {
                    MapMainView.Instance.Title = _title;
                }
                OnPropertyChanged("Title");
            }
        }

        ObservableCollection<ToolbarModel> _toolbar;
        public ObservableCollection<ToolbarModel> Toolbar
        {
            get { return _toolbar; }
            set
            {
                _toolbar = value;
                if (MapMainView.Instance != null)
                {
                    MapMainView.Instance.Toolbar = _toolbar;
                }
                OnPropertyChanged("Toolbar");
            }
        }

        public PageControl()
        {
            Toolbar = new ObservableCollection<ToolbarModel>();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            OnToolbar();

            if (MapMainView.Instance != null)
            {
                MapMainView.Instance.Title = Title;
                if (Toolbar.Count > 0)
                {
                    Toolbar.Clear();
                }

            }
        }

        public virtual void OnToolbar()
        {
            if (MapMainView.Instance != null)
            {
                if (Toolbar == null)
                {
                    MapMainView.Instance.Toolbar = null;
                    return;
                }

                MapMainView.Instance.Toolbar = new ObservableCollection<ToolbarModel>(Toolbar);

            }
        }

    }
}
