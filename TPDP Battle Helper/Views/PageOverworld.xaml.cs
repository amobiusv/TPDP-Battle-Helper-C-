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

        private MainWindow mainWindow;

        public PageOverworld(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;
            CompositionTarget.Rendering += Loop;
        }

        private void Loop(object sender, EventArgs e)
        {

            // Reposition
            Rectangle bounds = GameHook.FindGameBounds();
            mainWindow.Reposition(bounds, page, main_left, main_right);

            // Switch pages
            bool transitioned = mainWindow.ScreenTransition(this);
            if (transitioned)
            {
                CompositionTarget.Rendering -= Loop;
            }

        }

    }
}
