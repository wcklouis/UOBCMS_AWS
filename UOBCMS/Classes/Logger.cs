using System;
using DevExpress.Utils.About;
using System.Diagnostics;
using DevExpress.Xpo.Logger;
using log4net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using TencentCloud.Cms.V20190321.Models;
using TencentCloud.Postgres.V20170312.Models;
using TencentCloud.Ecm.V20190719.Models;
using log4net.Config;
using TencentCloud.Wedata.V20210820.Models;

namespace UOBCMS.Classes
{
    public static class Logger
    {
        public static int INFO = 0;
        public static int ERROR = 1;
        public static int DEBUG = 2;

        public static int LogLvl { get; private set; }
        public static string LogDir { get; private set; }

        // Log4Net
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(Logger));

        public static void SetConfiguration(string logLvl, string logDir)
        {
            LogLvl = int.TryParse(logLvl, out int level) ? level : INFO; // Default to INFO if parsing fails
            LogDir = logDir;

            // Ensure log directory exists
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }

            // Configure log4net
            var log4netConfigPath = Path.Combine(LogDir, "log4net.config");
            if (File.Exists(log4netConfigPath))
            {
                XmlConfigurator.Configure(new FileInfo(log4netConfigPath));
            }
        }

        public static void LogErrorMessage(string strSource, string strMethodName, string username, string strMsg, int severity)
        {
            if (severity >= LogLvl)
            {
                string fullMessage = $"{strSource}\\{strMethodName}\t{username}\t{strMsg}";

                /*using (StreamWriter w = File.AppendText(Directory + "\\log" + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
                {
                    string currentTime = DateTime.Now.ToString();
                    w.WriteLine(currentTime + "\t" + logLvl + "\t" + severity + "\t" + strSource + "\t" + username + "\t" + strMsg);
                    w.Close();
                }*/

                // Log4Net
                if (severity == INFO)
                    log.Info(fullMessage);
                else if (severity == DEBUG)
                    log.Debug(fullMessage);
                else if (severity == ERROR)
                    log.Error(fullMessage);
            }
        }
    }    
}
