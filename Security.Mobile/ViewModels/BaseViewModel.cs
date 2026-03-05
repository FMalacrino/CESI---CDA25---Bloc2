using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Security.Data.Models;
using Security.Mobile.Services;

namespace Security.Mobile.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        protected const string registerView = "RegisterView";
        protected const string resourceListView = "ResourceListView";

        protected static readonly DataService data = new();
        protected static readonly ToastService toast = new();
        protected static readonly NavigationService navigation = new();

        public class RegisteredMessage(Credential value) : ValueChangedMessage<Credential>(value)
        { }
    }
}