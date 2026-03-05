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
using CommunityToolkit.Mvvm.Messaging;
using Security.Data.Models;
using Security.Mobile.Services;

namespace Security.Mobile.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool? passwordMatch;

        public Credential Credential { get; set; } = new();
        public string ConfirmedPassword { get; set; } = "";

        [RelayCommand]
        public async Task Register()
        {
            PasswordMatch = Credential.Password == ConfirmedPassword;
            if (!PasswordMatch.Value)
                return;

            try
            {
                await data.authenticationRepository.Register(Credential);
                await toast.ShowShortToast("Votre compte a bien été créé");
                WeakReferenceMessenger.Default.Send(new RegisteredMessage(Credential)); // Prévenir que l'utilisateur vient de s'enregistrer
                await navigation.Pop();
            }
            catch
            {
                //TODO Message d'erreur
            }
        }
    }
}