
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using TPDP_Battle_Helper.Views;

namespace TPDP_Battle_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private double LastRepositionTime = 0;

        private PageOverworld pageOverworld;
        private PageBattleMain pageBattleMain;

        public MainWindow()
        {
            InitializeComponent();

            pageOverworld = new PageOverworld(this);
            pageBattleMain = new PageBattleMain(this);

            frame.Content = pageOverworld;
            pageOverworld.Active = true;
        }

        public void Reposition(Rectangle currentBounds, Page page, Grid main_left, Grid main_right)
        {
            // Delta Time
            double currentTime = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
            double deltaTime = currentTime - LastRepositionTime;
            if (deltaTime < 1000 / App.REPOSITION_REFRESH_RATE) return;
            LastRepositionTime = currentTime - currentTime % (1000 / App.REPOSITION_REFRESH_RATE);

            // Preparations
            int sidebarHeight = currentBounds.Height - (int)App.TITLEBAR_HEIGHT;
            int newWidth = (int)(sidebarHeight * App.ASPECT_RATIO);
            int sidebarWidth = (newWidth - currentBounds.Width) / 2;

            // Place entire window
            window.Left = currentBounds.Left - sidebarWidth;
            window.Top = currentBounds.Top;
            window.Width = newWidth;
            window.Height = currentBounds.Height;
            page.Width = newWidth - App.WINDOW_WIDTH_OFFSET;
            page.Height = sidebarHeight;

            // Resize margins
            main_left.Width = sidebarWidth;
            main_left.Height = sidebarHeight;
            main_right.Width = sidebarWidth;
            main_right.Height = sidebarHeight;
        }

        public bool ScreenTransition(Page page)
        {
            bool inBattle = GameHook.ReadAddress(0x93C0DF, 1)[0] != 0x00;

            // BattleMain
            if (inBattle)
            {
                if (page.GetType() != typeof(PageBattleMain))
                {
                    frame.Content = pageBattleMain;
                    LastRepositionTime = 0;
                    pageBattleMain.Active = true;
                    return true;
                }
            }

            //Overworld
            else
            {
                if (page.GetType() != typeof(PageOverworld))
                {
                    frame.Content = pageOverworld;
                    LastRepositionTime = 0;
                    pageOverworld.Active = true;
                    return true;
                }
            }

            return false;

        }

    }
}
