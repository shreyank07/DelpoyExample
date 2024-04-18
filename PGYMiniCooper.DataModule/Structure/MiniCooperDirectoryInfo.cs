using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public static class MiniCooperDirectoryInfo
    {
        public static readonly string CompanyPath = @"C:\Prodigy_Technovations";
        public static readonly string AppPath = CompanyPath + @"\PGY-MiniCooper Analyzer";
        public static readonly string BufferPath = AppPath + @"\Buffer";
        public static readonly string LoadBufferPath = BufferPath + @"\Load Data";
        public static readonly string ExportBufferPath = BufferPath + @"\Export Data";
        public static readonly string ConfigurationPath = AppPath + @"\Configuration";
        public static readonly string DeviceConfigPath = ConfigurationPath + @"\Device Config\";
        public static readonly string TriggerConfigPath = ConfigurationPath + @"\Trigger\";
        public static readonly string DefaultSettingFile = ConfigurationPath + @"\defaultConfig.xml";
        public static readonly string CurrentSettingsFile = ConfigurationPath + @"\CurrentConfiguration.xml";
        public static string TraceFilePath = AppPath + @"\Trace File";
        public static readonly string ImagePath = AppPath + @"\Images\";
        public static readonly string ReportPath = AppPath + @"\Report";
        public static readonly string ErrorPath = AppPath + @"\Error";
        public static readonly string MasterScriptPath = ConfigurationPath + @"\Master Script";
        public static readonly string SlaveScriptPath = ConfigurationPath + @"\Slave Script";
        public static string ApplicationName = "PGY-Minicooper Software";


        static MiniCooperDirectoryInfo()
        {
            try
            {
                var pathValues = typeof(MiniCooperDirectoryInfo)
                         .GetFields()
                         .Select(field => field.GetValue(typeof(MiniCooperDirectoryInfo))).ToList();
                foreach (var path in pathValues)
                {
                    if (path.ToString().EndsWith("."))
                        CreateDirectoryIfNotExist((string)path);
                }

            }
            catch
            {

            }
        }
        /// <summary>
        /// Deletes all the contents of the specified directory
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DeleteContents(string dirPath)
        {
            if (dirPath == null)
                return;
            System.IO.DirectoryInfo di = new DirectoryInfo(@dirPath);
            try
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(dirPath, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(@path))
            {
                Directory.CreateDirectory(@path);
            }
        }
    }
}
