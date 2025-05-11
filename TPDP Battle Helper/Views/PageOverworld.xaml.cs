using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TPDP_Battle_Helper.Views
{
    /// <summary>
    /// Interaction logic for PageOverworld.xaml
    /// </summary>
    public partial class PageOverworld : Page
    {

        private double LastContentTime = 0;

        private MainWindow mainWindow;
        public bool Active = false;

        public PageOverworld(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;

            CompositionTarget.Rendering += Loop;
        }

        private void Loop(object sender, EventArgs e)
        {
            if (!Active) return;

            // Delta Time
            double currentTime = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
            double deltaTime = currentTime - LastContentTime;
            if (deltaTime < 1000 / App.CONTENT_REFRESH_RATE) return;
            LastContentTime = currentTime - currentTime % (1000 / App.CONTENT_REFRESH_RATE);

            // Reposition
            Rectangle bounds = GameHook.FindGameBounds();
            mainWindow.Reposition(bounds, page, main_left, main_right);

            // Get map id
            byte[] mapIdTmp = GameHook.ReadAddress(0x9969BC, 2);
            ushort mapId = (ushort) (mapIdTmp[0] + mapIdTmp[1] * 0x0100);

            SetMinimapImage(mapId);

            // Switch pages
            bool transitioned = mainWindow.ScreenTransition(this);
            if (transitioned) Active = true;

        }

        private void SetMinimapImage(ushort mapId)
        {
            string mapStr = mapId.ToString();
            while (mapStr.Length < 3) mapStr = "0" + mapStr;

            BitmapImage image = new BitmapImage(new Uri("/Resources/UI/map_unknown.png", UriKind.Relative));
            if (File.Exists("Content/Map/Map" + mapStr + ".png"))
            {
                image = new BitmapImage(new Uri("/Content/Map/Map" + mapStr + ".png", UriKind.Relative));
            }
            minimap.Source = image;
            minimap.Width = main_right.Width * 0.85;
            Thickness margin = new Thickness(0);
            margin.Top = main_right.Height * 0.02;
            minimap.Margin = margin;

            minimap_id.Content = "" + mapStr;

        }

    }
}
