using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TMStudio.Models
{
    public class ItemPropertiesModel : INotifyPropertyChanged
    {
        private object _value;
        private int _index;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Type { get; set; }
        public string Name { get; set; }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BoolValue));
                OnPropertyChanged(nameof(IntValue));
                OnPropertyChanged(nameof(StringValue));
                OnPropertyChanged(nameof(DoubleValue));
            }
        }

        public object Items { get; set; }

        // Propiedades tipadas para binding
        public bool BoolValue
        {
            get
            {
                if (Value == null) return false;
                if (Value is bool b) return b;
                if (bool.TryParse(Value.ToString(), out bool result)) return result;
                if (int.TryParse(Value.ToString(), out int intVal)) return intVal != 0;
                return false;
            }
            set
            {
                Value = value;
            }
        }

        public int IntValue
        {
            get
            {
                if (Value == null) return 0;
                if (Value is int i) return i;
                if (int.TryParse(Value.ToString(), out int result)) return result;
                if (bool.TryParse(Value.ToString(), out bool boolVal)) return boolVal ? 1 : 0;
                return 0;
            }
            set
            {
                Value = value;
            }
        }

        public double DoubleValue
        {
            get
            {
                if (Value == null) return 0.0;
                if (Value is double d) return d;
                if (Value is int i) return i;
                if (double.TryParse(Value.ToString(), out double result)) return result;
                return 0.0;
            }
            set
            {
                Value = value;
            }
        }

        public string StringValue
        {
            get => Value?.ToString() ?? string.Empty;
            set => Value = value;
        }
    }
}