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
        [Option('i', "input-folder", HelpText = "Path to Folder of Folders with issue images in 'date_number.jpeg' format. Defaults to ./InputImages")]
        public string InputFolder { get; set; } = $"{Path.Combine(Directory.GetCurrentDirectory(), "InputImages")}";

        [Option('o', "output-folder", HelpText = "Folder where generated PDFs will be dumped. Defaults to ./OutputPdfs")]
        public string OutputFolder { get; set; } = $"{Path.Combine(Directory.GetCurrentDirectory(), "OutputPDFs")}";

        [Option('f', "file-naming", HelpText =
            "File Naming Covention to use, insert {0} to indicate where folder name should be inserted.\n Defaults to: \"{0} - NYLJ.pdf\"")]

        public string FileNamingConvention { get; set; } = "{0} - NYLJ.pdf";

        [Option('q', "quality", HelpText = "Compression Quality of Image. Defaults to 90")]
        public int Quality { get; set; } = 20;

        [Option('s', "skip-existing", HelpText = "Skips folders that have PDFS already generated in output directory.")]
        public bool SkipExistingFolders { get; set; }

        [Option('n', "no-parallel",  HelpText = "Disables parallelism otherwise will default to the number of cores in system. ")]
        public bool NoParallelism { get; set; }

        [Option('m', "max-image-size", HelpText =
            "Will progressively lower image quality on a per image basis until the image is below the specified size (in KB)")]
        public uint MaxImageSize { get; set; } = 0;


        [Option('r', "rescale",  HelpText = "Rescale the Image by multiplicative factor")]
        public float Scale { get; set; } = 1;

    }
}
