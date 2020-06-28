using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LuckyHome.Common
{
    public class SchemaInfo
    {
        static Func<string, string, string> aggregate = (x, y) => $"{x}={y.Substring(y.LastIndexOf('.')+1)}";
        static int maxFileLength = 255 - 100;

        public string NameSpaceAndClass
        {
            get;
            set;
        }

        public string MethodToRun
        {
            get;
            set;
        }

        public List<string> MethodParameters
        {
            get;
            set;
        } = new List<string>();


        public int MethodType
        {
            get;
            set;
        }

        public EMethodType EMethodType => (EMethodType)MethodType;

        public int ClassType
        {
            get;
            set;
        }

        public string AssambleName
        {
            get;
            set;
        }

        public string[] StartAppProject
        {
            get;
            set;
        }

        public List<ClassInfo> DepandancyClasses
        {
            get;
            set;
        } = new List<ClassInfo>();


        public InputValue[] InputValues
        {
            get;
            set;
        } = new InputValue[0];
        public string FullMethodBasedUniqueName { get; set; }

        public string GetMethodBasedUniqueName()
        {
            var tempParams = MethodParameters.Any() ? MethodParameters.Aggregate(aggregate) : "";

            string temp = $"{NameSpaceAndClass}={MethodToRun}={tempParams}.json";
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char item in invalidFileNameChars)
            {
                temp = temp.Replace(item.ToString(), "");
            }
            if (temp.Length > maxFileLength)
            {
                return temp.Substring(temp.Length - maxFileLength).Trim();
            }
            return temp;
        }
    }

    public class SchemaInfoCommon
    {
        public static string FileName => "SchemaInfoCommon.json";

        public string SolutionDir
        {
            get;
            set;
        }

        public SchemaInfo SchemaInfo
        {
            get;
            set;
        }

        public string GetFilePath(string solutionDir = null)
        {
            if (solutionDir != null)
            {
                SolutionDir = solutionDir;
            }
            return SolutionDir + "\\" + FileName;
        }

        public string GetFileData(string solutionDir = null)
        {
            string tempPath = GetFilePath(solutionDir);
            if (File.Exists(tempPath))
            {
                return File.ReadAllText(tempPath);
            }
            return null;
        }

        public bool SetFileData(string data, string solutionDir = null)
        {
            string tempPath = GetFilePath(solutionDir);
            File.WriteAllText(tempPath, data);
            return true;
        }
    }

    public enum EMethodType
    {
        vsCMAccessPublic = 1,
        vsCMAccessPrivate = 2,
        vsCMAccessProject = 4,
        vsCMAccessProtected = 8,
        vsCMAccessProjectOrProtected = 12,
        vsCMAccessDefault = 32,
        vsCMAccessAssemblyOrFamily = 64,
        vsCMAccessWithEvents = 128
    }
    public class ClassInfo
    {
        public string NameSpaceAndInterfaceName { get; set; }
        public string NameSpaceAndMappedClassName { get; set; }
        public string AssambleName { get; set; }

    }

    public class InputValue
    {
        public string InputName { get; set; }
        public string InputType { get; set; }
        public object DefaultValue { get; set; }

    }

    public class getClassData
    {
        public getClassData() { }

        public void getClassDataMethod() {
            Console.WriteLine("data");
        }
    }
}
