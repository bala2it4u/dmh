using Newtonsoft.Json;
using Run.Method.NowCore.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Configuration;

namespace Run.Method.NowCore
{
    class Program
    {
        public class AssemblyLoader : AssemblyLoadContext
        {
            private static List<string> dlls = new List<string>();

            
            public static Assembly CreateAssemblyLoader(string path)
            {
                //this.folderPath = folderPath;
                var this1 = new AssemblyLoader();
                var asm = this1.LoadFromAssemblyPath(path);

                //var applicationAssembly = Assembly.Load(new AssemblyName(path));
                var context = DependencyContext.Load(asm);

                if (context != null)
                {
                    for (var i = 0; i < context.CompileLibraries.Count; i++)
                    {
                        var library = context.CompileLibraries[i];
                        IEnumerable<string> referencePaths;
                        try
                        {
                            referencePaths = library.ResolveReferencePaths();
                        }
                        catch (InvalidOperationException e)
                        {
                            continue;
                        }

                        //add the references
                       dlls.AddRange(referencePaths);
                    }
                }
                //dlls = this1.dlls;
                return asm;
            }

            public AssemblyLoader()
            {
                
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                var res = DependencyContext.Default.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
                if (res.Count > 0)
                {
                    return Assembly.Load(new AssemblyName(res.First().Name));
                }
                else
                {
                    var tempRunningAssbly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == assemblyName.FullName);
                    if (tempRunningAssbly != null)
                    {
                        return tempRunningAssbly;
                    }
                    var tempName = $"{assemblyName.Name}.dll";
                    var tempName1 = $"{assemblyName.Name}.exe";
                    var dll = dlls.FirstOrDefault(x => x.EndsWith(tempName) || x.EndsWith(tempName1));
                    if (dll != null && !assemblyName.Name.StartsWith("System."))
                    {
                        var asl = AssemblyLoader.CreateAssemblyLoader(dll);
                        return asl;
                    }
                }
                return Assembly.Load(assemblyName);
            }
        }

        public class lazyMapper
        {
            public object Factory(Type type, ParameterInfo[] paremTypeParemType)
            {
                object[] tempCreatedInstance = new object[0];
                if (paremTypeParemType.Length != 0)
                {
                    tempCreatedInstance = createInstance(paremTypeParemType);
                }
                return Activator.CreateInstance(type, tempCreatedInstance);
            }
        }

        static Func<string, string, string> aggregate = (x, y) => $"{x}={y}";

        private static Type[] userInputType = new Type[6]
        {
        typeof(string),
        typeof(int),
        typeof(long),
        typeof(double),
        typeof(float),
        typeof(DateTime)
        };

        private static SchemaInfo MainSchemaInfo = null;
        //private static List<string> MainAssemblyReferance = null;
        private static Assembly MainAssembly;

        private static readonly MethodInfo factoryMethod = typeof(lazyMapper).GetMethod("Factory");

