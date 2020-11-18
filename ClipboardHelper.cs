using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClipboardImageSaver
{
    public class ClipboardHelper
    {
        const int hello = 3;


        public string prefix;
        public string path;


        public string FileName
        {
            get
            {
                return Enumerable.Range(1, int.MaxValue).Select(
                    n => Path.Combine(path, $"{prefix}_{DateTime.Now:yyyy-MM-dd}_{n}.png")).First(p => !File.Exists(p));
            }
        }

        public ImageFormat imageFormat;

        public ClipboardHelper(string prefix, string path, ImageFormat imageFormat)
        {
            this.prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.imageFormat = imageFormat ?? throw new ArgumentNullException(nameof(imageFormat));
        }


        public void SaveClipboardImage()
        {
            if(Clipboard.ContainsImage())
            {
                Clipboard.GetImage().Save(FileName, imageFormat);
                Process.Start("explorer.exe" , path);
            }
        }
    }
}
