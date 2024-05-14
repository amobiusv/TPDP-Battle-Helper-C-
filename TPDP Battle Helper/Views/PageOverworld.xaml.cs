using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            // Switch pages
            bool transitioned = mainWindow.ScreenTransition(this);
            if (transitioned) Active = true;

        }

    }
}
