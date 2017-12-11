using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using MyNotepad.Services.Models;

namespace MyNotepad.ConnectedServices.Services
{
    public class FileService
    {
        public async Task SaveAsync(FileInfo model)
        {
            if (model != null)
                await Windows.Storage.FileIO.WriteTextAsync(model.Ref, model.Text);
        }

        public async Task<FileInfo> LoadAsync(Windows.Storage.StorageFile file)
        {
            // add to jumplist if it is supported. Not all devices support jumplists.
            // this is the list that appears when you right-click on your application icon.
            // recently used items can be pinned to the top of the list.
            if (Windows.UI.StartScreen.JumpList.IsSupported())
            {
                var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
                jumpList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.None;

                // make sure that the number is under 5 so that the list doesn't become cumbersome.
                while (jumpList.Items.Count() > 4)
                    jumpList.Items.RemoveAt(jumpList.Items.Count() - 1);

                if (!jumpList.Items.Any(x => x.Arguments == file.Path))
                {
                    var recentFileReference = Windows.UI.StartScreen.JumpListItem.CreateWithArguments(file.Path, file.DisplayName);
                    jumpList.Items.Add(recentFileReference);
                }

                await jumpList.SaveAsync();
            }

            // add to most recently used list
            // by default, you may not have access to open the file.
            var recentlyUsedList = StorageApplicationPermissions.MostRecentlyUsedList;

            // again make sure that the contents of the most recently used list are not getting too big.
            while (recentlyUsedList.Entries.Count() >= recentlyUsedList.MaximumItemsAllowed)
                recentlyUsedList.Remove(recentlyUsedList.Entries.First().Token);

            // add the item to the most recently used list.
            if (!recentlyUsedList.Entries.Any(x => x.Metadata == file.Path))
                recentlyUsedList.Add(file, file.Path);

            // retrieve a token that represents the future access for this list.
            // this will allow the application to access the list later the same day or some future day.
            // so we need to maintain the permission granted by the user when the application is installed which gets implemented inside App.xaml.
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            futureAccessList.Add(file);

            // build model
            return new FileInfo
            {
                Text = await Windows.Storage.FileIO.ReadTextAsync(file),
                Name = file.DisplayName,
                Ref = file
            };
        }
    }
}
