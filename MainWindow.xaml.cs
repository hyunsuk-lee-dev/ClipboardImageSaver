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

namespace ClipboardImageSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string ThemeRegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        readonly Key firstKey = Key.S;
        readonly Key secondKey = Key.LeftAlt;

        NotifyIcon notifyIcon;
        ContextMenuStrip notifyContextMenu;
        ClipboardHelper clipboardHelper;
        bool previousFirstKeyToggled = true;
        bool previousSecondKeyToggled = true;

        

        public MainWindow()
        {
            
            InitializeComponent();

            clipboardHelper = new ClipboardHelper("Screenshot", @"C:\Users\leehs\Pictures\Screenshots", ImageFormat.Png);

            // Capture Keyobard input for using short-cut key.
            CompositionTarget.Rendering += OnCompositionTargetRendering;

            notifyContextMenu = new ContextMenuStrip();

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += OnExitItemClicked;

            notifyContextMenu.Items.Add(exitItem);

            notifyIcon = new NotifyIcon();

            
            // 0 is Dark, 1 is Light.
            int theme = (int)Registry.GetValue(ThemeRegistryKey, "SystemUsesLightTheme", string.Empty);

            notifyIcon.Icon =  theme == 1 ? Properties.Resources.ClipboardBlack : Properties.Resources.ClipboardWhite;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = notifyContextMenu;
            notifyIcon.Text = "Clipboard Image Saver";

            notifyIcon.DoubleClick += OnNotifyIconDoubleClicked;

            Hide();

            previousFirstKeyToggled = Keyboard.IsKeyToggled(firstKey);
            previousSecondKeyToggled = Keyboard.IsKeyToggled(secondKey);
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            bool firstKeydown = Keyboard.IsKeyDown(firstKey) && previousFirstKeyToggled != Keyboard.IsKeyToggled(firstKey);
            bool secondKeydown = Keyboard.IsKeyDown(secondKey) && previousSecondKeyToggled != Keyboard.IsKeyToggled(secondKey);

            if(firstKeydown)
                previousFirstKeyToggled = Keyboard.IsKeyToggled(firstKey);

            if(secondKeydown)
                previousSecondKeyToggled = Keyboard.IsKeyToggled(secondKey);

            if((firstKeydown && Keyboard.IsKeyDown(secondKey)) || (secondKeydown && Keyboard.IsKeyDown(firstKey)))
            {
                clipboardHelper.SaveClipboardImage();
            }

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
