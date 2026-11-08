
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Tomlyn;
using Tomlyn.Model;
using TPDP_Battle_Helper.Data;
using TPDP_Battle_Helper.Data.Dex;
using TPDP_Battle_Helper.Data.Skill;

namespace TPDP_Battle_Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public List<string> GAME_TITLES = new List<string>();

        // Tracks whether the game was hooked on the previous reconnect tick, so we
        // can detect the transition "was running -> now gone" and close the tool.
        private bool _gameWasRunning = false;

        public static double ASPECT_RATIO;
        public static long TITLEBAR_HEIGHT;
        public static long WINDOW_WIDTH_OFFSET;

        public static long REPOSITION_REFRESH_RATE;
        public static long CONTENT_REFRESH_RATE;

        protected override void OnStartup(StartupEventArgs e)
        {
            //Config file
            LoadSettings();

            // Populate data
            ElementalType.Init();
            SkillDex.Init();
            PuppetDex.Init();

            // Hook to the game
            GameHook.Init(GAME_TITLES);

            // If the game isn't running yet (or the window title didn't match at
            // startup), keep trying to attach on a timer so the helper connects
            // as soon as the game is launched. Cheap no-op once already hooked.
            DispatcherTimer hookTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            hookTimer.Tick += (s, e) =>
            {
                bool wasRunning = _gameWasRunning;
                GameHook.Init(GAME_TITLES);
                bool isRunning = GameHook.IsHooked;
                if (wasRunning && !isRunning)
                {
                    // The game was running and is now gone: shut the helper down
                    // so it doesn't linger after the game closes.
                    Application.Current.Shutdown();
                    return;
                }
                _gameWasRunning = isRunning;
            };
            hookTimer.Start();

            base.OnStartup(e);
        }

        protected void LoadSettings()
        {

            // Load file
            string toml = File.ReadAllText(".\\Config\\AppSettings.toml");
            TomlTable table = TomlTable.From(Toml.Parse(toml));

            TomlTable mainSection = (TomlTable)table["MAIN"];
            TomlArray titleList = (TomlArray)mainSection["game_title"];
            foreach (string title in titleList)
            {
                GAME_TITLES.Add(title);
            }

            TomlTable dimensionsSection = (TomlTable)table["DIMENSIONS"];
            string aspectRatioStr = (string)dimensionsSection["window_aspect_ratio"];
            string[] aspectRatioSplit = aspectRatioStr.Split(':');
            ASPECT_RATIO = Double.Parse(aspectRatioSplit[0]) / Double.Parse(aspectRatioSplit[1]);
            TITLEBAR_HEIGHT = (long)dimensionsSection["titlebar_height"];
            WINDOW_WIDTH_OFFSET = (long)dimensionsSection["window_width_offset"];

            TomlTable refreshRateSection = (TomlTable)table["REFRESH_RATE"];
            REPOSITION_REFRESH_RATE = (long)refreshRateSection["reposition_refresh_rate"];
            CONTENT_REFRESH_RATE = (long)refreshRateSection["content_refresh_rate"];

            // Optional [ADDRESSES] section: per-game-build memory addresses
            // (absolute VAs). Missing keys keep the built-in defaults, which
            // match the AI-translated ver1.103-era build.
            if (table.TryGetValue("ADDRESSES", out object? addrObj) && addrObj is TomlTable addrSection)
            {
                if (addrSection.TryGetValue("battle_flag", out object? v)) GameHook.AddrBattleFlag = Convert.ToUInt32(v);
                if (addrSection.TryGetValue("battle_context_menu", out v)) GameHook.AddrBattleContextMenu = Convert.ToUInt32(v);
                if (addrSection.TryGetValue("map_id", out v)) GameHook.AddrMapId = Convert.ToUInt32(v);
                if (addrSection.TryGetValue("player_puppet", out v)) GameHook.AddrPlayerPuppet = Convert.ToUInt32(v);
                if (addrSection.TryGetValue("enemy_puppet", out v)) GameHook.AddrEnemyPuppet = Convert.ToUInt32(v);
                if (addrSection.TryGetValue("player_skills", out v)) GameHook.AddrPlayerSkills = Convert.ToUInt32(v);
            }

        }

    }

}
