
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
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

        private PuppetEntity? LastPlayerPuppet = null;
        private PuppetEntity? LastEnemyPuppet = null;

        private Rectangle? LastBounds = null;

        public BattleMain()
        {
            InitializeComponent();

            CompositionTarget.Rendering += Loop;
        }

        private void Loop(object sender, EventArgs e)
        {

            // Reposition
            Rectangle bounds = GameHook.FindGameBounds();
            Reposition(bounds);

            // Get player puppet
            short playerPuppetId = GameHook.ReadAddress(0xC59FDB, 2).Select(b => (short)b).ToArray()[0];
            byte playerPuppetStyleIdx = GameHook.ReadAddress(0xC59FDD, 1)[0];
            PuppetEntity? playerPuppet = PuppetEntity.FindPuppet(playerPuppetId, playerPuppetStyleIdx);

            if (RequiresUpdating(playerPuppet, LastPlayerPuppet, bounds))
            {
                SetPlayerTypes(playerPuppet);
                SetPlayerWeaknesses(playerPuppet);
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
            }
            LastEnemyPuppet = enemyPuppet;

            LastBounds = bounds;

        }

        private void Reposition(Rectangle currentBounds)
        {
            int sidebarHeight = currentBounds.Height - App.TITLE_BAR_HEIGHT;
            int newWidth = (int)(sidebarHeight * App.ASPECT_RATIO);
            int sidebarWidth = (newWidth - currentBounds.Width) / 2;

            // Place entire window
            window.Left = currentBounds.Left - sidebarWidth;
            window.Top = currentBounds.Top;
            window.Width = newWidth;
            window.Height = currentBounds.Height;

            // Resize margins
            main_left.Width = sidebarWidth;
            main_left.Height = sidebarHeight;
            main_right.Width = sidebarWidth;
            main_right.Height = sidebarHeight;
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

            if (puppet == null) {
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

        private void PopulateResistanceImages(ElementalType[] types, Label label, WrapPanel panel, Grid sidebar)
        {
            if (types.Length > 0)
            {
                panel.Children.Clear();
                foreach (ElementalType type in types)
                {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(type.FilePath, UriKind.Relative));
                    image.Width = sidebar.Width * 0.25;
                    Thickness margin = new Thickness(sidebar.Width * 0.05, 0, 0, sidebar.Height * 0.01);
                    image.Margin = margin;
                    panel.Children.Add(image);
                }
                label.Visibility = Visibility.Visible;
                panel.Visibility = Visibility.Visible;
            }
            else
            {
                label.Visibility = Visibility.Collapsed;
                panel.Visibility = Visibility.Collapsed;
            }
        }

    }



}