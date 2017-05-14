using System;

namespace MyNotepad.Services
{
    public class ToastService
    {
        public void ShowToast(Models.FileInfo file, string message = "Success")
        {
            var image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Template10.png";

            if (file.Ref == null)
            {
                var content = new NotificationsExtensions.Toasts.ToastContent()
                {
                    Launch = "",
                    Visual = new NotificationsExtensions.Toasts.ToastVisual()
                    {
                        TitleText = new NotificationsExtensions.Toasts.ToastText()
                        {
                            Text = "User Cancelled."
                        },
                        BodyTextLine1 = new NotificationsExtensions.Toasts.ToastText()
                        {
                            Text = "File save cancelled."
                        },
                        AppLogoOverride = new NotificationsExtensions.Toasts.ToastAppLogo()
                        {
                            Crop = NotificationsExtensions.Toasts.ToastImageCrop.Circle,
                            Source = new NotificationsExtensions.Toasts.ToastImageSource(image)
                        }
                    },
                    Audio = new NotificationsExtensions.Toasts.ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new Windows.UI.Notifications.ToastNotification(content.GetXml());
                var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
                notifier.Show(notification);
            }
            else
            {
                var content = new NotificationsExtensions.Toasts.ToastContent()
                {
                    Launch = file.Ref.Path,
                    Visual = new NotificationsExtensions.Toasts.ToastVisual()
                    {
                        TitleText = new NotificationsExtensions.Toasts.ToastText()
                        {
                            Text = message
                        },
                        BodyTextLine1 = new NotificationsExtensions.Toasts.ToastText()
                        {
                            Text = file.Name
                        },
                        AppLogoOverride = new NotificationsExtensions.Toasts.ToastAppLogo()
                        {
                            Crop = NotificationsExtensions.Toasts.ToastImageCrop.Circle,
                            Source = new NotificationsExtensions.Toasts.ToastImageSource(image)
                        }
                    },
                    Audio = new NotificationsExtensions.Toasts.ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new Windows.UI.Notifications.ToastNotification(content.GetXml());
                var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
                notifier.Show(notification);
            }
            
        }
        public void ShowToast(string message = "File not found")
        {
            var image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Template10.png";

            var content = new NotificationsExtensions.Toasts.ToastContent()
                {
                    Launch = "",
                    Visual = new NotificationsExtensions.Toasts.ToastVisual()
                    {
                        TitleText = new NotificationsExtensions.Toasts.ToastText()
                        {
                            Text = "Request Cancelled."
                        },
                        BodyTextLine1 = new NotificationsExtensions.Toasts.ToastText()
                        {
                            Text = "File not found."
                        },
                        AppLogoOverride = new NotificationsExtensions.Toasts.ToastAppLogo()
                        {
                            Crop = NotificationsExtensions.Toasts.ToastImageCrop.Circle,
                            Source = new NotificationsExtensions.Toasts.ToastImageSource(image)
                        }
                    },
                    Audio = new NotificationsExtensions.Toasts.ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new Windows.UI.Notifications.ToastNotification(content.GetXml());
                var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
                notifier.Show(notification);
        }
    }
}
