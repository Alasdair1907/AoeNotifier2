using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Util
{
    class Tools
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
