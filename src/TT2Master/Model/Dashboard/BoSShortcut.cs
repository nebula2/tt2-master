using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Navigation;
using TT2Master.Model.Tournament;
using TT2Master.Resources;

namespace TT2Master.Model.Dashboard
{
    [ShortcutAttribute(4, "BoSPercent", true)]
    public class BoSShortcut : DashboardShortcut
    {
        public override int ID { get; set; } = 4;

        public override string Header { get; set; } = AppResources.BoSPercent;

        private bool _hasContent = true;
        public override bool HasContent { get => _hasContent; set => SetProperty(ref _hasContent, value); }

        private string _textColor;
        public new string TextColor { get => _textColor; set=> SetProperty(ref _textColor, value); }

        public override ICommand ItemTappedAction { get; protected set; }
        public override Func<Task> LoadItem { get; set; }

        public BoSShortcut(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            TextColor = "#FFFFFF";

            LoadItem = async () =>
            {
                try
                {
                    #region set bos value
                    ArtifactHandler.LoadArtifacts();
                    ArtifactHandler.FillArtifacts(App.Save);

                    double perc = LocalSettingsORM.UseMasterBoSDisplay
                                ? ArtifactHandler.CalculateLifeTimeSpentPercentage(ArtifactHandler.Artifacts.Where(x => x.ID == "Artifact22").First().RelicsSpent)
                                : ArtifactHandler.CalculateLifeTimeSpentPercentageForDummies(ArtifactHandler.Artifacts.Where(x => x.ID == "Artifact22").First().RelicsSpent);


                    Content = $"{Math.Round(perc, 2)} %";
                    #endregion

                    #region set bos text color

                    double bosGoal = 0;

                    var settings = await App.DBRepo.GetArtOptSettingsByID("1");

                    if (settings != null)
                    {
                        bosGoal = TournamentHandler.IsPlayerInTournament() ? settings.BosTourneyRoyalty : settings.BoSRoyalty;
                    }

                    TextColor = perc < bosGoal ? "#C8175A" : "#FFFFFF";
                    #endregion

                }
                catch (Exception e)
                {
                    Content = "?";
                    Logger.WriteToLogFile($"ERROR BoSShortcut:{e.Message}");
                }
            };

            ItemTappedAction = new DelegateCommand(async () =>
            {
                try
                {
                    string bosDisplayChoice = LocalSettingsORM.UseMasterBoSDisplay ? AppResources.UseIngameBoSDisplay : AppResources.UseMasterBoSDisplay;

                    // promt user
                    string response = await _dialogService.DisplayActionSheetAsync(AppResources.InfoHeader
                    , AppResources.CancelText
                    , AppResources.DestroyText
                    , new string[] {
                        AppResources.CopyLtrValue
                        , bosDisplayChoice
                    });

                    // handle user cancel
                    if (response == AppResources.CancelText || response == AppResources.DestroyText)
                    {
                        return;
                    }

                    // copy LTR value
                    if (response == AppResources.CopyLtrValue)
                    {
                        double ltr = App.Save.CurrentRelics
                        + ArtifactHandler.GetLifeTimeSpentOnAll()
                        + ArtifactCostHandler.CostSum(ArtifactHandler.Artifacts.Where(x => x.Level > 0).Count(), ArtifactHandler.Artifacts.Where(x => x.EnchantmentLevel > 0).Count());

                        CrossClipboard.Current.SetText(ltr.ToString());
                        await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, $"LTR {AppResources.Copied}", AppResources.OKText);
                        return;
                    }

                    // change bos display setting and reload
                    if (response == bosDisplayChoice)
                    {
                        LocalSettingsORM.UseMasterBoSDisplay = !LocalSettingsORM.UseMasterBoSDisplay;
                        await LoadItem();
                        return;
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ERROR BoSShortcut:{e.Message}");
                    return;
                }
            });
        }
    }
}
