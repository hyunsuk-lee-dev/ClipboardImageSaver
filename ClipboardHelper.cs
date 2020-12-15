using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClipboardImageSaver
{
    public class ClipboardHelper
    {
        public string prefix;
        public string path;
        public ImageFormat imageFormat;

        public string FileName
        {
            get
            {
                // Create file path with prefix and format and index.
                return Enumerable.Range(1, int.MaxValue).Select(
                    n => Path.Combine(path, $"{prefix}_{DateTime.Now:yyyy-MM-dd}_{n.ToString("00")}.png")).First(p => !File.Exists(p));
            }
        }

        public ClipboardHelper(string prefix, string path, ImageFormat imageFormat)
        {
            this.prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.imageFormat = imageFormat ?? throw new ArgumentNullException(nameof(imageFormat));
        }

        public void SaveClipboardImage()
        {
            // Save clipboard data only when clipboard contains image.
            if(Clipboard.ContainsImage())
            {
                Clipboard.GetImage().Save(FileName, imageFormat);
                Process.Start("explorer.exe", path);
                Clipboard.SetText(path);
            }
        }
    }
}
