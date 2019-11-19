using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using ShellProgressBar;

namespace PdfZipper
{
    class Program
    {


        private static Options Options { get; set; }
        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(ops =>
                {
                    Options = ops;
                    Execute();
                });


        }

        private static void Execute()
        {
            var searchFolder = Options.InputFolder; //"C:\\Users\\adam.hess\\Desktop\\New York Law Journal";
            if (!Directory.Exists(searchFolder))
            {
                Console.WriteLine($"Input Directory Does not exist. Exiting application. {Options.InputFolder}");
                return;
            }

            var folders = Directory.GetDirectories(searchFolder).Where(m => Regex.IsMatch(m, "\\d")).ToList();

            if (!folders.Any())
            {
                Console.WriteLine($"No Folders found in directory {Options.InputFolder}");
                return;
            }
            Directory.CreateDirectory(Options.OutputFolder);

            using var folderProgressBar = new ProgressBar(folders.Count, "Processing Folders", ConsoleColor.White);
            foreach (var folder in folders)
            {
                folderProgressBar.Tick(folder);
                ProcessFolder(folder, folderProgressBar);
            }
        }

        private static void ProcessFolder(string folder, ProgressBar parentProgressBar)
        {
            var imageFiles = Directory.GetFiles(folder, "*_*.jpg").OrderBy(m => m).ToList();

            if (!imageFiles.Any())
            {
                Console.WriteLine($"No Images found in skipping folder: {folder}");
                return;
            }
            var folderName = Path.GetFileName(folder);
            Console.WriteLine($"Processing Directory: {folderName}  with {imageFiles.Count()} found");
            var pdfName = $"{folderName} - NYLJ.pdf";
            using var doc = new PdfDocument()
            {
                Options =
                {
                    NoCompression = false,
                    CompressContentStreams = true,
                    UseFlateDecoderForJpegImages = PdfUseFlateDecoderForJpegImages.Automatic,
                    FlateEncodeMode = PdfFlateEncodeMode.BestCompression
                }
            };
            using var imgfactory = new ImageFactory();
            using var imageProgressBar = parentProgressBar.Spawn(imageFiles.Count, "Processing Image");
            foreach (var imageFile in imageFiles)
            {
                
                using var memStream = CompressImage(imgfactory, imageFile);
                AddPageToPdf(memStream, doc);
                imageProgressBar.Tick($"Processing: {imageFile}");
            }

            if (doc.PageCount <= 0) return;

            doc.Save(Path.Combine(Options.OutputFolder, pdfName));
            Console.WriteLine($"Created PDF: {pdfName}");
        }

        private static void AddPageToPdf(Stream memStream, PdfDocument doc)
        {
            using var xImage = XImage.FromStream(memStream);
            var pdfPage = new PdfPage(doc)
            {
                Width = xImage.PixelWidth,
                Height = xImage.PixelHeight
            };
            using var gfx = XGraphics.FromPdfPage(pdfPage);
            gfx.DrawImage(xImage, 0, 0, xImage.PixelWidth, xImage.PixelHeight);
            doc.AddPage(pdfPage);
        }

        private static Stream CompressImage(ImageFactory imgfactory, string imageFile)
        {
            var memStream = new MemoryStream();
            imgfactory.Load(imageFile)
                .Quality(Options.Quality)
                .Save(memStream);
            return memStream;
        }

    }
}
