using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Identifiers;
using FlaUI.UIA3;
using FlaUI.UIA3.Identifiers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeLab.AutoEverything.Auto;

namespace CodeLab.AutoEverything
{
    namespace Auto
    {
        public enum PropertyState
        {
            AutomationIdProperty,
            BoundingRectangleProperty,
            CenterPointProperty,
            ClassNameProperty,
            ClickablePointProperty,
            ControllerForProperty,
            ControlTypeProperty,
            IsContentElementProperty,
            IsControlElementProperty,
            IsDataValidForFormProperty,
            IsDialogProperty,
            IsEnabledProperty,
            IsKeyboardFocusableProperty,
            IsOffscreenProperty,
            IsPasswordProperty,
            IsPeripheralProperty,
            IsRequiredForFormProperty,
            ItemStatusProperty,
            ItemTypeProperty,
            LabeledByProperty,
            NameProperty,
            SizeProperty,
            VisualEffectsProperty,
            IsAnnotationPatternAvailableProperty,
            IsDockPatternAvailableProperty,
            IsDragPatternAvailableProperty,
            IsDropTargetPatternAvailableProperty,
            IsExpandCollapsePatternAvailableProperty,
            IsGridItemPatternAvailableProperty,
            IsGridPatternAvailableProperty,
            IsInvokePatternAvailableProperty,
            IsItemContainerPatternAvailableProperty,
            IsLegacyIAccessiblePatternAvailableProperty,
            IsMultipleViewPatternAvailableProperty,
            IsObjectModelPatternAvailableProperty,
            IsRangeValuePatternAvailableProperty,
            IsScrollItemPatternAvailableProperty,
            IsScrollPatternAvailableProperty,
            IsSelectionItemPatternAvailableProperty,
            IsSelectionPatternAvailableProperty,
            IsSelectionPattern2AvailableProperty,
            IsSpreadsheetPatternAvailableProperty,
            IsSpreadsheetItemPatternAvailableProperty,
            IsStylesPatternAvailableProperty,
            IsSynchronizedInputPatternAvailableProperty,
            IsTableItemPatternAvailableProperty,
            IsTablePatternAvailableProperty,
            IsTextChildPatternAvailableProperty,
            IsTextEditPatternAvailableProperty,
            IsTextPatternAvailableProperty,
            IsTextPattern2AvailableProperty,
            IsTogglePatternAvailableProperty,
            IsTransformPatternAvailableProperty,
            IsTransformPattern2AvailableProperty,
            IsValuePatternAvailableProperty,
            IsVirtualizedItemPatternAvailableProperty,
            IsWindowPatternAvailableProperty,
        }

        public class AutoCore : IDisposable
        {
            public static UIA3Automation? UI { get; set; }
            public AutoCore()
            {
                UI = new UIA3Automation();
            }
            public void Dispose()
            {
                UI?.Dispose();
                UI = null;
            }
        }

        public abstract class AutoFinder
        {
            public PropertyId? Property { get; set; }
            public string? Name { get; set; }
            public AutomationElement? Element { get; set; }
            public int TimeOut { get; set; } = 5000;
            public virtual List<AutomationElement>? FindAllDescendants()
            {
                return Element?.FindAllDescendants().ToList();
            }
        }

        public class AutoDeskFinder : AutoFinder
        {
            public PropertyConditionFlags Flag { get; set; }
                = PropertyConditionFlags.None;
            public AutoDeskFinder(PropertyState prop, string name)
            {
                Name = name;
                Property = (PropertyId?)typeof(AutomationObjectIds)
                    ?.GetField(prop.ToString())
                    ?.GetValue(null);
            }

            public AutomationElement? GetElement(PropertyConditionFlags flag)
            {
                var mb = new Menu.MenuBase<Item.MenuItem>();
                var times = 0;
                var mt = new Item.MenuItem($"搜尋循環次數: {times++}", 0);
                mb.Add(mt);
                mb.Show();

                Flag = flag;
                SpinWait.SpinUntil(() =>
                {
                    mt.Title = $"搜尋循環次數: {times++}";
                    mb.ReShow();
                    Element = AutoCore.UI?.GetDesktop().FindFirstChild(
                        new PropertyCondition(
                            Property,
                            Name,
                            Flag
                        )
                    );
                    return Element != null;
                }, TimeOut);

                return Element;
            }

