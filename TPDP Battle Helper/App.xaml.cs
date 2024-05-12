using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;

namespace TPDP_Battle_Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            GameHook.Init("TPDP Shard of Dreams");
            Rectangle rect = GameHook.FindGameBounds();
            short allyId = GameHook.ReadAddress(0xC59FDB, 2).Select(b => (short)b).ToArray()[0];

            base.OnStartup(e);
        }

    }

}
