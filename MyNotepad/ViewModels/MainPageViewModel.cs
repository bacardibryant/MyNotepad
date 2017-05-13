using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyNotepad.Views;

namespace MyNotepad.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            //add event listener to the view model in the constructor.
            //this way whenever the viewmodel is constructed it will listen for the
            //FileReceived event which in essence simply calls the load method on the
            //FileService.
            App.FileReceived += async (s, e) => File = await _FileService.LoadAsync(e);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private Models.FileInfo _File;
        public Models.FileInfo File
        {
            get { return _File; }
            set
            {
                _File = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(File)));
            }
        }

        Services.FileService _FileService = new Services.FileService();
        
        public async void Open()
        {
            // prompt a picker
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeFilter.Add(".txt");

            var file = await picker.PickSingleFileAsync();
            if (file == null)
                await new Windows.UI.Popups.MessageDialog("No file selected.").ShowAsync();
            else
                File = await _FileService.LoadAsync(file);
        }

        public async void Save()
        {
            if (File == null)
            {
                Frame frame = (Frame)Window.Current.Content;
                MainPage page = (MainPage)frame.Content;
                TextBox textBox = (TextBox)page.FindName("textBox");
                File = new Models.FileInfo
                {
                    Text = textBox.Text ?? ""
                };
                GetPicker(File);
            }
            else
                await _FileService.SaveAsync(File);
        }

        public async void GetPicker(Models.FileInfo model)
        {
            if (EnsureUnsnapped())
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    // write to file
                    await Windows.Storage.FileIO.WriteTextAsync(file, model.Text);
                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        await new Windows.UI.Popups.MessageDialog($"{file.Name} was saved.").ShowAsync();
                    }
                    else
                    {
                        await new Windows.UI.Popups.MessageDialog($"{file.Name} could not be saved.").ShowAsync();
                    }
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("Save cancelled.").ShowAsync();
                }
            }
        }

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped) || Windows.UI.ViewManagement.ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                new Windows.UI.Popups.MessageDialog("Window must be unsnapped.").ShowAsync();
            }

            return unsnapped;
        }
    }
}
