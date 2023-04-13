using Common.DataTransferObjects.AppUser;
using DataAccess.UnitOfWorks.PayamanDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IPayamanDBUnitOfWork _PayamanDBUnitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public UserController(IPayamanDBUnitOfWork PayamanDBUnitOfWork, IAuthenticationService authenticationService)
        {
            _PayamanDBUnitOfWork = PayamanDBUnitOfWork;
            _authenticationService = authenticationService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequestDetail request)
        {
            var response = await _authenticationService.Login(request);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDetail request)
        {
            var response = await _authenticationService.Register(request);

            return Ok(response);
        }
    }

}
