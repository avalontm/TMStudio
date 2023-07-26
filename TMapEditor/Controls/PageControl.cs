using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMapEditor.Models;
using TMapEditor.Views.MainPage;

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
                if (MainViewControl.Instance != null)
                {
                    MainViewControl.Instance.Title = _title;
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
                if (MainViewControl.Instance != null)
                {
                    MainViewControl.Instance.Toolbar = _toolbar;
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

            if (MainViewControl.Instance != null)
            {
                MainViewControl.Instance.Title = Title;
                if (Toolbar.Count > 0)
                {
                    Toolbar.Clear();
                }

            }
        }

        public virtual void OnToolbar()
        {
            if (MainViewControl.Instance != null)
            {
                if (Toolbar == null)
                {
                    MainViewControl.Instance.Toolbar = null;
                    return;
                }

                MainViewControl.Instance.Toolbar = new ObservableCollection<ToolbarModel>(Toolbar);

            }
        }

    }
}
