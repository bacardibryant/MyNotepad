using System;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyNotepad
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public App() { InitializeComponent(); }

        Frame RootFrame => (Window.Current.Content as Frame)
            ?? (Window.Current.Content = new Frame()) as Frame;

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            //is the application being launched by the jumplist?
            //if so then the tileid is set to app and there would be arguments supplied.
            var jumpList = e.TileId == "App" && !string.IsNullOrEmpty(e.Arguments);
            if (jumpList)
            {
                try
                {
                    //if it is a jumplist item, then you can locate the file by the path provided in the arguments.
                    var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(e.Arguments);
                    if (RootFrame.Content == null)
                        // then navigate to the page passing the file.
                        RootFrame.Navigate(typeof(MyNotepad.Views.MainPage), file);
                    else
                        // if we've already navigated to the file, because the content is not null, then no need in
                        // navigating again.
                        FileReceived?.Invoke(this, file);
                }
                catch (Exception) { throw; }
            }
            else
            {
                RootFrame.Navigate(typeof(MyNotepad.Views.MainPage), e.Arguments);
            }
            Window.Current.Activate();
        }

        //this event fires when a user double-clicks on a text file.
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            //should this be removed or called prior to custom code.
            //base.OnFileActivated(args);

            //if no files passed, gracefully exit event handler.
            if (!args.Files.Any())
                return;

            //if the application is already running, no need to navigate, just raise the event handler to load the file.
            //otherwise navigate to the main page and pass in the file to the OnNavigatedTo event handler.
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                FileReceived?.Invoke(this, args.Files.First() as Windows.Storage.StorageFile);
            else
                RootFrame.Navigate(typeof(MyNotepad.Views.MainPage), args.Files.First());

            Window.Current.Activate();
        }

        //handle file received while application is running.
        public static event EventHandler<Windows.Storage.StorageFile> FileReceived;

        #region TEMPLATE CODE
        /*
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MyNotepad.Views.MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        */
        #endregion
    }
}
