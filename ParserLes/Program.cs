using ParserLes.DataFolder;
using ParserLes.SQLWorker;
using System;
using System.Threading.Tasks;

namespace ParserLes
{
    class Program
    {
        static void Main(string[] args)
        {
            bool flag = true;
            Parser parser = new Parser(150000,  10);
           Task.Run (()=> parser.ParseStart());
            ConsoleKey key;
            while (flag)
            {
                Console.WriteLine("Нажмите Escape для выхода");
                key = Console.ReadKey().Key;
                if(key== ConsoleKey.Escape)
                {
                    flag = false;
                    return;
                }

            }
                
            
        }
    }
}
