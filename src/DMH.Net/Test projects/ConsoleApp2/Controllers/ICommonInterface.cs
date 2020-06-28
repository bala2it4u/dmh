using System;

namespace ConsoleApp2.Controllers
{
    public interface ICommonInterface<T1, T2>
    {
        T2 Run(T1 typeClass);
    }


    public class CommonInterface<T1, T2> : ICommonInterface<T1, T2>
    {
        public T2 Run(T1 typeClass)
        {
            Console.WriteLine("CommonInterface <T1, T2> ");

            return default(T2);
        }
    }

    public class CommonInterface1 : ICommonInterface<string, string>
    {
        public string Run(string typeClass)
        {
            Console.WriteLine("CommonInterface <T1, T2> "+ typeClass);

            return default(string);
        }
    }
}