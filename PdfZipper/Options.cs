using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace PdfZipper
{
    public class Options
    {
        [Option('i', "input-folder", HelpText = "Path to Folder of Folders with issue images in 'date_number.jpeg'")]
        public string InputFolder { get; set; } = $"{Path.Combine(Directory.GetCurrentDirectory(), "Images")}";

        [Option('o', "output-folder", HelpText = "Folder where generated PDFs will be dumped")]
        public string OutputFolder { get; set; }

    }
}
