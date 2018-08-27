using Kitablet.Windows;
using global::Windows.UI.Notifications;

[assembly: Xamarin.Forms.Dependency(typeof(ToastNotificationManagerRenderer))]
namespace Kitablet.Windows
{
    public class ToastNotificationManagerRenderer : IMessage
    {
        public void LongAlert(string message)
        {
            var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var toeastElement = notificationXml.GetElementsByTagName("text");
            toeastElement[0].AppendChild(notificationXml.CreateTextNode(message));
            var toastNotification = new ToastNotification(notificationXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }

        public void ShortAlert(string message)
        {
            var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var toeastElement = notificationXml.GetElementsByTagName("text");
            toeastElement[0].AppendChild(notificationXml.CreateTextNode(message));
            var toastNotification = new ToastNotification(notificationXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }
    }
}
