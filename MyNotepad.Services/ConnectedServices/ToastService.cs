using MyNotepad.Services.Models;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using System;
using Windows.UI.Notifications;

namespace MyNotepad.ConnectedServices.Services
{
    public class ToastService
    {
        public void ShowToast(FileInfo file, string message = "Success")
        {
            var image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Template10.png";

            if (file.Ref == null)
            {
                var content = new ToastContent()
                {
                    Launch = "",
                    Visual = new ToastVisual()
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
                    Audio = new ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new ToastNotification(content.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
                
            }
            else
            {
                
                var content = new ToastContent()
                {
                    Launch = file.Ref.Path,
                    Visual = new ToastVisual()
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
                    Audio = new ToastAudio()
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

            var content = new ToastContent()
                {
                    Launch = "",
                    Visual = new ToastVisual()
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
                    Audio = new ToastAudio()
                    {
                        Src = new Uri("ms-winsoundevent:Notification.IM")
                    }
                };
                var notification = new ToastNotification(content.GetXml());
                var notifier = ToastNotificationManager.CreateToastNotifier();
                notifier.Show(notification);
        }
    }
}
