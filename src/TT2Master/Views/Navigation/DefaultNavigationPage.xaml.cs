
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TT2Master
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultNavigationPage : NavigationPage
    {
        public DefaultNavigationPage()
        {
            InitializeComponent();

            this.Popped += DefaultNavigationPage_Popped;

            Pushed += DefaultNavigationPage_Pushed;
        }

        private void DefaultNavigationPage_Pushed(object sender, NavigationEventArgs e)
        {

        }

        private void DefaultNavigationPage_Popped(object sender, NavigationEventArgs e)
        {

        }
    }
}