using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Security.Data.Repositories;
using Security.Data.Repositories.Mock;
using Security.Mobile.Services;

namespace Security.Mobile.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DataService dataService = new();

        [ObservableProperty]
        private bool? isLogin; // Modifier en cours de route

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        [RelayCommand]
        public async Task Login()
        {
            //Debug.WriteLine("----- " + Email + Password);
            try { IsLogin = await dataService.authenticationRepository.Login(Email, Password); }
            catch { IsLogin = false; }
        }

        [RelayCommand]
        public async Task Register()
        {
        }
    }
}