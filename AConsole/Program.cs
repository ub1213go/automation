using ASqlite;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AConsole.MyAutofac;
using System.Security.Cryptography.X509Certificates;

namespace AConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            var auto = new Autofac();
            auto.Register(typeof(ConsoleMenu));
            auto.Register(typeof(ConsoleMenuInteractor));
            var menu = auto.Get<ConsoleMenuInteractor>();

            //var str = "123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345123451234512345";
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Add(str);
            //menu.Start();
            //Console.WriteLine(menu.View.PageCount); 

            var ci = new ConsoleItem("-----");
            ci.x = 10;
            ci.y = 20;

            menu.View.Render(ci);
        }
    }

    public class ConsoleMenuInteractor
    {
        int Position = -1;

        public ConsoleMenuView View;

        ConsoleMenuItem? Current => Menu.GetItem(Position);

        ConsoleMenu Menu { get; set; }

        public ConsoleMenuInteractor(ConsoleMenu cm)
        {
            View = new ConsoleMenuView(cm);
            Menu = cm;
        }

        public ConsoleMenuInteractor Add(string str)
        {
            Menu.AddItem(str);

            return this;
        }

        public bool Next()
        {
            var condi = Position < Menu.Items.Count - 1;
            if(condi)
            {
                Current?.ResetState();
                Position++;
            }

            return condi;
        }

        public bool Before()
        {
            var con = Position > 0;
            if(con)
            {
                Current?.ResetState();
                Position--;
            }

            return con;
        }

        public ConsoleMenuInteractor Start()
        {
            var loop = true;
            var page = 1;
            while (loop)
            {
                Console.Clear();
                View.SetPage(page);
                View.Show();
                var inp = Console.ReadKey(true);
                switch (inp.Key)
                {
                    case ConsoleKey.Escape:
                        loop = false;
                        break;
                    case ConsoleKey.DownArrow:
                        if (Next())
                        {
                            Current?.Select();
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (Before())
                        {
                            Current?.Select();
                        }
                        break;
                    case ConsoleKey.F:
                        page++;
                        break;
                    case ConsoleKey.B:
                        page--;
                        break;
                    //case ConsoleKey.C:
                    //    Console.Clear();
                    //    break;
                }
            }
            return this;
        }
    }

    public class ConsoleMenuView : IRenderable<ConsoleItem>
    {
        public ConsoleMenu Menu { get; set; }

        int LineHeightMax => Console.WindowHeight - 1;

        int Position = -1;

        public ConsoleItem? Current =>
            Menu?.GetItem(Position);

        int LinePosition = 0;

        /// <summary>
        /// 計算 Menu 的 Items 等於幾張頁數
        /// </summary>
        public int PageCount => Menu.Items.Sum(p => p.LineCount) / LineHeightMax + 1;

        public ConsoleMenuView(ConsoleMenu menu)
        {
            Menu = menu;
        }

        public IEnumerable<string> FormatMenuItems()
        {
            var start = LinePosition;
            foreach(var item in Menu.GetItems(Position, Menu.Items.Count))
            {
                for (int j = start; j < item.LineCount; j++)
                {
                    yield return item.GetLine(j)?.ToString() ?? "";
                }
                start = 0;
            }
        }

        /// <summary>
        /// 設置可視範圍
        /// </summary>
        /// <param name="page">從 1 開始</param>
        public void SetPage(int page)
        {
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException($"{nameof(page)} 不能小於 1");
            }

            // 根據頁數判斷第幾行結束
            var lineStart = LineHeightMax * (page - 1);
            for(int i = 0; i < Menu.Items.Count; i++)
            {
                var item = Menu.Items[i];

                if (lineStart >= item.LineCount)
                {
                    lineStart -= item.LineCount;
                    continue;
                }

                Position = i;
                LinePosition = lineStart;
                break;
            }
        }

        public void Show()
        {
            var max = LineHeightMax;

            foreach(var line in FormatMenuItems())
            {
                if (max-- > 0)
                    Console.WriteLine(line);
                else
                    break;
            }
        }

        public void Render(ConsoleItem item)
        {
            var (x, y) = Console.GetCursorPosition();
            Console.SetCursorPosition(item.x, item.y);
            Console.WriteLine(item);
            Console.SetCursorPosition(x, y);
        }
    }

    public class ConsoleMenuPage
    {

    }

    public class ConsoleMenu
    {
        public List<ConsoleMenuItem> Items { get; set; }
            = new List<ConsoleMenuItem>();

        public ConsoleMenu AddItem(string item)
        {
            Items.Add(
                ConsoleMenuItem.Factory.NewOne(item)
            );

            return this;
        }

        public ConsoleMenuItem? GetItem(int position)
        {
            return Items.ElementAtOrDefault(position);
        }

        /// <param name="start">index 起點</param>
        /// <param name="end">index 結束</param>
        public IEnumerable<ConsoleMenuItem> GetItems(int start, int end)
        {
            for(int i = Math.Max(0, start); i < end; i++)
            {
                yield return Items[i];
            }
        }
    }

    public class ConsoleItem : IPoint
    {
        public string Item { get; set; }

        public int x { get; set; }
        public int y { get; set; }

        protected int IndentCount = 3;

        public ConsoleItem(string item = "")
        {
            Item = item;
        }

        public override string ToString()
        {
            return Item;
        }

        public virtual string Indent(int index)
        {
            return new string(' ', IndentCount);
        }
    }

    public class ConsoleStateItem : ConsoleItem
    {
        public enum EItemState
        {
            None,
            Selected,
        }

        EItemState State { get; set; }

        public ConsoleStateItem(string item) : base(item)
        {
            State = EItemState.None;
        }

        public void Select()
        {
            State = EItemState.Selected;
        }

        public void ResetState()
        {
            State = EItemState.None;
        }

        /// <param name="index">行的 index</param>
        public override string Indent(int index)
        {
            if(index == 0)
            {
                switch (State)
                {
                    case EItemState.Selected:
                        return " > ";
                    case EItemState.None:
                        return "   ";
                }
            }

            return base.Indent(index);
        }

    }

    public class ConsoleMenuItem : ConsoleStateItem
    {
        public int LineWidthMax => LineMaxLength() - IndentCount;
        public int LineCount => Item.Length / LineWidthMax;
        public int Index = 0;

        public static ConsoleMenuItemFactory Factory
            = new ConsoleMenuItemFactory();

        public class ConsoleMenuItemFactory
        {
            public int Count = 0;
            public ConsoleMenuItem NewOne(string str)
            {
                var cmi = new ConsoleMenuItem(str);
                cmi.Index = Count++;
                return cmi;
            }
        }

        public ConsoleMenuItem(string item) : base(item)
        {

        }

        public int LineMaxLength()
        {
            return Console.WindowWidth;
        }

        /// <summary>
        /// 取得每行該顯示的文字
        /// </summary>
        public ConsoleMenuItemLine? GetLine(int lineIdx)
        {
            if(LineCount < lineIdx)
            {
                return null;
            }

            switch (lineIdx)
            {
                case 0:
                    return new ConsoleMenuItemLine(
                        this,
                        Item.Substring(
                            LineWidthMax * lineIdx,
                            LineWidthMax - 3
                        ),
                        lineIdx
                    );
                default:
                    return new ConsoleMenuItemLine(
                        this,
                        Item.Substring(
                            LineWidthMax * lineIdx,
                            LineWidthMax
                        ),
                        lineIdx
                    );
            }
        }
    }

    public class ConsoleMenuItemLine
    {
        ConsoleMenuItem MenuItem;
        int LineIndex;
        public string Line { get; set; }
        public ConsoleMenuItemLine(ConsoleMenuItem menuItem, string line, int lineIndex)
        {
            MenuItem = menuItem;
            Line = line;
            LineIndex = lineIndex;
        }

        public override string ToString()
        {
            switch (LineIndex)
            {
                case 0:
                    return MenuItem.Indent(LineIndex) +
                        (MenuItem.Index + 1).ToString() + ". " +
                        Line;
                default:
                    return MenuItem.Indent(LineIndex) + Line;
            }
        }
    }

    public interface IPoint
    {
        int x { get; set; }
        int y { get; set; }
    }

    public interface IRenderable<T>
    {
        void Render(T obj);
    }
}

