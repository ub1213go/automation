using Automation;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutomationController : ControllerBase
    {
        private static Lazy<App> App = new Lazy<App>(()=> new App());
        private App app = App.Value;

        private readonly ILogger<AutomationController> _logger;

        public AutomationController(ILogger<AutomationController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// �Ұʳ��|�{��
        /// </summary>
        [HttpGet("StartRun")]
        public ActionResult StartRun()
        {
            app.Start(@"D:\ETAX\BLR\BLRMENU.exe", @"D:\ETAX\BLR");
            var ac = new ICommand[]
            {
                new FindAndClickCommand(app, "�T�����i",
                    p => p.Name == "����"
                ),
                new KeyBoardCommand(app, VirtualKeyShort.ENTER),
                new FindAndClickCommand(app, "���ɵ{��",
                    p => p.ClassName == "TButton"
                        && p.BoundingRectangle.X == 1096
                ),
                new KeyBoardCommand(app, VirtualKeyShort.ENTER),
                new KeyBoardCommand(app, VirtualKeyShort.ENTER),
            };
            
            foreach(var c in ac)
            {
                c.Call();
            }

            return Ok();
        }

        /// <summary>
        /// ���|���q�@
        /// </summary>
        [HttpGet("RunStep1")]
        public ActionResult RunStep1()
        {
            var ac = new ICommand[]
            {
                new FindCommand(app, "��~�|���u���ɨt��"),
            };
            
            foreach(var c in ac)
            {
                c.Call();
            }

            if(app.CurrentElement != null)
            {
                app.CurrentElement?.SetForeground();
                Mouse.MoveTo(150, 230);
                Mouse.LeftClick();
                Keyboard.Type("82440940");
                Keyboard.Pressing(VirtualKeyShort.ENTER);
            }
            return Ok();
        }

        /// <summary>
        /// �W�Ǹ��
        /// </summary>
        [HttpPost("UploadFile")]
        public ActionResult UploadFile(IFormFile uploadedFile)
        {
            var path = @$"C:\Users\ub\Desktop\AServer\{uploadedFile.FileName}";

            using (var fs = new FileStream(path, FileMode.Create))
            {
                uploadedFile.OpenReadStream().CopyTo(fs);
            }

            return Ok();
        }
    }
}