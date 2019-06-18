using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JWTCommonLibForDotNetCore.Services;
using JWTCommonLibForDotNetCore.Entities;
using JWTCommonLibForDotNetCore.Controllers.DataMember;

namespace JWTCommonLibForDotNetCore.Controllers
{
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
        public IActionResult Authenticate([FromBody]AuthenticateRequest identityParam)
        {
            var identity = _identityService.Authenticate(identityParam.Username, identityParam.Password);

            if (identity == null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            return Ok(new AuthenticateResponse(identity));
        }

        [HttpPost("revoke")]
        [Authorize]
        public IActionResult Revoke()
        {
            var accesToken = Request.Headers["Authorization"];
            _identityService.RevokeToken(accesToken);
            return Ok();
        }

    }
}
