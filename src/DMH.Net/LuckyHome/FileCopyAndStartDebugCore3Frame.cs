using System.IO;
using System.Web.Helpers;
using EnvDTE;
using LuckyHome.Common;

namespace LuckyHome
{
    public class FileCopyAndStartDebugCore3Frame
    {
        public bool Run(SchemaInfo schemaInfo, string solutionDir, DTE dte)
        {
            string tempDebug = Path.GetDirectoryName(schemaInfo.StartAppProject[0]) + "\\";
            string tempProjectName = Path.GetFileNameWithoutExtension(schemaInfo.StartAppProject[0]);
            //Path.Combine(Path.GetDirectoryName(project.FullName),
            //project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString());

            var runPath = Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\";
            foreach(var file in Directory.GetFiles(Path.Combine(runPath, "netcoreapp3.1")))
            {
                File.Copy(file, tempDebug +Path.GetFileName(file), overwrite: true);
            }
            //File.Copy(runPath + "Run.Me.Now.exe", tempDebug + "Run.Me.Now.exe", overwrite: true);
            //File.Copy(runPath + "LuckyHome.Common.dll", tempDebug + "LuckyHome.Common.dll", overwrite: true);
            //File.Copy(runPath + "System.Web.Helpers.dll", tempDebug + "System.Web.Helpers.dll", overwrite: true);
            if (File.Exists(tempDebug + SchemaInfo.FileName))
            {
                File.Delete(tempDebug + SchemaInfo.FileName);
            }
            /*
            if (File.Exists(solutionDir + "\\luckyhome.config"))
            {
                File.Copy(solutionDir + "\\luckyhome.config", tempDebug + "Run.Me.Now.exe.config", true);
            }
            else if (File.Exists(tempDebug + "luckyhome.config"))
            {
                File.Copy(tempDebug + "luckyhome.config", tempDebug + "Run.Me.Now.exe.config", true);
            }
            else if (File.Exists(tempDebug + tempProjectName + ".dll.config"))
            {
                File.Copy(tempDebug + tempProjectName + ".dll.config", tempDebug + "Run.Me.Now.exe.config", overwrite: true);
            }
            else if (File.Exists(tempDebug + tempProjectName + ".exe.config"))
            {
                File.Copy(tempDebug + tempProjectName + ".exe.config", tempDebug + "Run.Me.Now.exe.config", overwrite: true);
            }*/
            string fileName = tempDebug + "Run.Method.NowCore3.exe";
            
            var process = System.Diagnostics.Process.Start(fileName);
            CommandRunThisMethodContextMenu.Attach(dte, process.Id);
            
            File.WriteAllText(tempDebug + SchemaInfo.FileName, Json.Encode(schemaInfo));
            File.WriteAllText(schemaInfo.FullMethodBasedUniqueName, Json.Encode(schemaInfo));

            return true;
        }
    }
}
