using System;

namespace LuckyHome.Common
{
    // complete ILogger interface implementation


    public static class LogManager
    {
        static LogManager()
        {
            var runPath = System.IO.Path.GetDirectoryName(typeof(LogManager).Assembly.Location) + "\\log4net.config";
            var logCfg = new System.IO.FileInfo(runPath);
            log4net.Config.XmlConfigurator.Configure(logCfg);
        }
        public static ILogger GetLogger(Type type)
        {
            return new Logger(type);
        }

        public static ILogger GetLogger(string name = null)
        {
            return new Logger(name);
        }
    }
}
