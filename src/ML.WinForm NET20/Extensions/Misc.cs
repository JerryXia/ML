using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ML.WinForm.Extensions
{
    public static partial class ExtensionMethod
    {
        public static T FindControl<T>(this Control startingControl, string id) where T : Control
        {
            T foundControl = default(T);

            int controlCount = startingControl.Controls.Count;

            foreach (Control c in startingControl.Controls)
            {
                if (c is T && string.Equals(id, c.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    foundControl = c as T;
                    break;
                }
                else
                {
                    foundControl = FindControl<T>(c, id);
                    if (foundControl != null)
                    {
                        break;
                    }
                }
            }
            return foundControl;
        }


        public static Bitmap LoadBitmapFromResource(this Assembly assembly, string imageResourcePath)
        {
            var stream = assembly.GetManifestResourceStream(imageResourcePath);
            return stream != null ? new Bitmap(stream) : null;
        }


        /// <summary> 
        /// Convert a (A)RGB string to a Color object 
        /// </summary> 
        /// <param name="argb">An RGB or an ARGB string</param> 
        /// <returns>a Color object</returns> 
        public static Color ToColor(this string argb)
        {
            argb = argb.Replace("#", "");
            byte a = System.Convert.ToByte("ff", 16);
            byte pos = 0;
            if (argb.Length == 8)
            {
                a = System.Convert.ToByte(argb.Substring(pos, 2), 16);
                pos = 2;
            }
            byte r = System.Convert.ToByte(argb.Substring(pos, 2), 16);
            pos += 2;
            byte g = System.Convert.ToByte(argb.Substring(pos, 2), 16);
            pos += 2;
            byte b = System.Convert.ToByte(argb.Substring(pos, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }

    }
}
