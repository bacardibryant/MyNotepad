using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using System;
using Windows.UI.Notifications;

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
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            AppLogoOverride = new ToastGenericAppLogo
                            {
                                HintCrop = ToastGenericAppLogoCrop.Circle,
                                Source = image
                            },
                            Children =
                            {
                                new AdaptiveText {Text = "File save cancelled." }
                            },
                            Attribution = new ToastGenericAttributionText
                            {
                                Text = "User Cancelled."
                            },
                        }
                    },
                    Audio = new NotificationsExtensions.Toasts.ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new ToastNotification(content.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
                
            }
            else
            {
                
                var content = new NotificationsExtensions.Toasts.ToastContent()
                {
                    Launch = file.Ref.Path,
                    Visual = new NotificationsExtensions.Toasts.ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            AppLogoOverride = new ToastGenericAppLogo
                            {
                                HintCrop = ToastGenericAppLogoCrop.Circle,
                                Source = image
                            },
                            Children =
                            {
                                new AdaptiveText {Text = file.Name }
                            },
                            Attribution = new ToastGenericAttributionText
                            {
                                Text = message
                            },
                        }
                    },
                    Audio = new NotificationsExtensions.Toasts.ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new ToastNotification(content.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
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
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            AppLogoOverride = new ToastGenericAppLogo
                            {
                                HintCrop = ToastGenericAppLogoCrop.Circle,
                                Source = image
                            },
                            Children =
                            {
                                new AdaptiveText {Text = "File not found." }
                            },
                            Attribution = new ToastGenericAttributionText
                            {
                                Text = "Request Cancelled."
                            },
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
