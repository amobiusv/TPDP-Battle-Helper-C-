
using System.IO;
using System.Windows;
using Tomlyn;
using Tomlyn.Model;
using TPDP_Battle_Helper.Data;
using TPDP_Battle_Helper.Data.Dex;

namespace TPDP_Battle_Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public List<string> GAME_TITLES = new List<string>();

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
            PuppetDex.Init();

            // Hook to the game
            GameHook.Init(GAME_TITLES);

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

        }

    }

}
