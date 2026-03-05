using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Security.Data.Models;

namespace Security.Mobile.ViewModels
{
    public partial class ResourceListViewModel : BaseViewModel
    {
        public ObservableCollection<Security.Data.Models.Resource> Models { get; } = [];

        [RelayCommand]
        private async Task Refresh()
        {
            Models.Clear();
            foreach (var item in await data.resource.GetForUser())
                Models.Add(item);
        }
    }
}