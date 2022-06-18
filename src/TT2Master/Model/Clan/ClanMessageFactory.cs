using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// Handles the interaction from savefile clanmessages to those in db
    /// </summary>
    public static class ClanMessageFactory
    {

        /// <summary>
        /// Deletes old <see cref="ClanMessage"/> from DB
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteOldClanMessageAsync()
        {
            try
            {
                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync"));
                int maxAmount = JfTypeConverter.ForceInt(LocalSettingsORM.GetClanMessageAmount());

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync: maxAmount: {maxAmount}"));

                var items = await App.DBRepo.GetAllClanMessageAsync();

                if (items == null)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync: items is null: {App.DBRepo.StatusMessage}"));
                }

                if (items.Count <= maxAmount)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync: {items.Count} items in DB. less than maxAmount. Done"));
                    return true;
                }

                var delList = items.OrderBy(x => x.MessageID).Take(items.Count - maxAmount).ToList();

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync: having {delList.Count} items to delete"));

                int del = await App.DBRepo.DeleteClanMessages(delList);

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync: deleted {del} items. Done"));

                return true;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"DeleteOldClanMessageAsync Error: {e.Message}"));
                return false;
            }
            
        }

        /// <summary>
        /// Saves new clanmessages from savefile
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> SaveNewClanMessages()
        {
            OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages Start"));

            try
            {
                #region Validation
                if (App.Save.ThisClan == null)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: App.Save.ThisClan is null."));
                    return false;
                }

                if (App.Save.ThisClan.Messages == null)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: App.Save.ThisClan.Messages is null."));
                    return false;
                }
                #endregion

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: Clandata is valid"));

                #region Save data
                // get latest message id
                int lastId = await App.DBRepo.GetLastKnownClanMsgID();

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: last known messageId: {lastId}"));

                // insert all messages with higher id
                var newMsgList = App.Save.ThisClan.Messages.Where(x => x.MessageID > lastId).ToList();

                // error in dbrepo
                if (newMsgList == null)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: newMsgList is null. {App.DBRepo.StatusMessage}"));
                    return false;
                }

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: got {newMsgList.Count} messages to store"));


                // no new messages or different clan with higher clanmsg-id
                if (newMsgList.Count == 0)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: newMsgList.Count is 0."));
                    return true;
                }

                // save data
                int savedRecords = await App.DBRepo.AddNewClanMessageListAsync(newMsgList);

                // no data is saved. probably due to an error
                if (savedRecords == 0)
                {
                    OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: savedRecords is 0. {App.DBRepo.StatusMessage}"));
                    return false;
                }
                #endregion

                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"SaveNewClanMessages: saved {savedRecords} records. i am done."));

                // everything is fine
                return true;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("ClanMessageFactory", new InformationEventArgs($"Error in SaveNewClanMessages: {e.Message}"));
                return false;
            }
            

        }

        /// <summary>
        /// Deletes old clan data by checking the current clancode
        /// </summary>
        /// <returns></returns>
        public static async Task<int> DeleteOldClanDataAsync() => await App.DBRepo.DeleteOldClanMsgData(App.Save.ThisPlayer.ClanCurrent);

        #region E+D
        /// <summary>
        /// Delegate for <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when I think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;
        #endregion
    }
}
