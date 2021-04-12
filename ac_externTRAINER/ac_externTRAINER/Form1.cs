using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ac_externTRAINER.utils;
using HotKeyUtilities;
using Memory;

namespace ac_externTRAINER
{
    public partial class Form1 : Form
    {
        #region Initialize Program / Atatch Handle
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            keySettings.Init();
            LocalMemory.OpenProcess(LocalMemory.GetProcIdFromName("ac_client"));
            setKeys();

            CheckForIllegalCrossThreadCalls = false;
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x8000 | 0x20);

            GetWindowRect(handle, out rect);
            this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);

            this.Left = rect.left;
            this.Top = rect.top;

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                GetWindowRect(handle, out rect);
                this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);

                this.Left = rect.left;
                this.Top = rect.top;

                int PID = LocalMemory.GetProcIdFromName("ac_client");
                if (PID > 0) { } else
                {
                    Application.Exit();
                }

                Thread.Sleep(100);
            }
        }
        #endregion

        #region Class / Variables / DLLImports
        Mem LocalMemory = new Mem();
        MemoryOffsets addrs = new MemoryOffsets();
        TrainerVariables vars = new TrainerVariables();
        GlobalKeyboardHook gkh = new GlobalKeyboardHook();
        KeySettings keySettings = new KeySettings();

        public const string WINDOW_NAME = "AssaultCube";

        public static RECT rect;

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        public static IntPtr handle = FindWindow(null, WINDOW_NAME);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string IpClassName, string IpWindowName);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT IpRect);
        #endregion

        #region HotKey setup / HotKey handle
        private void setKeys()
        {
            gkh.HookedKeys.Add(keySettings.freezeAmmoKey);
            gkh.HookedKeys.Add(keySettings.flickerHealthKey);
            gkh.HookedKeys.Add(keySettings.flyHackKey);
            gkh.HookedKeys.Add(keySettings.invisibleToggleKey);
            gkh.HookedKeys.Add(keySettings.ammoCountUpKey);
            gkh.HookedKeys.Add(keySettings.sniperOnlyZommKey);

            gkh.HookedKeys.Add(keySettings.showOverlayToggelKey);
            gkh.HookedKeys.Add(keySettings.respawnKey);

            FreezeAmmoB.Text = FreezeAmmoB.Text.Replace("{HotKey}", keySettings.freezeAmmoKey.ToString());
            FlickerHealthB.Text = FlickerHealthB.Text.Replace("{HotKey}", keySettings.flickerHealthKey.ToString());
            FlyHackB.Text = FlyHackB.Text.Replace("{HotKey}", keySettings.flyHackKey.ToString());
            InvisibleB.Text = InvisibleB.Text.Replace("{HotKey}", keySettings.invisibleToggleKey.ToString());
            AmmoCountUpB.Text = AmmoCountUpB.Text.Replace("{HotKey}", keySettings.ammoCountUpKey.ToString());
            SniperOnlyZoomB.Text = SniperOnlyZoomB.Text.Replace("{HotKey}", keySettings.sniperOnlyZommKey.ToString());

            RespawnB.Text = RespawnB.Text.Replace("{HotKey}", keySettings.respawnKey.ToString());

            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
        }

        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keySettings.freezeAmmoKey)
            {
                FreezeAmmoCheat();
            }
            else if (e.KeyCode == keySettings.flickerHealthKey)
            {
                FlickerHealthCheat();
            }
            else if (e.KeyCode == keySettings.flyHackKey)
            {
                FlyHack();
            }
            else if (e.KeyCode == keySettings.invisibleToggleKey)
            {
                InvisibleToggle();
            }
            else if (e.KeyCode == keySettings.ammoCountUpKey)
            {
                AmmoCountUpCheat();
            }
            else if (e.KeyCode == keySettings.sniperOnlyZommKey)
            {
                SniperOnlyZoomCheat();
            }

            else if (e.KeyCode == keySettings.showOverlayToggelKey)
            {
                toggelShowOV();
            }
            else if (e.KeyCode == keySettings.respawnKey)
            {
                Respawn();
            }

            e.Handled = true;
        }

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }


        #endregion

        #region Cheat Functions / Essential Code

        #region [[Essentials]]
        private void toggelShowOV()
        {
            if (vars.OVShown)
            {
                vars.OVShown = false;
                this.Opacity = .0;
            }
            else
            {
                vars.OVShown = true;
                this.Opacity = .75;
            }
        }

        private void Respawn()
        {
            LocalMemory.WriteMemory(addrs.deadStatusAddr, "byte", "1");
        }

        public struct RECT
        {
            public int left, top, right, bottom;
        }
        #endregion

        #region FreezeAmmo Cheat
        private void button1_Click(object sender, EventArgs e){}

        private void FreezeAmmoCheat()
        {
            if (vars.FreezeAmmoRunning)
            {
                vars.FreezeAmmoRunning = false;

                OnOff1.Text = "Off";
                OnOff1.ForeColor = Color.Red;
                timer1.Stop();
            }
            else
            {
                vars.FreezeAmmoRunning = true;

                vars.current_akimboAmmo = LocalMemory.ReadInt(addrs.akimboAmmoAddr).ToString();
                vars.current_pistolAmmo = LocalMemory.ReadInt(addrs.pistolAmmoAddr).ToString();
                vars.current_arAmmo = LocalMemory.ReadInt(addrs.arAmmoAddr).ToString();
                vars.current_smgAmmo = LocalMemory.ReadInt(addrs.smgAmmoAddr).ToString();
                vars.current_sniperAmmo = LocalMemory.ReadInt(addrs.sniperAmmoAddr).ToString();
                vars.current_shotgunAmmo = LocalMemory.ReadInt(addrs.shotgunAmmoAddr).ToString();
                vars.current_carbineAmmo = LocalMemory.ReadInt(addrs.carbineAmmoAddr).ToString();

                OnOff1.Text = "ON";
                OnOff1.ForeColor = Color.LimeGreen;
                timer1.Start();
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            LocalMemory.WriteMemory(addrs.akimboAmmoAddr, "int", vars.current_akimboAmmo);
            LocalMemory.WriteMemory(addrs.pistolAmmoAddr, "int", vars.current_pistolAmmo);
            LocalMemory.WriteMemory(addrs.arAmmoAddr, "int", vars.current_arAmmo);
            LocalMemory.WriteMemory(addrs.smgAmmoAddr, "int", vars.current_smgAmmo);
            LocalMemory.WriteMemory(addrs.sniperAmmoAddr, "int", vars.current_sniperAmmo);
            LocalMemory.WriteMemory(addrs.shotgunAmmoAddr, "int", vars.current_shotgunAmmo);
            LocalMemory.WriteMemory(addrs.carbineAmmoAddr, "int", vars.current_carbineAmmo);
        }
        #endregion

        #region FlickerHealth Cheat
        private void FlickerHealthCheat()
        {
            if (vars.FlickerHealthRunning)
            {
                vars.FlickerHealthRunning = false;

                LocalMemory.WriteMemory(addrs.amourLvlAddr, "int", vars.current_amourLvl);
                LocalMemory.WriteMemory(addrs.healthLvlAddr, "int", "100");

                OnOff2.Text = "Off";
                OnOff2.ForeColor = Color.Red;
                timer2.Stop();
            }
            else
            {
                vars.FlickerHealthRunning = true;

                vars.current_amourLvl = LocalMemory.ReadInt(addrs.amourLvlAddr).ToString();

                OnOff2.Text = "ON";
                OnOff2.ForeColor = Color.LimeGreen;
                timer2.Start();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (LocalMemory.ReadInt(addrs.amourLvlAddr) == 250)
            {
                LocalMemory.WriteMemory(addrs.amourLvlAddr, "int", "500");
            }
            else
            {
                LocalMemory.WriteMemory(addrs.amourLvlAddr, "int", "250");
            }

            if (LocalMemory.ReadInt(addrs.healthLvlAddr) == 250)
            {
                LocalMemory.WriteMemory(addrs.healthLvlAddr, "int", "500");
            }
            else
            {
                LocalMemory.WriteMemory(addrs.healthLvlAddr, "int", "250");
            }
        }
        #endregion

        #region Fly Hack
        private void FlyHack()
        {
            if (vars.FlyHackRunning)
            {
                vars.FlyHackRunning = false;

                LocalMemory.WriteMemory(addrs.flyToggleAddr, "byte", "0");

                OnOff3.Text = "Off";
                OnOff3.ForeColor = Color.Red;
            }
            else
            {
                vars.FlyHackRunning = true;

                LocalMemory.WriteMemory(addrs.flyToggleAddr, "byte", "1");

                OnOff3.Text = "ON";
                OnOff3.ForeColor = Color.LimeGreen;
            }
        }
        #endregion

        #region Invisible Hack
        private void InvisibleToggle()
        {
            if (vars.InvisibleRunning)
            {
                vars.InvisibleRunning = false;

                LocalMemory.WriteMemory(addrs.speedAddr, "int", "0");

                OnOff4.Text = "Off";
                OnOff4.ForeColor = Color.Red;
            }
            else
            {
                vars.InvisibleRunning = true;

                LocalMemory.WriteMemory(addrs.speedAddr, "int", "999948288");

                OnOff4.Text = "ON";
                OnOff4.ForeColor = Color.LimeGreen;
            }
        }
        #endregion
         
        #region AmmoCountUp Cheat
        private void AmmoCountUpCheat()
        {
            if (vars.AmmoCountUpRunning)
            {
                vars.AmmoCountUpRunning = false;

                LocalMemory.WriteMemory(addrs.akimboAmmoAddr, "int", vars.current_akimboAmmo);
                LocalMemory.WriteMemory(addrs.pistolAmmoAddr, "int", vars.current_pistolAmmo);
                LocalMemory.WriteMemory(addrs.arAmmoAddr, "int", vars.current_arAmmo);
                LocalMemory.WriteMemory(addrs.smgAmmoAddr, "int", vars.current_smgAmmo);
                LocalMemory.WriteMemory(addrs.sniperAmmoAddr, "int", vars.current_sniperAmmo);
                LocalMemory.WriteMemory(addrs.shotgunAmmoAddr, "int", vars.current_shotgunAmmo);
                LocalMemory.WriteMemory(addrs.carbineAmmoAddr, "int", vars.current_carbineAmmo);

                OnOff5.Text = "Off";
                OnOff5.ForeColor = Color.Red;
            }
            else
            {
                vars.AmmoCountUpRunning = true;

                vars.current_akimboAmmo = LocalMemory.ReadInt(addrs.akimboAmmoAddr).ToString();
                vars.current_pistolAmmo = LocalMemory.ReadInt(addrs.pistolAmmoAddr).ToString();
                vars.current_arAmmo = LocalMemory.ReadInt(addrs.arAmmoAddr).ToString();
                vars.current_smgAmmo = LocalMemory.ReadInt(addrs.smgAmmoAddr).ToString();
                vars.current_sniperAmmo = LocalMemory.ReadInt(addrs.sniperAmmoAddr).ToString();
                vars.current_shotgunAmmo = LocalMemory.ReadInt(addrs.shotgunAmmoAddr).ToString();
                vars.current_carbineAmmo = LocalMemory.ReadInt(addrs.carbineAmmoAddr).ToString();

                LocalMemory.WriteMemory(addrs.akimboAmmoAddr, "int", "-1");
                LocalMemory.WriteMemory(addrs.pistolAmmoAddr, "int", "-1");
                LocalMemory.WriteMemory(addrs.arAmmoAddr, "int", "-1");
                LocalMemory.WriteMemory(addrs.smgAmmoAddr, "int", "-1");
                LocalMemory.WriteMemory(addrs.sniperAmmoAddr, "int", "-1");
                LocalMemory.WriteMemory(addrs.shotgunAmmoAddr, "int", "-1");
                LocalMemory.WriteMemory(addrs.carbineAmmoAddr, "int", "-1");

                OnOff5.Text = "ON";
                OnOff5.ForeColor = Color.LimeGreen;
            }
        }
        #endregion

        #region SniperOnlyZoom Cheat
        private void SniperOnlyZoomCheat()
        {
            if (vars.SniperOnlyZoomRunning)
            {
                vars.SniperOnlyZoomRunning = false;

                LocalMemory.WriteMemory(addrs.scopeOvAddr, "bytes", "85 37 04 00 00 68 03 03 00 00");

                OnOff6.Text = "Off";
                OnOff6.ForeColor = Color.Red;
            }
            else
            {
                vars.SniperOnlyZoomRunning = true;

                LocalMemory.WriteMemory(addrs.scopeOvAddr, "bytes", "84 37 04 00 00 68 03 03 00 00");

                OnOff6.Text = "ON";
                OnOff6.ForeColor = Color.LimeGreen;
            }
        }
        #endregion

        #endregion
    }
}