using AoeNotifier.Model;
using AoeNotifier.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Resources;

namespace AoeNotifier.Engine
{
    class Storage
    {

        private static readonly string storageFolderName = "AoeNotifier_Data";
        private static readonly string filtersFileName = "aoeNotifierFilters.file";
        private static readonly string notificationsLogo = "aoeNotifierLogo.gif";
        public static void SaveFilters(Filters filters)
        {
            TextWriter writer = null;

            try
            {
                string contents = JsonConvert.SerializeObject(filters);
                writer = new StreamWriter(getPath(), false, Encoding.UTF8);
                writer.Write(contents);
                writer.Close();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            
        }

        public static Filters LoadFilters()
        {
            TextReader reader = null;
            try
            {
                if (!File.Exists(getPath()))
                {
                    return new Filters();
                }

                reader = new StreamReader(getPath(), Encoding.UTF8);
                string contents = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(contents))
                {
                    return new Filters();
                }

                return JsonConvert.DeserializeObject<Filters>(contents);
            } finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private static string getBasePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string folderPath = Path.Combine(appData, storageFolderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        private static string getPath()
        {
            return Path.Combine(getBasePath(), filtersFileName);
        }

        public static string GetLogoPath()
        {
            string baseFolder = getBasePath();
            string logoPath = Path.Combine(baseFolder, notificationsLogo);

            if (!File.Exists(logoPath))
            {
                StreamResourceInfo info = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/aoeNotifierLogo.gif"));

                using (Stream file = File.Create(logoPath))
                {
                    Tools.CopyStream(info.Stream, file);
                }
            }

            return logoPath;
        }
    }
}
