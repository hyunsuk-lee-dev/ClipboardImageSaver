using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ClipboardImageSaver
{
    public class ClipboardHelper
    {
        #region Private Variables
        private string prefix;
        private string path;
        private ImageFormat imageFormat;
        #endregion

        #region Properties
        private string FileName
        {
            get
            {
                // Create file path with prefix and format and index.
                return Enumerable.Range(1, int.MaxValue).Select(
                    n => Path.Combine(path, $"{prefix}_{DateTime.Now:yyyy-MM-dd}_{n.ToString("00")}.png")).First(p => !File.Exists(p));
                // TODO: File extension should be matched with image format
            }
        }
        #endregion

        #region Constructor
        public ClipboardHelper(string prefix, string path, ImageFormat imageFormat)
        {
            this.prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.imageFormat = imageFormat ?? throw new ArgumentNullException(nameof(imageFormat));
        }
        #endregion

        #region Public Methods
        public void SaveClipboardImage()
        {
            // Save clipboard data only when clipboard contains image.
            if(Clipboard.ContainsImage())
            {
                Clipboard.GetImage().Save(FileName, imageFormat);

                ProcessStartInfo explorerProcess = new ProcessStartInfo();
                explorerProcess.FileName = path;
                explorerProcess.Verb = "open";
                explorerProcess.UseShellExecute = true;
                Process.Start(explorerProcess);

                Clipboard.SetText(path);
            }
        }
        #endregion
    }
}
