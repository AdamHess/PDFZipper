using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;

namespace PdfZipper
{
    class Program
    {

        private const string SaveLocation = "./ExportPdfs/";
        private static readonly string ImageSaveLocation = $"{Directory.GetCurrentDirectory()}/CompressedImages/{{0}}/{{1}}";
        private static readonly string PdfSaveLocation = $"{Directory.GetCurrentDirectory()}/PDF/{{0}}";
        private const int Quality = 20;
        static void Main(string[] args)
        {
            var searchFolder =  "C:\\Users\\adam.hess\\Desktop\\New York Law Journal";
            var folders = Directory.GetDirectories(searchFolder).Where(m => Regex.IsMatch(m, "\\d")).ToList();
            Directory.CreateDirectory(string.Format(PdfSaveLocation,""));

            
            foreach (var folder in folders)
            {
                ProcessFolder(folder);
            }
        }

        private static void ProcessFolder(string folder)
        {
            var imageFiles = Directory.GetFiles(folder, "*_*.jpg").OrderBy(m => m);
            var folderName = Path.GetFileName(folder);
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
            foreach (var imageFile in imageFiles)
            {
                using var memStream = CompressImage(imgfactory, imageFile);
                AddPageToPdf(memStream, doc);
            }

            if (doc.PageCount > 0)
            {
                doc.Save(string.Format(PdfSaveLocation, pdfName));
            }
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
                .Quality(Quality)
                .Constrain(new Size((int)(imgfactory.Image.Width/1.25), (int)(imgfactory.Image.Height/1.25)))
                .Save(memStream);
            return memStream;
        }
    }
}
