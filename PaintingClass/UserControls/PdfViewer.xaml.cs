using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO.Packaging;

namespace PaintingClass.UserControls
{
	/// <summary>
	/// Interaction logic for PdfViewer.xaml
	/// </summary>
	public partial class PdfViewer : UserControl
	{
		private class PDFPage
		{
            public double verticalOffset;
            public double pageHeight;
            public uint pageIndex;
            public bool locked = false;
        }
        int mlock = 0;
        private PdfDocument PDFDoc; 
        List<PDFPage> PDFPages = new();
        public bool isEmpty = true;
        private List<PDFPage> loadedPages = new();
		public PdfViewer()
		{
			InitializeComponent();
			MainScrollViewer.ScrollChanged += MainScrollViewer_ScrollChanged;

        }

		private async void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
            int forwardPageBuffer = 4;
            int backwardPageBuffer = 2;

            Interlocked.Increment(ref mlock);
            int init = mlock;
            await Task.Delay(100);
            if (init != mlock)
                return;

            if (!isEmpty && PDFDoc != null && PDFPages.Count != 0)
            {
                int st = 0, dr = PDFPages.Count;
                int res = 0;
                while (st <= dr)
                {
                    int md = (st + dr) / 2;
                    if (PDFPages[md].verticalOffset <= MainScrollViewer.VerticalOffset)
                    {
                        res = md;
                        st = md + 1;
                    }
                    else
                        dr = md - 1;
                }
                
                MainWindow.instance.Title = $"{MainScrollViewer.VerticalOffset} -> res:[{res}]={PDFPages[res].verticalOffset}";
                var items = PagesContainer.Items;
                for (int i = res-1; i >res- backwardPageBuffer && i>-1; i--)
				{
                    if (((Image)items[i]).Source == null && !PDFPages[i].locked)
                    {
                        PDFPages[i].locked = true;
                        loadedPages.Add(PDFPages[i]);
                        var bitmap = await PageToBitmapAsync(PDFDoc.GetPage((uint)i));
                        ((Image)items[i]).Source = bitmap;
                        PDFPages[i].locked = false;
                    }
                }
                for (int i = res; i < res + forwardPageBuffer && i < PDFPages.Count; i++)
                {
                    if (((Image)items[i]).Source == null && !PDFPages[i].locked)
                    {
                        PDFPages[i].locked = true;
                        loadedPages.Add(PDFPages[i]);
                        var bitmap = await PageToBitmapAsync(PDFDoc.GetPage((uint)i));
                        ((Image)items[i]).Source = bitmap;
                        PDFPages[i].locked = false;
                    }
                }
                foreach(var page in loadedPages)
				{
                    if(page.pageIndex <res - backwardPageBuffer || page.pageIndex> res + forwardPageBuffer)
                        ((Image)items[(int)page.pageIndex]).Source = null;
                }
            }
        }


        public void SetPdfTo(string _path)
        {
            var path = System.IO.Path.GetFullPath(_path);

            StorageFile.GetFileFromPathAsync(path).AsTask().ContinueWith(t => PdfDocument.LoadFromFileAsync(t.Result).AsTask()).Unwrap().ContinueWith(t2 => PdfToImages(t2.Result), TaskScheduler.FromCurrentSynchronizationContext());

            isEmpty = false;
        }

        private static async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
        {
            BitmapImage image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream.AsStream();
                image.EndInit();
            }

            return image;
        }

        private async Task PdfToImages(PdfDocument pdfDoc)
        {
            PDFDoc = pdfDoc;
            var items = PagesContainer.Items;
            items.Clear();
            PDFPages.Clear();
            loadedPages.Clear();

            if (pdfDoc == null) return;
            double currentHeight = 4;

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                using (var page = pdfDoc.GetPage(i))
                {
                    var dim = page.Dimensions.ArtBox;
                    var image = new Image
                    {
                        Source = null,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 4, 0, 4),
                        Height = PagesContainer.ActualWidth/dim.Width*dim.Height,
                        Width = PagesContainer.ActualWidth
                    };
                    if(i<4)
					{
                        var bitmap = await PageToBitmapAsync(page);
                        image.Source = bitmap;
                    }
                    PDFPages.Add(new PDFPage() { pageIndex = i, pageHeight = image.Height, verticalOffset = currentHeight });
                    currentHeight += image.Height + 8;
                    image.MouseDown += IntiateDragAndDrop;
                    items.Add(image);
                }
            }
        }

        private void IntiateDragAndDrop(object sender, MouseButtonEventArgs e)
        {
            var bitmapImage = ((Image)sender).Source;
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapImage));
            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);
            var data = new DataObject();
            data.SetData("Object", ms);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
        }
    }
}
