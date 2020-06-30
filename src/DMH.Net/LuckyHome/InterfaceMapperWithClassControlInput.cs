namespace LuckyHome
{
    using EnvDTE;
    using LuckyHome.Common;
    using System;
    using System.Collections.Generic;

    public class InterfaceMapperWithClassControlInput : IInterfaceMapperWithClassControlInput
    {
        public ClassInfo[] ClassInfos
        {
            get;
            set;
        }

        public List<Project> ProjectNames
        {
            get;
            set;
        }

        public string ProjectNameSelected
        {
            get;
            set;
        }

        public Func<Project, List<CodeClass>> GetClasses
        {
            get;
            internal set;
        }

        public Action<CodeClass[]> CallbackOption
        {
            get;
            internal set;
        }

        public Action Close
        {
            get;
            internal set;
        }

        public SchemaInfoCommon SchemaInfoCommon
        {
            get;
            internal set;
        }

        public Action ClearCache
        {
            get;
            internal set;
        }

        public bool LastRunFound { get; internal set; }

        public Action CallbackLastRunOption
        {
            get;
            internal set;
        }
    }
}