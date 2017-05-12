using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyNotepad.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            textBox.Focus(FocusState.Programmatic);
            textBox.AcceptsReturn = true;
        }

        //override the on navigated to at the page level as well to handle the jump list.
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //the parameter being passed from the on launched event handler in app
            //is the file itself.
            // verify that the file is of type storage file then pass it to the view model via
            // the service as normal.
            // this loads the contents of the file into the view from the jump list.
            if(e.Parameter is StorageFile)
            {
                var service = new Services.FileService();
                var model = await service.LoadAsync(e.Parameter as StorageFile);
                ViewModel.File = model;
            }
        }
    }
}
