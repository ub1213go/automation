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
            return Ok();
        }

        /// <summary>
        /// ���|���q�@
        /// </summary>
        [HttpGet("RunStep1")]
        public ActionResult RunStep1()
        {
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