using System.IO;
using Microsoft.Win32;

namespace MarkdownJsonEditor.Services
{
    public class ImageService
    {
        public string? SelectImage()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true) return openFileDialog.FileName;
            return null;
        }
        public string GenerateMarkdownImage(string path, string alt = "Image", string? align = null, string? width = null)
        {
            var name = System.IO.Path.GetFileName(path);
            var md = "![" + alt + "](assets/images/" + name + ")";
            if (align != null || width != null)
            {
                var a = new System.Collections.Generic.List<string>();
                if (align != null) a.Add("align=" + align);
                if (width != null) a.Add("width=" + width);
                md += "{ " + string.Join(" ", a) + " }";
            }
            return md;
        }
    }
}

