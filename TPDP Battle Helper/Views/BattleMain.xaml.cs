
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPDP_Battle_Helper.Data;

namespace TPDP_Battle_Helper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BattleMain : Window
    {

        public BattleMain()
        {
            InitializeComponent();

            CompositionTarget.Rendering += Reposition;
            CompositionTarget.Rendering += Loop;
        }

        private void Reposition(object sender, EventArgs e)
        {
            Rectangle bounds = GameHook.FindGameBounds();
            int newWidth = (int) ((bounds.Height - App.TITLE_BAR_HEIGHT) * App.ASPECT_RATIO);
            int sidebarWidth = (newWidth - bounds.Width) / 2;

            // Place entire window
            window.Left = bounds.Left - sidebarWidth;
            window.Top = bounds.Top;
            window.Width = newWidth;
            window.Height = bounds.Height;

            // Resize margins
            main_left.Width = sidebarWidth;
            main_right.Width = sidebarWidth;
        }

        private void Loop(object sender, EventArgs e)
        {

            // Get player puppet
            short playerPuppetId = GameHook.ReadAddress(0xC59FDB, 2).Select(b => (short)b).ToArray()[0];
            byte playerPuppetStyleIdx = GameHook.ReadAddress(0xC59FDD, 1)[0];
            PuppetEntity? playerPuppet = PuppetEntity.FindPuppet(playerPuppetId, playerPuppetStyleIdx);

            if (playerPuppet != null)
            {
                SetPlayerTypes(playerPuppet);
            }

            // Get enemy puppet
            short enemyPuppetId = GameHook.ReadAddress(0xC5A510, 2).Select(b => (short)b).ToArray()[0];
            byte enemyPuppetStyleIdx = GameHook.ReadAddress(0xC5A512, 1)[0];
            PuppetEntity? enemyPuppet = PuppetEntity.FindPuppet(enemyPuppetId, enemyPuppetStyleIdx);

            if (enemyPuppet != null)
            {
                SetEnemyTypes(enemyPuppet);
            }

        }

        private void SetPlayerTypes(PuppetEntity puppet)
        {
            ElementalType type1 = puppet.PuppetStyle.Type1;
            ElementalType? type2 = puppet.PuppetStyle.Type2;

            player_type1.Source = new BitmapImage(new Uri(type1.FilePath, UriKind.Relative));
            player_type1.Width = main_left.Width * 0.6;
            Thickness margin1 = player_type1.Margin;
            margin1.Top = main_left.ActualHeight * 0.02;
            margin1.Right = main_left.ActualWidth * 0.1;
            player_type1.Margin = margin1;

            player_type2.Source = new BitmapImage(new Uri(type2.FilePath, UriKind.Relative));
            player_type2.Width = main_left.Width * 0.6;
            Thickness margin2 = player_type2.Margin;
            margin2.Top = main_left.ActualHeight * 0.02 + player_type1.ActualHeight + main_left.ActualHeight * 0.01;
            margin2.Right = main_left.ActualWidth * 0.1;
            player_type2.Margin = margin2;
        }

        private void SetEnemyTypes(PuppetEntity puppet)
        {
            ElementalType type1 = puppet.PuppetStyle.Type1;
            ElementalType? type2 = puppet.PuppetStyle.Type2;

            enemy_type1.Source = new BitmapImage(new Uri(type1.FilePath, UriKind.Relative));
            enemy_type1.Width = main_right.Width * 0.6;
            Thickness margin1 = enemy_type1.Margin;
            margin1.Top = main_right.ActualHeight * 0.02;
            margin1.Left = main_right.ActualWidth * 0.1;
            enemy_type1.Margin = margin1;

            enemy_type2.Source = new BitmapImage(new Uri(type2.FilePath, UriKind.Relative));
            enemy_type2.Width = main_right.Width * 0.6;
            Thickness margin2 = enemy_type2.Margin;
            margin2.Top = main_right.ActualHeight * 0.02 + enemy_type1.ActualHeight + main_right.ActualHeight * 0.01;
            margin2.Left = main_right.ActualWidth * 0.1;
            enemy_type2.Margin = margin2;
        }

    }



}