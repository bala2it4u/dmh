using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        internal static List<string> FindProjectReferance(Project project)
        {
            List<string> referenceNames = new List<string>();
            var vsproject = project.Object as VSLangProj.VSProject;
            // note: you could also try casting to VsWebSite.VSWebSite
            foreach (VSLangProj.Reference reference in vsproject.References)
            {
                if (reference.SourceProject == null)
                {
                    var tempRefName = reference.Name;
                    //referenceNames.Add(reference.Name);
                    if (reference.Path == "" || tempRefName.StartsWith("Microsoft") ||
                    tempRefName.StartsWith("Autofac") ||
                    tempRefName.StartsWith("Newtonsoft") ||
                    tempRefName.StartsWith("<") ||
                    tempRefName == "MS" ||
                    tempRefName.StartsWith("System"))
                        continue;

                    // This is an assembly reference
                    //var fullName = GetFullName(reference);
                   // var path = string.Format("{0}, Version ={1}.{2}.{3}.{4}, Culture ={5}, PublicKeyToken ={6}",
                   //         reference.Path,
                   //         reference.MajorVersion, reference.MinorVersion, reference.BuildNumber, reference.RevisionNumber,
                   //         reference.Culture ?? "neutral",
                   //         reference.PublicKeyToken ?? "null");
                    var tempAssembly = Assembly.LoadFile(reference.Path);
                    referenceNames.AddRange(
                    tempAssembly.GetTypes().Where(x => x.IsPublic && x.IsClass).Select(x => $"{tempRefName}, {x.FullName}"));
                }
                else
                {
                    // This is a project reference
                }
            }
            return referenceNames;
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
                        FindClassSub(c.Members, project, data, false);
                    }
                    else
                    {
                        CodeNamespace ns = element as CodeNamespace;
                        if (ns != null)
                        {
                            FindClassSub(ns.Members, project, data, false);
                        }
                    }
                }

                //return data;
            }

            CodeClassDic[project.Name] = data;
            return data;
        }

        internal static void FindClassSub(CodeElements elements, Project main, List<CodeClass> data, bool includeReferenace)
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
                            if (!includeReferenace)
                            {
                                break;
                            }
                        }
                        data.Add(c);
                        FindClassSub(c.Members, main, data, includeReferenace);
                    }
                    else
                    {
                        CodeNamespace ns = element as CodeNamespace;
                        if (ns != null)
                        {
                            FindClassSub(ns.Members, main, data, includeReferenace);
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
