using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TMFormat.Helpers;

namespace TMStudio.Models
{
    public class PropertiesModel : ObservableObject
    {
        object? _value;
        public string Id { get; set; }
        public string? Name { get; set; }
        public object? Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }
        public object? Items { get; set; }
        public int Type { get; set; }
        public ICommand? Action { get; set; }
        public object? Bind { get; set; }
        public bool IsEnabled { get; set; }
    }
}
