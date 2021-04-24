using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using PaintingClass.Tabs;
using Microsoft.Win32;
using System.IO;
using PaintingClass.PaintTools.Interfaces;

namespace PaintingClass.PaintTools
{
	public class PdfTool : PaintTool, IToolSelected
	{
        public override int priority => 3;

        public override Control GetControl()
        {
            var cc = new ContentControl() { Height = 40 };
            Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/PDF.png")) };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            cc.Content = image;
            return cc;
        }

        /// <summary>
		/// Se produce atunci cand acest tool este selectat
		/// </summary>
		public void SelectToolEventHandler(PaintTool tool)
		{
            if(tool is PdfTool)
			{
                if (owner.pdfViewer.isEmpty)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "PDf files(*.pdf) | *.pdf";
                    dialog.ShowDialog();
                    if (File.Exists(dialog.FileName) == true)
                    {
                        try
                        {
                            owner.pdfViewer.SetPdfTo(dialog.FileName);
                            owner.ShowPdfViewer();
                        }
                        catch // daca este vreo eroare informam userul
                        {
                            MessageBox.Show("Fisierul nu are un format valid", "Eroare");
                            return;
                        }
                    }
                }
                else
                    owner.ShowPdfViewer();
			}
		}
    }
}
