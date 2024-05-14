
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TPDP_Battle_Helper.Views;

namespace TPDP_Battle_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            frame.Content = new PageOverworld(this);
        }

        public void Reposition(Rectangle currentBounds, Page page, Grid main_left, Grid main_right)
        {
            int sidebarHeight = currentBounds.Height - App.TITLE_BAR_HEIGHT;
            int newWidth = (int)(sidebarHeight * App.ASPECT_RATIO);
            int sidebarWidth = (newWidth - currentBounds.Width) / 2;

            // Place entire window
            window.Left = currentBounds.Left - sidebarWidth;
            window.Top = currentBounds.Top;
            window.Width = newWidth;
            window.Height = currentBounds.Height;
            page.Width = newWidth - 16;
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
                    frame.Content = new PageBattleMain(this);
                    return true;
                }
            }

            //Overworld
            else
            {
                if (page.GetType() != typeof(PageOverworld))
                {
                    frame.Content = new PageOverworld(this);
                    return true;
                }
            }

            return false;

        }

    }
}
