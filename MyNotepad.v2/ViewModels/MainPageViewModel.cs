using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using MyNotepad.v2.Views;
using Windows.UI.Xaml.Controls;
using MyNotepad.Services.Models;
using MyNotepad.ConnectedServices.Services;
using Windows.UI.Xaml;

namespace MyNotepad.v2.ViewModels
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

        private FileInfo _File;
        public FileInfo File
        {
            get { return _File; }
            set
            {
                _File = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(File)));
            }
        }

        FileService _FileService = new FileService();
        ToastService _ToastService = new ToastService();

        public async void Create()
        {
            // Create sample file; replace if exists.
            StorageFolder storageFolder =
               ApplicationData.Current.LocalFolder;
           StorageFile sampleFile = await storageFolder.CreateFileAsync("Untitled.txt",
                   CreationCollisionOption.ReplaceExisting);

            File = await _FileService.LoadAsync(sampleFile);

            await sampleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public async void Open()
        {
            // load file open picker
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeFilter.Add(".txt");

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                _ToastService.ShowToast("No file selected.");
            }
            else
            {
                File = await _FileService.LoadAsync(file);
            }
        }

        public async void Save()
        {
            //if this is a new file, (meaning no file was opened, then build the fileinfo model)
            if (File == null)
            {

                // set text value to contents of textbox or to an empty string.
                File = new FileInfo
                {
                    Text = textBox.Text ?? ""
                };

                // use a save file picker to allow the user to save the file.
                GetPicker(File);
            }

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
                }
                catch (Exception ex)
                {
                    // if there was an error, display a toast notifying the user.
                    _ToastService.ShowToast(File, $"Save failed: {ex.Message}");
                }
            }
        }

        public async void GetPicker(FileInfo model)
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
                CachedFileManager.DeferUpdates(file);

                // write to file
                await FileIO.WriteTextAsync(file, model.Text);

                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                // populate model fields to be used in toast notifications.
                model.Name = file.Name;
                model.Ref = file;

                // update the property so that the INotifyPropertyChangedEvent is fired.
                File = model;

                // when the file update status is complete, notify the user that the file save was successful.
                if (status == FileUpdateStatus.Complete)
                {
                    _ToastService.ShowToast(model, $"{file.Name} successfully saved.");
                }
                else
                {
                    // if the file update status did not complete for some reason, notify the user that the save failed.
                    _ToastService.ShowToast(model, $"{file.Name} could not be saved.");
                }
            }
            else
            {
                //again if the user clicked cancel on the save picker, then notify the user that the save was cancelled.
                _ToastService.ShowToast(model, "Save cancelled.");

                //if user cancels the save, set the file to null.
                File = null;
            }

            //restore contents of textbox.
            textBox.Text = model.Text;
        }
    }
}