            public override string ToString()
            {
                return Property?.Name + ": \"" + Name + "\" " + Flag + " " + TimeOut / 1000 + "秒";
            }
        }

        public class AutoElementFinder : AutoFinder
        {
            public PropertyConditionFlags Flag { get; set; }
                = PropertyConditionFlags.None;
            public AutoElementFinder(PropertyState prop, string name, AutomationElement ele)
            {
                Element = ele;
                Name = name;
                Property = (PropertyId?)typeof(AutomationObjectIds)
                    ?.GetField(prop.ToString())
                    ?.GetValue(null);
            }
            public AutomationElement? GetElement(PropertyConditionFlags flag)
            {
                var mb = new Menu.MenuBase<Item.MenuItem>();
                var times = 0;
                var mt = new Item.MenuItem($"搜尋循環次數: {times++}", 0);
                mb.Add(mt);
                mb.Show();

                Flag = flag;
                SpinWait.SpinUntil(() =>
                {
                    mt.Title = $"搜尋循環次數: {times++}";
                    mb.ReShow();
                    var tar = Element?.FindFirstDescendant(
                            new PropertyCondition(
                                Property,
                                Name,
                                flag
                            )
                        );

                    if (tar != null)
                    {
                        Element = tar;
                    }

                    return tar != null;
                }, 5000);

                return Element;
            }

            public override string ToString()
            {
                return Property?.Name + ": \"" + Name + "\" " + Flag + " " + TimeOut / 1000 + "秒";
            }
        }

        public class AutoKeyboard
        {
            public AutoKeyboard Next { get; set; }
            public AutoKeyboard()
            {

            }
            public AutoKeyboard Add(AutoKeyboard ak)
            {
                if (Next == null)
                {
                    Next = ak;
                }
                else
                {
                    Next.Add(ak);
                }
                return this;
            }
        }

        public class AutoRun : IDisposable
        {
            public AutoCore? Core { get; set; }
            public AutomationElement? Container { get; set; }
            public AutoRun()
            {
                Core = new AutoCore();
            }

            public void Dispose()
            {
                Core?.Dispose();
                Core = null;
            }

            public AutomationElement[]? Run(string windowTitle)
            {
                var fd = new AutoDeskFinder(
                        PropertyState.NameProperty,
                        windowTitle
                    );

                Console.WriteLine($"開始搜尋 {fd}");
                var ele1 = fd.GetElement(PropertyConditionFlags.MatchSubstring);
                if (ele1 != null)
                {
                    Container = ele1;
                    Console.WriteLine("搜尋成功");
                }
                else
                {
                    Console.WriteLine("搜尋失敗");
                }

                Container?.Focus();

                var alldesc = Container?.FindAllDescendants();

                return alldesc;
            }

            public AutomationElement[]? RunByID(AutomationElement ele, string id)
            {
                var fd = new AutoElementFinder(
                        PropertyState.AutomationIdProperty,
                        id,
                        ele
                    );

                Console.WriteLine($"開始搜尋 {fd}");
                var ele1 = fd.GetElement(PropertyConditionFlags.None);
                if (ele1 != null)
                {
                    Container = ele1;
                    Console.WriteLine("搜尋成功");
                }
                else
                {
                    Console.WriteLine("搜尋失敗");
                }

                Container?.Focus();

                var alldesc = Container?.FindAllDescendants();

                return alldesc;
            }
        }

        public static class AutomationElementExtension
        {
            public static string ToStringImpl(this AutomationElement obj)
            {
                Func<string, int, int> sf = (s, i) =>
                {
                    var len = 0;
                    if (s != null)
                    {
                        foreach (var c in s)
                        {
                            len += char.IsDigit(c) || char.IsAscii(c) ? 1 : 2;
                        }
                    }

                    return i - (int)Math.Floor((double)len / 8);
                };

                var sb = new StringBuilder();
                var s = "";
                sb.Append("Name: ");
                try { s = obj.Name; } catch (Exception) { }
                sb.Append(s);
                sb.Append(new string('\t', sf(s, 3)));
                sb.Append(", ClassName: ");
                s = "";
                try { s = obj.ClassName; } catch (Exception) { }
                sb.Append(s);
                sb.Append(", ControlType: ");
                s = "";
                try { s = obj.ControlType.ToString(); } catch (Exception) { }
                sb.Append(s);
                sb.Append(", Rectangle: ");
                s = "";
                try { s = obj.Properties.BoundingRectangle.ToString(); } catch (Exception) { }
                sb.Append(s);

                return sb.ToString();
            }
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

