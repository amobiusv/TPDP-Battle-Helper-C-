
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPDP_Battle_Helper.Data;

namespace TPDP_Battle_Helper.Views
{
    /// <summary>
    /// Interaction logic for PageBattleMain.xaml
    /// </summary>
    public partial class PageBattleMain : Page
    {

        private double LastContentTime = 0;

        private MainWindow mainWindow;
        public bool Active = false;

        private PuppetEntity? LastPlayerPuppet = null;
        private PuppetEntity? LastEnemyPuppet = null;

        private Rectangle? LastBounds = null;

        public PageBattleMain(MainWindow window)
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

            // Get player puppet
            short playerPuppetId = GameHook.ReadAddress(0xC59FDB, 2).Select(b => (short)b).ToArray()[0];
            byte playerPuppetStyleIdx = GameHook.ReadAddress(0xC59FDD, 1)[0];
            PuppetEntity? playerPuppet = PuppetEntity.FindPuppet(playerPuppetId, playerPuppetStyleIdx);

            if (RequiresUpdating(playerPuppet, LastPlayerPuppet, bounds))
            {
                SetPlayerTypes(playerPuppet);
                SetPlayerWeaknesses(playerPuppet);
                SetPlayerBaseStats(playerPuppet);
            }
            LastPlayerPuppet = playerPuppet;

            // Get enemy puppet
            short enemyPuppetId = GameHook.ReadAddress(0xC5A510, 2).Select(b => (short)b).ToArray()[0];
            byte enemyPuppetStyleIdx = GameHook.ReadAddress(0xC5A512, 1)[0];
            PuppetEntity? enemyPuppet = PuppetEntity.FindPuppet(enemyPuppetId, enemyPuppetStyleIdx);

            if (RequiresUpdating(playerPuppet, LastEnemyPuppet, bounds))
            {
                SetEnemyTypes(enemyPuppet);
                SetEnemyWeaknesses(enemyPuppet);
                SetEnemyBaseStats(enemyPuppet);
            }
            LastEnemyPuppet = enemyPuppet;

            LastBounds = bounds;

            // Switch pages
            bool transitioned = mainWindow.ScreenTransition(this);
            if (transitioned) Active = true;

        }

        private bool RequiresUpdating(PuppetEntity? currentPuppet, PuppetEntity? expectedPuppet, Rectangle currentBounds)
        {

            if (LastBounds == null) return true;
            if (((Rectangle)LastBounds).Width != currentBounds.Width) return true;
            if (((Rectangle)LastBounds).Height != currentBounds.Height) return true;

            if (currentPuppet == null || expectedPuppet == null) return true;
            if (expectedPuppet.PuppetSpecies.InternalId != currentPuppet.PuppetSpecies.InternalId) return true;
            if (expectedPuppet.PuppetStyle.StyleId != currentPuppet.PuppetStyle.StyleId) return true;

            return false;
        }

        private void SetPlayerTypes(PuppetEntity? puppet)
        {

            if (puppet == null)
            {
                player_type1.Visibility = Visibility.Hidden;
                player_type2.Visibility = Visibility.Hidden;
            }
            else
            {
                ElementalType type1 = puppet.PuppetStyle.Type1;
                ElementalType? type2 = puppet.PuppetStyle.Type2;

                player_type1.Source = new BitmapImage(new Uri(type1.FilePath, UriKind.Relative));
                player_type1.Width = main_left.Width * 0.6;
                Thickness margin1 = player_type1.Margin;
                margin1.Top = main_left.Height * 0.02;
                margin1.Right = main_left.Width * 0.1;
                player_type1.Margin = margin1;
                player_type1.Visibility = Visibility.Visible;

                if (type2 == null)
                {
                    player_type2.Visibility = Visibility.Hidden;
                }
                else
                {
                    player_type2.Source = new BitmapImage(new Uri(type2.FilePath, UriKind.Relative));
                    player_type2.Width = main_left.Width * 0.6;
                    Thickness margin2 = player_type2.Margin;
                    margin2.Top = main_left.Height * 0.1;
                    margin2.Right = main_left.Width * 0.1;
                    player_type2.Margin = margin2;
                    player_type2.Visibility = Visibility.Visible;
                }
            }
        }

