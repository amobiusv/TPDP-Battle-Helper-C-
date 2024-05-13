
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using TPDP_Battle_Helper.Libraries;

namespace TPDP_Battle_Helper
{
   
    internal class GameHook
    {

        /* ********* *
         * VARIABLES *
         * ********* */

        #region Variables

        protected static Process? gameProcess = null;
        protected static IntPtr? gameHandle = null;

        protected static nint startAdress = -1;

        #endregion

        /* ******* *
         * IMPORTS *
         * ******* */

        #region DLL Imports

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);

        #endregion

        /* **** *
         * INIT *
         * **** */

        #region Init

        public static void Init(string windowName)
        {

            Process[] processList = Process.GetProcesses();

            // Seek game window
            foreach (Process process in processList)
            {
                string processTitle = process.MainWindowTitle;
                if (!String.IsNullOrEmpty(processTitle))
                {
                    if (processTitle.IndexOf(windowName) == 0)
                    {
                        gameProcess = process;
                        break;
                    }
                }
            }

            // Initialize readable Handle
            if (gameProcess != null)
            {
                gameHandle = Kernel32.OpenProcess((uint)(Kernel32.ProcessAccessFlags.VirtualMemoryRead | Kernel32.ProcessAccessFlags.QueryInformation), true, (uint) gameProcess.Id);
                if (gameProcess.MainModule != null)
                    startAdress = gameProcess.MainModule.BaseAddress;
            }

        }

        #endregion

        public static Rectangle FindGameBounds()
        {
            if (gameProcess == null) return new Rectangle(0, 0, 0, 0);

            Rectangle rect = new Rectangle();
            GetWindowRect(gameProcess.MainWindowHandle, out rect);

            return new Rectangle(rect.Left, rect.Top, rect.Width - rect.Left, rect.Height - rect.Top);
        }

        public static byte[] ReadAddress(uint address, uint length)
        {
            if (gameHandle == null) return [];

            byte[] buffer = new byte[length];
            int bytesRead = 0;
            Kernel32.ReadProcessMemory(
                (IntPtr) gameHandle,
                (int) (startAdress + address),
                buffer,
                buffer.Length,
                ref bytesRead
            );
            return buffer;
        }

    }
}
