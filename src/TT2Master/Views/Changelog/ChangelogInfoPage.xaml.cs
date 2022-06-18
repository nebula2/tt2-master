using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TT2Master
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangelogInfoPage : ContentPage
	{
		public ChangelogInfoPage()
		{
            InitializeComponent();

            PrintNavStack();
		}

        private void PrintNavStack()
        {
            Debug.WriteLine(Navigation.NavigationStack.Count);
        }

        protected override void OnAppearing()
        {
            Debug.WriteLine("ChangelogInfoPage.OnAppearing()");
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine("ChangelogInfoPage.OnDisappearing()");
            base.OnDisappearing();
        }

        protected override void OnBindingContextChanged()
        {
            PrintNavStack();
            Debug.WriteLine("ChangelogInfoPage.OnBindingContextChanged()");
            base.OnBindingContextChanged();
        }
    }
}