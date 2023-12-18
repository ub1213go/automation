using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Identifiers;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using FlaUI.UIA3.Identifiers;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Automation
{
    public class Automation
    {
        private FlaUI.Core.Application App;
        public void Open()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"D:\ETAX\BLR\BLRMENU.exe";
            psi.WorkingDirectory = @"D:\ETAX\BLR";

            var app = FlaUI.Core.Application.Launch(psi);

            if (app == null) return;

            using (new Auto())
            {
                var win = FlaUI.Core.Application.Attach(app.ProcessId).GetMainWindow(Auto.automation);


                var ps = GetProcessByWindowTitle("建檔程式", 5000);

                if(ps == null) return;

                var sub_win = FlaUI.Core.Application.Attach(ps.Id)
                    .GetMainWindow(Auto.automation);

                Console.WriteLine(sub_win);

                Auto.WindowFinderRequest[] requests = new Auto.WindowFinderRequest[]
                {
                    new Auto.WindowFinderRequest()
                    {
                        WindowText = "訊息公告",
                        Flag = PropertyConditionFlags.MatchSubstring,
                        ControlText = "關閉",
                        TimeOut = 5000,
                        Action = (ele) =>
                        {
                            ele.AsButton().Click();
                            Keyboard.Pressing(VirtualKeyShort.ENTER);
                            Thread.Sleep(3000);
                        }
                    },
                    //new Auto.WindowFinderRequest()
                    //{
                    //    WindowText = "",
                    //    Flag = PropertyConditionFlags.None,
                    //    ControlText = "關閉",
                    //    TimeOut = 5000,
                    //    Action = (ele) =>
                    //    {
                    //        ele.AsButton().Click();
                    //        //SpinWait.SpinUntil(() => !ele?.IsAvailable ?? true, 5000);
                    //        //Thread.Sleep(5000);
                    //    }
                    //},
                    new Auto.WindowFinderRequest()
                    {
                        WindowText = "",
                        Flag = PropertyConditionFlags.None,
                        ControlText = "確定",
                        TimeOut = 5000,
                        Action = (ele) =>
                        {
                            ele.AsButton().Click();
                        }
                    },
                    new Auto.WindowFinderRequest()
                    {
                        WindowText = "(BLRWS005)版本與訊息檢查",
                        Flag = PropertyConditionFlags.None,
                        ControlText = "關閉",
                        TimeOut = 5000,
                        Action = (ele) =>
                        {
                            ele.AsButton().Click();
                        }
                    },
                };


                var wf = new Auto.WindowFinder();
                foreach(var req in requests)
                {
                    wf.Handle(req);
                }

                AutomationElement? autoform = null;
                SpinWait.SpinUntil(() =>
                {
                    autoform = Auto.automation?.GetDesktop().FindFirstChild(
                            new PropertyCondition(
                                AutomationObjectIds.NameProperty,
                                "營業稅離線建檔系統",
                                PropertyConditionFlags.MatchSubstring
                            )
                        );

                    if(autoform != null)
                    {
                        autoform.SetForeground();
                        Mouse.Position = new System.Drawing.Point(150, 230);
                        //Mouse.MoveTo(150, 230);
                        Mouse.LeftClick();
                        Keyboard.Type("82440940");
                        Keyboard.Pressing(VirtualKeyShort.ENTER);
                    }

                    return autoform != null;
                }, 5000);

                //var ele = autoform?.FindFirst(TreeScope.Descendants,
                //    new PropertyCondition(
                //        AutomationObjectIds.AutomationIdProperty,
                //        50004
                //    )
                //);

                //ele?.Focus();

                autoform = Auto.automation?.GetDesktop().FindFirstChild(
                        new PropertyCondition(
                            AutomationObjectIds.NameProperty,
                            "營業稅離線建檔系統",
                            PropertyConditionFlags.MatchSubstring
                        )
                    );
                if(autoform != null)
                {
                    autoform = autoform?.FindFirstDescendant(
                        new PropertyCondition(
                            AutomationObjectIds.NameProperty,
                            "工作區",
                            PropertyConditionFlags.None
                        )
                    );
                }

                if (autoform != null)
                {
                    Thread.Sleep(2000);
                    var ele = autoform?.FindFirstDescendant(
                        new PropertyCondition(
                            AutomationObjectIds.ClassNameProperty,
                            "TCDBMaskEdit",
                            PropertyConditionFlags.None
                        )
                    );

                    ele.Focus();
                    Keyboard.Type("11209");
                }
            }
        }


        public void Test1()
        {
            //var app = FlaUI.Core.Application.Launch("notepad.exe");
            using (var automation = new UIA3Automation())
            {
                //var app = FlaUI.Core.Application.LaunchStoreApp("Power Automate");
                var app = FlaUI.Core.Application.Launch("D:\\Program Files (x86)\\Power Automate Desktop\\PAD.Console.Host.exe");
                //Console.WriteLine(app.ProcessId);
                var pid = GetProcessByWindowTitle("Power Automate", 10, 2)?.Id ?? 0;
                var window = FlaUI.Core.Application.Attach(pid).GetMainWindow(automation);



                new Auto.ElementBase(window).GetControl<TextBox>("搜尋流程").Text = "TEST";
                var test = new Auto.ElementBase(window).GetControl<AutomationElement>("TEST");
                Mouse.Position = test.BoundingRectangle.Location;
                //new Auto.ElementBase(window).GetControl<Button>("執行").Click();
                //Keyboard.Press(VirtualKeyShort.CONTROL)
            }
        }
        public void Test2()
        {
            //var app = FlaUI.Core.Application.Launch("calc.exe");
            var app = FlaUI.Core.Application.LaunchStoreApp("calc.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                var button1 = window.FindFirstDescendant(cf => cf.ByText("1"))?.AsButton();
                button1?.Invoke();
            }
        }
        public void Test3()
        {
            var automation = new UIA3Automation();
            var app = FlaUI.Core.Application.Launch("D:\\Program Files (x86)\\Power Automate Desktop\\PAD.Console.Host.exe");
            //var app = FlaUI.Core.Application.Launch("notepad");
            var window = app.GetMainWindow(automation);
            window.Focus();
            var textBox = window.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            //textBox.Enter("Hello world");
            Console.ReadKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowTitle">視窗名稱</param>
        /// <param name="tryTime">嘗試次數</param>
        /// <param name="interval">迴圈間隔，秒為單位</param>
        /// <returns></returns>
        public Process GetProcessByWindowTitle(string windowTitle, int tryTime, int interval = 1)
        {
            while (tryTime-- > 0)
            {
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    if (process.MainWindowTitle == windowTitle)
                    {
                        return process;
                    }
                }
                System.Threading.Thread.Sleep(interval * 1000);
            }

            return null;
        }
        public Process? GetProcessByWindowTitle(string windowTitle, int timeout)
        {
            SpinWait.SpinUntil(() =>
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.MainWindowTitle.Contains(windowTitle))
                    {
                        Console.WriteLine(process.MainWindowTitle);
                        return true;
                    }
                }
                return false;
            }, timeout);

            return Process.GetProcesses().FirstOrDefault(p =>
                p.MainWindowTitle.Contains(windowTitle)
            );
        }
    }

    public class Auto : IDisposable
    {
        public static UIA3Automation? automation 
            = new UIA3Automation();

        public interface ICopyable<T>
            where T : new()
        {
            void CopyTo(T other);
            public T DeepCopy()
            {
                T t = new T();
                CopyTo(t);
                return t;
            }
        }
        public class ElementBase : ICopyable<ElementBase>
        {
            public ElementBase()
            {

            }
            public ElementBase(Window window) 
            {
                Window = window;
            }
            public ElementBase(Window window, IElementStrategy elementStrategy) : this(window)
            {
                ElementStrategy = elementStrategy;
            }

            protected AutomationElement? AutomationElement { get; set; }
            protected Window? Window { get; set; }
            protected IElementStrategy ElementStrategy { get; set; } 
                = new NameElement();

            protected AutomationElement? GetElement(FlaUI.Core.Identifiers.PropertyId propertyId,
                object value,
                int dept = 0
                )
            {
                if (dept < 0) throw new ArgumentException();

                var condition = new PropertyCondition(propertyId, value);
                var elements = Window?.FindAll(TreeScope.Descendants, condition);
                var ele = elements?.ElementAt(dept);
                return ele;
            }
            public virtual T? GetControl<T>(object value)
                where T : AutomationElement
            {
                var ele = GetElement(
                    ElementStrategy.GetPropertyId(),
                    value
                    )?.As<T>();
                Console.WriteLine($"取得 {typeof(T).Name}-{ele?.Name}");
                return ele;
            }

            public void CopyTo(ElementBase other)
            {
                other.Window = Window;
                other.ElementStrategy = ElementStrategy;
            }
        }
        public interface IElementStrategy
        {
            PropertyId GetPropertyId();
        }
        public class NameElement : IElementStrategy
        {
            public PropertyId GetPropertyId() => 
                AutomationObjectIds.NameProperty; 
        }
        public class WindowFinder
        {
            public void Handle(WindowFinderRequest request)
            {
                if (request == null) return;

                SpinWait.SpinUntil(() =>
                {
                    var form = automation?.GetDesktop().FindFirstChild(
                            new PropertyCondition(
                                AutomationObjectIds.NameProperty,
                                request.WindowText,
                                request.Flag
                            )
                        );

                    if(form != null)
                    {
                        form.SetForeground();
                        var control = new Auto.ElementBase(form.AsWindow())?
                            .GetControl<Button>(request.ControlText); 

                        request.Action(control);
                    }

                    return form != null;
                }, request.TimeOut);
            }

        }
        public class WindowFinderRequest : ICopyable<WindowFinderRequest>
        {
            public string? WindowText { get; set; }
            public string? ControlText { get; set; }
            public PropertyConditionFlags Flag { get; set; } 
                = PropertyConditionFlags.MatchSubstring;
            public int TimeOut { get; set; }
            public Action<AutomationElement?>? Action { get; set; }

            public void CopyTo(WindowFinderRequest other)
            {
                other.WindowText = WindowText;
                other.ControlText = ControlText;
                other.Flag = Flag;
                other.TimeOut = TimeOut;
                other.Action = Action;
            }
        }

        public void Dispose()
        {
            automation?.Dispose();
            automation = null;
        }
    }
}
