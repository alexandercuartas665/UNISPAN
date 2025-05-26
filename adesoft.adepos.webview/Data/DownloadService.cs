using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace adesoft.adepos.webview.Data
{
    public static class DownloadService
    {
        public static void DownloadZip(string dirPath, string zipFilename)
        {
            if (File.Exists(zipFilename))
            {                
                File.Delete(zipFilename);    
            }

            ZipFile.CreateFromDirectory(dirPath, zipFilename);
        }
    }
}
