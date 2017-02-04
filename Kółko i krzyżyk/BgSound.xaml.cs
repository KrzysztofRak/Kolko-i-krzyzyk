using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kółko_i_krzyżyk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BgSound : Page
    {
        MediaElement mysong;

        public BgSound()
        {
            this.InitializeComponent();
        }

        public async void playBgSound()
        {
            mysong = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("bg_sound.wav");
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.MediaEnded += new RoutedEventHandler(mediaEnded);
            mysong.AutoPlay = true;
            mysong.IsLooping = true;
            mysong.SetSource(stream, file.ContentType);

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private void mediaEnded(object sender, RoutedEventArgs e)
        {
            mysong.Play();
        }
    }
}
