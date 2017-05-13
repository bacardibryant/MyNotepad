using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace MyNotepad.Services
{
    public class FileService
    {
        public async Task SaveAsync(Models.FileInfo model)
        {
            if (model != null)
                await Windows.Storage.FileIO.WriteTextAsync(model.Ref, model.Text);
        }

        public async Task<Models.FileInfo> LoadAsync(Windows.Storage.StorageFile file)
        {
            // add to jumplist if it is supported. Not all devices support jumplists.
            if (Windows.UI.StartScreen.JumpList.IsSupported())
            {
                var jList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
                jList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.None;

                // make sure that the number is under 5 so that the list doesn't become cumbersome.
                while (jList.Items.Count() > 4)
                    jList.Items.RemoveAt(jList.Items.Count() - 1);

                if (!jList.Items.Any(x=>x.Arguments == file.Path))
                {
                    var jItem = Windows.UI.StartScreen.JumpListItem.CreateWithArguments(file.Path, file.DisplayName);
                    jList.Items.Add(jItem);
                }

                await jList.SaveAsync();
            }

            // add to most recently used list
            // by default, you may not have access to open the file.
            var mruList = StorageApplicationPermissions.MostRecentlyUsedList;

            // again make sure that the contents of the most recently used list are not getting too big.
            while (mruList.Entries.Count() >= mruList.MaximumItemsAllowed)
                mruList.Remove(mruList.Entries.First().Token);

            // add the item to the most recently used list.
            if (!mruList.Entries.Any(x => x.Metadata == file.Path))
                mruList.Add(file, file.Path);

            // retrieve a token that represents the future access for this list.
            // this will allow the application to access the list later the same day or some future day.
            // so we need to maintain the permission granted by the user when the application is installed which gets implemented inside App.xaml.
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            futureAccessList.Add(file);

            // build model
            return new Models.FileInfo
            {
                Text = await Windows.Storage.FileIO.ReadTextAsync(file),
                Name = file.DisplayName,
                Ref = file
            };
        }
    }
}
