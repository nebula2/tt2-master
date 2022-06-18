using Prism.Navigation;

namespace TT2Master
{
    public class DefaultNavigationViewModel : ViewModelBase
    {
        public DefaultNavigationViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public override void OnNavigatedFrom(INavigationParameters parameters) => base.OnNavigatedFrom(parameters);

        public override void OnNavigatedTo(INavigationParameters parameters) => base.OnNavigatedTo(parameters);
    }
}