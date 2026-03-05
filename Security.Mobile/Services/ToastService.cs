using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Security.Mobile.Services
{
    public class ToastService
    {
        public async Task ShowShortToast(string message)
        {
            await ShowToast(message, ToastDuration.Short);
        }

        public async Task ShowToast(string message, ToastDuration duration)
        {
            var toast = Toast.Make(message, duration);
            await toast.Show();
        }
    }
}