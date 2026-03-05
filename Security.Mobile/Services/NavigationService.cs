using Security.Mobile.Views;

namespace Security.Mobile.Services
{
    public class NavigationService
    {
        //Dictionary<string, Type> pages = [];

        //public void Add(string view, Type type)
        //{
        //    pages.Add(view, type);
        //}

        public async Task Push(string view)
        {
            await Shell.Current.GoToAsync(view);

            // Si on n'utilise pas le Shell
            //Type type = pages[view];
            //Page page =   Activator.CreateInstance(type) as Page;
            //await Shell.Current.Navigation.PushAsync(page);
        }

        public async Task Pop()
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}