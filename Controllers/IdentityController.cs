using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JWTCommonLibForDotNetCore.Services;
using JWTCommonLibForDotNetCore.Entities;

namespace JWTCommonLibForDotNetCore.Controllers
{
  [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Identity identityParam)
        {
            var identity = _identityService.Authenticate(identityParam.Username, identityParam.Password);

            if (identity == null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            return Ok(identity);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var identity =  _identityService.GetAll();
            return Ok(identity);
        }
    }
}
