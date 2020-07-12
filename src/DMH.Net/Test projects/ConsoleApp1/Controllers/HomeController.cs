using ConsoleApp2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace LuckyHome
{
    public class LuckyHomeInterfaceClassMapper
    {

        public object Run(Type type)
        {
            if (type == typeof(ICommonInterface<IWTypeClass, IInterface>))
            {
                return new CommonInterface<IWTypeClass, IInterface>();
            }
            return null;
        }
    }
}
namespace ConsoleApp1.Controllers
{
    public enum EData {
        None=0,
        Super=1,
        Pass=3,
        Default=3
    }
    public static class staticClassTest
    {
        public static int staticTest(int? number, string data="summa", EData eDatauser= EData.Super)
        {
            return number.GetValueOrDefault() * 10;
        }
    }

    public class Interface : IInterface
    {
        public void About(string dataAbout, ClassInject classInject)
        {
            Console.WriteLine(dataAbout);
        }
    }

    public class HomeController : Controller
    {
        private readonly Lazy<IInterface> method;
        private readonly ICommonInterface<IWTypeClass, IInterface> commonInterface;

        public HomeController(Lazy<IInterface> method, ICommonInterface<IWTypeClass, IInterface> commonInterface) {
            this.method = method;
            this.commonInterface = commonInterface;
        }
        public class nestedClass : IInterface
        {
            public void About(string dataAbout, ClassInject classInject)
            {
                //throw new NotImplementedException();
            }
        }
        public ActionResult Index(string data)
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(string dataAbout, ClassInject classInject, Lazy<IInterface> method1,
            int? nulldata, CommonInterface<TTypeClass, WTypeClass> commonInterface1,
            CommonInterface1 commonInterface2)
        {
            ViewBag.Message = "Your application description page.....";
            if (string.IsNullOrWhiteSpace(dataAbout))
                dataAbout = "Your application description page.....";

            method.Value.About(dataAbout, classInject);
            method1.Value.About(dataAbout, new ClassInject(-1));
            commonInterface.Run(new WTypeClass());
            commonInterface1.Run(new TTypeClass());
            commonInterface2.Run("data");
            return View();
        }

        //static class, static function, private ,internal, proteced methods handle
        public static void staticTest(CommonInterface<TTypeClass, WTypeClass> commonInterface1)
        {
            commonInterface1.Run(new TTypeClass());
        }
        public static class staticClassTest1
        {
            public static void staticTest()
            {

            }
        }

        

        public class GericClass {

            
            public void genricMethod1()
            {

            }
            public void genricMethod<T>(T data)
            {

            }
            public static class staticClassTest2
            {
                public static void staticTest()
                {

                }
            }
        }
        public void genricMethod<T>(T data) {

        }

        private void privateMethod()
        {

        }
        internal void internalMethod() {

        }
        protected void protectedMethod() {

        }
        internal protected void internalprotectedMethod() { }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}