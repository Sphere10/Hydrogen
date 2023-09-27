// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public static class GraphicsExtensions {

	//public static void DrawCircle(this Graphics g, Pen pen, int x, int y, int radius) {
	//    g.DrawEllipse(pen, new Rectangle(x - radius, y - radius, radius * 2, radius * 2));
	//}

	//public static void DrawAlphaFirstPix(this Graphics gx, Bitmap image, int x, int y, int width, int height) {
	//    var attrib = new ImageAttributes();
	//    Color color = GetTransparentColor(image);
	//    var clientArea = new Rectangle(x, y, width, height);

	//    attrib.SetColorKey(color, color);
	//    gx.DrawImage(image, clientArea, 0, 0, clientArea.Width, clientArea.Height,
	//                 GraphicsUnit.Pixel, attrib);
	//}

	//public static void DrawAlphaFirstPix(this Graphics gx, Bitmap image, int x, int y, int width, int height,
	//                                     int offset) {
	//    var attrib = new ImageAttributes();
	//    Color color = GetTransparentColor(image);
	//    var clientArea = new Rectangle(x, y, width, height);

	//    attrib.SetColorKey(color, color);
	//    gx.DrawImage(image, clientArea, offset, 0, clientArea.Width, clientArea.Height,
	//                 GraphicsUnit.Pixel, attrib);
	//}

	//public static void DrawAlphaFirstPix(this Graphics gx, Bitmap image, Rectangle recDest, Rectangle recSrc) {
	//    var attrib = new ImageAttributes();
	//    Color color = GetTransparentColor(image);

	//    attrib.SetColorKey(color, color);
	//    gx.DrawImage(image, recDest, recSrc.X, recSrc.Y, recSrc.Width, recSrc.Height,
	//                 GraphicsUnit.Pixel, attrib);
	//}

	//public static void DrawAlphaColor(this Graphics gx, Bitmap image, Color color, int x, int y) {
	//    var attrib = new ImageAttributes();
	//    var recDest = new Rectangle(x, y, image.Width, image.Height);

	//    attrib.SetColorKey(color, color);
	//    gx.DrawImage(image, recDest, x, y, image.Width, image.Height,
	//                 GraphicsUnit.Pixel, attrib);
	//}

	//public static void DrawAlphaColor(this Graphics gx, Bitmap image, Color color, Rectangle recDest,
	//                                  Rectangle recSrc) {
	//    var attrib = new ImageAttributes();

	//    attrib.SetColorKey(color, color);
	//    gx.DrawImage(image, recDest, recSrc.X, recSrc.Y, recSrc.Width, recSrc.Height,
	//                 GraphicsUnit.Pixel, attrib);
	//}

	//public static Bitmap CopyRegion(Bitmap srcBitmap, Rectangle section) {
	//    // Create the new bitmap and associated graphics object
	//    var bmp = new Bitmap(section.Width, section.Height);
	//    using (Graphics g = Graphics.FromImage(bmp)) {
	//        // Draw the specified section of the source bitmap to the new one
	//        g.DrawImage(srcBitmap, section.X, section.Y);

	//        // Clean up
	//        g.Dispose();
	//    }
	//    // Return the bitmap
	//    return bmp;
	//}

	//public static void DrawJustifiedString(this Graphics g, int x, int y, int width, string text, Font textFont,
	//                                       Color c) {
	//    DrawJustifiedString(g, x, y, width, text, textFont, c, false);
	//}

	//public static void DrawJustifiedString(this Graphics g, int x, int y, int width, string text, Font textFont,
	//                                       Color c, bool format) {
	//    if (format)
	//        text = GetSizedString(width, text, textFont);

	//    int curX = x, curY = y;

	//    using (var b = new SolidBrush(c)) {
	//        foreach (string line in text.Split('\n')) {
	//            if (text.Split('\n').Last() == line)
	//                g.DrawString(line, textFont, b, curX, curY);

	//            else {
	//                string[] words = line.Split(' ');
	//                SizeF lineMeasure = getLineMeasure(line, g, textFont);
	//                int spaceWidth = (width - (int)lineMeasure.Width) /
	//                                 (words.Length > 1 ? words.Length - 1 : words.Length);

	//                foreach (string word in words) {
	//                    SizeF wordMeasure = g.MeasureString(word, textFont);
	//                    g.DrawString(word, textFont, b, curX, curY);
	//                    curX += (int)wordMeasure.Width + spaceWidth;
	//                }

	//                curY += (int)lineMeasure.Height;
	//                curX = x;
	//            }
	//        }
	//    }
	//}


	//public static int GetSizedStringHeight(string text, Font textFont) {
	//    int height;

	//    using (Graphics gr = Graphics.FromImage(_bm))
	//        height = (int)gr.MeasureString(text, textFont).Height + 5;

	//    return height;
	//}

	//public static SizeF GetEllipsisStringMeasure(int width, string Text, Font TextFont) {
	//    using (Graphics gr = Graphics.FromImage(_bm)) {
	//        Text = EllipsisWord(width, Text, TextFont);
	//        SizeF ret = gr.MeasureString(Text, TextFont);

	//        return ret;
	//    }
	//}

	//public static string EllipsisWord(int width, string Text, Font TextFont) {
	//    var sb = new StringBuilder();
	//    var sbLine = new StringBuilder();

	//    using (Graphics gr = Graphics.FromImage(_bm)) {
	//        if (gr.MeasureString(Text, TextFont).Width < width)
	//            return Text;

	//        foreach (char s in Text) {
	//            sbLine.Append(s).Append("...");

	//            SizeF tLength = gr.MeasureString(sbLine.ToString(), TextFont);

	//            if (tLength.Width > width) {
	//                sb.Append("...");
	//                break;
	//            }
	//            sb.Append(s);
	//            sbLine = new StringBuilder(sb.ToString());
	//        }
	//    }

	//    return sb.ToString();
	//}

	//public static string GetSizedString(int width, string Text, Font TextFont) {
	//    if (string.IsNullOrEmpty(Text))
	//        return "";

	//    string buffer = "";
	//    string full = "";

	//    using (Graphics gr = Graphics.FromImage(_bm)) {
	//        string[] words = Text.Split(' ');
	//        foreach (string s in words) {
	//            string temp;

	//            if (string.IsNullOrEmpty(buffer))
	//                temp = s;
	//            else
	//                temp = buffer + " " + s;
	//            SizeF tLength = gr.MeasureString(temp, TextFont);

	//            if (tLength.Width > width) {
	//                if (string.IsNullOrEmpty(full))
	//                    full = buffer;
	//                else
	//                    full += "\n" + buffer;
	//                buffer = s;

	//                if (gr.MeasureString(buffer, TextFont).Width > width)
	//                    buffer = splitWord(width, buffer, TextFont);
	//            } else {
	//                buffer = temp;
	//            }
	//        }
	//    }

	//    if (string.IsNullOrEmpty(full))
	//        full = buffer;
	//    else
	//        full += "\n" + buffer;

	//    return full;
	//}


	//public static void DrawRoundedRectangle(this Graphics g, Color color, Rectangle rec, int radius,
	//                                   RoundedCorners corners) {
	//    using (var b = new SolidBrush(color)) {
	//        int x = rec.X;
	//        int y = rec.Y;
	//        int diameter = radius * 2;
	//        var horiz = new Rectangle(x, y + radius, rec.Width, rec.Height - diameter);
	//        var vert = new Rectangle(x + radius, y, rec.Width - diameter, rec.Height);

	//        g.FillRectangle(b, horiz);
	//        g.FillRectangle(b, vert);

	//        if ((corners & RoundedCorners.TopLeft) == RoundedCorners.TopLeft)
	//            g.FillEllipse(b, x, y, diameter, diameter);
	//        else
	//            g.FillRectangle(b, x, y, diameter, diameter);

	//        if ((corners & RoundedCorners.TopRight) == RoundedCorners.TopRight)
	//            g.FillEllipse(b, x + rec.Width - (diameter + 1), y, diameter, diameter);
	//        else
	//            g.FillRectangle(b, x + rec.Width - (diameter + 1), y, diameter, diameter);

	//        if ((corners & RoundedCorners.BottomLeft) == RoundedCorners.BottomLeft)
	//            g.FillEllipse(b, x, y + rec.Height - (diameter + 1), diameter, diameter);
	//        else
	//            g.FillRectangle(b, x, y + rec.Height - (diameter + 1), diameter, diameter);

	//        if ((corners & RoundedCorners.BottomRight) == RoundedCorners.BottomRight)
	//            g.FillEllipse(b, x + rec.Width - (diameter + 1), y + rec.Height - (diameter + 1), diameter, diameter);
	//        else
	//            g.FillRectangle(b, x + rec.Width - (diameter + 1), y + rec.Height - (diameter + 1), diameter,
	//                            diameter);
	//    }
	//}


	//public static void DrawRoundedBorder(this Graphics g, Color color, Rectangle rec,
	//                                     int radius, int borderWidth, RoundedCorners corners) {
	//    using (Bitmap b = new Bitmap(rec.Width, rec.Height))
	//    using (Graphics gb = Graphics.FromImage(b)) {
	//        var gfRec = new Rectangle(0, 0, rec.Width, rec.Height);
	//        gb.Clear(Color.Green);

	//        gb.DrawRoundedRectangle(color, gfRec, radius, corners);

	//        gfRec.Height -= borderWidth << 1;
	//        gfRec.Width -= borderWidth << 1;
	//        gfRec.X += borderWidth;
	//        gfRec.Y += borderWidth;
	//        gb.DrawRoundedRectangle(Color.Green, gfRec, radius - borderWidth, corners);

	//        var maskAttr = new ImageAttributes();
	//        maskAttr.SetColorKey(Color.Green, Color.Green);

	//        g.DrawImage(b, rec, 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, maskAttr);
	//    }
	//}


	//public static void DrawRoundedRectangleBorder(this Graphics g, Color color, Color bColor, Rectangle rec,
	//                                                     int radius, int borderWidth, RoundedCorners corners) {
	//    g.DrawRoundedRectangle(bColor, rec, radius, corners);

	//    if (borderWidth == 0)
	//        return;

	//    rec.Height -= borderWidth * 2;
	//    rec.Width -= borderWidth * 2;
	//    rec.X += borderWidth;
	//    rec.Y += borderWidth;
	//    g.DrawRoundedRectangle(color, rec, radius - borderWidth, corners);
	//}

	//public static void DrawRoundedRectangleBorder(this Graphics g, Color color, Color bColor, Rectangle rec,
	//                                              int radius, int borderWidth) {
	//    DrawRoundedRectangleBorder(g, color, bColor, rec, radius, borderWidth, RoundedCorners.All);
	//}

	//public static void DrawRoundedRectangleGradiantFill(this Graphics g, Color start, Color end, Rectangle rec,
	//                                                    int radius, RoundedCorners corners) {
	//    DrawRoundedRectangleBorderGradiantFill(g, start, end, Color.White, rec, radius, 0, corners);
	//}

	//public static void DrawRoundedRectangleGradiantFill(this Graphics g, Color start, Color end, Rectangle rec,
	//                                                    int radius) {
	//    DrawRoundedRectangleBorderGradiantFill(g, start, end, Color.White, rec, radius, 0);
	//}

	//public static void DrawRoundedRectangleBorderGradiantFill(this Graphics g, Color start, Color end, Color bColor,
	//                                                          Rectangle rec, int radius, int borderWidth,
	//                                                          RoundedCorners corners) {
	//    using (var b = new Bitmap(rec.Width, rec.Height))
	//    using (Graphics gGf = Graphics.FromImage(b))
	//    using (var bMask = new Bitmap(rec.Width, rec.Height))
	//    using (Graphics gMask = Graphics.FromImage(bMask)) {
	//        var gfRec = new Rectangle(0, 0, rec.Width, rec.Height);
	//        gGf.GradientFill(gfRec, start, end, GradientFillDirection.Vertical);

	//        gMask.Clear(Color.White);
	//        gMask.DrawRoundedRectangleBorder(Color.Green, bColor, gfRec, radius, borderWidth, corners);

	//        var maskAttr = new ImageAttributes();
	//        maskAttr.SetColorKey(Color.Green, Color.Green);
	//        gGf.DrawImage(bMask, gfRec, 0, 0, gfRec.Width, gfRec.Height, GraphicsUnit.Pixel, maskAttr);

	//        g.DrawAlphaFirstPix(b, rec, gfRec);
	//    }
	//}


	//public static void GradientFill(this Graphics g, Rectangle rec, Color fromColor, Color toColor, GradientFillDirection direction, Blend blend = null) {
	//    int angle = 0;
	//    switch (direction) {
	//        case GradientFillDirection.Horizontal:
	//            angle = 0;
	//            break;
	//        case GradientFillDirection.Vertical:
	//            angle = 90;
	//            break;
	//    }
	//    g.GradientFill(rec, fromColor, toColor, angle, blend);
	//}

	//public static void GradientFill(this Graphics g, Rectangle rec, Color fromColor, Color toColor, int angle = 0, Blend blend = null) {
	//    if (fromColor != Color.Empty || toColor != Color.Empty) {
	//        if (fromColor == toColor) {
	//            using (Brush brush = new SolidBrush(fromColor)) {
	//                g.FillRectangle(brush, rec);
	//            }
	//        } else {
	//            using (LinearGradientBrush brush = new LinearGradientBrush(rec, fromColor, toColor, angle)) {
	//                if (blend != null) {
	//                    brush.Blend = blend;

	//                }

	//                g.FillRectangle(brush, rec);
	//            }
	//        }
	//    }
	//}

	//public static void DrawRoundedRectangleBorderGradiantFill(this Graphics g, Color start, Color end, Color bColor,
	//                                                          Rectangle rec, int radius, int borderWidth) {
	//    DrawRoundedRectangleBorderGradiantFill(g, start, end, bColor, rec, radius, borderWidth, RoundedCorners.All);
	//}

	//#region Auxillary Fields & Methods

	//private static readonly Bitmap _bm = new Bitmap(1, 1);


	//private static Color GetTransparentColor(Bitmap image) {
	//    return image.GetPixel(0, 0);
	//}

	//private static string splitWord(int width, string Text, Font TextFont) {
	//    string buffer = "";
	//    string full = "";
	//    Bitmap bm = new Bitmap(100, 100);
	//    var gr = Graphics.FromImage(bm);

	//    foreach (var s in Text.ToCharArray()) {
	//        string temp = "";

	//        if (string.IsNullOrEmpty(buffer))
	//            temp = s + "";
	//        else
	//            temp = buffer + s;
	//        var tLength = gr.MeasureString(temp, TextFont);

	//        if (tLength.Width > width) {
	//            if (string.IsNullOrEmpty(full))
	//                full = buffer;
	//            else
	//                full += "\n" + buffer;
	//            buffer = s + "";

	//            if (gr.MeasureString(buffer, TextFont).Width > width)
	//                buffer = splitWord(width, buffer, TextFont);
	//        } else {
	//            buffer = temp;
	//        }
	//    }
	//    bm.Dispose();
	//    gr.Dispose();

	//    if (string.IsNullOrEmpty(full))
	//        full = buffer;
	//    else
	//        full += "\n" + buffer;

	//    return full;
	//}


	//private static SizeF getLineMeasure(string line, Graphics g, Font textFont) {
	//    string temp = line.Replace(" ", "");
	//    return g.MeasureString(temp, textFont);
	//}

	//private struct Rect {
	//    public int Left, Top, Right, Bottom;
	//    public Rect(Rectangle r) {
	//        Left = r.Left;
	//        Top = r.Top;
	//        Bottom = r.Bottom;
	//        Right = r.Right;
	//    }
	//}

	//#endregion
}
