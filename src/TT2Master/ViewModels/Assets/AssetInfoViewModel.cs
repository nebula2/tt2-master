using Prism.Navigation;
using System.Collections.ObjectModel;
using TT2Master.Model.Assets;
using TT2Master.Resources;

namespace TT2Master.ViewModels.Assets
{
    public class AssetInfoViewModel : ViewModelBase
    {
        private void FillCollection()
        {
            AssetTypes = new ObservableCollection<AssetTypeViewModel>();

            foreach (var item in AssetManager.AssetTypes)
            {
                var atvm = new AssetTypeViewModel()
                {
                    Assets = item.Assets,
                    AssetState = item.AssetState,
                    AzureContainer = item.AzureContainer,
                    CurrentVersion = item.CurrentVersion,
                    Identifier = item.Identifier,
                    IsAssetStateSave = item.IsAssetStateSave,
                    StoredVersion = item.StoredVersion,
                };

                atvm.TranslateAssetState();

                AssetTypes.Add(atvm);
            }
        }

        private ObservableCollection<AssetTypeViewModel> _assetTypes;

        public ObservableCollection<AssetTypeViewModel> AssetTypes { get => _assetTypes; set => SetProperty(ref _assetTypes, value); }

        public AssetInfoViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.Problem;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            FillCollection();

            base.OnNavigatedTo(parameters);
        }
    }
}