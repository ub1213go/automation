﻿using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public enum EAction
    {
        Click,
        Focus
    }
    public enum EType
    {
        Window,
        Control
    }
    public interface IAutoJsonState 
    {
        public object? Execute();
    }

    public class AutoJsonAction : IAutoJsonState
    {
        public AutoJsonAction(string action)
        {

        }

        public object? Execute()
        {
            return null;
        }
    }
    public class AutoJsonType : IAutoJsonState
    {
        public AutoJsonType(string type)
        {

        }

        public object? Execute()
        {
            return null;
        }
    }
    public class AutoJson2
    {
        public string? Type { get; set; }
        public string? Action { get; set; }
        public string? ID { get; set; }
        public string? Parent { get; set; }
        public string? Name { get; set; }
        public string? From { get; set; }
        public AutoJsonType? JsonType
        {
            get
            {
                return new AutoJsonType(Type);
            }
        }
        public EAction EnumAction
        {
            get
            {
                if(!Enum.TryParse(Action, out EAction enum_action))
                {
                    throw new JsonReaderException($"Action 解析錯誤，不包含 {Action}");
                }

                return enum_action;
            }
        }
        public override string ToString()
        {
            return $"{nameof(ID)}: {ID}, " +
                $"{nameof(Parent)}: {Parent}, " +
                $"{nameof(Name)}: {Name}, " +
                $"{nameof(From)}: {From}, " +
                $"{nameof(Type)}: {Type}, " +
                $"{nameof(Action)}: {Action}";
        }
        public string Test1()
        {
            return @"
[
    {
        ""ID"": ""1"",
        ""Parent"": """",
        ""From"": ""Title"",
        ""Name"": ""訊息公告"",
        ""Type"": ""Window"",
        ""Action"": ""Focus""
    },
    {
        ""ID"": ""2"",
        ""Parent"": ""1"",
        ""From"": ""Name"",
        ""Name"": ""關閉"",
        ""Type"": ""Control"",
        ""Action"": ""Click""
    }
]
";
        }
    }
    public class AutoJson
    {
        public string? Type { get; set; }
        public string? Action { get; set; }
        public string? ID { get; set; }
        public string? Parent { get; set; }
        public string? Name { get; set; }
        public string? From { get; set; }
        public EType? EnumType
        {
            get
            {
                if (!Enum.TryParse(Type, out EType enum_type))
                {
                    throw new JsonReaderException($"Type 解析錯誤，不包含 {Type}");
                }

                return enum_type;
            }
        }
        public EAction EnumAction
        {
            get
            {
                if(!Enum.TryParse(Action, out EAction enum_action))
                {
                    throw new JsonReaderException($"Action 解析錯誤，不包含 {Action}");
                }

                return enum_action;
            }
        }
        public override string ToString()
        {
            return $"{nameof(ID)}: {ID}, " +
                $"{nameof(Parent)}: {Parent}, " +
                $"{nameof(Name)}: {Name}, " +
                $"{nameof(From)}: {From}, " +
                $"{nameof(Type)}: {Type}, " +
                $"{nameof(Action)}: {Action}";
        }
        public string Test1()
        {
            return @"
[
    {
        ""ID"": ""1"",
        ""Parent"": """",
        ""From"": ""Title"",
        ""Name"": ""訊息公告"",
        ""Type"": ""Window"",
        ""Action"": ""Focus""
    },
    {
        ""ID"": ""2"",
        ""Parent"": ""1"",
        ""From"": ""Name"",
        ""Name"": ""關閉"",
        ""Type"": ""Control"",
        ""Action"": ""Click""
    }
]
";
        }
    }

    /// <summary>
    /// 包裝了 UIA3
    /// </summary>
    public class Core
    {
        private static Lazy<Core> _Instance
            = new Lazy<Core>(()=> new Core());
        private Lazy<UIA3Automation> _AutoCore 
            = new Lazy<UIA3Automation>(()=> new UIA3Automation());

        public static Core Instance 
            => _Instance.Value;
        public UIA3Automation AutoCore 
            => _AutoCore.Value;

        private Core() { }
    }

    /// <summary>
    /// User 使用的所有 API Facade
    /// </summary>
    public class AutoService
    {
        public Core Core { get; set; } = Core.Instance;
        public ConditionFactory ConditionFactory => Core.AutoCore.ConditionFactory;

        public AutoUI? GetWindow(string title)
        {
            List<IntPtr> handles = GetOpenWindowHandles();

            var dict = new Dictionary<IntPtr, string>();

            var len = 100;
            var sb = new StringBuilder(len);
            foreach(var handle in handles)
            {
                sb.Clear();
                GetWindowText(handle, sb, len);
                var text = sb.ToString();
                if (text.Contains(title))
                {
                    // 建立自己封裝的 AutoUI
                    var autoUI = new AutoUIWindow(
                        Core,
                        Core.AutoCore.FromHandle(handle)
                    );
                    return autoUI;
                }
                dict.Add(handle, text);
            }
            return null;
        }
        /// <summary>
        /// 從父取得視窗
        /// </summary>
        public AutoUI? GetWindowByParent(AutoUI parent, PropertyCondition propertyCondition)
        {
            return parent.Reduce(new AutoConditionByWindow(propertyCondition));
        }
        /// <summary>
        /// 從父取得甕志向
        /// </summary>
        public AutoUI? GetControlByParent(AutoUI parent, PropertyCondition propertyCondition)
        {
            return parent.Reduce(new AutoConditionByControl(propertyCondition));
        }
        /// <summary>
        /// 從父取得特定位置的 Control
        /// </summary>
        public AutoUI? GetControlByParent(AutoUI parent, int position)
        {
            return parent.Reduce(position);
        }
        /// <summary>
        /// 映射方法並呼叫
        /// </summary>
        public object? InvokeMethod(object obj, string method_name, params object[] args)
        {
            var type = obj.GetType();
            var method = type.GetMethod(method_name);
            var parameters = method?.GetParameters();
            if (parameters == null || (parameters?.Length ?? 0) == 0) 
                throw new Exception($"{method?.Name} 此方法無參數，與預期不同");

            object[] props = new object[parameters?.Length ?? 0];
            for (int i = 0; i < props.Length; i++)
            {
                if (parameters[i].HasDefaultValue)
                {
                    props[i] = parameters[i].DefaultValue;
                }
                else
                {
                    props[i] = args[i];
                }
            }

            return method?.Invoke(obj, props);
        }
        /// <summary>
        /// 回傳視窗 handle 與名稱
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<string, string>> GetAllWindowTitle()
        {
            var handles = GetOpenWindowHandles();
            var sb = new StringBuilder(100);
            foreach(var handle in handles)
            {
                var processId = 0;
                GetWindowThreadProcessId(handle, out processId);

                //var ths = Process.GetProcessById(processId).Threads;
                //foreach (ProcessThread th in ths)
                //{
                //    EnumThreadWindows(th.Id, (hWnd, lParam) =>
                //    {
                //        GetWindowText(hWnd, sb, 100);
                //        sb.Insert(0, "hwd: " + handle.ToInt32() + ", pid: " + processId.ToString() + ", ");
                //        return true;
                //    }, IntPtr.Zero);
                //    yield return sb.ToString();
                //}

                GetWindowText(handle, sb, 100);
                sb.Insert(0, "hwd: " + handle.ToInt32() + ", pid: " + processId.ToString() + ", ");

                yield return Tuple.Create(handle.ToInt32().ToString(), sb.ToString());
            }
        }
        public IEnumerable<string> GetWindowListByParent(AutoUI parent)
        {
            foreach(var child in parent.AutomationElement.FindAllChildren())
            {
                yield return child.ToString();
            }
        }
        /// <summary>
        /// 傳回 AutomationId 和名稱
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<string, string>> GetControlListByParent(AutoUI parent)
        {
            foreach(var child in parent.AutomationElement.FindAllChildren())
            {
                yield return Tuple.Create(child.Name, child.ToString());
            }
        }

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        public delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadWndProc lpfn, IntPtr lParam);

        protected static List<IntPtr> GetOpenWindowHandles()
        {
            List<IntPtr> windowHandles = new List<IntPtr>();

            EnumWindows((hWnd, lParam) =>
            {
                // Skip invisible windows
                if (!IsWindowVisible(hWnd))
                    return true;

                // Get the window title
                const int nChars = 256;
                System.Text.StringBuilder title = new System.Text.StringBuilder(nChars);
                if (GetWindowText(hWnd, title, nChars) >= 0)
                {
                    // Print or store the window handle
                    //Console.WriteLine($"Window Title: {title}, Handle: {hWnd}");
                    windowHandles.Add(hWnd);
                }

                return true;
            }, IntPtr.Zero);

            return windowHandles;
        }
    }


    // todo: UI 實作 Visitor Pattern 可以 Reduce 成子 UI 或 Control
    // todo: 這個做為基類
    public abstract class AutoUI
    {
        public Core Core { get; set; }
        public AutomationElement? AutomationElement { get; set; }
        public AutoUI(Core core, AutomationElement? ae = null)
        {
            Core = core;
            AutomationElement = ae;
        }
        public abstract AutoUI Reduce(AutoCondition autoCondition);
        public abstract AutoUI Reduce(int position);
    }
    public class AutoUIWindow : AutoUI
    {
        public AutoUIWindow(Core core, AutomationElement? ae) : base(core, ae) { }

        public override AutoUI Reduce(AutoCondition autoCondition)
        {
            return autoCondition.Transform(this);
        }
        public override AutoUI Reduce(int position)
        {
            return new AutoUIWindow(
                Core,
                AutomationElement?.FindAllChildren().ElementAt(position)
            );
        }
    }
    public class AutoUIControl : AutoUI
    {
        public AutoUIControl(Core core, AutomationElement? ae) : base(core, ae) { }

        public override AutoUI Reduce(AutoCondition autoCondition)
        {
            return autoCondition.Transform(this);
        }
        public override AutoUI Reduce(int position)
        {
            return new AutoUIControl(
                Core,
                AutomationElement?.FindAllChildren().ElementAt(position)
            );
        }
    }

    public abstract class AutoCondition
    {
        public PropertyCondition PropertyCondition { get; set; }
        public AutoCondition(PropertyCondition propertyCondition) 
        { 
            PropertyCondition = propertyCondition;
        }

        public abstract AutoUI Transform(AutoUI autoUI);
    }
    public class AutoConditionByWindow : AutoCondition
    {
        public AutoConditionByWindow(PropertyCondition propertyCondition) : base(propertyCondition)
        {

        }
        public override AutoUI Transform(AutoUI autoUI)
        {
            return new AutoUIWindow(
                autoUI.Core,
                autoUI.AutomationElement?
                    .FindFirstDescendant(PropertyCondition)
            );
        }
    }
    public class AutoConditionByControl : AutoCondition
    {
        public AutoConditionByControl(PropertyCondition propertyCondition) : base(propertyCondition)
        {

        }
        public override AutoUI Transform(AutoUI autoUI)
        {
            return new AutoUIControl(
                autoUI.Core,
                autoUI.AutomationElement?
                    .FindFirstDescendant(PropertyCondition)
            );
        }
    }

    namespace Draw
    {
        public class Draw
        {
            public Rectangle rect { get; set; }
            public Task? task { get; set; }
            public CancellationTokenSource? cts { get; set; }
            public Draw(Rectangle rect)
            {
                this.rect = rect;
            }
            public Draw(AutoUI? autoUI)
            {
                this.rect = autoUI?.AutomationElement?.BoundingRectangle ?? new Rectangle();
            }

            public void Start()
            {
                cts = new CancellationTokenSource();
                task = Task.Factory.StartNew(DrawLoop, cts.Token);
            }
            public void Stop()
            {
                cts?.Cancel();
            }
            public void DrawLoop()
            {
                while (true)
                {
                    //cts?.Token.ThrowIfCancellationRequested();
                    if (cts?.Token.IsCancellationRequested ?? false)
                    {
                        return;
                    }

                    DrawHollowRedRectangle(rect);
                    Thread.Sleep(1000);
                }
            }
            public void DrawHollowRedRectangle(Rectangle rectangle)
            {
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    using (Pen redPen = new Pen(Color.Red, 5))
                    {
                        // 使用 Graphics 對象畫中空的矩形
                        graphics.DrawRectangle(redPen, rectangle);
                    }
                }
            }
        }
    }
}
