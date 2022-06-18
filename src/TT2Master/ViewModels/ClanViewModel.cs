using Prism.Navigation;

namespace TT2Master
{
    public class ClanViewModel : ViewModelBase
    {
        public ClanViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Clan";
        }
    }
}

