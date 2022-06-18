using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TT2Master
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EquipAdvSetPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
		public EquipAdvSetPopupPage()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing() => base.OnAppearing();

        protected override void OnDisappearing() => base.OnDisappearing();

        #region Animation
        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin() => base.OnAppearingAnimationBegin();

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd() => base.OnAppearingAnimationEnd();

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin() => base.OnDisappearingAnimationBegin();

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd() => base.OnDisappearingAnimationEnd();

        protected override Task OnAppearingAnimationBeginAsync() => base.OnAppearingAnimationBeginAsync();

        protected override Task OnAppearingAnimationEndAsync() => base.OnAppearingAnimationEndAsync();

        protected override Task OnDisappearingAnimationBeginAsync() => base.OnDisappearingAnimationBeginAsync();

        protected override Task OnDisappearingAnimationEndAsync() => base.OnDisappearingAnimationEndAsync();
        #endregion

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
    }
}