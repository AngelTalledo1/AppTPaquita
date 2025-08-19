using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public static class ColorExtensions
    {
        public static PdfColor ToPdfColor(this Microsoft.Maui.Graphics.Color color)
        {
            return new PdfColor((byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255));
        }
    }
}
