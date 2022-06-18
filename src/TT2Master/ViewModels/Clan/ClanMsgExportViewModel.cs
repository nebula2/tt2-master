using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;

namespace TT2Master
{
    public class ClanMsgExportViewModel : ViewModelBase
    {
        #region Properties
        private bool _isMessageExportWished = true;
        public bool IsMessageExportWished { get => _isMessageExportWished; set => SetProperty(ref _isMessageExportWished, value); }

        private bool _isBuildExportWished = true;
        public bool IsBuildExportWished { get => _isBuildExportWished; set => SetProperty(ref _isBuildExportWished, value); }

        private bool _isRaidExportWished = true;
        public bool IsRaidExportWished { get => _isRaidExportWished; set => SetProperty(ref _isRaidExportWished, value); }

        private bool _isRaidResultExportWished = true;
        public bool IsRaidResultExportWished { get => _isRaidResultExportWished; set => SetProperty(ref _isRaidResultExportWished, value); }

        private bool _isMemberExportWished = true;
        public bool IsMemberExportWished { get => _isMemberExportWished; set => SetProperty(ref _isMemberExportWished, value); }

        private bool _isClipboardExportWished = false;
        public bool IsClipboardExportWished { get => _isClipboardExportWished; set => SetProperty(ref _isClipboardExportWished, value); }

        private double _maxMsgAmount;
        /// <summary>
        /// Max Amount of Messages to export
        /// </summary>
        public double MaxMsgAmount
        {
            get => _maxMsgAmount;
            set
            {
                if (value != _maxMsgAmount)
                {
                    SetProperty(ref _maxMsgAmount, value);
                }
            }
        }

        public ICommand ExportCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        public ClanMsgExportViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.ExportTitle;

            _dialogService = dialogService;
            _navigationService = navigationService;

            MaxMsgAmount = 99999;

            CancelCommand = new DelegateCommand(async () => 
            {
                var result = await _navigationService.GoBackAsync();
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            } );
            ExportCommand = new DelegateCommand(async () => await ExportExecute());
        }
        #endregion

        #region Command methods
        private async Task<bool> ExportExecute()
        {
            string strToExport = await GetClanMessagesCSVString();

            if (!IsClipboardExportWished)
            {
                //filename
                string name = $"{DateTime.Now.ToString("yyyyMMdd")}_clanmessages.csv";
                //get path and dir
                string path = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPathName(name);
                string dir = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPath();

                FileHelper.DeleteFile(dir, path);
                string savedTo = FileHelper.WriteFileToDownloads(strToExport, name);

                await ToastSender.SendToastAsync(AppResources.FileSavedToText + $" {savedTo}", _dialogService);
            }
            else
            {
                CrossClipboard.Current.SetText(strToExport);
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
            }

            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Little export of clan messages
        /// </summary>
        private async Task<string> GetClanMessagesCSVString()
        {
            try
            {
                var exp = new List<ExportClanMessage>();

                var storedMsgs = await App.DBRepo.GetAllClanMessageAsync();
                if (storedMsgs == null) return "no items found";

                int counter = 0;

                storedMsgs = storedMsgs.OrderByDescending(x => x.MessageID).ToList();

                //populate list
                foreach (var item in storedMsgs)
                {
                    var member = new ExportClanMessage()
                    {
                        MessageID = item.MessageID,
                        ClanMessageType = item.ClanMessageType,
                        PlayerIdFrom = item.PlayerIdFrom,
                        MemberName = item.MemberName,
                        Message = item.Message,
                        TimeStamp = item.TimeStamp,
                    };

                    counter++;

                    if(counter > MaxMsgAmount)
                    {
                        continue;
                    }

                    #region Export decision
                    // Message Export
                    if (IsMessageExportWished && member.ClanMessageType == "Message")
                    {
                        exp.Add(member);
                        continue;
                    }

                    //Build Export
                    if(IsBuildExportWished && member.ClanMessageType == "BuildShare")
                    {
                        exp.Add(member);
                        continue;
                    }

                    //Raid Export
                    if(IsRaidExportWished && member.ClanMessageType.ToUpper().IndexOf("RAID") >= 0 && member.ClanMessageType != "RaidAttackSummaryShare")
                    {
                        exp.Add(member);
                        continue;
                    }

                    // Raid result export
                    if (IsRaidResultExportWished && member.ClanMessageType == "RaidAttackSummaryShare")
                    {
                        exp.Add(member);
                        continue;
                    }

                    //Member Export
                    if ( IsMemberExportWished && (member.ClanMessageType != "BuildShare"
                        && member.ClanMessageType != "Message"
                        && !(member.ClanMessageType.ToUpper().IndexOf("RAID") >= 0)
                        && member.ClanMessageType != "MakeItRain"))
                    {
                        exp.Add(member);
                        continue;
                    }
                    #endregion
                }

                // build export string
                string expStr = ExportClanMessage.GetHeaderLine();

                foreach (var item in exp)
                {
                    expStr += item.ToString();
                }

                return expStr;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion
    }
}