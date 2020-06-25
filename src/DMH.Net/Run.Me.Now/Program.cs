using LuckyHome.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Run.Me.Now
{
    partial class Program
    {
        static Type[] userInputType = new[]{
            typeof(string),typeof(int), typeof(long), typeof(double), typeof(float), typeof(DateTime)
            };
        static SchemaInfo MainSchemaInfo = null;
        private static Assembly MainAssembly;

        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            Console.WriteLine("starting...");
            //Console.WriteLine("reading url args :" + AppContext.BaseDirectory);

            var schemapath = System.IO.Path.Combine(AppContext.BaseDirectory, "schemaInfo.json");
            //Console.ReadKey();
            while (System.IO.File.Exists(schemapath) == false)
            {
                System.Threading.Thread.Sleep(1000 * 5);
            }

            MainSchemaInfo = Json.Decode<SchemaInfo>(
            System.IO.File.ReadAllText(schemapath));
            var appPath = AppContext.BaseDirectory + MainSchemaInfo.AssambleName;

            MainAssembly = Assembly.Load(MainSchemaInfo.AssambleName);

            Type typeYouWant = MainAssembly.GetType(MainSchemaInfo.NameSpaceAndClass);
            
            var constructor = typeYouWant.GetConstructors();
            var classTypeParem = constructor.Length > 0 ? constructor[0].GetParameters() : new ParameterInfo[0];
            object[] createdInstance = classTypeParem.Length == 0 ? new object[0] :
                                            createInstance(classTypeParem);

            object instance = constructor.Length > 0 ? Activator.CreateInstance(typeYouWant, createdInstance) : null;
            MethodInfo method = getMethod(typeYouWant);

            classTypeParem = method.GetParameters();
            createdInstance = classTypeParem.Length == 0 ? new object[0] :
                                            createInstance(classTypeParem);
            //method.MakeGenericMethod(

            method.Invoke(instance, createdInstance);
            //Console.WriteLine("enter to end");
            //Console.ReadKey();
        }

        private static MethodInfo getMethod(Type typeYouWant)
        {
            switch (MainSchemaInfo.EMethodType)
            {
                case EMethodType.vsCMAccessPublic:
                    return typeYouWant.GetMethod(MainSchemaInfo.MethodToRun);
                case EMethodType.vsCMAccessPrivate:
                case EMethodType.vsCMAccessProject:
                case EMethodType.vsCMAccessProtected:
                case EMethodType.vsCMAccessProjectOrProtected:
                case EMethodType.vsCMAccessDefault:
                case EMethodType.vsCMAccessAssemblyOrFamily:
                case EMethodType.vsCMAccessWithEvents:
                    return typeYouWant.GetMethod(MainSchemaInfo.MethodToRun, BindingFlags.NonPublic | BindingFlags.Instance);
                default:
                    return typeYouWant.GetMethod(MainSchemaInfo.MethodToRun, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        private static object[] createInstance(ParameterInfo[] classTypeParem)
        {
            List<object> createdInstance = new List<object>();
            foreach (var item in classTypeParem)
            {
                var paremType = item.ParameterType;
                if (
                    userInputType.Contains(paremType)
                    || userInputType.Contains(Nullable.GetUnderlyingType(paremType)) 
                    || paremType.IsEnum)
                {
                    if (paremType.IsEnum)
                    {
                        createdInstance.Add(getEnumValue(item));
                    }
                    else {
                    createdInstance.Add(
                        getTypeValue(item));
                    }
                    continue;
                }

                if (paremType.IsInterface)
                {
                    var matchClass = MainSchemaInfo.DepandancyClasses.FirstOrDefault(x =>
                    x.NameSpaceAndInterfaceName == paremType.FullName);
                    paremType = MainAssembly.GetType(matchClass.NameSpaceAndMappedClassName);
                    if (paremType == null)
                    {
                        paremType = Assembly.Load(matchClass.AssambleName).GetType(matchClass.NameSpaceAndMappedClassName);
                    }
                }
                if (paremType == null)
                {
                    createdInstance.Add(null);

                }
                if (false == paremType.IsClass)
                {
                    createdInstance.Add("");
                    continue;
                }

                if (paremType.FullName.StartsWith("System.Lazy"))
                {
                    var tempType = paremType.GetGenericArguments()[0];

                    if (tempType.IsInterface)
                    {
                        var matchClass = MainSchemaInfo.DepandancyClasses.FirstOrDefault(x =>
                        x.NameSpaceAndInterfaceName == tempType.FullName);
                        tempType = MainAssembly.GetType(matchClass.NameSpaceAndMappedClassName);
                        if (tempType == null)
                        {
                            tempType = Assembly.Load(matchClass.AssambleName).GetType(matchClass.NameSpaceAndMappedClassName);
                        }

                        var paremTypeConstructors = tempType.GetConstructors();
                        var paremTypeParemType = paremTypeConstructors.Length > 0 ? paremTypeConstructors[0].GetParameters() : new ParameterInfo[0];
                        object[] tempCreatedInstance = new object[0];
                        if (paremTypeParemType.Length > 0)
                        {
                            tempCreatedInstance = createInstance(paremTypeParemType);
                        }

                        //var tempTypeInstance = Activator.CreateInstance(tempType, tempCreatedInstance);
                        var lazyMapper = new lazyMapper();
                        var methodCall = Expression.Call(Expression.Constant(lazyMapper),
                                     factoryMethod,
                                     Expression.Constant(tempType),
                                     Expression.Constant(tempCreatedInstance));
                        var cast = Expression.Convert(methodCall, tempType);
                        var lambda = Expression.Lambda(cast).Compile();
                        //var lazyType = typeof(Lazy<>).MakeGenericType(new[] { tempType });

                        createdInstance.Add(Activator.CreateInstance(paremType, lambda));
                    }
                    else
                    {
                        throw new NotSupportedException("only lazy type interface supported");
                    }
                }
                else
                {

                    var paremTypeConstructors = paremType.GetConstructors();
                    var paremTypeParemType = paremTypeConstructors.Length > 0 ? paremType.GetConstructors()[0].GetParameters() : new ParameterInfo[0];
                    object[] tempCreatedInstance = new object[0];
                    if (paremTypeParemType.Length > 0)
                    {
                        tempCreatedInstance = createInstance(paremTypeParemType);
                    }

                    createdInstance.Add(
                                 Activator.CreateInstance(paremType, tempCreatedInstance));
                }
            }

            return createdInstance.ToArray();
        }
        public class lazyMapper
        {
            public object Factory(Type type, object[] someArgument)
            {
                // Magic resolving going on here, not interesting
                return Activator.CreateInstance(type, someArgument);
            }
        }
        private readonly static MethodInfo factoryMethod = typeof(lazyMapper).GetMethod(nameof(lazyMapper.Factory));

        private static object getEnumValue(ParameterInfo item)
        {
            var findData = MainSchemaInfo.InputValues.FirstOrDefault(x => x.InputName == item.Name);
            if (null == findData || findData.DefaultValue == null)
                return null;
            var enumValue = findData.DefaultValue.ToString();
            return Enum.Parse(item.ParameterType, enumValue.Substring(enumValue.LastIndexOf('.') + 1));
                //Enum.ToObject(item.ParameterType, enumValue.Substring(enumValue.LastIndexOf('.')));
        }

        private static object getTypeValue(ParameterInfo item)
        {
            var findData = MainSchemaInfo.InputValues.FirstOrDefault(x => x.InputName == item.Name);
            if (null == findData || findData.DefaultValue == null)
                return null;

            Type type = Type.GetType(findData.InputType);
            type = type == null ? Nullable.GetUnderlyingType(type) : type;

            if (type == null)
                return null;

            return Convert.ChangeType(findData.DefaultValue, type);
            /*switch (type.Name)
            {
                case "Int32":
                    return Convert.ToInt32(findData.DefaultValue ?? 0);
                case "Int64":
                    return Convert.ToInt64(findData.DefaultValue ?? 0);
                case "Double":
                    return Convert.ToDouble(findData.DefaultValue ?? 0);
                case "Single":
                    return Convert.ToSingle(findData.DefaultValue ?? 0);
                case "Decimal":
                    return Convert.ToDecimal(findData.DefaultValue ?? 0);
                case "Boolean":
                    return Convert.ToBoolean(findData.DefaultValue ?? 0);
                case "DateTime":
                    return Convert.ToDateTime(findData.DefaultValue ?? DateTime.MinValue);

                default:
                    return findData.DefaultValue;
            }*/
        }
    }
}
