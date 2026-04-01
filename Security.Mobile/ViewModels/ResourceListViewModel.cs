using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Security.Mobile.ViewModels
{
    public partial class ResourceListViewModel : BaseViewModel
    {
        public ObservableCollection<Data.Models.Resource> Items { get; } = [];

        [RelayCommand]
        private async Task Refresh()
        {
            Items.Clear();
            foreach (var item in await data.resource.GetForUser())
                Items.Add(item);
        }

        [RelayCommand]
        private async Task ToggleFavorite(Data.Models.Resource item)
        {
            int index = Items.IndexOf(item);
            item.IsFavorite = !item.IsFavorite;
            item = await data.resource.Update(item.Id, item);
            Items[index] = item;
        }
    }
}