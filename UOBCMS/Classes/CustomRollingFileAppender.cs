using log4net.Appender;

namespace UOBCMS.Classes
{
    public class CustomRollingFileAppender : RollingFileAppender
    {
        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = "log";
                base.File = GetLogFileName();
            }
        }

        protected override void OpenFile(string fileName, bool append)
        {
            base.OpenFile(GetLogFileName(), append);
        }

        private string GetLogFileName()
        {
            string dateSuffix = DateTime.Now.ToString("yyyy-MM-dd");

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"log-{dateSuffix}.log");
        }
    }
}
