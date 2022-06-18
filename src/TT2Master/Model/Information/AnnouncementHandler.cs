using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master.Model.Information
{
    public static class AnnouncementHandler
    {
        private static int _myClientVersion = ChangelogList.Changelog.Max(x => x.Version);

        private const string _postAnnouncementsEndpointUrl = @"";

        public static bool IsHavingUnseenItems { get; private set; } = false;

        public static bool IsHavingUpdateRequiredItem { get; private set; } = false;

        public static List<DbAnnouncement> Announcements { get; private set; } = new List<DbAnnouncement>();


        /// <summary>
        /// Downloads announcements and stores them in DB
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DownloadAnnouncementsAsync()
        {
            try
            {
                var newAnnouncements = new List<Announcement>();
                // get new items from server
                if (CrossConnectivity.Current.IsConnected)
                {
                    newAnnouncements = await GetAnnouncementsFromServerAsync();
                }

                // store new items in db
                foreach (var item in newAnnouncements)
                {
#if RELEASE
                    if (item.Id >= 20000)
                    {
                        continue;
                    } 
#endif

                    await App.DBRepo.UpsertDbAnnouncementAsync(item.GetNewDbAnnouncement());
                }

                await UpdateLocalAnnouncementsAsync();

                return true;

            }
            catch (Exception)
            {
                //Loggers.Logger.WriteToLogFile($"Error on DownloadAnnouncementsAsync: {ex.Message} {ex.Data}");
                return false;
            }
        }

        public static async Task<bool> UpdateLocalAnnouncementsAsync()
        {
            try
            {
                // get all items from db and fill property
                var dbItems = await App.DBRepo.GetAllDbAnnouncementAsync();

                FillAnnouncements(dbItems);

                // check fuckup fallback
                var fucks = Announcements.Where(x => x.ID > 9000 && x.Header == "FUCK")?.ToList();
                if (fucks != null && fucks.Count > 0)
                {
                    foreach (var item in dbItems)
                    {
                        if (item.IsUpdateRequired)
                        {
                            await App.DBRepo.DeleteDbAnnouncementByIDAsync(item.ID);
                        }
                    }

                    foreach (var item in fucks)
                    {
                        await App.DBRepo.DeleteDbAnnouncementByIDAsync(item.ID);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                //Loggers.Logger.WriteToLogFile($"Error on UpdateLocalAnnouncementsAsync: {ex.Message} {ex.Data}");
                return false;
            }
        }

        private static async Task<List<Announcement>> GetAnnouncementsFromServerAsync()
        {
            try
            {
                var req = await GetRequestAsync();

                var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                
                if (string.IsNullOrEmpty(_postAnnouncementsEndpointUrl))
                {
                    throw new Exception("Post announcements endpoint url is empty. Needs to be done (point to azure function)");
                }

                using var resonse = await client.PostAsync(_postAnnouncementsEndpointUrl, content);
                using var respContent = resonse.Content;
                string tr = await respContent.ReadAsStringAsync();
                return  JsonConvert.DeserializeObject<List<Announcement>>(tr) ?? new List<Announcement>();
            }
            catch (Exception)
            {
                //Loggers.Logger.WriteToLogFile($"Error on GetAnnouncementsFromServerAsync: {ex.Message} {ex.Data}");
                return new List<Announcement>();
            }
        }

        private static async Task<AnnouncementRequest> GetRequestAsync()
        {
            var os = Device.RuntimePlatform == Device.Android ? ClientOs.Android : ClientOs.iOS;

            return new AnnouncementRequest
            {
                CurrentAppVersion = _myClientVersion,
                Os = os,
                LatestClientId = await App.DBRepo.GetLatestDbAnnouncementID(),
            };
        }

        public static bool IsUpdateRequired(DbAnnouncement itemToCheck)
        {
            var os = Device.RuntimePlatform == Device.Android ? ClientOs.Android : ClientOs.iOS;

            if (   itemToCheck.IsUpdateRequired
                && _myClientVersion >= itemToCheck.AppVersionMin
                && _myClientVersion <= itemToCheck.AppVersionMax
                && (itemToCheck.Os == ClientOs.All || itemToCheck.Os == os))
            {
                return true;
            }

            return false;
        }

        public static bool IsUpdateRequired(Announcement itemToCheck)
        {
            var os = Device.RuntimePlatform == Device.Android ? ClientOs.Android : ClientOs.iOS;

            if (itemToCheck.IsUpdateRequired
                && _myClientVersion >= itemToCheck.AppVersionMin
                && _myClientVersion <= itemToCheck.AppVersionMax
                && (itemToCheck.Os == ClientOs.All || itemToCheck.Os == os))
            {
                return true;
            }

            return false;
        }

        private static void FillAnnouncements(List<DbAnnouncement> dbAnnouncements)
        {
            Announcements = dbAnnouncements;

            if (dbAnnouncements.Where(x => !x.IsSeen).Any())
            {
                IsHavingUnseenItems = true;
            }

            if(dbAnnouncements.Where(x => IsUpdateRequired(x)).Any())
            {
                IsHavingUpdateRequiredItem = true;
            }
        }

        public static DbAnnouncement GetNewDbAnnouncement(this Announcement announcement)
        {
            return new DbAnnouncement
            {
                ID = announcement.Id,
                Os = announcement.Os,
                AppVersionMin = announcement.AppVersionMin,
                AppVersionMax = announcement.AppVersionMax,
                Header = announcement.Header,
                Body = announcement.Body,
                IsSeen = false,
                IsUpdateRequired = announcement.IsUpdateRequired,
            };
        }
    }
}
