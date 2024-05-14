using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using TPDP_Battle_Helper.Data;
using TPDP_Battle_Helper.Data.Dex;

namespace TPDP_Battle_Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static double ASPECT_RATIO = 16d / 9d;
        public static int TITLE_BAR_HEIGHT = 40;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Populate Data
            ElementalType.Init();
            PuppetDex.Init();

            // Hook to the game
            GameHook.Init("TPDP Shard of Dreams");

            base.OnStartup(e);
        }

    }

}
