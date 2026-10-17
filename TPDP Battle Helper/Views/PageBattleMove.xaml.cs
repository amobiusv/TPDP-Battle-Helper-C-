
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

        // 实时更新：缓存上一次真正显示出的原始值，用于变化检测。
        // 不再缓存 PuppetEntity 对象——游戏内存里各字段更新并不同步
        // （例如切换人形时 id 先变、技能 id 滞后一帧），用对象判等会漏刷新。
        private Rectangle? LastBounds = null;
        private ushort lastPlayerId = 0, lastPlayerStyle = 0;
        private ushort lastEnemyId = 0, lastEnemyStyle = 0;
        private ushort lastSkill1 = 0, lastSkill2 = 0, lastSkill3 = 0, lastSkill4 = 0;
        private bool lastPlayerValid = false, lastEnemyValid = false, lastSkillsValid = false;

        public PageBattleMove(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;

            CompositionTarget.Rendering += Loop;
        }

        private void Loop(object sender, EventArgs e)
        {
            if (!Active) return;
            if (!GameHook.IsHooked) return;

            // Delta Time
            double currentTime = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
            double deltaTime = currentTime - LastContentTime;
            if (deltaTime < 1000 / App.CONTENT_REFRESH_RATE) return;
            LastContentTime = currentTime - currentTime % (1000 / App.CONTENT_REFRESH_RATE);

            // Reposition
            Rectangle bounds = GameHook.FindGameBounds();
            mainWindow.Reposition(bounds, page, main_left, main_right);
            bool boundsChanged = LastBounds == null
                || ((Rectangle)LastBounds).Width != bounds.Width
                || ((Rectangle)LastBounds).Height != bounds.Height;

            // === 实时读取：每帧都从内存取最新值 ===

            // 我方人形
            byte[] playerPuppetIdTmp = GameHook.ReadAddress(0xC59FDB, 2);
            ushort playerPuppetId = (ushort)(playerPuppetIdTmp[0] + playerPuppetIdTmp[1] * 0x0100);
            byte playerPuppetStyleIdx = GameHook.ReadAddress(0xC59FDB + 0x02, 1)[0];
            PuppetEntity? playerPuppet = PuppetEntity.FindPuppet(playerPuppetId, playerPuppetStyleIdx);
            bool playerValid = playerPuppet != null;

            // 敌方人形
            byte[] enemyPuppetIdTmp = GameHook.ReadAddress(0xC5A510, 2);
            ushort enemyPuppetId = (ushort)(enemyPuppetIdTmp[0] + enemyPuppetIdTmp[1] * 0x0100);
            byte enemyPuppetStyleIdx = GameHook.ReadAddress(0xC5A510 + 0x02, 1)[0];
            PuppetEntity? enemyPuppet = PuppetEntity.FindPuppet(enemyPuppetId, enemyPuppetStyleIdx);
            bool enemyValid = enemyPuppet != null;

            // 我方技能（4 个）
            ushort playerSkillId1 = ReadU16(0xC59FED);
            ushort playerSkillId2 = ReadU16(0xC59FED + 0x02);
            ushort playerSkillId3 = ReadU16(0xC59FED + 0x04);
            ushort playerSkillId4 = ReadU16(0xC59FED + 0x06);
            // 切换动画瞬间技能槽会短暂读成 0，视为瞬态，跳过更新以保留上一帧
            bool skillsValid = playerSkillId1 != 0 && playerSkillId2 != 0 && playerSkillId3 != 0 && playerSkillId4 != 0;

            // 变化检测：原始值变了才重绘（瞬态无效值跳过，避免空白闪烁/冻结）
            bool playerChanged = boundsChanged || (playerValid && (!lastPlayerValid || playerPuppetId != lastPlayerId || playerPuppetStyleIdx != lastPlayerStyle));
            bool enemyChanged  = boundsChanged || (enemyValid && (!lastEnemyValid || enemyPuppetId != lastEnemyId || enemyPuppetStyleIdx != lastEnemyStyle));
            bool skillChanged  = boundsChanged || (skillsValid && (!lastSkillsValid || playerSkillId1 != lastSkill1 || playerSkillId2 != lastSkill2 || playerSkillId3 != lastSkill3 || playerSkillId4 != lastSkill4));

            if (playerChanged && playerValid)
            {
                Debug.WriteLine("Displaying Player Puppet: " + playerPuppet!.PuppetSpecies.PuppetName + " (" + playerPuppet.PuppetStyle.StyleName + ")");
                SetPlayerTypes(playerPuppet);
                SetPlayerWeaknesses(playerPuppet);
                lastPlayerId = playerPuppetId;
                lastPlayerStyle = playerPuppetStyleIdx;
                lastPlayerValid = true;
            }

            if (enemyChanged && enemyValid)
            {
                Debug.WriteLine("Displaying Enemy Puppet: " + enemyPuppet!.PuppetSpecies.PuppetName + " (" + enemyPuppet.PuppetStyle.StyleName + ")");
                SetEnemyTypes(enemyPuppet);
                SetEnemyWeaknesses(enemyPuppet);
                lastEnemyId = enemyPuppetId;
                lastEnemyStyle = enemyPuppetStyleIdx;
                lastEnemyValid = true;
            }

            // 技能倍率依赖「技能属性 × 敌方属性」，任一方变化都要重算
            if ((skillChanged || enemyChanged) && skillsValid && enemyValid)
            {
                Debug.WriteLine("Displaying moves for Player Puppet: " + (playerPuppet?.PuppetSpecies.PuppetName ?? "?"));
                SetPlayerSkills(enemyPuppet, playerSkillId1, playerSkillId2, playerSkillId3, playerSkillId4);
                lastSkill1 = playerSkillId1;
                lastSkill2 = playerSkillId2;
                lastSkill3 = playerSkillId3;
                lastSkill4 = playerSkillId4;
                lastSkillsValid = true;
            }

            LastBounds = bounds;

            // Switch pages
            bool transitioned = mainWindow.ScreenTransition(this);
            if (transitioned) Active = true;

        }

        private ushort ReadU16(uint offset)
        {
            byte[] b = GameHook.ReadAddress(offset, 2);
            return (ushort)(b[0] + b[1] * 0x0100);
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
