
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPDP_Battle_Helper.Data;
using TPDP_Battle_Helper.Data.Skill;

namespace TPDP_Battle_Helper.Views
{
    /// <summary>
    /// Interaction logic for PageBattleMain.xaml
    /// </summary>
    public partial class PageBattleMove : Page
    {

        private double LastContentTime = 0;

        private MainWindow mainWindow;
        public bool Active = false;

        private PuppetEntity? LastPlayerPuppet = null;
        private PuppetEntity? LastEnemyPuppet = null;

        private Rectangle? LastBounds = null;

        public PageBattleMove(MainWindow window)
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

            // Puppet addresses
            byte battleContextMenu = GameHook.ReadAddress(0x93C164, 1)[0];

            // Get player puppet
            uint playerPuppetAddress = 0xC59FDB;
            byte[] playerPuppetIdTmp = GameHook.ReadAddress(playerPuppetAddress, 2);
            ushort playerPuppetId = (ushort)(playerPuppetIdTmp[0] + playerPuppetIdTmp[1] * 0x0100);
            byte playerPuppetStyleIdx = GameHook.ReadAddress(playerPuppetAddress + 0x02, 1)[0];
            PuppetEntity? playerPuppet = PuppetEntity.FindPuppet(playerPuppetId, playerPuppetStyleIdx);

            bool playerRequiresUpdating = RequiresUpdating(playerPuppet, LastPlayerPuppet, bounds);
            if (playerRequiresUpdating)
            {
                if (playerPuppet != null)
                    Debug.WriteLine("Displaying Player Puppet: " + playerPuppet.PuppetSpecies.PuppetName + " (" + playerPuppet.PuppetStyle.StyleName + ")");
                SetPlayerTypes(playerPuppet);
                SetPlayerWeaknesses(playerPuppet);
            }

            // Get enemy puppet
            uint enemyPuppetAddress = 0xC5A510;
            byte[] enemyPuppetIdTmp = GameHook.ReadAddress(enemyPuppetAddress, 2);
            ushort enemyPuppetId = (ushort)(enemyPuppetIdTmp[0] + enemyPuppetIdTmp[1] * 0x0100);
            byte enemyPuppetStyleIdx = GameHook.ReadAddress(enemyPuppetAddress + 0x02, 1)[0];
            PuppetEntity? enemyPuppet = PuppetEntity.FindPuppet(enemyPuppetId, enemyPuppetStyleIdx);

            bool enemyRequiresUpdating = RequiresUpdating(enemyPuppet, LastEnemyPuppet, bounds);
            if (enemyRequiresUpdating)
            {
                if (enemyPuppet != null)
                    Debug.WriteLine("Displaying Enemy Puppet: " + enemyPuppet.PuppetSpecies.PuppetName + " (" + enemyPuppet.PuppetStyle.StyleName + ")");
                SetEnemyTypes(enemyPuppet);
                SetEnemyWeaknesses(enemyPuppet);
            }

            // Get player skills
            uint playerSkillsAddress = 0xC59FED;
            byte[] playerSkillIdTmp = GameHook.ReadAddress(playerSkillsAddress, 2);
            ushort playerSkillId1 = (ushort)(playerSkillIdTmp[0] + playerSkillIdTmp[1] * 0x0100);
            playerSkillIdTmp = GameHook.ReadAddress(playerSkillsAddress + 0x02, 2);
            ushort playerSkillId2 = (ushort)(playerSkillIdTmp[0] + playerSkillIdTmp[1] * 0x0100);
            playerSkillIdTmp = GameHook.ReadAddress(playerSkillsAddress + 0x04, 2);
            ushort playerSkillId3 = (ushort)(playerSkillIdTmp[0] + playerSkillIdTmp[1] * 0x0100);
            playerSkillIdTmp = GameHook.ReadAddress(playerSkillsAddress + 0x06, 2);
            ushort playerSkillId4 = (ushort)(playerSkillIdTmp[0] + playerSkillIdTmp[1] * 0x0100);