        private static void Main(string[] args)
        {
            var temp = new ConfigurationBuilder()
              .AddJsonFile("Run.Method.NowCore.deps.json", optional: true)
              //.AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
              //.AddEnvironmentVariables()
              .Build();

            Console.Title = "Run Method Now core2.1";
            Console.WriteLine("starting...");
            string schemapath = Path.Combine(AppContext.BaseDirectory, SchemaInfo.FileName);
            while (!File.Exists(schemapath))
            {
                Thread.Sleep(5000);
            }
            Debug.WriteLine("starting...");
            MainSchemaInfo = JsonConvert.DeserializeObject<SchemaInfo>(File.ReadAllText(schemapath));
            string appPath = AppContext.BaseDirectory + MainSchemaInfo.AssambleName + ".dll";

            //var type = asm.GetType("MyClassLib.SampleClasses.Sample");
            MainAssembly = AssemblyLoader.CreateAssemblyLoader(appPath);
            Debug.WriteLine("main assembly loaded " + (MainAssembly != null));
            Console.WriteLine("main assembly loaded " + (MainAssembly != null));
            Type typeYouWant = MainAssembly.GetType(MainSchemaInfo.NameSpaceAndClass);
            Debug.WriteLine("main class loaded " + (typeYouWant != null));
            Console.WriteLine("main class loaded " + (typeYouWant != null));
            ConstructorInfo[] constructor = typeYouWant.GetConstructors();
            ParameterInfo[] classTypeParem2 = (constructor.Length != 0) ? constructor[0].GetParameters() : new ParameterInfo[0];
            object[] createdInstance2 = (classTypeParem2.Length == 0) ? new object[0] : createInstance(classTypeParem2);
            object instance = (constructor.Length != 0) ? Activator.CreateInstance(typeYouWant, createdInstance2) : null;
            MethodInfo method = getMethod(typeYouWant);
            Debug.WriteLine("main method loaded " + (method != null));
            Console.WriteLine("main method loaded " + (method != null));
            if (method == null)
            {
                Console.WriteLine(MainSchemaInfo.MethodToRun + " method not found error");
                Console.ReadKey();
            }
            else
            {
                classTypeParem2 = method.GetParameters();
                createdInstance2 = ((classTypeParem2.Length == 0) ? new object[0] : createInstance(classTypeParem2));
                var output = method.Invoke(instance, createdInstance2);
                if (output != null)
                {
                    Debug.WriteLine("output printing in json:");
                    Console.WriteLine("output printing in json:");
                    Debug.WriteLine(JsonConvert.SerializeObject(output));
                    Console.WriteLine(JsonConvert.SerializeObject(output));
                    Thread.Sleep(5 * 1000);
                }
            }
        }
        /*
        //get the avaialble references based on the HostingEnvironment variable
        public static IEnumerable<string> GetReferences(string path)
        {
            var references = new List<string>();
            var asl = new AssemblyLoader(Path.GetDirectoryName(path));
            var asm = asl.LoadFromAssemblyPath(path);

            //var applicationAssembly = Assembly.Load(new AssemblyName(path));
            var context = DependencyContext.Load(asm);

            if (context != null)
            {
                for (var i = 0; i < context.CompileLibraries.Count; i++)
                {
                    var library = context.CompileLibraries[i];
                    IEnumerable<string> referencePaths;
                    try
                    {
                        referencePaths = library.ResolveReferencePaths();
                    }
                    catch (InvalidOperationException e)
                    {
                        continue;
                    }

                    //add the references
                    references.AddRange(referencePaths);
                }
            }

            return references;
        }*/

        private static MethodInfo getMethod(Type typeYouWant)
        {
            Debug.WriteLine("getMethod " + MainSchemaInfo.MethodToRun + " from type " + typeYouWant);
            var tempMethod = (from x in typeYouWant.GetMethods()
                                                 where x.Name == MainSchemaInfo.MethodToRun
                                                 select x).ToList();
            if (tempMethod.Count() == 0)
            {
                Debug.WriteLine("getMethod not found");
                return null;
            }
            if (tempMethod.Count() == 1)
            {
                Debug.WriteLine("getMethod found");
                return tempMethod.First();
            }
            Debug.WriteLine("getMethod overloading found");

            string typeParam = MainSchemaInfo.MethodParameters.Any() ?
                MainSchemaInfo.MethodParameters.Aggregate(aggregate) : "";
            return tempMethod.FirstOrDefault(delegate (MethodInfo x)
            {
                var a = (from x1 in x.GetParameters()
                         select commonTypeName(x1.ParameterType)
                            ).ToList();
                var a1 = a.Any() ? a.Aggregate(aggregate) : "";
                return (!(a1 != typeParam)) ? true : false;
            });
        }

        private static string commonTypeName(Type type)
        {
            Debug.WriteLine("convert to common type " + type);
            return type.ToString().Replace("`1", "").Replace("`2", "")
                .Replace('[', '<')
                .Replace(']', '>')
                .Replace(",", ", ");
        }

