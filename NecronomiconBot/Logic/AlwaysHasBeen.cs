using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;
using System.Drawing;

namespace NecronomiconBot.Logic
{
    public static class AlwaysHasBeen
    {
        public static Stream GetImage(string astronaut1, string astronaut2, string wait)
        {
            Mat mat = new Mat(@".\assets\wait-its-all-ohio.png");

            PutCenteredText(mat, wait, new OpenCvSharp.Point(425, 200), HersheyFonts.HersheyComplex, 0.75, new Scalar(0, 0, 0), 2, LineTypes.AntiAlias);
            PutCenteredText(mat, wait, new OpenCvSharp.Point(425, 200), HersheyFonts.HersheyComplex, 0.75, new Scalar(255, 255, 255), 1, LineTypes.AntiAlias);

            PutCenteredText(mat, astronaut1, new OpenCvSharp.Point(430, 245), HersheyFonts.HersheyComplex, 0.5, new Scalar(0, 0, 0), 2, LineTypes.AntiAlias);
            PutCenteredText(mat, astronaut1, new OpenCvSharp.Point(430, 245), HersheyFonts.HersheyComplex, 0.5, new Scalar(255, 255, 255), 1, LineTypes.AntiAlias);

            PutCenteredText(mat, astronaut2, new OpenCvSharp.Point(755, 80), HersheyFonts.HersheyComplex, 0.75, new Scalar(0, 0, 0), 2, LineTypes.AntiAlias);
            PutCenteredText(mat, astronaut2, new OpenCvSharp.Point(755, 80), HersheyFonts.HersheyComplex, 0.75, new Scalar(255, 255, 255), 1, LineTypes.AntiAlias);

            PutCenteredText(mat, "Always has been", new OpenCvSharp.Point(600, 35), HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 0), 2, LineTypes.AntiAlias);
            PutCenteredText(mat, "Always has been", new OpenCvSharp.Point(600, 35), HersheyFonts.HersheyComplex, 1, new Scalar(255, 255, 255), 2, LineTypes.AntiAlias);


            MemoryStream stream = new MemoryStream();
            mat.WriteToStream(stream);
            stream.Position = 0;


            return stream;
        }

        private static SizeF MeasureString(string s, Font f)
        {
            using (var image = new Bitmap(1,1))
            using (var g = Graphics.FromImage(image))
            {
                return g.MeasureString(s, f);
            }
        }
        private static void PutCenteredText(Mat mat, string text, OpenCvSharp.Point org, HersheyFonts font, double fontScale, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8)
        {
            var size = MeasureString(text, SystemFonts.DefaultFont);
            int width = (int)(size.Width * fontScale / 0.6);
            mat.PutText(text, new OpenCvSharp.Point(org.X - width, org.Y), font, fontScale, color, thickness, lineType);
        }
    }
}
