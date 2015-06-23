using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace KinderChat_UAP.Tools
{
    public static class ChatDebugger
    {
        public static async Task SendMessageDialogAsync(string message, Exception ex)
        {
            var dialog = new MessageDialog((string.Concat(message, Environment.NewLine, Environment.NewLine, ex.Message)));
            await dialog.ShowAsync();
        }
    }
}
