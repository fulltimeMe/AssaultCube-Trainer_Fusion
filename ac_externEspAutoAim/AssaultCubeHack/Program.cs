using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using MYClasses;

namespace AssaultCubeHack {

    class Program {
        const Int32 SW_MINIMIZE = 6;

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);

        private static void MinimizeConsoleWindow()
        {
            IntPtr hWndConsole = GetConsoleWindow();
            ShowWindow(hWndConsole, 0);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Console.Title = "ESP & AimClip Runtime";
            Thread TH = new Thread(CheckForProc);
            TH.Start();
            MinimizeConsoleWindow();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AssaultHack());       
        }

        static void CheckForProc()
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("ac_client");
                if (processes.Length == 0)
                {
                    Environment.Exit(-1);
                }
            }
        }

        public string hotkeyStrFromFile(string filePath)
        {
            string res = string.Empty;
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] sub = line.Split(':');
                if (sub[0] == "AutoAimKey")
                {
                    res = sub[1].Replace(" ", string.Empty);
                }
            }
            return res;
        }

    }

    public class myStuff
    {
        public string name = "Fusion";
        public string version = "0.0.1";
        public Color playerTextColor = Color.FromArgb(66, 182, 245);
        public Color ClientTextColor = Color.FromArgb(221, 66, 245);
        public Font myFont = new Font("Unispace", 14, FontStyle.Bold);
        public Keys myAimKey = new STK_Converter().GetKeyFromString(new Program().hotkeyStrFromFile("config/config.txt"));
        public Color TeamC = Color.FromArgb(52, 216, 235);
        public Color EnemyC = Color.FromArgb(162, 0, 255);
    }
}