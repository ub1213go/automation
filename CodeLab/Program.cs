using ASqlite;
using Dapper;

namespace Program
{
    public class Program
    {
        static void Main(string[] args)
        {
            var sql = new Sqlite();

            //sql.DropTable(typeof(AutomationFlow));
            //sql.CreateTable(typeof(AutomationFlow));
            //sql.Create(new AutomationFlow()
            //{
            //    FlowName = "自動報稅",
            //    FlowSeries = 1,
            //    FlowContent = "",
            //});

            var items = sql.Read<AutomationFlow>(0, 10);
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
            //var res = sql.Read<int>("SELECT COUNT(*) FROM AutomationFlow");
            //var res = sql.Read<int>("SELECT MAX(FlowSeries) FROM AutomationFlow");
            //var res = sql.Conn.QuerySingle<int>("SELECT MAX(FlowSeries) FROM AutomationFlow WHERE FlowName = @FlowName", new { FlowName = "自動報稅"});

            //Console.WriteLine(res);
        }
    }

    [ST(Name = "AutomationFlow")]
    public class AutomationFlow
    {
        [SC(Name = "ID", Type = SCAttribute.CType.INT, IsPrimaryKey = true, AutoIncrement = true)]
        public long ID { get; set; }
        [SC(Name = "FlowName", Type = SCAttribute.CType.NVARCHAR, IsIndex = true, Len = 100)]
        public string? FlowName { get; set; }
        [SC(Name = "FlowSeries", Type = SCAttribute.CType.INT, IsIndex = true)]
        public long FlowSeries { get; set; }
        [SC(Name = "FlowContent", Type = SCAttribute.CType.NVARCHAR, IsIndex = true, Len = 4000)]
        public string? FlowContent { get; set; }
        //[SC(Name = "CreateDateTime", Type = SCAttribute.CType.DATETIME, Default = "CURRENT_TIMESTAMP")]
        [SC(Name = "CreateDateTime", Type = SCAttribute.CType.DATETIME, Default = "(DATETIME('now','localtime')")]
        public DateTime CreateDateTime { get; set; }
        [SC(Name = "CreateUser", Type = SCAttribute.CType.VARCHAR, Len = 20, Default = "''")]
        public string? CreateUser { get; set; }
        //[SC(Name = "UpdateDateTime", Type = SCAttribute.CType.DATETIME, Default = "CURRENT_TIMESTAMP")]
        [SC(Name = "UpdateDateTime", Type = SCAttribute.CType.DATETIME, Default = "(DATETIME('now','localtime')")]
        public DateTime UpdateDateTime { get; set; }
        [SC(Name = "UpdateUser", Type = SCAttribute.CType.VARCHAR, Len = 20, Default = "''")]
        public string? UpdateUser { get; set; }

        public override string ToString()
        {
            return $"{ID}. FlowName: {FlowName}, FlowContent: {FlowContent} CreateDateTime: {CreateDateTime}, CreateUser: {CreateUser}";
        } 
    }
}






