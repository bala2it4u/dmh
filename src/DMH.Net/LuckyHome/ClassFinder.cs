using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyHome
{
    public static class ClassFinder
    {

       /* internal static CodeClass FindClass(CodeElements elements, string className, ref CodeElement mainProject)
        {
            return FindClass(elements, x => x.Name == className, ref mainProject);
        }*/

        static Dictionary<string, ClassStore> ClassesByName = new Dictionary<string, ClassStore>();
        static Dictionary<string, ClassStore> ClassesByInterfaces = new Dictionary<string, ClassStore>();

        static Dictionary<string, List<CodeClass>> CodeClassDic = new Dictionary<string, List<CodeClass>>();

        internal static CodeClass FindClass(CodeElements elements, string ClassName, ref CodeElement mainProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (CodeElement element in elements)
            {
                var tempName = element.FullName;
                if (tempName.StartsWith("Microsoft.") ||
                    tempName.StartsWith("System."))
                    continue;

                Debug.WriteLine(tempName);

                if (element is CodeNamespace)// || element is CodeClass)
                {/*
                    CodeClass c = element as CodeClass;
                    if (c != null)// && c.Access == vsCMAccess.vsCMAccessPublic)
                    {
                        Debug.WriteLine(c.FullName);

                        if (funcMethod(c))//)c.Name == className)
                        {
                            mainProject = element;
                            return c;
                        }
                        CodeClass subClass = FindClass(c.Members, funcMethod, ref mainProject);
                        if (subClass != null)
                        {
                            mainProject = element;
                            return subClass;
                        }
                    }
                    */
                    CodeNamespace ns = element as CodeNamespace;
                    if (ns != null)
                    {
                        CodeClass cc = ns.Members.Item(ClassName) as CodeClass;
                        if (cc != null)
                        {
                            mainProject = element;
                            return cc;
                        }
                        cc = FindClassSub(ns.Members, ClassName, ref mainProject);
                        if (cc != null)
                        {
                            mainProject = element;
                            return cc;
                        }
                    }
                }
            }
            return null;
        }

        internal static CodeClass FindClassSub(CodeElements elements, string className, ref CodeElement mainProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (CodeElement element in elements)
            {
                var tempName = element.FullName;
                if (tempName.StartsWith("Microsoft.") ||
                    tempName.StartsWith("System."))
                    continue;

                Debug.WriteLine(tempName);

                if (element is CodeNamespace)// || element is CodeClass)
                {/*
                    CodeClass c = element as CodeClass;
                    if (c != null)// && c.Access == vsCMAccess.vsCMAccessPublic)
                    {
                        Debug.WriteLine(c.FullName);

                        if (funcMethod(c))//)c.Name == className)
                        {
                            mainProject = element;
                            return c;
                        }
                        CodeClass subClass = FindClass(c.Members, funcMethod, ref mainProject);
                        if (subClass != null)
                        {
                            mainProject = element;
                            return subClass;
                        }
                    }
                    */
                    CodeNamespace ns = element as CodeNamespace;
                    if (ns != null)
                    {
                        CodeClass cc = ns.Members.Item(className) as CodeClass;
                        if (cc != null)
                        {
                            mainProject = element;
                            return cc;
                        }
                        cc = FindClassSub(ns.Members, className, ref mainProject);
                        if (cc != null)
                        {
                            mainProject = element;
                            return cc;
                        }
                    }
                }
            }
            return null;
        }

        internal static List<Project> FindProjects(Solution sln)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<Project> list = new List<Project>();
            list.AddRange(sln.Projects.Cast<Project>());
            //.Where(x => x.UniqueName.EndsWith("csproj")));

            for (int i = 0; i < list.Count; i++)
            {
                // OfType will ignore null's.
                if (list[i].ProjectItems != null)
                {
                    list.AddRange(list[i].ProjectItems.Cast<ProjectItem>().
                        Select(x => x.SubProject).OfType<Project>());//.Where(x => x.CodeModel != null));
                }
            }
            list = list.Where(x => x.UniqueName.EndsWith("csproj")).ToList();

            return list;
        }

        internal static void ClearCache()
        {
            CodeClassDic.Clear();
        }

        internal static List<CodeClass> FindProjectClass(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var data = new List<CodeClass>();
            if (CodeClassDic.TryGetValue(project.Name, out data))
                return data;

            data = new List<CodeClass>();

            foreach (CodeElement element in project.CodeModel.CodeElements)
            {
                var tempName = element.FullName;
                if (tempName.StartsWith("Microsoft") ||
                    tempName.StartsWith("Autofac") || 
                    tempName.StartsWith("Newtonsoft") || 
                    tempName.StartsWith("<") ||
                    tempName == "MS" ||
                    tempName.StartsWith("System"))
                    continue;

                if (element is CodeNamespace || element is CodeClass)
                {
                    Debug.WriteLine("main " +tempName);
                    CodeClass c = element as CodeClass;
                    if (c != null)// && c.Access == vsCMAccess.vsCMAccessPublic)
                    {
                        Debug.WriteLine(c.FullName);
                        data.Add(c);
                        FindClassSub(c.Members, project, data);
                    }
                    else
                    {
                        CodeNamespace ns = element as CodeNamespace;
                        if (ns != null)
                        {
                            FindClassSub(ns.Members, project, data);
                        }
                    }
                }

                //return data;
            }

            CodeClassDic[project.Name] = data;
            return data;
        }

        internal static void FindClassSub(CodeElements elements, Project main, List<CodeClass> data)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (CodeElement element in elements)
            {

                if (element is CodeNamespace || element is CodeClass)
                {
                    var tempName = element.FullName;
                    if (tempName.StartsWith("Microsoft") ||
                    tempName.StartsWith("System"))
                        continue;

                    Debug.WriteLine(tempName);
                    CodeClass c = element as CodeClass;
                    if (c != null)// && c.Access == vsCMAccess.vsCMAccessPublic)
                    {
                        if (false == tempName.StartsWith(main.Name))
                        {
                            break;
                        }
                        data.Add(c);
                        FindClassSub(c.Members, main, data);
                    }
                    else
                    {
                        CodeNamespace ns = element as CodeNamespace;
                        if (ns != null)
                        {
                            FindClassSub(ns.Members, main, data);
                        }
                    }
                }
            }
        }

    }

    public class ClassStore
    {
        public string ClassFullName { get; set; }
        public string[] InterfaceFullName { get; set; }
        public CodeClass CodeClass { get; set; }
    }
}
