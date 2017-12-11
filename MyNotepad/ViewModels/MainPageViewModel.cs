using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyNotepad.Views;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyNotepad.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // get the current frame so that you can get the contents of the textbox.
        public static Frame frame = (Frame)Window.Current.Content;
        public static MainPage page = (MainPage)frame.Content;
        public static TextBox textBox = (TextBox)page.FindName("textBox");

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
        Services.ToastService _ToastService = new Services.ToastService();

        public async void Create()
        {
            // Create sample file; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("Untitled.txt",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);

            File = await _FileService.LoadAsync(sampleFile);

            await sampleFile.DeleteAsync(Windows.Storage.StorageDeleteOption.PermanentDelete);
        }

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
                //await new Windows.UI.Popups.MessageDialog("No file selected.").ShowAsync();
            _ToastService.ShowToast("No file selected.");
            else
                File = await _FileService.LoadAsync(file);
        }

        public async void Save()
        {
            //if this is a new file, (meaning no file was opened, then build the fileinfo model)
            if (File == null)
            {
                // get the current frame so that you can get the contents of the textbox.
                //Frame frame = (Frame)Window.Current.Content;
                //MainPage page = (MainPage)frame.Content;
                //TextBox textBox = (TextBox)page.FindName("textBox");

                // set text value to contents of textbox or to an empty string.
                File = new Models.FileInfo
                {
                    Text = textBox.Text ?? ""
                };

                // use a save file picker to allow the user to save the file.
                GetPicker(File);
            }
            // not sure if this is a good idea.
            //I'm trying to force avoid saving in appdata folder.
            //But what about phones and cloud storage
            else if (File.Ref.Path.Contains("AppData")) 
            {
                GetPicker(File);
            }
            else
            {
                try
                {
                    // if the file existed, try and save.
                    await _FileService.SaveAsync(File);

                    // display a toast notification to let the user know that the file was saved.
                    _ToastService.ShowToast(File, "File successfully saved.");
                } catch (Exception ex)
                {
                    // if there was an error, display a toast notifying the user.
                    _ToastService.ShowToast(File, $"Save failed: {ex.Message}");
                }
            }
        }

        public async void GetPicker(Models.FileInfo model)
        {
            //if the application window is snapped the save file picker will not display
            //so ensure that it isn't snapped.
            //var unSnapped = await EnsureUnsnapped();
            var unSnapped = true;
            if (unSnapped)
            {
                //instantiate a save file picker and set it's default location to the 'Documents' folder.
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "Untitled.txt";

                // the save file picker returns an instance of windows storage file when the file is saved.
                // if the user clicks cancel on the save dialog, the storage file object will be null.
                StorageFile file = await savePicker.PickSaveFileAsync();

                // if the file is not null
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    
                    // write to file
                    await Windows.Storage.FileIO.WriteTextAsync(file, model.Text);
                    
                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

                    // populate model fields to be used in toast notifications.
                    model.Name = file.Name;
                    model.Ref = file;

                    // update the property so that the INotifyPropertyChangedEvent is fired.
                    File = model;

                    // when the file update status is complete, notify the user that the file save was successful.
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        // moved from using MessageDialogs to ToastNotifications
                        //await new Windows.UI.Popups.MessageDialog($"{file.Name} was saved.").ShowAsync();
                        _ToastService.ShowToast(model, $"{file.Name} successfully saved.");
                    }
                    else
                    {
                        // if the file update status did not complete for some reason, notify the user that the save failed.

                        // moved from using MessageDialogs to ToastNotifications
                        //await new Windows.UI.Popups.MessageDialog($"{file.Name} could not be saved.").ShowAsync();
                        _ToastService.ShowToast(model, $"{file.Name} could not be saved.");
                    }
                }
                else
                {
                    //again if the user clicked cancel on the save picker, then notify the user that the save was cancelled.

                    // moved from using MessageDialogs to ToastNotifications
                    //await new Windows.UI.Popups.MessageDialog("Save cancelled.").ShowAsync();
                    _ToastService.ShowToast(model, "Save cancelled.");

                    //if user cancels the save, set the file to null.
                    File = null;
                }
                //restore contents of textbox.
                textBox.Text = model.Text;
            }
        }

        //TODO:Address obsolete code.
        //internal async Task<bool> EnsureUnsnapped()
        //{
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            //bool unsnapped = ((Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped) ||
            //                    Windows.UI.ViewManagement.ApplicationView.TryUnsnap());

            //if (!unsnapped)
            //{
            //    await new Windows.UI.Popups.MessageDialog("Window must be unsnapped.").ShowAsync();
            //}

            //return unsnapped;
            //return true;
        //}
    }
}