        private static object[] createInstance(ParameterInfo[] classTypeParem)
        {
            List<object> createdInstance = new List<object>();
            Debug.WriteLine("create Instance create parem");
            foreach (ParameterInfo item in classTypeParem)
            {
                Type paremType = item.ParameterType;
                if (userInputType.Contains(paremType) || userInputType.Contains(Nullable.GetUnderlyingType(paremType)) || paremType.IsEnum)
                {
                    if (paremType.IsEnum)
                    {
                        createdInstance.Add(getEnumValue(item));
                    }
                    else
                    {
                        createdInstance.Add(getTypeValue(item));
                    }
                    continue;
                }
                if (paremType.IsInterface || paremType.IsAbstract)
                {
                    string tempTypeName = commonTypeName(paremType);
                    ClassInfo matchClass = MainSchemaInfo.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempTypeName);

                    if (matchClass == null || string.IsNullOrWhiteSpace(matchClass.NameSpaceAndMappedClassName))
                    {
                        createdInstance.Add(findTypeFromAssebly(paremType));
                        Debug.WriteLine(tempTypeName + "->" + matchClass?.NameSpaceAndMappedClassName);
                        continue;
                    }

                    Assembly tempAssembly = findAssebly(matchClass);
                    if (tempAssembly == null)
                    {
                        createdInstance.Add(null);
                        Debug.WriteLine(tempTypeName + "-> null");
                        continue;
                    }
                    paremType = GetGenericArgumentType(paremType, matchClass, tempAssembly);
                }
                if (paremType == null)
                {
                    createdInstance.Add(null);
                    Debug.WriteLine("paremType is -> null");
                    continue;
                }
                if (!paremType.IsClass)
                {
                    createdInstance.Add(null);
                    Debug.WriteLine("paremType not IsClass set null");
                }
                else if (paremType.FullName.StartsWith("System.Lazy"))
                {
                    Type tempType = paremType.GetGenericArguments()[0];
                    if (tempType.IsInterface)
                    {
                        string tempTypeName = commonTypeName(tempType);
                        ClassInfo matchClass = MainSchemaInfo.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempTypeName);

                        if (matchClass == null || string.IsNullOrWhiteSpace(matchClass.NameSpaceAndMappedClassName))
                        {
                            createdInstance.Add(findTypeFromAssebly(paremType));
                            //createdInstance.Add(null);
                            Debug.WriteLine(tempTypeName + "lazy ->" + matchClass?.NameSpaceAndMappedClassName);
                            continue;
                        }
                        var tempAssebly = findAssebly(matchClass);
                        if (tempAssebly == null)
                        {
                            createdInstance.Add(null);
                            Debug.WriteLine(tempTypeName + "lazy -> null");
                            continue;
                        }
                        tempType = GetGenericArgumentType(tempType, matchClass, tempAssebly);
                    }

