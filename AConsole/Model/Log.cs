using AConsole.Model.ConsoleUI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public class Log<T> : DynamicObject
    {
        public readonly T subject;
        public List<string> stack
            = new List<string>();
        public Log(T obj)
        {
            stack.Add(new string('-', Console.WindowWidth / 2 - 4) + "歷史紀錄" + new string('-', Console.WindowWidth / 2 - 4));
            subject = obj;
            ToLog();
        }

        //public static I As<I>() where I : class
        //{
        //    if (!typeof(I).IsInterface)
        //    {
        //        throw new ArgumentException("I must be an interface type!");
        //    }

        //    return new Log<T>(new T()).ActLike<I>();
        //}

        public void ToLog()
        {
            var cur = new ConsoleCursor();
            var rect = new Model.Rectangle(0, Console.WindowHeight - 5, Console.WindowWidth, Console.WindowHeight);
            cur.Clear(rect);
            cur.Render(rect, String.Join(Environment.NewLine, stack));
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            try
            {
                var msg = $"Call {subject.GetType().Name}.{binder.Name} " + String.Join(", ", args.Select(p=>$"\"{p}\""));

                stack.Insert(1, msg);
                if(stack.Count > 5)
                {
                    stack.RemoveRange(5, stack.Count - 5);
                }

                ToLog();

                result = subject.GetType().GetMethod(binder.Name).Invoke(subject, args);
                return true;
            }
            catch(Exception err)
            {
                Console.Write($"Failure Call {subject.GetType().Name}.{binder.Name}  {err.Message}");
                result = null;
                return false;
            }
        }
    }    

}