    namespace Menu
    {
        /// <summary>
        /// 循環執行搜尋
        /// </summary>
        public class MenuLoop : Observer.Subscriber
        {
            public Auto.AutoRun ar { get; set; }   
                = new Auto.AutoRun();

            public MenuSelect ms { get; set; }
                = new MenuSelect();

            public MenuLoop()
            {
                ms.SelectedEvent?.Subscribe(this);
            }

            protected AutomationElement? Root { get; set; }

            public override void Update(Event.EventData e)
            {
                if (e is Event.EventAE ae)
                {
                    if(ae.AutoElement == null)
                    {
                        throw new ArgumentException("AutoElement is empty.");
                    }
                    // 畫出框線
                    //var d = new Draw(ae.AutoElement.BoundingRectangle);
                    //d.Start();

                    RecursionRun(ae.AutoElement);
                }
                else throw new InvalidDataException("Event is not a autoContext type.");
            }

            public void FirstRun(string title)
            {
                //if(title == "")
                //{
                //    throw new ArgumentException("title is empty.");
                //}

                var alldesc = ar.Run(title);
                Root = ar.Container;
                for (int i = 0; i < alldesc?.Length; i++)
                {
                    var e = alldesc[i];
                    if (e != null)
                    {
                        try
                        {
                            ms.Add(new Item.MenuSelectAEItem(
                                    e.ToStringImpl(),
                                    ms.Items.Count,
                                    e
                                )
                            );
                        } catch { }
                    }
                }
                ms.Show();
            }

            public void RecursionRun(AutomationElement ele)
            {
                if (ele == null)
                {
                    throw new ArgumentException("AutomationElement is empty.");
                }

                var alldesc = ele.FindAllDescendants();
                if(alldesc?.Length == 0)
                {
                    alldesc = Root?.FindAllDescendants();
                }
                ms.Clear();
                ms.Items
                    .Where(p=>p.Selected)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.HighLight = false;
                    });
                ms.Items.Clear();

                for (int i = 0; i < alldesc?.Length; i++)
                {
                    var e = alldesc[i];
                    if (e != null)
                    {
                        try
                        {
                            ms.Add(new Item.MenuSelectAEItem(
                                    e.ToStringImpl(),
                                    ms.Items.Count,
                                    e
                                )
                            );
                        } catch { }
                    }
                }
                ms.ReShow();
            }
        }

        public class MenuSelect : MenuBase<Item.MenuSelectItem>
        {
            public Event.EventSelect? SelectedEvent { get; set; }
                = new Event.EventSelect();

            public MenuSelect()
            {
                Console.WindowWidth = 170;
            }

            public override MenuSelect Show()
            {
                if(Items.Count == 0) return this;
                CurrentIndex = 0;
                StartIndex = Console.GetCursorPosition().Top;
                Items[CurrentIndex].Selected = true;
                Items.OrderBy(p => p.Index)
                    .ToList()
                    .ForEach(p =>
                    {
                        Console.WriteLine(p.ToString());
                    });

                while (true)
                {
                    var key = Console.ReadKey(true);

                    if(key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else if(key.Key == ConsoleKey.UpArrow)
                    {
                        if(CurrentIndex - 1 >= 0)
                        {
                            SelectSwitch(Items[CurrentIndex]);
                            CurrentIndex--;
                            SelectSwitch(Items[CurrentIndex]);
                        }
                    }
                    else if(key.Key == ConsoleKey.DownArrow)
                    {
                        if(CurrentIndex + 1 < Items.Count)
                        {
                            SelectSwitch(Items[CurrentIndex]);
                            CurrentIndex++;
                            SelectSwitch(Items[CurrentIndex]);
                        }
                    }
                    else if(key.Key == ConsoleKey.Enter)
                    {
                        // 判斷是否 Enter 於相同的項目
                        // 是的話觸發選擇的事件
                        var tar = Items.FirstOrDefault(p => p.HighLight);
                        if(tar != null && tar.Index == CurrentIndex)
                        {
                            SelectedEvent?.Notify(new Event.EventAE
                            {
                                Message = tar.Title,
                                AutoElement = (AutomationElement?)tar.Source ?? null
                            });
                            break;
                        }

                        foreach (var item in Items.Where(p => p.HighLight))
                        {
                            HighLightSwitch(Items[item.Index]);
                        }
                        HighLightSwitch(Items[CurrentIndex]);
                    }

                    Console.SetCursorPosition(0, StartIndex + Items.Count);
                }

                return this;
            }

            public override MenuSelect Add(Item.MenuSelectItem item)
            {
                Items.Add(item);
                return this;
            }

            protected void SelectSwitch(Item.MenuSelectItem item, bool? turn = null)
            {
                var idx = ConsoleIndex(item.Index);
                item.Selected = turn == null ? !item.Selected : (bool)turn;
                Clear(idx);
                Console.SetCursorPosition(0, idx);
                Console.WriteLine(item.ToString());
            }

            protected void HighLightSwitch(Item.MenuSelectItem item, bool? turn = null)
            {
                var idx = ConsoleIndex(item.Index);
                item.HighLight = turn == null ? !item.HighLight : (bool)turn;
                Clear(idx);
                Console.SetCursorPosition(0, idx);
                Console.WriteLine(item.ToString());
            }
        }

