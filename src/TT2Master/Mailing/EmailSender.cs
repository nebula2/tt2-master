using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Loggers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TT2Master.Mailing
{
    /// <summary>
    /// Sends emails using Xamarin.Essentials
    /// </summary>
    public class EmailSender
    {
        /// <summary>
        /// Send error mail to dev with logfiles attached
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendErrorEmail()
        {
            bool success = false;
            try
            {
                success = await SendEssentialsMailAsync();
            }
            catch
            {
                if(Device.RuntimePlatform == Device.Android)
                success = Xamarin.Forms.DependencyService.Get<ISendLogfile>().SendLogfile();
            }

            return success;
        }

        private List<EmailAttachment> GetAttachments()
        {
            var atts = new List<EmailAttachment>();

            string startLogfilename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName(Logger.LogName);
            if (!string.IsNullOrWhiteSpace(startLogfilename) && File.Exists(startLogfilename))
            {
                atts.Add(new EmailAttachment(startLogfilename));
            }

            string optLogfilename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName(OptimizeLogger.LogName);
            if (!string.IsNullOrWhiteSpace(optLogfilename) && File.Exists(optLogfilename))
            {
                atts.Add(new EmailAttachment(optLogfilename));
            }

            // save file
            if (!LocalSettingsORM.IsReadingDataFromSavefile)
            {
                string exportfilePath = Xamarin.Forms.DependencyService.Get<IDirectory>().GetTapTitansExportFilePath();

                if (File.Exists(exportfilePath))
                {
                    try
                    {
                        atts.Add(new EmailAttachment(exportfilePath));
                    }
                    catch { }
                }
            }

            // from here on android only
            if (Device.RuntimePlatform == Device.iOS) return atts;

            string widgetLogfilename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetWidgetLoggerPath();
            if (!string.IsNullOrWhiteSpace(widgetLogfilename) && File.Exists(widgetLogfilename))
            {
                atts.Add(new EmailAttachment(widgetLogfilename));
            }

            string rpmLogfilename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName(RpMLogger.LogName);
            if (!string.IsNullOrWhiteSpace(rpmLogfilename) && File.Exists(rpmLogfilename))
            {
                atts.Add(new EmailAttachment(rpmLogfilename));
            }

            string clanExpLogfilename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName("ClanExportLogger.txt");
            if (!string.IsNullOrWhiteSpace(clanExpLogfilename) && File.Exists(clanExpLogfilename))
            {
                atts.Add(new EmailAttachment(clanExpLogfilename));
            }

            string autoServiceLogger = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName("AutoServiceLogger.txt");
            if (!string.IsNullOrWhiteSpace(autoServiceLogger) && File.Exists(autoServiceLogger))
            {
                atts.Add(new EmailAttachment(autoServiceLogger));
            }

            // save file
            if (LocalSettingsORM.IsReadingDataFromSavefile && File.Exists(LocalSettingsORM.TT2SavefilePath))
            {
                string savefilePath = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName("mySave.adat");

                File.Copy(LocalSettingsORM.TT2SavefilePath, savefilePath, true);

                if (!string.IsNullOrWhiteSpace(savefilePath))
                {
                    atts.Add(new EmailAttachment(savefilePath));
                }
            }

            if (LocalSettingsORM.IsReadingDataFromSavefile 
                && !string.IsNullOrWhiteSpace(LocalSettingsORM.AbyssalSavefilePath) && File.Exists(LocalSettingsORM.AbyssalSavefilePath))
            {
                string savefilePath = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName("myAbyssalSave.adat");

                File.Copy(LocalSettingsORM.AbyssalSavefilePath, savefilePath, true);

                if (!string.IsNullOrWhiteSpace(savefilePath))
                {
                    atts.Add(new EmailAttachment(savefilePath));
                }
            }

            return atts;
        }

        private async Task<bool> SendEssentialsMailAsync()
        {
            try
            {
                // create message
                var message = new EmailMessage
                {
                    Subject = "TT2Master",
                    Body = "Describe your issue here",
                    To = new List<string>() { "???@????.de" },
                };

                // add attachments
                message.Attachments = GetAttachments();

                // send
                await Email.ComposeAsync(message);

                return true;
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                try
                {
                    Logger.WriteToLogFile($"EmailSender FeatureNotSupported Error: {fbsEx.Message}");
                }
                catch (Exception) { }
                return false;
            }
            catch (Exception ex)
            {
                // Some other exception occurred
                try
                {
                    Logger.WriteToLogFile($"EmailSender Error: {ex.Message}");
                }
                catch (Exception) { }
                return false;
            }
        }
    }
}
