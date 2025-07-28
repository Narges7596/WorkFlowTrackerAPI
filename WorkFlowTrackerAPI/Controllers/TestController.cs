using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public TestController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Connection")]
        public DateTime Connection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()", null);
        }

        [HttpGet]
        public string Test()
        {
            return "The application is up and running.";
        }
    }
}