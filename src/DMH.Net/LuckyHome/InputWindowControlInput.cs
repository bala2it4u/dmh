using System;
using System.Collections.Generic;
using EnvDTE;
using LuckyHome.Common;

namespace LuckyHome
{
    internal class InputWindowControlInput
    {
        public List<Project> ProjectNames { get; set; }
        public Action CallbackOption { get; set; }
        public SchemaInfoCommon SchemaInfoCommon { get; set; }
        public SchemaInfo SchemaInfo { get; set; }
        public Action Close { get; set; }
        public SchemaInfo LastRunFound { get; set; }
    }
}