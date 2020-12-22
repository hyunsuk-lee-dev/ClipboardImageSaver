using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using Application = System.Windows.Forms.Application;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Forms.VisualStyles;

namespace ClipboardImageSaver
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly int hotkeyId = 31197;
        private readonly uint altModifier = 0x0001;
        private readonly uint virtualKeyS = 0x53;
        private readonly string ThemeRegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private NotifyIcon notifyIcon;
        private ContextMenuStrip notifyContextMenu;
        private ClipboardHelper clipboardHelper;

        private HwndSource hwndSource;
        private WindowInteropHelper helper;

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow()
        {
            InitializeComponent();

            clipboardHelper = new ClipboardHelper("Screenshot", @"C:\Users\leehs\Pictures\Screenshots", ImageFormat.Png);

            notifyContextMenu = new ContextMenuStrip();

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += OnExitItemClicked;

            notifyContextMenu.Items.Add(exitItem);

            notifyIcon = new NotifyIcon();

            // 0 is Dark, 1 is Light.
            int theme = (int)Registry.GetValue(ThemeRegistryKey, "SystemUsesLightTheme", string.Empty);

            notifyIcon.Icon = theme == 1 ? Properties.Resources.ClipboardBlack : Properties.Resources.ClipboardWhite;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = notifyContextMenu;
            notifyIcon.Text = "Clipboard Image Saver";

            notifyIcon.DoubleClick += OnNotifyIconDoubleClicked;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            helper = new WindowInteropHelper(this);
            hwndSource = HwndSource.FromHwnd(helper.Handle);
            hwndSource.AddHook(new HwndSourceHook(HwndHook));
            RegisterHotKey();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Hide();
        }
        protected override void OnClosed(EventArgs e)
        {
            hwndSource.RemoveHook(new HwndSourceHook(HwndHook));
            hwndSource = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

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

    }
}
