using System;
using System.Collections.Generic;
using EnvDTE;
using LuckyHome.Common;

namespace LuckyHome
{
    public interface IInterfaceMapperWithClassControlInput
    {
        Action CallbackLastRunOption { get; }
        Action<CodeClass[]> CallbackOption { get; }
        ClassInfo[] ClassInfos { get; set; }
        Action ClearCache { get; }
        Action Close { get; }
        Func<Project, List<CodeClass>> GetClasses { get; }
        Func<Project, List<string>> GetRefClasses { get; }
        bool LastRunFound { get; }
        List<Project> ProjectNames { get; set; }
        string ProjectNameSelected { get; set; }
        SchemaInfoCommon SchemaInfoCommon { get; }
    }
}