        public class MenuBase<T>
            where T : Item.MenuItem, new()
        {
            public List<T> Items { get; set; }
                = new List<T>();

            public int StartIndex = 0;
            public int CurrentIndex = 0;

            public MenuBase()
            {

            }

            public virtual MenuBase<T> Show()
            {
                if (Items.Count == 0) return this;

                StartIndex = Console.GetCursorPosition().Top;

                Items.OrderBy(p => p.Index)
                    .ToList()
                    .ForEach(p =>
                    {
                        Console.WriteLine(p.ToString());
                    });

                return this;
            }

            public MenuBase<T> Clear(int index = -1)
            {
                if (index == -1)
                {
                    ClearLine(StartIndex, Items.Count);
                    return this;
                }
                var (x, y) = Console.GetCursorPosition();
                Console.SetCursorPosition(0, index);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(x, y);

                return this;
            }

            public MenuBase<T> ClearLine(int startIndex, int number)
            {
                var (x, y) = Console.GetCursorPosition();
                Console.SetCursorPosition(0, startIndex);
                for (int i = 0; i < number; i++)
                {
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                }
                Console.SetCursorPosition(x, y);

                return this;
            }

            public MenuBase<T> ReShow()
            {
                var (x, y) = Console.GetCursorPosition();
                Console.SetCursorPosition(0, StartIndex);
                Show();
                Console.SetCursorPosition(x, y);
                return this;
            }

            public virtual MenuBase<T> Add(T item)
            {
                Items.Add(item);
                return this;
            }

            public Item.MenuItem this[int index]
            {
                get
                {
                    return Items[index];
                }
            }

            protected int ConsoleIndex(int MenuIndex)
            {
                var nIndex = StartIndex;

                return nIndex + MenuIndex;
            }
        }
    }

    namespace Copy
    {
        public interface IDeepCopyable<T>
            where T : new()
        {
            public void CopyTo(T obj);
            public T DeepCopy()
            {
                var t = new T();
                CopyTo(t);
                return t;
            }
        }

        public static class ICopyableExtension
        {
            public static T DeepCopy<T>(this IDeepCopyable<T> obj)
                where T : new()
            {
                return obj.DeepCopy();
            }

            public static T DeepCopy<T>(this T obj)
                where T : MenuItem, new()
            {
                return ((IDeepCopyable<T>)obj).DeepCopy();
            }
        }
    }

    namespace Item
    {
        public class MenuSelectAEItem : MenuSelectItem
        {
            public AutomationElement? ele;

            public Draw.Draw? draw { get; set; }

            public MenuSelectAEItem(string title, int index, object source)
                : base(title, index, source)
            {
                if(source is AutomationElement)
                {
                    ele = (AutomationElement)source;
                }
            }

            public override void Update(Event.EventData e)
            {
                if(ele != null)
                {
                    var val = (ItemState?)GetType()?
                        .GetProperty(e.Message ?? "")?
                        .GetValue(this);

                    if (val.HasValue && val.Value == ItemState.Selected)
                    {
                        if (draw == null)
                        {
                            draw = new Draw.Draw(ele.BoundingRectangle);
                        }
                        draw.Start();
                    }
                    else
                    {
                        draw.Stop();
                    }

                }
            }
        }

