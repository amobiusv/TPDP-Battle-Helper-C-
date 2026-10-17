
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion

        /* **** *
         * INIT *
         * **** */

        #region Init

        /// <summary>
        /// True once we have successfully opened a handle to the game process.
        /// Used by the render loop so it can safely no-op before the game is up.
        /// </summary>
        public static bool IsHooked => gameHandle != null;

        public static void Init(List<string> windowNames)
        {
            // If we were previously hooked but the process has since exited,
            // drop the now-stale handle. Otherwise IsHooked would stay true
            // forever and we could never detect that the game closed.
            if (gameHandle != null && (gameProcess == null || gameProcess.HasExited))
            {
                gameHandle = null;
                gameProcess = null;
                startAdress = -1;
            }

            // Already connected (and alive): don't re-open / re-move the window every tick.
            if (IsHooked) return;

            Process[] processList = Process.GetProcesses();

            // Seek game window
            foreach (Process process in processList)
            {
                string processTitle = process.MainWindowTitle;
                if (!String.IsNullOrEmpty(processTitle))
                {
                    bool found = false;
                    foreach (string windowName in windowNames)
                    {
                        if (processTitle.IndexOf(windowName) == 0)
                        {
                            gameProcess = process;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            }

            // Initialize readable Handle
            if (gameProcess != null)
            {
                gameHandle = Kernel32.OpenProcess((uint)(Kernel32.ProcessAccessFlags.VirtualMemoryRead | Kernel32.ProcessAccessFlags.QueryInformation), true, (uint) gameProcess.Id);
                if (gameProcess.MainModule != null)
                    startAdress = gameProcess.MainModule.BaseAddress;
                Rectangle rect = FindGameBounds();
                if (gameProcess.MainWindowHandle != null)
                    MoveWindow(gameProcess.MainWindowHandle, 0, 0, rect.Width, rect.Height, true);
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
            // Game not running / not yet found: return a zero-filled buffer so
            // callers indexing [0] don't throw and crash the render loop.
            if (gameHandle == null) return new byte[(int)length];

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
