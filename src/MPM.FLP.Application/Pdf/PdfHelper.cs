using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MPM.FLP.Pdf
{
    public class PdfHelper
    {
        /// <summary>
        /// This method adds watermark text under pdf content
        /// </summary>
        /// <param name="pdfData">pdf content bytes</param>
        /// <param name="watermarkText">text to be shown as watermark</param>
        /// <param name="font">base font</param>
        /// <param name="fontSize">font sieze</param>
        /// <param name="angle">angle at which watermark needs to be shown</param>
        /// <param name="color">water mark color</param>
        /// <param name="realPageSize">pdf page size</param>
        public static void AddWaterMarkText(PdfContentByte pdfData, string watermarkText, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize)
        {
            var gstate = new PdfGState { FillOpacity = 0.35f, StrokeOpacity = 0.3f };
            pdfData.SaveState();
            pdfData.SetGState(gstate);
            pdfData.SetColorFill(color);
            pdfData.BeginText();
            pdfData.SetFontAndSize(font, fontSize);
            var x = (realPageSize.Right + realPageSize.Left) / 2;
            var y = (realPageSize.Bottom + realPageSize.Top) / 2;

            // Original single line watermark text. Uncomment this if you don't need multiple line feature
            // pdfData.ShowTextAligned(Element.ALIGN_CENTER, watermarkText, x, y, angle);

            // This handles multiple lines separated by \n character
            string[] textLines = watermarkText.Split('\n');
            int lineHeight = (int)fontSize - 10;
            int totalLineHeight = lineHeight * textLines.Count();

            float ratio = (x / y);
            for (int i = 0; i < textLines.Count(); i++)
            {
                float ordinate = (y + (totalLineHeight / 2)) - (i * lineHeight);
                float abscissa = (x * 2) - (ratio * ordinate);
                pdfData.ShowTextAligned(Element.ALIGN_CENTER, textLines[i], abscissa, ordinate, angle);
            }

            pdfData.EndText();
            pdfData.RestoreState();
        }

        /// <summary>
        /// This method calls another method to add watermark text for each page
        /// </summary>
        /// <param name="bytes">byte array of Pdf</param>
        /// <param name="baseFont">Base font</param>
        /// <param name="watermarkText">Text to be added as watermark</param>
        /// <returns>Pdf bytes array having watermark added</returns>
        public static byte[] AddWatermark(byte[] bytes, string watermarkText)
        {
            using (var ms = new MemoryStream(10 * 1024))
            {
                PdfReader reader = new PdfReader(bytes);

                PdfStamper stamper = new PdfStamper(reader, ms);

                var pages = reader.NumberOfPages;
                for (var i = 1; i <= pages; i++)
                {
                    var dc = stamper.GetOverContent(i);
                    AddWaterMarkText(dc, watermarkText, BaseFont.CreateFont(), 75, 45, BaseColor.Gray, reader.GetPageSizeWithRotation(i));
                }
                stamper.Close();
                reader.Close();

                return ms.ToArray();
            }
        }
    }
}
