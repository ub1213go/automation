using Automation;
using Automation.Draw;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Runtime.InteropServices;
using static Automation.AutoService;

namespace Program
{
    public class Program
    {
        static void Main(string[] args)
        {
            var autoService = new AutoService();
            var ja = JsonConvert.DeserializeObject<JArray>(new AutoJson().Test1());
            var table = new Dictionary<string, AutoUI>();
            if (ja == null) return;
            foreach(JObject j in ja)
            {
                var aj = j.ToObject<AutoJson>();

                if (aj != null)
                {
                    AutoUI? ele = null;
                    PropertyCondition? pc = null;
                    if (!String.IsNullOrEmpty(aj.Parent))
                    {
                        var parameters = autoService.Core.AutoCore
                            .ConditionFactory?
                            .GetType()?
                            .GetMethod("By" + aj.From)?
                            .GetParameters();
                        var providerArgs = new object[]
                        {
                            aj?.Name ?? ""
                        };
                        if ((parameters?.Length ?? 0) == 0) throw new Exception($"{aj.From} 此方法無參數，與預期不同");

                        object[] props = new object[parameters.Length];
                        for (int i = 0; i < props.Length; i++)
                        {
                            if (i < providerArgs.Length)
                            {
                                props[i] = providerArgs[i];
                            }
                            else if (parameters[i].HasDefaultValue)
                            {
                                props[i] = parameters[i].DefaultValue;
                            }
                            else
                            {
                                throw new ArgumentException("not enough arguments provided");
                            }
                        }
                        pc = (PropertyCondition?)autoService.Core.AutoCore
                            .ConditionFactory?
                            .GetType()?
                            .GetMethod("By" + aj.From)?
                            .Invoke(
                                autoService.Core.AutoCore.ConditionFactory,
                                new object[] {aj?.Name ?? ""}
                            );
                        if (pc == null) throw new DataException("PropertyCondition 是空的");  
                        switch (aj?.EnumType)
                        {
                            case AutoJson.EType.Window:
                                ele = autoService.GetWindowByParent(table[aj?.Name ?? ""], pc);
                                break;
                            case AutoJson.EType.Control:
                                ele = autoService.GetControlByParent(table[aj?.Name ?? ""], pc);
                                break;
                        }
                    }
                    else
                    {
                        ele = autoService.GetWindow(aj?.Name ?? "");
                    }

                    if(ele != null)
                    {
                        table.TryAdd(aj?.ID ?? "", ele);

                        switch (aj?.EnumAction)
                        {
                            case AutoJson.EAction.Click:
                                ele?.AutomationElement?.AsButton().Click();
                                break;
                            case AutoJson.EAction.Focus:
                                ele?.AutomationElement?.Focus();
                                break;
                        }
                    }
                }
            }

            Console.ReadKey();
        }
    }

    public class AutoJson
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
                if(!Enum.TryParse(Type, out EType enum_type))
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
        ""Name"": ""Notepad++"",
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
}





//AutomationElement? auto = null;
//SpinWait.SpinUntil(() =>
//{
//    auto = Auto.UI?.GetDesktop().FindFirstChild(
//            new PropertyCondition(
//                AutomationObjectIds.NameProperty,
//                "營業稅離線建檔系統",
//                PropertyConditionFlags.MatchSubstring
//            )
//        );

//    return auto != null;
//}, 5000);

//auto.SetForeground();
//Mouse.Position = new System.Drawing.Point(150, 230);
////Mouse.MoveTo(150, 230);
//Mouse.LeftClick();
//Keyboard.Type("82440940");
//Keyboard.Pressing(VirtualKeyShort.ENTER);

//SpinWait.SpinUntil(() =>
//{
//    var tmp = auto?.FindAllDescendants(
//        new PropertyCondition(
//            AutomationObjectIds.ClassNameProperty,
//            "TCDBMaskEdit",
//            PropertyConditionFlags.None
//        )
//    )
//    .OrderBy(p=>p.Properties.BoundingRectangle.Value.Y)
//    .FirstOrDefault();

//    if(tmp != null)
//    {
//        tmp.Focus();
//    }
    
//    return tmp != null;
//}, 5000);

