using Plugin.LocalNotification;
using System;
using System.Threading.Tasks;
using TT2Master.Droid;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(NotificationHelper))]
namespace TT2Master.Droid
{
    public class NotificationHelper : ISendNotification
    {
        public void ShowNotification(string header, string content)
        {
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = header,
                Description = content,
            };
            _ = Task.Run(() => NotificationCenter.Current.Show(notification));
            //LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.ic_stat_notifications_active;
            //CrossLocalNotifications.Current.Show(header, content, 101, DateTime.Now);
        }
    }
}
