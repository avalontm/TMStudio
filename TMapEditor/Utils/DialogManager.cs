using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMStudio.Dialogs;

namespace TMStudio.Utils
{
    public static  class DialogManager
    {
        public static async Task Show(string message = "")
        {
            WaitDialogView.Instance.Show(message);
            await Task.Delay(10);
        }

        public static async Task Close()
        {
            WaitDialogView.Instance.Close();
            await Task.Delay(10);
        }

        public static async Task<bool> Display(string title, string message, string accept, string cancel = "")
        {
            await DialogView.Instance.Show(title, message, accept, cancel);
            return DialogView.Instance.Response;
        }
    }
}
