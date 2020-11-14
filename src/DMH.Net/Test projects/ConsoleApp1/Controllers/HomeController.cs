using ConsoleApp2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using System.IO;

namespace LuckyHome
{
    public class sample
    {

        public void Run()
        {
            var dirs = Directory.GetDirectories(@"D:\moto g4 plus baclkup\Videos");
            foreach (var dir in dirs)
            {
               var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    string date = "";
                    var datetime = DateTime.Now;
                    DateTime creationTime = File.GetCreationTime(file);
                        var tempFile = Path.GetFileNameWithoutExtension(file);
                    try
                    {
                        // Read oritinal file creation time  
                        int index = tempFile.IndexOf("20");
                        date = tempFile.Substring(index, 4 + 4);
                        datetime = new DateTime(int.Parse(date.Remove(4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6)), 0, 0, 0);
                    }
                    catch
                    {
                        continue;
                    }
                    // Display creation time  
                    Console.WriteLine(creationTime.ToString("MM/dd/yyyy HH:mm:ss"));
                    // Manually override previous creation time to now  
                    File.SetCreationTime(file, datetime);
                    Console.WriteLine(File.GetCreationTime(file).ToString("MM/dd/yyyy HH:mm:ss"));
                }
            }

            //if (type == typeof(ICommonInterface<IWTypeClass, IInterface>))
            //{
            //    return new CommonInterface<IWTypeClass, IInterface>();
            //}
            //return null;
        }

        public void runAlbum()
        {
            var dir = @"D:\moto g4 plus baclkup\Songs";
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {

                byte[] b = new byte[128];
                string sTitle;
                string sSinger;
                string sAlbum;
                string sYear;
                string sComm;

                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    fs.Seek(-128, SeekOrigin.End);
                    fs.Read(b, 0, 128);
                }
                bool isSet = false;
                String sFlag = System.Text.Encoding.Default.GetString(b, 0, 3);
                if (sFlag.CompareTo("TAG") == 0)
                {
                    System.Console.WriteLine("Tag   is   setted! ");
                    isSet = true;
                }

                if (isSet)
                {
                    sAlbum = System.Text.Encoding.Default.GetString(b, 63, 30);
                    System.Console.WriteLine("Album: " + sAlbum);
                    Path.GetInvalidFileNameChars().All(x => { sAlbum = sAlbum.Replace(x.ToString(), ""); return true; });
                    if (string.IsNullOrWhiteSpace(sAlbum))
                    {
                        continue;
                    }
                    var tempDir = Path.Combine(dir, sAlbum);
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }
                    File.Move(file, Path.Combine(tempDir, Path.GetFileName(file)));
                }
            }
            System.Console.WriteLine("Any   key   to   exit! ");
            System.Console.Read();
        }
    }
}

namespace LuckyHome
{
    public class LuckyHomeInterfaceClassMapper1
    {
        private readonly static ILifetimeScope scope;

        static LuckyHomeInterfaceClassMapper1()
        {
            //scope = AutofacConfig.Register().BeginLifetimeScope();
            //HttpConfiguration config = new HttpConfiguration();
            //scope = AutofacConfigFE.Register(config, false).BeginLifetimeScope();
        }

        public object Run(Type type)
        {
            var output = scope.Resolve(type);
            if (output == null)
            {
            }
            return output;

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
        public static int staticTest(DateTime now, int? number=100, string data = "summa", EData eDatauser= EData.Pass)
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
        private Lazy<IInterface> method;
        private readonly ICommonInterface<IWTypeClass, IInterface> commonInterface;

        public HomeController(Lazy<IInterface> method, ICommonInterface<IWTypeClass, IInterface> commonInterface) {
            this.method = method;
            this.commonInterface = commonInterface;
        }

        public void LuckyHomeUp(string methodName, object[] input)
        {
            if (methodName == "Index")
            {
                //this.method = new Lazy<IInterface>(()=>new nestedClass());
                input[0] = "1234567890";
            }
        }
        public void LuckyHomeDown(string methodName, object[] input, object output)
        {

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
            method.Value.About("", null);
            commonInterface.Run(null);
            Console.WriteLine(DateTime.Now);
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
            return View(dataAbout);
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