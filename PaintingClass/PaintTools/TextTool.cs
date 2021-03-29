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
using System.Runtime.InteropServices;

namespace PaintingClass.PaintTools
{
    class TextTool : PaintTool
    {
        public override int priority => 0;

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "Text";
            return label;
        }

        double size = 3;
        Typeface typeface = new Typeface(new FontFamily("Times New Roman"),
                                FontStyles.Normal,
                                FontWeights.Normal,
                                FontStretches.Normal);
        StringBuilder text = new StringBuilder("dsadsa");
        GlyphRunDrawing glyphDrawing;
        GlyphTypeface glyphTypeface;
        Point initialPos;
        public override void MouseDown(Point position)
        {
            Window.GetWindow(whiteboard).KeyDown += TextTool_KeyDown;
            initialPos = position;

            glyphDrawing = new GlyphRunDrawing(new SolidColorBrush(Colors.Black), null);
            whiteboard.collection.Add(glyphDrawing);

        }
        #region Utilz
        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }
        #endregion
        private void TextTool_KeyDown(object sender, KeyEventArgs e)
        {
                text.Append(GetCharFromKey(e.Key));
                double[] advanceWidths = new double[text.Length];
                ushort[] glyphIndexes = new ushort[text.Length];
                double totalWidth = 0;
                if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                    throw new InvalidOperationException("No glyphtypeface found");
                for (int i = 0; i < text.Length; i++)
                {
                    ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[i]];
                    glyphIndexes[i] = glyphIndex;

                    double width = glyphTypeface.AdvanceWidths[glyphIndex] * size;
                    advanceWidths[i] = width;

                    totalWidth += width;
                }
                glyphDrawing.GlyphRun = new GlyphRun(glyphTypeface, 0, false, size, glyphIndexes, initialPos, advanceWidths, null, null, null, null, null, null);
        }

        public override void MouseDrag(Point position)
        {
        }

        public override void MouseUp()
        {
            //MainWindow.instance.roomManager.SendWhiteboardMessage(WhiteboardMessage.SerialzieDrawing(glyphDrawing));
        }
    }
}
