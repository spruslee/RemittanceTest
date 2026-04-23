using Microsoft.AspNetCore.Mvc;
using RemittanceTest.Models;
using RemittanceTest.Services;

namespace RemittanceTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemittanceController(IRemittanceService service) : ControllerBase
    {
        // TODO: 1. 請透過建構子注入 (Constructor Injection) 引入 IRemittanceService
        private readonly IRemittanceService _service = service;

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            // TODO: 2. 呼叫 Service 執行取消邏輯
            var result = _service.CancelRemittance(id);
            // TODO: 3. 根據 Service 回傳的結果，回傳相對應的 HTTP 狀態碼 (Ok / BadRequest / NotFound)
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            else if (result.Message == "查無此資料")
            {
                return NotFound(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}