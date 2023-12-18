using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using FlaUI.UIA3.Identifiers;
using System.Diagnostics;

namespace Automation
{
    #region Menu
    public class Menu
    {
        public List<string> Items { get; set; }
            = new List<string>();
    }
    #endregion

    #region Command
    public interface ICommand
    {
        void Call();
    }

    /// <summary>
    /// 鍵盤命令
    /// </summary>
    public class KeyBoardCommand : ICommand
    {
        protected App app;
        protected VirtualKeyShort Key;
        protected int Wait;
        public KeyBoardCommand(App app, VirtualKeyShort key, int wait = 1000)
        {
            this.app = app;
            Key = key;
            Wait = wait;
        }
        public void Call()
        {
            Keyboard.Pressing(Key);
            Thread.Sleep(Wait);
        }
    }

    /// <summary>
    /// 從桌面找到視窗並點擊按鈕 <br/>
    /// 點擊過渡按鈕
    /// </summary>
    public class FindAndClickCommand : ICommand
    {
        protected App app;
        protected string Title;
        protected Func<AutomationElement?, bool> Predicate;
        public FindAndClickCommand(App app, string title, Func<AutomationElement?, bool> predicate)
        {
            this.app = app;
            Title = title;
            Predicate = predicate;
        }

        public void Call()
        {
            app.Find(Title)
                .GetControl
                .By(Predicate)
                .Click();
        }
    }

    /// <summary>
    /// 從桌面找到視窗 <br/>
    /// </summary>
    public class FindCommand : ICommand
    {
        protected App app;
        protected string Title;
        public FindCommand(App app, string title)
        {
            this.app = app;
            Title = title;
        }

        public void Call()
        {
            app.Find(Title);
        }
    }

    public class InputCommand : ICommand
    {
        public void Call()
        {

        }
    }

    public class ClickCommand : ICommand
    {
        public void Call()
        {

        }
    }
    #endregion

    #region App
    /// <summary>
    /// 所有操作的入口
    /// </summary>
    public class App : IDisposable
    {
        public UIA3Automation? UI { get; set; }
            = new UIA3Automation();
        public AutomationElement? CurrentElement { get; set; }
        public AppElement GetControl => new AppElement(this);
        public App()
        {
            UI = new UIA3Automation();
        }

        public App Start(string path, string? startPath = null)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = path;

            if (startPath != null)
                info.WorkingDirectory = startPath;

            CurrentElement = FlaUI.Core.Application
                .Launch(info)
                .GetMainWindow(UI);

            return this;
        }

        public App DigFind(string name)
        {
            var f = new Finder(CurrentElement);

            CurrentElement = f.Find(name);

            return this;
        }

        public App Find(string name)
        {
            var f = new Finder(UI);

            CurrentElement = f.Find(name);

            return this;
        }

        public void Dispose()
        {
            UI?.Dispose();
            UI = null;
        }
    }

    /// <summary>
    /// App 內 Control 元素
    /// </summary>
    public class AppElement
    {
        protected App app;
        protected string? ClassName;
        protected WeakReference<AutomationElement>[]? Elements;
        protected HashSet<Func<AutomationElement?, bool>> Conditions
            = new HashSet<Func<AutomationElement?, bool>>();
        public AppElement(App app)
        {
            this.app = app;
        }

        public AppElement By(Func<AutomationElement?, bool> predicate)
        {
            Conditions.Add(predicate);
            return this;
        }

        public AppElement Click()
        {
            var condition = Conditions.Last();

            Elements = app.CurrentElement?
                .FindAllDescendants()
                .Select(p => new WeakReference<AutomationElement>(p))
                .ToArray();

            if (Elements != null)
            {
                AutomationElement? tar = null;
                Elements.FirstOrDefault(p =>
                {
                    p.TryGetTarget(out var element);

                    try
                    {
                        return condition(element);
                    }
                    catch
                    {
                        return false;
                    }
                })?.TryGetTarget(out tar);

                if (tar != null)
                {
                    app.CurrentElement?.Focus();
                    tar.AsButton().Click();
                }
            }

            return this;
        }

    }

    /// <summary>
    /// 最基礎的尋找元素，搜尋視窗
    /// </summary>
    public class Finder
    {
        public enum EState
        {
            Desktop,
            Window
        }
        protected UIA3Automation? UI;
        protected AutomationElement? Element;
        protected EState State { get; set; }
        public Finder(UIA3Automation? ui)
        {
            UI = ui;
            State = EState.Desktop;
        }

        public Finder(AutomationElement? ele)
        {
            Element = ele;
            State = EState.Window;
        }

        /// <summary>
        /// 尋找元素
        /// </summary>
        public AutomationElement? Find(string name)
        {
            AutomationElement? ele = Handle(name);
            if (ele != null)
            {
                Broadcast.Instance.Show($"找到了 {name}");
            }

            return ele;
        }

        /// <summary>
        /// 尋找元素 Template
        /// </summary>
        protected virtual AutomationElement? Handle(string name)
        {
            AutomationElement? ele = null;
            switch (State)
            {
                case EState.Desktop:
                    if (!String.IsNullOrEmpty(name))
                    {
                        ele = UI?.GetDesktop().FindFirstChild(
                            new PropertyCondition(
                                AutomationObjectIds.NameProperty,
                                name,
                                PropertyConditionFlags.MatchSubstring
                            )
                        );
                    }
                    else
                    {
                        ele = UI?.GetDesktop().FindFirstChild(
                            new PropertyCondition(
                                AutomationObjectIds.NameProperty,
                                name,
                                PropertyConditionFlags.None
                            )
                        );
                    }
                    break;
                case EState.Window:
                    if (!String.IsNullOrEmpty(name))
                    {
                        ele = Element?.FindFirstDescendant(
                            new PropertyCondition(
                                AutomationObjectIds.NameProperty,
                                name,
                                PropertyConditionFlags.MatchSubstring
                            )
                        );
                    }
                    else
                    {
                        ele = UI?.GetDesktop().FindFirstChild(
                            new PropertyCondition(
                                AutomationObjectIds.NameProperty,
                                name,
                                PropertyConditionFlags.None
                            )
                        );
                    }
                    break;
            }

            return ele;
        }
    }
    #endregion

    /// <summary>
    /// 廣播
    /// </summary>
    public class Broadcast
    {
        private static Lazy<Broadcast> _Instance
            = new Lazy<Broadcast>(() => new Broadcast());
        public static Broadcast Instance
            => _Instance.Value;

        public void Show(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public interface IDeepCopy<T>
        where T : new()
    {
        public void CopyTo(T obj);

        public T DeepCopy()
        {
            var obj = new T();
            CopyTo(obj);
            return obj;
        }
    }

    public static class IDeepCopyExtension
    {
        public static T DeepCopy<T>(this T obj)
            where T : IDeepCopy<T>, new()
        {
            return ((IDeepCopy<T>)obj).DeepCopy();
        }

    }

}