        private void SetEnemyTypes(PuppetEntity? puppet)
        {
            if (puppet == null)
            {
                enemy_type1.Visibility = Visibility.Hidden;
                enemy_type2.Visibility = Visibility.Hidden;
            }
            else
            {
                ElementalType type1 = puppet.PuppetStyle.Type1;
                ElementalType? type2 = puppet.PuppetStyle.Type2;

                enemy_type1.Source = new BitmapImage(new Uri(type1.FilePath, UriKind.Relative));
                enemy_type1.Width = main_right.Width * 0.6;
                Thickness margin1 = enemy_type1.Margin;
                margin1.Top = main_right.Height * 0.02;
                margin1.Left = main_right.Width * 0.1;
                enemy_type1.Margin = margin1;
                enemy_type1.Visibility = Visibility.Visible;

                if (type2 == null)
                {
                    player_type2.Visibility = Visibility.Hidden;
                }
                else
                {
                    enemy_type2.Source = new BitmapImage(new Uri(type2.FilePath, UriKind.Relative));
                    enemy_type2.Width = main_right.Width * 0.6;
                    Thickness margin2 = enemy_type2.Margin;
                    margin2.Top = main_right.Height * 0.1;
                    margin2.Left = main_right.Width * 0.1;
                    enemy_type2.Margin = margin2;
                    enemy_type2.Visibility = Visibility.Visible;
                }
            }
        }

        private void SetPlayerWeaknesses(PuppetEntity? puppet)
        {

            Thickness margin = player_weaknesses.Margin;
            margin.Top = main_left.Height * 0.2;
            player_weaknesses.Margin = margin;

            if (puppet == null)
            {
                player_super_weak_label.Visibility = Visibility.Collapsed;
                player_super_weak.Visibility = Visibility.Collapsed;
                player_weak_label.Visibility = Visibility.Collapsed;
                player_weak.Visibility = Visibility.Collapsed;
                player_resistant_label.Visibility = Visibility.Collapsed;
                player_resistant.Visibility = Visibility.Collapsed;
                player_super_resistant_label.Visibility = Visibility.Collapsed;
                player_super_resistant.Visibility = Visibility.Collapsed;
                player_immune_label.Visibility = Visibility.Collapsed;
                player_immune.Visibility = Visibility.Collapsed;
            }

            else
            {
                PopulateResistanceImages(puppet.PuppetStyle.IsSuperWeakTo(), player_super_weak_label, player_super_weak, main_left);
                PopulateResistanceImages(puppet.PuppetStyle.IsWeakTo(), player_weak_label, player_weak, main_left);
                PopulateResistanceImages(puppet.PuppetStyle.IsResistantTo(), player_resistant_label, player_resistant, main_left);
                PopulateResistanceImages(puppet.PuppetStyle.IsSuperResistantTo(), player_super_resistant_label, player_super_resistant, main_left);
                PopulateResistanceImages(puppet.PuppetStyle.IsImmuneTo(), player_immune_label, player_immune, main_left);
            }

        }

        private void SetEnemyWeaknesses(PuppetEntity? puppet)
        {

            Thickness margin = enemy_weaknesses.Margin;
            margin.Top = main_right.Height * 0.2;
            enemy_weaknesses.Margin = margin;

            if (puppet == null)
            {
                enemy_super_weak_label.Visibility = Visibility.Collapsed;
                enemy_super_weak.Visibility = Visibility.Collapsed;
                enemy_weak_label.Visibility = Visibility.Collapsed;
                enemy_weak.Visibility = Visibility.Collapsed;
                enemy_resistant_label.Visibility = Visibility.Collapsed;
                enemy_resistant.Visibility = Visibility.Collapsed;
                enemy_super_resistant_label.Visibility = Visibility.Collapsed;
                enemy_super_resistant.Visibility = Visibility.Collapsed;
                enemy_immune_label.Visibility = Visibility.Collapsed;
                enemy_immune.Visibility = Visibility.Collapsed;
            }

            else
            {
                PopulateResistanceImages(puppet.PuppetStyle.IsSuperWeakTo(), enemy_super_weak_label, enemy_super_weak, main_right);
                PopulateResistanceImages(puppet.PuppetStyle.IsWeakTo(), enemy_weak_label, enemy_weak, main_right);
                PopulateResistanceImages(puppet.PuppetStyle.IsResistantTo(), enemy_resistant_label, enemy_resistant, main_right);
                PopulateResistanceImages(puppet.PuppetStyle.IsSuperResistantTo(), enemy_super_resistant_label, enemy_super_resistant, main_right);
                PopulateResistanceImages(puppet.PuppetStyle.IsImmuneTo(), enemy_immune_label, enemy_immune, main_right);
            }

        }