                    if (tempType == null)
                    {
                        createdInstance.Add(null);
                        Debug.WriteLine("paremType lazy is -> null");
                        continue;
                    }
                    ConstructorInfo[] paremTypeConstructors2 = tempType.GetConstructors();
                    ParameterInfo[] paremTypeParemType2 = (paremTypeConstructors2.Length != 0) ? paremTypeConstructors2[0].GetParameters() : new ParameterInfo[0];
                    lazyMapper lazyMapper = new lazyMapper();
                    MethodCallExpression methodCall = Expression.Call(Expression.Constant(lazyMapper), factoryMethod, Expression.Constant(tempType), Expression.Constant(paremTypeParemType2));
                    UnaryExpression cast = Expression.Convert(methodCall, tempType);
                    Delegate lambda = Expression.Lambda(cast).Compile();
                    createdInstance.Add(Activator.CreateInstance(paremType, lambda));
                }
                else
                {
                    ConstructorInfo[] paremTypeConstructors = paremType.GetConstructors();
                    ParameterInfo[] paremTypeParemType = (paremTypeConstructors.Length != 0) ? paremType.GetConstructors()[0].GetParameters() : new ParameterInfo[0];
                    object[] tempCreatedInstance = new object[0];
                    if (paremTypeParemType.Length != 0)
                    {
                        tempCreatedInstance = createInstance(paremTypeParemType);
                    }
                    Debug.WriteLine("paremType is class");

                    createdInstance.Add(Activator.CreateInstance(paremType, tempCreatedInstance));
                }
            }
            return createdInstance.ToArray();
        }

        private static Type GetGenericArgumentType(Type paremType, ClassInfo matchClass, Assembly tempAssembly)
        {
            Debug.WriteLine("get genric Argument is called");
            var tempParemType = tempAssembly.GetType(matchClass.NameSpaceAndMappedClassName);
            if (tempParemType != null)
                return tempParemType;

            if (matchClass.NameSpaceAndMappedClassName.Contains('<') &&
                                    paremType.GetGenericArguments().Any())
            {
                var args = paremType.GetGenericArguments();
                var tempNameSpaceAndMappedClassName = matchClass.NameSpaceAndMappedClassName;
                tempNameSpaceAndMappedClassName = tempNameSpaceAndMappedClassName.Remove(tempNameSpaceAndMappedClassName.IndexOf('<')) + '`' + args.Length;
                paremType = tempAssembly.GetType(tempNameSpaceAndMappedClassName);

                Debug.WriteLine("get genric Argument found");
                return paremType.MakeGenericType(args);
            }
            Debug.WriteLine("get genric Argument not match");

            return null;
        }

        private static Assembly findAssebly(ClassInfo matchClass)
        {
            var tempAssblyName = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name ==
            matchClass.AssambleName); ;//MainAssemblyReferance.FirstOrDefault(x=> x.Contains(matchClass.AssambleName));
            if (tempAssblyName != null)
            {
                return tempAssblyName;
                //List<string> tempref = new List<string>();
                //return AssemblyLoader.CreateAssemblyLoader(tempAssblyName, ref tempref);
            }
            Assembly tempAssbly = null;

            foreach (var item in MainSchemaInfo.StartAppProject.Select(x => Path.GetDirectoryName(x)))
            {
                var tempPath = Path.Combine(item, matchClass.AssambleName);
                if (File.Exists(tempPath + ".dll"))
                {
                    tempAssbly = Assembly.LoadFile(tempPath + ".dll");
                    if (tempAssbly != null)
                        return tempAssbly;
                }
                if (File.Exists(tempPath + ".exe"))
                {
                    tempAssbly = Assembly.LoadFile(tempPath + ".exe");
                    if (tempAssbly != null)
                        return tempAssbly;
                }
            }
            return null;
        }

        private static object findTypeFromAssebly(Type matchClass)
        {
            Debug.WriteLine("findTypeFromAssebly " + matchClass);
            //foreach (var item in MainSchemaInfo.StartAppProject)
            {
                var tempAssembly = MainAssembly;// AssemblyLoader.CreateAssemblyLoader(item);
                
                var tempLuckyHomeInterfaceClassMapper = tempAssembly.GetType("LuckyHome.LuckyHomeInterfaceClassMapper", false, true);
                Debug.WriteLine("LuckyHome.LuckyHomeInterfaceClassMapper found " + tempLuckyHomeInterfaceClassMapper != null);
                if (tempLuckyHomeInterfaceClassMapper != null)
                {
                    var tempMethod = tempLuckyHomeInterfaceClassMapper.GetMethod("Run");
                    Debug.WriteLine("Run method found " + tempMethod != null);
                    if (tempLuckyHomeInterfaceClassMapper != null && tempMethod.GetParameters().Length == 1 &&
                        tempMethod.GetParameters()[0].ParameterType == typeof(Type))
                    {
                        var tempI = Activator.CreateInstance(tempLuckyHomeInterfaceClassMapper, new object[0]);
                        var tempI1 = tempMethod.Invoke(tempI, new[] { matchClass });
                        Debug.WriteLine(matchClass + " -> " + tempI1);
                        return tempI1;
                    }
                }
            }
            return null;
        }

        private static object getEnumValue(ParameterInfo item)
        {
            InputValue findData = MainSchemaInfo.InputValues.FirstOrDefault((InputValue x) => x.InputName == item.Name);
            if (findData == null || findData.DefaultValue == null)
            {
                Debug.WriteLine(item.Name + " enum -> value not found");
                return null;
            }
            string enumValue = findData.DefaultValue.ToString();
            Debug.WriteLine(item.Name + " enum -> " + enumValue);
            return Enum.Parse(item.ParameterType, enumValue.Substring(enumValue.LastIndexOf('.') + 1));
        }

        private static object getTypeValue(ParameterInfo item)
        {
            InputValue findData = MainSchemaInfo.InputValues.FirstOrDefault((InputValue x) => x.InputName == item.Name);
            if (findData == null || findData.DefaultValue == null)
            {
                Debug.WriteLine(item.Name + " Value -> not found");
                return null;
            }
            Type type2 = Type.GetType(findData.InputType);
            type2 = ((type2 == null) ? Nullable.GetUnderlyingType(type2) : type2);
            if (type2 == null)
            {
                Debug.WriteLine(findData.InputType + " -> not found");
                return null;
            }
            Debug.WriteLine(findData.InputType + " -> DefaultValue -> " + findData.DefaultValue);
            return Convert.ChangeType(findData.DefaultValue, type2);
        }
    }
}


