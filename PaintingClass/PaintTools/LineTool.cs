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
using PaintingClass.PaintTools;
using PaintingClass.Networking;


namespace PaintingClass.PaintTools
{
	class LineTool : PaintTool
	{
		public override int priority => 2;

		public override Control GetControl()
		{
			var cc = new ContentControl() { Height = 40 };
			Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/line.png")) };
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
			cc.Content = image;
			return cc;
		}


		GeometryDrawing drawing;
		LineGeometry line;

		public override void MouseDown(Point position)
		{

			drawing = new GeometryDrawing();
			line = new LineGeometry(position, position);
			drawing.Pen = new Pen(owner.globalBrush, owner.globalBrushThickness);
			drawing.Geometry = line;
			whiteboard.collection.Add(drawing);
		}

		public override void MouseDrag(Point position)
		{
			line.EndPoint = position;
		}

		public override void MouseUp()
		{
			drawing.Freeze();//extra performanta
			MessageUtils.SendNewDrawing(drawing, whiteboard.collection.Count - 1);

			drawing = null;
			line = null;
		}
	}
}