        private void PopulateResistanceImages(ElementalType[] types, FrameworkElement label, WrapPanel panel, Grid sidebar)
        {
            if (types.Length > 0)
            {
                panel.Children.Clear();
                foreach (ElementalType type in types)
                {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(type.FilePath, UriKind.Relative));
                    image.Width = sidebar.Width * 0.25;
                    Thickness imageMargin = new Thickness(sidebar.Width * 0.05, 0, 0, sidebar.Height * 0.01);
                    image.Margin = imageMargin;
                    panel.Children.Add(image);
                }
                Thickness panelMargin = new Thickness(sidebar.Width * 0.04, 0, 0, 0);
                panel.Margin = panelMargin;
                panel.Visibility = Visibility.Visible;

                Thickness labelMargin = new Thickness(sidebar.Width * 0.04, sidebar.Height * 0.01, sidebar.Width * 0.04, sidebar.Height * 0.01);
                label.Margin = labelMargin;
                label.Visibility = Visibility.Visible;
            }
            else
            {
                label.Visibility = Visibility.Collapsed;
                panel.Visibility = Visibility.Collapsed;
            }
        }

        private void SetPlayerBaseStats(PuppetEntity? puppet)
        {

            if (puppet == null)
            {
                player_stats.Visibility = Visibility.Hidden;
            }
            else
            {

                Thickness labelMargin = new Thickness(main_left.Width * 0.05, 0, 0, main_left.Height * 0.015);
                player_stat_speed_label.Width = main_left.Width * 0.4;
                player_stat_speed_label.Margin = labelMargin;

                labelMargin.Bottom += main_left.Height * 0.035;
                player_stat_sdef_label.Width = main_left.Width * 0.4;
                player_stat_sdef_label.Margin = labelMargin;

                labelMargin.Bottom += main_left.Height * 0.035;
                player_stat_satk_label.Width = main_left.Width * 0.4;
                player_stat_satk_label.Margin = labelMargin;

                labelMargin.Bottom += main_left.Height * 0.035;
                player_stat_fdef_label.Width = main_left.Width * 0.4;
                player_stat_fdef_label.Margin = labelMargin;

                labelMargin.Bottom += main_left.Height * 0.035;
                player_stat_fatk_label.Width = main_left.Width * 0.4;
                player_stat_fatk_label.Margin = labelMargin;

                labelMargin.Bottom += main_left.Height * 0.035;
                player_stat_hp_label.Width = main_left.Width * 0.4;
                player_stat_hp_label.Margin = labelMargin;

                double baseHP = puppet.PuppetStyle.BaseStats.HP / 250.0;
                double baseFAtk = puppet.PuppetStyle.BaseStats.FAtk / 165.0;
                double baseFDef = puppet.PuppetStyle.BaseStats.FDef / 205.0;
                double baseSAtk = puppet.PuppetStyle.BaseStats.SAtk / 165.0;
                double baseSDef = puppet.PuppetStyle.BaseStats.SDef / 205.0;
                double baseSpeed = puppet.PuppetStyle.BaseStats.Spd / 205.0;
                Brush brush;

                Thickness barMargin = new Thickness(main_left.Width * 0.45, 0, 0, main_left.Height * 0.02);
                player_stat_speed_bar.Width = main_left.Width * 0.5 * baseSpeed;
                player_stat_speed_bar.Height = main_left.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseSpeed)), (byte)(255 * baseSpeed), (byte)(64 * baseSpeed)));
                player_stat_speed_bar.Fill = brush;
                player_stat_speed_bar.Margin = barMargin;

                barMargin.Bottom += main_left.Height * 0.035;
                player_stat_sdef_bar.Width = main_left.Width * 0.5 * baseSDef;
                player_stat_sdef_bar.Height = main_left.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseSDef)), (byte)(255 * baseSDef), (byte)(64 * baseSDef)));
                player_stat_sdef_bar.Fill = brush;
                player_stat_sdef_bar.Margin = barMargin;

                barMargin.Bottom += main_left.Height * 0.035;
                player_stat_satk_bar.Width = main_left.Width * 0.5 * baseSAtk;
                player_stat_satk_bar.Height = main_left.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseSAtk)), (byte)(255 * baseSAtk), (byte)(64 * baseSAtk)));
                player_stat_satk_bar.Fill = brush;
                player_stat_satk_bar.Margin = barMargin;

                barMargin.Bottom += main_left.Height * 0.035;
                player_stat_fdef_bar.Width = main_left.Width * 0.5 * baseFDef;
                player_stat_fdef_bar.Height = main_left.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseFDef)), (byte)(255 * baseFDef), (byte)(64 * baseFDef)));
                player_stat_fdef_bar.Fill = brush;
                player_stat_fdef_bar.Margin = barMargin;

                barMargin.Bottom += main_left.Height * 0.035;
                player_stat_fatk_bar.Width = main_left.Width * 0.5 * baseFAtk;
                player_stat_fatk_bar.Height = main_left.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseFAtk)), (byte)(255 * baseFAtk), (byte)(64 * baseFAtk)));
                player_stat_fatk_bar.Fill = brush;
                player_stat_fatk_bar.Margin = barMargin;

                barMargin.Bottom += main_left.Height * 0.035;
                player_stat_hp_bar.Width = main_left.Width * 0.5 * baseHP;
                player_stat_hp_bar.Height = main_left.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseHP)), (byte)(255 * baseHP), (byte)(64 * baseHP)));
                player_stat_hp_bar.Fill = brush;
                player_stat_hp_bar.Margin = barMargin;

                player_stats.Visibility = Visibility.Visible;

            }

        }

        private void SetEnemyBaseStats(PuppetEntity? puppet)
        {

            if (puppet == null)
            {
                enemy_stats.Visibility = Visibility.Hidden;
            }
            else
            {

                Thickness labelMargin = new Thickness(main_right.Width * 0.05, 0, 0, main_right.Height * 0.015);
                enemy_stat_speed_label.Width = main_right.Width * 0.4;
                enemy_stat_speed_label.Margin = labelMargin;

                labelMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_sdef_label.Width = main_right.Width * 0.4;
                enemy_stat_sdef_label.Margin = labelMargin;

                labelMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_satk_label.Width = main_right.Width * 0.4;
                enemy_stat_satk_label.Margin = labelMargin;

                labelMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_fdef_label.Width = main_right.Width * 0.4;
                enemy_stat_fdef_label.Margin = labelMargin;

                labelMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_fatk_label.Width = main_right.Width * 0.4;
                enemy_stat_fatk_label.Margin = labelMargin;

                labelMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_hp_label.Width = main_right.Width * 0.4;
                enemy_stat_hp_label.Margin = labelMargin;

                double baseHP = puppet.PuppetStyle.BaseStats.HP / 250.0;
                double baseFAtk = puppet.PuppetStyle.BaseStats.FAtk / 165.0;
                double baseFDef = puppet.PuppetStyle.BaseStats.FDef / 205.0;
                double baseSAtk = puppet.PuppetStyle.BaseStats.SAtk / 165.0;
                double baseSDef = puppet.PuppetStyle.BaseStats.SDef / 205.0;
                double baseSpeed = puppet.PuppetStyle.BaseStats.Spd / 205.0;
                Brush brush;

                Thickness barMargin = new Thickness(main_right.Width * 0.45, 0, 0, main_right.Height * 0.02);
                enemy_stat_speed_bar.Width = main_right.Width * 0.5 * baseSpeed;
                enemy_stat_speed_bar.Height = main_right.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseSpeed)), (byte)(255 * baseSpeed), (byte)(64 * baseSpeed)));
                enemy_stat_speed_bar.Fill = brush;
                enemy_stat_speed_bar.Margin = barMargin;

                barMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_sdef_bar.Width = main_right.Width * 0.5 * baseSDef;
                enemy_stat_sdef_bar.Height = main_right.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseSDef)), (byte)(255 * baseSDef), (byte)(64 * baseSDef)));
                enemy_stat_sdef_bar.Fill = brush;
                enemy_stat_sdef_bar.Margin = barMargin;

                barMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_satk_bar.Width = main_right.Width * 0.5 * baseSAtk;
                enemy_stat_satk_bar.Height = main_right.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseSAtk)), (byte)(255 * baseSAtk), (byte)(64 * baseSAtk)));
                enemy_stat_satk_bar.Fill = brush;
                enemy_stat_satk_bar.Margin = barMargin;

                barMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_fdef_bar.Width = main_right.Width * 0.5 * baseFDef;
                enemy_stat_fdef_bar.Height = main_right.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseFDef)), (byte)(255 * baseFDef), (byte)(64 * baseFDef)));
                enemy_stat_fdef_bar.Fill = brush;
                enemy_stat_fdef_bar.Margin = barMargin;

                barMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_fatk_bar.Width = main_right.Width * 0.5 * baseFAtk;
                enemy_stat_fatk_bar.Height = main_right.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseFAtk)), (byte)(255 * baseFAtk), (byte)(64 * baseFAtk)));
                enemy_stat_fatk_bar.Fill = brush;
                enemy_stat_fatk_bar.Margin = barMargin;

                barMargin.Bottom += main_right.Height * 0.035;
                enemy_stat_hp_bar.Width = main_right.Width * 0.5 * baseHP;
                enemy_stat_hp_bar.Height = main_right.Height * 0.02;
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 * (1 - baseHP)), (byte)(255 * baseHP), (byte)(64 * baseHP)));
                enemy_stat_hp_bar.Fill = brush;
                enemy_stat_hp_bar.Margin = barMargin;

                enemy_stats.Visibility = Visibility.Visible;

            }

        }

    }
}
