﻿using System;
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
			Label label = new Label();
			label.Content = "Line";
			return label;
		}


		GeometryDrawing drawing;
		LineGeometry line;

		public override void MouseDown(Point position)
		{

			drawing = new GeometryDrawing();
			line = new LineGeometry(position, position);
			drawing.Pen = new Pen(owner.globalBrush, 0.2);
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
			MainWindow.instance.roomManager.PackAndSend(PaintingClassCommon.PacketType.WhiteboardMessage, MessageUtils.SerialzieDrawing(drawing));

			drawing = null;
			line = null;
		}
	}
}
