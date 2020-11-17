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

namespace ClipboardImageSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon notifyIcon;
        ContextMenuStrip notifyContextMenu;

        ClipboardHelper clipboardHelper;

        public MainWindow()
        {
            InitializeComponent();

            clipboardHelper = new ClipboardHelper("Screenshot", @"C:\Users\leehs\Pictures\Screenshots", ImageFormat.Png);

            CompositionTarget.Rendering += OnCompositionTargetRendering;

            notifyIcon = new NotifyIcon();
            notifyContextMenu = new ContextMenuStrip();
            notifyIcon.Icon = Properties.Resources.Clipboard;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = notifyContextMenu;
            notifyIcon.Text = "Clipboard Image Saver";

            notifyIcon.DoubleClick += OnNotifyIconDoubleClicked;

            this.Hide();

            previousFirstKeyToggled = Keyboard.IsKeyToggled(firstKey);
            previousSecondKeyToggled = Keyboard.IsKeyToggled(secondKey);

        }

        bool previousFirstKeyToggled = true;
        bool previousSecondKeyToggled = true;

        Key firstKey = Key.S;
        Key secondKey = Key.LeftAlt;

        int count = 0;

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
                count++;
                clipboardHelper.SaveClipboardImage();
                //Activate;
            }


            countLabel.Content = "Count: " + count;

            firstIsDown.IsChecked = Keyboard.IsKeyDown(firstKey);
            firstIsToggled.IsChecked = Keyboard.IsKeyToggled(firstKey);
            firstIsPreviousToggled.IsChecked = previousFirstKeyToggled;
            firstIsPressed.IsChecked = firstKeydown;


            secondIsDown.IsChecked = Keyboard.IsKeyDown(secondKey);
            secondIsToggled.IsChecked = Keyboard.IsKeyToggled(secondKey);
            secondIsPreviousToggled.IsChecked = previousSecondKeyToggled;
            secondIsPressed.IsChecked = secondKeydown;
        }

        private void OnNotifyIconDoubleClicked(object sender, EventArgs e)
        {
            this.Show();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
