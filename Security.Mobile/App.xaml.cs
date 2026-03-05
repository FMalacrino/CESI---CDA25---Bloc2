using HorusStudio.Maui.MaterialDesignControls;
using Security.Mobile.Views;

namespace Security.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MaterialDesignControls.InitializeComponents();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}