using Prism.Navigation;
using System.Linq;
using TT2Master.Model.Arti;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public class ArtifactDetailViewModel : ViewModelBase
    {
        #region Properties
        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        private Artifact _thisArtifact;
        public Artifact ThisArtifact { get => _thisArtifact; set => SetProperty(ref _thisArtifact, value); }
        #endregion

        #region Ctor
        public ArtifactDetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.ArtifactDetailHeader;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// When navigating to this - load some stuff
        /// </summary>
        /// <param name="parameters">Artifact</param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            ThisArtifact = string.IsNullOrEmpty((string)parameters["art"])
                ? new Artifact()
                : ArtifactHandler.Artifacts.Where(x => x.ID == (string)parameters["art"]).FirstOrDefault();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}