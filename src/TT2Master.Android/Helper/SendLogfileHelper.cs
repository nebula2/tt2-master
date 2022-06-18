using Android.Content;
using Android.OS;
using System;
using System.Collections.Generic;
using TT2Master.Droid;
using TT2Master.Loggers;
using Xamarin.Forms;

[assembly: Dependency(typeof(SendLogfileHelper))]
namespace TT2Master.Droid
{
    public class SendLogfileHelper : ISendLogfile
    {
        /// <summary>
        /// Sends a Mail with logs attached
        /// </summary>
        public bool SendLogfile()
        {
            try
            {
                #region Getting Logfile-Names
                var helper = new DirectoryHelper();
                var dbPathHelper = new DBPathHelper();
                string startLogfilename = helper.GetDocumentName(Logger.LogName);
                string optLogfilename = helper.GetDocumentName(OptimizeLogger.LogName);
                string widgetLogfilename = helper.GetDocumentName(WidgetLogger.LogName);
                string rpmLogfilename = helper.GetDocumentName(RpMLogger.LogName);
                string savefilePath = dbPathHelper.DBPath("mySave.adat");
                #endregion

                #region Adding existant logfiles to attachments
                var attachments = new List<string>();

                //StartLog
                if (System.IO.File.Exists(startLogfilename))
                {
                    attachments.Add(startLogfilename);
                }

                //OptimizerLog
                if (System.IO.File.Exists(optLogfilename))
                {
                    attachments.Add(optLogfilename);
                }

                //WidgetLog
                if (System.IO.File.Exists(widgetLogfilename))
                {
                    attachments.Add(widgetLogfilename);
                }

                //RpM-Log
                if (System.IO.File.Exists(rpmLogfilename))
                {
                    attachments.Add(rpmLogfilename);
                }

                // save file
                if (System.IO.File.Exists(savefilePath))
                {
                    attachments.Add(savefilePath);
                }
                #endregion

                Email("????@????.de", "TT2Master", attachments);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"SendLogfileHelper EX: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Helper
        /// </summary>
        /// <param name="emailTo"></param>
        /// <param name="subject"></param>
        /// <param name="emailText"></param>
        /// <param name="filePaths"></param>
        public static void Email(string emailTo, string subject, List<string> filePaths)
        {
            try
            {
                var builder = new StrictMode.VmPolicy.Builder();
                StrictMode.SetVmPolicy(builder.Build());

                var email = new Intent(Intent.ActionSendMultiple);
                email.SetType("text/plain");
                email.PutExtra(Intent.ExtraEmail, new string[] { emailTo });
                email.PutExtra(Android.Content.Intent.ExtraSubject, subject);

                var uris = new List<IParcelable>();

                filePaths.ForEach(file =>
                {
                    var fileIn = new Java.IO.File(file);
                    var uri = Android.Net.Uri.FromFile(fileIn);
                    uris.Add(uri);
                });

                email.PutParcelableArrayListExtra(Intent.ExtraStream, uris);

                var intent = Intent.CreateChooser(email, "TT2Master");
                intent.AddFlags(ActivityFlags.NewTask);

                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"Exceptionmsg in Email:  {ex.Message}; EX_Data {ex.Data}");
            }
        }
    }
}