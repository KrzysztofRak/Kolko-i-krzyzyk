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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kółko_i_krzyżyk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        List<int> availableField = new List<int>();
       
        private char[] f = new char[9] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        private Image[] fields;
        char player = 'o';
        int numberOfEmptyFields = 9;
        bool gameEnded = false;
        bool playerVsComputer = false;
        Random rnd = new Random();
        int xPoints = 0, oPoints = 0; // Punkty graczy

        int diffLevel = 7; // 1-10: [1] - Easy peasy lemon squeezy, [10] - Good luck!

        public Game()
        {
            this.InitializeComponent();

            fields = new Image[9] {image_p1, image_p2, image_p3,
                                   image_p4, image_p5, image_p6,
                                   image_p7, image_p8, image_p9 };

            for (int i = 0; i < 9; i++)
            {
                availableField.Add(i);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter as string == "false")
            {
                playerVsComputer = false;
            }
            else
            {
                switch(e.Parameter as string)
                {
                    case "easy":
                        diffLevel = 3;
                        break;

                    case "normal":
                        diffLevel = 6;
                        break;

                    case "hard":
                        diffLevel = 9;
                        break;
                }

                playerVsComputer = true;
                BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/default.png"));
                image_whoseTurn.Source = image;
            }
        }

        private void setField(object sender, PointerRoutedEventArgs e)
        {
            if (gameEnded)
                return;

            int index = Array.IndexOf(fields, sender);

            if (f[index] == ' ')
            {
                f[index] = player;
                availableField.Remove(index);
                numberOfEmptyFields--;

                if (player == 'o')
                {
                    playSoundEffect("bubble.wav");

                    ((Image)sender).Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/circle.png"));
                }
                else
                {
                    if (!playerVsComputer)
                        playSoundEffect("sword.wav");

                    ((Image)sender).Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/cross.png"));
                }

                checkForWin(player);
            }
        }

        private async void playSoundEffect(string fileName)
        {
            MediaElement mysong = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync(fileName);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.SetSource(stream, file.ContentType);
            mysong.Play();
        }

        private void makeRandomMove()
        {
            int index = availableField[rnd.Next(0, availableField.Count)];

            setField(fields[index], null);
        }

        private bool checkForWin(char _player, bool byMiniMax = false) // byMiniMax - true jeżeli funkcja jest wywołana przez algorytm rekurencyjny MiniMax
        {
            if (f[0] == f[1] && f[1] == f[2] && f[0] == _player || // Sprawdza czy gracz (który wykonał właśnie ruch) wygrał
               f[3] == f[4] && f[4] == f[5] && f[3] == _player ||
               f[6] == f[7] && f[7] == f[8] && f[6] == _player ||
               f[0] == f[3] && f[3] == f[6] && f[0] == _player ||
               f[1] == f[4] && f[4] == f[7] && f[1] == _player ||
               f[2] == f[5] && f[5] == f[8] && f[2] == _player ||
               f[0] == f[4] && f[4] == f[8] && f[0] == _player ||
               f[2] == f[4] && f[4] == f[6] && f[2] == _player)
            {
                if (!byMiniMax) 
                {

                    if (_player == 'x') // Dolicza punkty dla 'X'
                    {
                        textBlock_winner.Text = "Krzyżyk zwycięża!";
                        textBlock_winner.Foreground = (SolidColorBrush)Resources["xColor"];
                        xPoints++;
                        textBlock_xPoints.Text = ": " + xPoints.ToString();
                    }
                    else // dla 'O'
                    {
                        textBlock_winner.Text = "Kółko zwycięża!";
                        textBlock_winner.Foreground = (SolidColorBrush)Resources["oColor"];
                        oPoints++;
                        textBlock_oPoints.Text = ": " + oPoints.ToString();
                    }

                    canvas.Visibility = Visibility.Visible;
                    gameEnded = true;
                }
                return true;
            }
            else if (numberOfEmptyFields == 0 && !byMiniMax) // Jeżeli nikt nie wygrał, a wszystkie pola zostały obstawione to jest remis
            {
                textBlock_winner.Text = "Remis!";
                textBlock_winner.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                canvas.Visibility = Visibility.Visible;
                gameEnded = true;
            }

            if (byMiniMax)
                return false;

            if (_player == 'x') // Jeżeli był gracz 'X' to teraz kolej gracza 'O'
            {
                player = 'o';
                if (!playerVsComputer)
                {
                    BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/minio.png"));
                    image_whoseTurn.Source = image;
                }
            }
            else // kolej gracza 'X'
            {
                player = 'x';

                if (!playerVsComputer)
                {
                    BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/minix.png"));
                    image_whoseTurn.Source = image;
                }

                if (!gameEnded && playerVsComputer) // Jeżeli gra się nie zakończyła i gracz gra z komputerem
                {
                    if (rnd.Next(0, 101) % diffLevel == 0)
                        makeRandomMove(); // Wykonuje ruch na losowym wolnym polu
                    else
                        minimax(player, 0); // Wywołuje algorytm rekurencyjny MiniMax
                }
            }
            return false;
        }


        private void image_end_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/button_koniec.png"));
            image_end.Source = image;
            Frame.Navigate(typeof(MainPage));
        }

        private void image_end_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/button_koniec_push.png"));
            image_end.Source = image;
        }

        private void image_reset_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/button_od-nowa.png"));
            image_reset.Source = image;

            canvas.Visibility = Visibility.Collapsed;

            if(!playerVsComputer)
            {
                image = new BitmapImage(new Uri(this.BaseUri, "/Assets/minio.png"));
                image_whoseTurn.Source = image;
            }

            f = new char[9] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            player = 'o';
            numberOfEmptyFields = 9;
            BitmapImage defaultImage = new BitmapImage(new Uri(this.BaseUri, "/Assets/default.png"));
            image_p1.Source = defaultImage;
            image_p2.Source = defaultImage;
            image_p3.Source = defaultImage;
            image_p4.Source = defaultImage;
            image_p5.Source = defaultImage;
            image_p6.Source = defaultImage;
            image_p7.Source = defaultImage;
            image_p8.Source = defaultImage;
            image_p9.Source = defaultImage;

            availableField.Clear();
            for (int i = 0; i < 9; i++)
            {
                availableField.Add(i);
            }

            gameEnded = false;
        }

        private void image_reset_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(this.BaseUri, "/Assets/button_od-nowa_push.png"));
            image_reset.Source = image;
        }

        // Algorytm MiniMax
        int minimax(char _player, int level)       
        {
            int licznik = 0;
            int j = 0;

            // sprawdzamy, czy jest wygrana

            for (int i = 0; i < 9; i++)
                if (f[i] == ' ')
                {
                    f[i] = _player;
                    j = i;  // gdyby był remis
                    licznik++;     // zliczamy wolne pola

                    bool test = checkForWin(_player, true);

                    f[i] = ' ';
                    if (test)
                    {
                
                        if (level == 0) setField(fields[i], null);
                        return _player == 'x' ? -1 : 1;
                    }
                }


            // sprawdzamy, czy jest remis

            if (licznik == 1)
            {
                if (level == 0) f[j] = _player;
                return 0;
            }

            // wybieramy najkorzystniejszy ruch dla gracza

            int v;
            int vmax;

            vmax = _player == 'x' ? 2 : -2;

            for (int i = 0; i < 9; i++)
            {
                if (f[i] == ' ')
                {
                    f[i] = _player;
                    v = minimax(_player == 'x' ? 'o' : 'x', level + 1);
                    f[i] = ' ';

                    if (((_player == 'x') && (v < vmax)) || ((_player == 'o') && (v > vmax)))
                    {
                        vmax = v; j = i;
                    }
                }
            }

            if (level == 0)
                setField(fields[j], null);

            return vmax;
        }
    }
}