//Keyboard.Type("11209");
//Keyboard.Pressing(VirtualKeyShort.ENTER);
//Thread.SpinWait(10000000);
//Keyboard.Pressing(VirtualKeyShort.ENTER);
//Thread.SpinWait(10000000);
//Keyboard.Pressing(VirtualKeyShort.DOWN);
//Thread.SpinWait(10000000);
//Keyboard.Pressing(VirtualKeyShort.ENTER);
//Thread.SpinWait(10000000);
//Keyboard.Pressing(VirtualKeyShort.ENTER);
//Thread.SpinWait(10000000);
//Keyboard.Pressing(VirtualKeyShort.DOWN);
//Thread.SpinWait(10000000);
//Keyboard.Pressing(VirtualKeyShort.ENTER);
//var e = new Excel(@"C:\Users\ub\Desktop\112年發票記錄.xlsx");
//var firstMonth = 0;
//for (int i = 0; i < e.dt.Rows.Count; i++)
////foreach (DataRow r in e.dt.Rows)
//{
//    var row = e.dt.Rows[i];
//    if (String.IsNullOrEmpty(row["發票號碼"].ToString()))
//    {
//        break;
//    }
//    if (firstMonth == 0)
//    {
//        firstMonth = Convert.ToInt32(row["月"].ToString());
//        SpinWait.SpinUntil(() =>
//        {
//            var tmp = auto?.FindAllDescendants(
//                new PropertyCondition(
//                    AutomationObjectIds.ClassNameProperty,
//                    "TCDBEdit",
//                    PropertyConditionFlags.None
//                )
//            )
//            .OrderBy(p=>p.Properties.BoundingRectangle.Value.Y)
//            .FirstOrDefault();

//            if(tmp != null)
//            {
//                tmp.Focus();
//                tmp.AsTextBox().Text = row["發票號碼"].ToString()?.Substring(2);
//                Thread.SpinWait(10000000);
//                Keyboard.Pressing(VirtualKeyShort.ENTER);
//                Thread.SpinWait(10000000);
//                Keyboard.Pressing(VirtualKeyShort.ENTER);
//            }

//            return tmp != null;
//        }, 5000);
//    }
//    if (row["買受人"].ToString() == "作廢")
//    {
//        Thread.SpinWait(30000000);
//        Keyboard.Pressing(VirtualKeyShort.ENTER);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.ENTER);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.DOWN);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.DOWN);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.DOWN);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.DOWN);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.DOWN);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.ENTER);
//        Thread.SpinWait(10000000);
//        Keyboard.Pressing(VirtualKeyShort.F2);
//        Thread.SpinWait(20000000);
//        continue;
//    }
//    if (firstMonth != Convert.ToInt32(row["月"].ToString()))
//    {

//        SpinWait.SpinUntil(() =>
//        {
//            var tmp = auto?.FindAllDescendants(
//                new PropertyCondition(
//                    AutomationObjectIds.ClassNameProperty,
//                    "TCDBMaskEdit",
//                    PropertyConditionFlags.None
//                )
//            )
//            .OrderBy(p=>p.Properties.BoundingRectangle.Value.Y)
//            .FirstOrDefault();

//            if(tmp != null)
//            {
//                tmp.AsTextBox().Text = $"112年{row["月"].ToString()?.PadLeft(2, '0')}月";

//                Thread.SpinWait(10000000);
//            }

//            return tmp != null;
//        }, 5000);

//        SpinWait.SpinUntil(() =>
//        {
//            var tmp = auto?.FindAllDescendants(
//                new PropertyCondition(
//                    AutomationObjectIds.ClassNameProperty,
//                    "TCDBEdit",
//                    PropertyConditionFlags.None
//                )
//            )
//            .OrderBy(p=>p.Properties.BoundingRectangle.Value.Y)
//            .FirstOrDefault();

//            if (tmp != null)
//            {
//                tmp.Focus();
//                Thread.SpinWait(50000000);
//                Keyboard.Pressing(VirtualKeyShort.ENTER);
//                Thread.SpinWait(10000000);
//                Keyboard.Pressing(VirtualKeyShort.ENTER);
//            }

//            return tmp != null;
//        }, 5000);
//    }
//    Keyboard.Type(row["統一編號"].ToString());
//    Thread.SpinWait(50000000);
//    Keyboard.Pressing(VirtualKeyShort.ENTER);
//    Thread.SpinWait(10000000);
//    Keyboard.Pressing(VirtualKeyShort.ENTER);
//    Thread.SpinWait(10000000);
//    Keyboard.Pressing(VirtualKeyShort.DOWN);
//    Thread.SpinWait(10000000);
//    Keyboard.Pressing(VirtualKeyShort.ENTER);
//    Keyboard.Type(row["銷售額"].ToString());
//    Keyboard.Pressing(VirtualKeyShort.ENTER);
//    Keyboard.Type(row["5%"].ToString());
//    Keyboard.Pressing(VirtualKeyShort.F2);
//    Thread.SpinWait(20000000);

//}

