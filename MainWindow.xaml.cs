using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Application = System.Windows.Forms.Application;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ClipboardImageSaver
{
    public partial class MainWindow : Window
    {
        #region Readonly Variable
        private readonly int hotkeyId = 31197;
        private readonly uint altModifier = 0x0001;
        private readonly uint virtualKeyS = 0x53;
        private readonly string themeRegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        #endregion

        #region Private Variable
        private NotifyIcon notifyIcon;
        private ContextMenuStrip notifyContextMenu;
        private ClipboardHelper clipboardHelper;

        private HwndSource hwndSource;
        private WindowInteropHelper helper;
        #endregion

        #region Properties
        // If registry key value is 0, theme is dark, else if value is 1, then light.
        bool IsLightTheme { get => (int)Registry.GetValue(themeRegistryKey, "SystemUsesLightTheme", string.Empty) == 1; } 
        #endregion

        #region Interop Method
        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            clipboardHelper = new ClipboardHelper("Screenshot", @"C:\Users\leehs\Pictures\Screenshots", ImageFormat.Png);

            notifyContextMenu = new ContextMenuStrip();

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += OnExitItemClicked;
            notifyContextMenu.Items.Add(exitItem);

            notifyIcon = new NotifyIcon();

            // If theme is light, use black icon, else use white icon.
            notifyIcon.Icon = IsLightTheme ? Properties.Resources.ClipboardBlack : Properties.Resources.ClipboardWhite;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = notifyContextMenu;
            notifyIcon.Text = "Clipboard Image Saver";

            notifyIcon.DoubleClick += OnNotifyIconDoubleClicked;
        }
        #endregion

        #region Protected Methods
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            helper = new WindowInteropHelper(this);
            hwndSource = HwndSource.FromHwnd(helper.Handle);
            hwndSource.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            hwndSource.RemoveHook(HwndHook);
            hwndSource = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }
        #endregion

        #region Private Methods
        private void RegisterHotKey()
        {
            RegisterHotKey(helper.Handle, hotkeyId, altModifier, virtualKeyS);
        }

        private void UnregisterHotKey()
        {
            UnregisterHotKey(helper.Handle, hotkeyId);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int hotkeyMessage = 0x0312;

            if(msg == hotkeyMessage && wParam.ToInt32() == hotkeyId)
            {
                OnHotKeyPressed();
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            clipboardHelper.SaveClipboardImage();
        }

        private void OnExitItemClicked(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Close();
            Application.Exit();
        }

        private void OnNotifyIconDoubleClicked(object sender, EventArgs e)
        {
            //Show();
        }
        #endregion
    }
}
