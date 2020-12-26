using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats;

namespace NecronomiconBot.Logic
{
    public static class AlwaysHasBeen
    {

        private static readonly string assetsFolder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "assets");

        public static Stream GetImage(string astronaut1, string astronaut2, string messageContent, bool debugRectangle = false)
        {
            MemoryStream stream = new MemoryStream();
            FontCollection fonts = new FontCollection();
            fonts.Install(Path.Combine(assetsFolder, "fonts", "arial.ttf"));
            Font captionFont = fonts.CreateFont("arial", 8);
            Rectangle messageContentRectangle = new Rectangle(150,50,450,165);
            Font astronaut1Font = fonts.CreateFont("arial", 7);
            Rectangle astronaut1Rectangle = new Rectangle(350, 225, 150, 75);
            Font astronaut2Font = fonts.CreateFont("arial", 8);
            Rectangle astronaut2Rectangle = new Rectangle(625,50,250,125);
            Font alwaysHasBeenFont = fonts.CreateFont("arial", 9);
            Rectangle alwaysHasBeenRectangle = new Rectangle(400, 0, 475, 50);
            using (Image img = Image.Load(Path.Combine(assetsFolder, "images", "wait-its-all-ohio.png")))
            {
                img.Mutate(ctx => ctx.DrawBoxedText(astronaut1Font, astronaut1, Color.White, Color.Black, astronaut1Rectangle, VerticalAlignment.Center, debugRectangle));
                img.Mutate(ctx => ctx.DrawBoxedText(astronaut2Font, astronaut2, Color.White, Color.Black, astronaut2Rectangle, VerticalAlignment.Center, debugRectangle));
                img.Mutate(ctx => ctx.DrawBoxedText(captionFont, messageContent, Color.White, Color.Black, messageContentRectangle, VerticalAlignment.Bottom, debugRectangle));
                img.Mutate(ctx => ctx.DrawBoxedText(alwaysHasBeenFont, "Always has been", Color.White, Color.Black, alwaysHasBeenRectangle, VerticalAlignment.Center, debugRectangle));
                img.SaveAsPng(stream);
            }
            stream.Position = 0;
            return stream;
        }
        private static IImageProcessingContext DrawBoxedText(this IImageProcessingContext processingContext,
            Font font,
            string text,
            Color mainColor,
            Color outlineColor,
            Rectangle rectangle,
            VerticalAlignment verticalAlignment,
            bool debugRectangle = false)
        {
            Size imgSize = processingContext.GetCurrentSize();
            float targetWidth = rectangle.Width;
            float targetHeight = rectangle.Height;

            float targetMinHeight = (int)(targetHeight * 0.95);

            var scaledFont = font;
            FontRectangle s = new FontRectangle(0, 0, float.MaxValue, float.MaxValue);

            float scaleFactor = (scaledFont.Size / 2);
            int trapCount = (int)scaledFont.Size * 2;
            if (trapCount < 10)
            {
                trapCount = 10;
            }

            bool isTooSmall = false;

            while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
            {
                if (s.Height > targetHeight)
                {
                    if (isTooSmall)
                    {
                        scaleFactor = scaleFactor / 2;
                    }

                    scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                    isTooSmall = false;
                }

                if (s.Height < targetMinHeight)
                {
                    if (!isTooSmall)
                    {
                        scaleFactor = scaleFactor / 2;
                    }
                    scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                    isTooSmall = true;
                }
                trapCount--;

                s = TextMeasurer.Measure(text, new RendererOptions(scaledFont)
                {
                    WrappingWidth = targetWidth
                });
            }

            var textGraphicOptions = new TextGraphicsOptions()
            {
                TextOptions = {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = verticalAlignment,
                    WrapTextWidth = targetWidth
                }
            };
            var position = new PointF(rectangle.X, 0);
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    break;
                case VerticalAlignment.Center:
                    position.Y = rectangle.Y + rectangle.Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    position.Y = rectangle.Y + rectangle.Height;
                    break;
                default:
                    break;
            }
            if (debugRectangle)
            {
                PointF[] points = { new PointF(rectangle.X, rectangle.Y), new PointF(rectangle.X, rectangle.Y + rectangle.Height), new PointF(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), new PointF(rectangle.X + rectangle.Width, rectangle.Y) };
                processingContext = processingContext.DrawPolygon(new Pen(Color.Red, 5), points);
            }
            return processingContext.DrawText(textGraphicOptions, text, scaledFont, Brushes.Solid(mainColor), Pens.Solid(outlineColor, font.Size / 10), position);
        }
    }
}
