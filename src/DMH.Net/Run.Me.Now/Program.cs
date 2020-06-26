using LuckyHome.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Run.Me.Now
{
    internal class Program
    {
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

        private static Assembly MainAssembly;

        private static readonly MethodInfo factoryMethod = typeof(lazyMapper).GetMethod("Factory");

        private static void Main(string[] args)
        {
            Console.Title = "Run Method Now";
            Console.WriteLine("starting...");
            string schemapath = Path.Combine(AppContext.BaseDirectory, "schemaInfo.json");
            while (!File.Exists(schemapath))
            {
                Thread.Sleep(5000);
            }
            MainSchemaInfo = Json.Decode<SchemaInfo>(File.ReadAllText(schemapath));
            string appPath = AppContext.BaseDirectory + MainSchemaInfo.AssambleName;
            MainAssembly = Assembly.Load(MainSchemaInfo.AssambleName);
            Type typeYouWant = MainAssembly.GetType(MainSchemaInfo.NameSpaceAndClass);
            ConstructorInfo[] constructor = typeYouWant.GetConstructors();
            ParameterInfo[] classTypeParem2 = (constructor.Length != 0) ? constructor[0].GetParameters() : new ParameterInfo[0];
            object[] createdInstance2 = (classTypeParem2.Length == 0) ? new object[0] : createInstance(classTypeParem2);
            object instance = (constructor.Length != 0) ? Activator.CreateInstance(typeYouWant, createdInstance2) : null;
            MethodInfo method = getMethod(typeYouWant);
            if (method == null)
            {
                Console.WriteLine(MainSchemaInfo.MethodToRun + " method not found error");
                Console.ReadKey();
            }
            else
            {
                classTypeParem2 = method.GetParameters();
                createdInstance2 = ((classTypeParem2.Length == 0) ? new object[0] : createInstance(classTypeParem2));
                method.Invoke(instance, createdInstance2);
            }
        }

        private static MethodInfo getMethod(Type typeYouWant)
        {
            IEnumerable<MethodInfo> tempMethod = from x in typeYouWant.GetMethods()
                                                 where x.Name == MainSchemaInfo.MethodToRun
                                                 select x;
            if (tempMethod.Count() == 0)
            {
                return null;
            }
            if (tempMethod.Count() == 1)
            {
                return tempMethod.First();
            }

            string typeParam = MainSchemaInfo.MethodParameters.Any()?
                MainSchemaInfo.MethodParameters.Aggregate(aggregate): "";
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
            return type.ToString().Replace("`1", "").Replace("`2", "")
                .Replace('[', '<')
                .Replace(']', '>')
                .Replace(",", ", ");
        }

        private static object[] createInstance(ParameterInfo[] classTypeParem)
        {
            List<object> createdInstance = new List<object>();
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
                if (paremType.IsInterface)
                {
                    string tempTypeName = commonTypeName(paremType);
                    ClassInfo matchClass = MainSchemaInfo.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempTypeName);
                    paremType = Assembly.Load(matchClass.AssambleName).GetType(matchClass?.NameSpaceAndMappedClassName ?? "");
                }
                if (paremType == null)
                {
                    createdInstance.Add(null);
                    continue;
                }
                if (!paremType.IsClass)
                {
                    createdInstance.Add(null);
                }
                else if (paremType.FullName.StartsWith("System.Lazy"))
                {
                    Type tempType = paremType.GetGenericArguments()[0];
                    if (tempType.IsInterface)
                    {
                        string tempTypeName = commonTypeName(tempType);
                        ClassInfo matchClass = MainSchemaInfo.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempTypeName);
                        tempType = Assembly.Load(matchClass.AssambleName).GetType(matchClass?.NameSpaceAndMappedClassName ?? "");
                    }
                    if (tempType == null)
                    {
                        createdInstance.Add(null);
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
                    createdInstance.Add(Activator.CreateInstance(paremType, tempCreatedInstance));
                }
            }
            return createdInstance.ToArray();
        }
        static Type GetType(Assembly assembly, string typeName, string interfaceParam)
        {
            Type type = assembly.GetType(typeName);
            if (type == null)
            {
                int index = typeName.IndexOf("`");
                if (index != -1)
                {
                    index += 2;
                    //var span = typeName.ToList();
                    type = Type.GetType(typeName.Remove(index));
                    var paremTypes = new List<Type>();
                    return type.MakeGenericType(paremTypes.ToArray());
                }
            }
            return type;
        }
        private static object getEnumValue(ParameterInfo item)
        {
            InputValue findData = MainSchemaInfo.InputValues.FirstOrDefault((InputValue x) => x.InputName == item.Name);
            if (findData == null || findData.DefaultValue == null)
            {
                return null;
            }
            string enumValue = findData.DefaultValue.ToString();
            return Enum.Parse(item.ParameterType, enumValue.Substring(enumValue.LastIndexOf('.') + 1));
        }

        private static object getTypeValue(ParameterInfo item)
        {
            InputValue findData = MainSchemaInfo.InputValues.FirstOrDefault((InputValue x) => x.InputName == item.Name);
            if (findData == null || findData.DefaultValue == null)
            {
                return null;
            }
            Type type2 = Type.GetType(findData.InputType);
            type2 = ((type2 == null) ? Nullable.GetUnderlyingType(type2) : type2);
            if (type2 == null)
            {
                return null;
            }
            return Convert.ChangeType(findData.DefaultValue, type2);
        }
    }
}
