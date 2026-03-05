
using Security.Mobile.Views;

namespace Security.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Ajout des vues vers lesquelles je peux naviguer
            Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        }
    }
}