            if (playerRequiresUpdating || enemyRequiresUpdating)
            {
                if (playerPuppet != null)
                    Debug.WriteLine("Displaying moves for Player Puppet: " + playerPuppet.PuppetSpecies.PuppetName + " (" + playerPuppet.PuppetStyle.StyleName + ")");
                SetPlayerSkills(enemyPuppet, playerSkillId1, playerSkillId2, playerSkillId3, playerSkillId4);
                LastPlayerPuppet = playerPuppet;
                LastEnemyPuppet = enemyPuppet;
            }

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
                    enemy_type2.Visibility = Visibility.Hidden;
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

        private void SetPlayerSkills(PuppetEntity? enemyPuppet, ushort skillId1, ushort skillId2, ushort skillId3, ushort skillId4)
        {
            
            SkillDexEntry skill1 = SkillDex.FindSkill(skillId1);
            SkillDexEntry skill2 = SkillDex.FindSkill(skillId2);
            SkillDexEntry skill3 = SkillDex.FindSkill(skillId3);
            SkillDexEntry skill4 = SkillDex.FindSkill(skillId4);

            Thickness labelMargin = new Thickness(0, 0, 0, 0);

            labelMargin.Bottom = main_right.Height * 0.23;
            player_skill_1.Width = main_left.Width * 0.8;
            player_skill_1.Margin = labelMargin;
            player_skill_2.Width = main_left.Width * 0.8;
            player_skill_2.Margin = labelMargin;

            labelMargin.Bottom = main_right.Height * 0.055;
            player_skill_3.Width = main_left.Width * 0.8;
            player_skill_3.Margin = labelMargin;
            player_skill_4.Width = main_left.Width * 0.8;
            player_skill_4.Margin = labelMargin;

            double? damageMultiplier1 = null;
            if (skill1 != null && enemyPuppet != null && skill1.SkillCategory.Damaging)
                damageMultiplier1 = skill1.SkillType.AttackMultiplierAgainst(enemyPuppet.PuppetStyle.Type1, enemyPuppet.PuppetStyle.Type2);
            ReplaceEffectivenessImage(player_skill_1, damageMultiplier1, false);
            
            double? damageMultiplier2 = null;
            if (skill2 != null && enemyPuppet != null && skill2.SkillCategory.Damaging)
                damageMultiplier2 = skill2.SkillType.AttackMultiplierAgainst(enemyPuppet.PuppetStyle.Type1, enemyPuppet.PuppetStyle.Type2);
            ReplaceEffectivenessImage(player_skill_2, damageMultiplier2, true);

            double? damageMultiplier3 = null;
            if (skill3 != null && enemyPuppet != null && skill3.SkillCategory.Damaging)
                damageMultiplier3 = skill3.SkillType.AttackMultiplierAgainst(enemyPuppet.PuppetStyle.Type1, enemyPuppet.PuppetStyle.Type2);
            ReplaceEffectivenessImage(player_skill_3, damageMultiplier3, false);

            double? damageMultiplier4 = null;
            if (skill4 != null && enemyPuppet != null && skill4.SkillCategory.Damaging)
                damageMultiplier4 = skill4.SkillType.AttackMultiplierAgainst(enemyPuppet.PuppetStyle.Type1, enemyPuppet.PuppetStyle.Type2);
            ReplaceEffectivenessImage(player_skill_4, damageMultiplier4, true);

        }

        private void ReplaceEffectivenessImage(Image imageObject, double? damageMultiplier, bool rightSide)
        {

            string sideString = "left";
            if (rightSide) sideString = "right";

            BitmapImage image;
            switch (damageMultiplier)
            {
                case 0.0:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/x0_" + sideString + ".png", UriKind.Relative));
                    break;
                case 0.25:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/x0.25_" + sideString + ".png", UriKind.Relative));
                    break;
                case 0.5:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/x0.5_" + sideString + ".png", UriKind.Relative));
                    break;
                case 1:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/x1_" + sideString + ".png", UriKind.Relative));
                    break;
                case 2:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/x2_" + sideString + ".png", UriKind.Relative));
                    break;
                case 4:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/x4_" + sideString + ".png", UriKind.Relative));
                    break;
                default:
                    image = new BitmapImage(new Uri("/Resources/Effectiveness/blank_" + sideString + ".png", UriKind.Relative));
                    break;
            }
            imageObject.Source = image;
        }

    }
}
