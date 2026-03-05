using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Security.Data.Models;
using Security.Data.Repositories;
using Security.Data.Repositories.Mock;
using Security.Mobile.Services;

namespace Security.Mobile.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool? isLogin; // Modifié en cours de route

        [ObservableProperty]
        private Credential credential = new();


        [RelayCommand]
        public async Task Load()
        {
            Credential = new Credential()
            {
                Email = await SecureStorage.GetAsync("Email"),
                Password = await SecureStorage.GetAsync("Password")
            };
        }

        [RelayCommand]
        public async Task Login()
        {
            try
            {
                IsLogin = await data.authenticationRepository.Login(Credential);
                
                await SecureStorage.SetAsync("Email", Credential.Email);
                await SecureStorage.SetAsync("Password", Credential.Password);

                await navigation.Push(resourceListView);
            }
            catch { IsLogin = false; }
        }

        [RelayCommand]
        public async Task Register()
        {
            WeakReferenceMessenger.Default.Register<RegisteredMessage>(this, (_, m) =>
            {   // Ce code est exécuté quand on reçoit le message uniquement
                WeakReferenceMessenger.Default.UnregisterAll(this); // se désabonner du message
                Credential = m.Value;
            });
            await navigation.Push(registerView);
        }
    }
}