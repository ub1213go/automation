using AConsole.Model;
using AConsole.Model.ConsoleUI;
using AConsole.MyAutofac;
using Automation;
using Automation.Draw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Views
{
    public class EditView : BaseView
    {
        protected AutoUI? autoUI;
        protected Draw? draw;
        public EditView(RoutesEntry routesEntry) : base(routesEntry)
        {
            Route = new ConsoleRoute("Edit", "編輯");
            Route.Hint.SetHint("Q: 離開");
            Route.Hint.SetHint("J: 下一個");
            Route.Hint.SetHint("K: 上一個");

            Route.Hint.SetHint("M: 紅框標記");
            Route.Hint.SetHint("Enter: 下一層");
            Route.Hint.SetHint("ESC: 上一層");
            Route.Hint.SetHint("R: 記錄此動作");
            Route.Hint.SetHint("F: 取得焦點");
            Route.Hint.SetHint("C: 點擊");
            Route.Hint.SetHint("I: 輸入");
            Route.Hint.SetHint("G: 5秒後取得焦點視窗");
            RefreshMenu(Route.Menu, ref autoUI);
            foreach (var d in RoutesEntry.DefaultKeyEvents)
            {
                Route.Subscription(d.Value, d.Key);
            }

            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                if (autoUI == null)
                    autoUI = new AutoUIWindow(Service.Core, Service.Core.AutoCore.FromHandle(IntPtr.Parse(Route.Menu.Current.Item2.Split(",").First().Replace("hwd: ", ""))));
                else
                    autoUI = Service.GetControlByParent(autoUI, Route.Menu.Position);
                RefreshMenu(Route.Menu, ref autoUI);
            }), ConsoleKey.Enter);
            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                if (autoUI != null)
                    autoUI = new AutoUIWindow(Service.Core, autoUI.AutomationElement?.Parent);
                RefreshMenu(Route.Menu, ref autoUI);
            }), ConsoleKey.Escape);
            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                if (draw != null)
                {
                    draw.Stop();
                    draw = null;
                }
                else
                {
                    // 紅框框起光標指到的項目
                    if (autoUI == null)
                    {
                        draw = new Draw(Service.GetWindow(Route.Menu.Current.Item2));
                        draw.Start();
                    }
                    else
                    {
                        draw = new Draw(Service.GetControlByParent(autoUI, Route.Menu.Position));
                        draw.Start();
                    }
                }
            }), ConsoleKey.M);
            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                Route.Menu.Notify(ConsoleKey.Enter);
                try
                {
                    autoUI?.AutomationElement?.Focus();
                }
                catch
                {

                }
            }), ConsoleKey.F);
            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                Route.Menu.Notify(ConsoleKey.Enter);
                try
                {
                    autoUI?.AutomationElement?.Click();
                }
                catch
                {

                }
            }), ConsoleKey.C);
            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                Thread.Sleep(5000);
                IntPtr hwd = AutoService.GetForegroundWindow();
                autoUI = new AutoUIWindow(Service.Core, Service.Core.AutoCore.FromHandle(hwd));
                //int pcsId = 0;
                //AutoService.GetWindowThreadProcessId(hwd, out pcsId);
                RefreshMenu(Route.Menu, ref autoUI);
            }), ConsoleKey.G);
        }

        public void RefreshMenu(ConsoleMenu menu, ref AutoUI? control)
        {
            menu.Clear();
            // 檢查是否為桌面，或者第一次讀取
            if(control == null || control.AutomationElement == null || control.AutomationElement.Parent == null)
            {
                control = null;
                foreach (var tupleWindowTitle in Service.GetAllWindowTitle())
                {
                    menu.SetMenu(tupleWindowTitle);
                }
            }
            else
            {
                foreach(var tupleElement in Service.GetControlListByParent(control))
                {
                    menu.SetMenu(tupleElement);
                }
            }
        }

    }
}
