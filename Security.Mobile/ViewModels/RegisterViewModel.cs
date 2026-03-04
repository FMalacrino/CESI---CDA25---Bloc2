using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Security.Mobile.Services;

namespace Security.Mobile.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly DataService dataService = new();

        [ObservableProperty]
        private bool? passwordMatch;

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmedPassword { get; set; } = "";

        [RelayCommand]
        public async Task Register()
        {
            if (Password != ConfirmedPassword)
            {
                PasswordMatch = false;
                return;
            }
            PasswordMatch = true;

            //Debug.WriteLine("----- " + Email + Password);
            try { 
                await dataService.authenticationRepository.Register(Email, Password);
                string text = "Enregistremet réussi";
                ToastDuration duration = ToastDuration.Short;
                //double fontSize = 14;

                var toast = Toast.Make(text, duration);

                await toast.Show();
            }
            catch { }
        }
    }
}