        public class MenuSelectItem : MenuItem, Copy.IDeepCopyable<MenuSelectItem>
        {
            public enum ItemState
            {
                Selected,
                NonSelected
            }
                
            public bool Selected { get; set; }

            public object? Source { get; set; }

            public bool HighLight {
                get
                {
                    return _HighLight;
                }
                set
                {
                    _HighLight = value;
                    switch (value)
                    {
                        case true:
                            State = ItemState.Selected;
                            break;
                        case false:
                            State = ItemState.NonSelected;
                            break;
                    }
                } 
            }

            protected bool _HighLight = false;

            protected Event.EventPropertyChange PCE 
                = new Event.EventPropertyChange();

            private ItemState _State = ItemState.NonSelected;
            public ItemState State {
                get
                {
                    return _State;
                }
                protected set
                {
                    _State = value;
                    PCE.Changed(nameof(State));
                }
            }

            public MenuSelectItem() 
            {
                PCE.Subscribe(this);
            }

            public MenuSelectItem(MenuItem item, object source) 
            {
                item.CopyTo(this);
                Source = source;
                PCE.Subscribe(this);
            }

            public MenuSelectItem(string title, int index, object source)
                : base(title, index)
            {
                Source = source;
                PCE.Subscribe(this);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                if (Selected)
                    sb.Append("> ");
                else
                    sb.Append("  ");

                switch (State)
                {
                    case ItemState.Selected:
                        sb.Append($"{Title} << 已選擇");
                        break;
                    case ItemState.NonSelected:
                        sb.Append($"{Title}");
                        break;
                }
                
                return sb.ToString();
            }

            public void CopyTo(MenuSelectItem obj)
            {
                base.CopyTo(obj);
                obj.Selected = Selected;
                obj.HighLight = HighLight;
                obj.Source = Source;
            }
        }

        public class MenuItem : Observer.Subscriber, Copy.IDeepCopyable<MenuItem>
        {
            public string? Title { get; set; }
            public int Index {  get; set; }

            public MenuItem()
            {

            }

            public MenuItem(string title, int index)
            {
                Title = title;
                Index = index;
            }

            public override string ToString()
            {
                return $"{Title}";
            }

            public void CopyTo(MenuItem obj)
            {
                obj.Title = Title;
                obj.Index = Index;
            }

            public override void Update(Event.EventData e)
            {
            }
        }
    }

    namespace Event
    {
        public class EventPropertyChange : Observer.Subscribable
        {
            public void Changed(string name)
            {
                Notify(new EventData
                {
                    Message = name
                });
            }
        }

        public class EventSelect : Observer.Subscribable { }

        public class EventAE : EventData
        {
            public AutomationElement? AutoElement { get; set; }
        }

        public class EventData
        {
            public string? Message { get; set; }
        }
    }

    namespace Observer
    {
        public class Subscribable : Observer.IObservable<Event.EventData>
        {
            private HashSet<EventSubscriber> Subscribers 
                = new HashSet<EventSubscriber>();

            public void Notify(Event.EventData e)
            {
                foreach(var sub in Subscribers)
                {
                    sub.Subscriber.Update(e);
                }
            }

            public void Subscribe(Observer.IObserver<Event.EventData> observer)
            {
                Subscribers.Add(
                    new EventSubscriber(this, observer)
                );
            }
            
            private class EventSubscriber
            {
                public Subscribable Notify { get; set; }
                public Observer.IObserver<Event.EventData> Subscriber { get; set; }
                public EventSubscriber(Subscribable notify, Observer.IObserver<Event.EventData> observer)
                {
                    Notify = notify;
                    Subscriber = observer;  
                }
            }
        }

        public abstract class Subscriber : IObserver<Event.EventData>
        {
            public abstract void Update(Event.EventData e);
        }

        public interface IObservable<T>
            where T : Event.EventData, new()
        {
            void Subscribe(IObserver<T> observer);  

            void Notify(T e);
        }

        public interface IObserver<T>
            where T : Event.EventData, new()
        {
            void Update(T e);
        }
    }
}

