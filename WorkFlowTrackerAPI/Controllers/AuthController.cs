using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.DTOs;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly ReusableSqls _reusableSqls;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _reusableSqls = new ReusableSqls(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserForRegisterationDto, User>();
            }));
        }

        [AllowAnonymous] // This allows unauthenticated users to access this endpoint
        [HttpPost("Register")]
        public IActionResult Register(UserForRegisterationDto userForRegisteration)
        {
            if (string.IsNullOrEmpty(userForRegisteration.Email) ||
                string.IsNullOrEmpty(userForRegisteration.Password) ||
                string.IsNullOrEmpty(userForRegisteration.PasswordConfirm))
                throw new Exception("Email, Password, and PasswordConfirm cannot be empty");

            if (userForRegisteration.Password != userForRegisteration.PasswordConfirm)
                throw new Exception("Passwords do not match");

            UserForLoginDto userForLogin = new UserForLoginDto
            {
                Email = userForRegisteration.Email,
                Password = userForRegisteration.Password
            };

            if (!_authHelper.SetPassword(userForLogin))
                throw new Exception("Fail to register user");

            // If the Auth table insert was successful, insert into the Users table
            User userComplete = _mapper.Map<User>(userForRegisteration);
            userComplete.Active = true; // Set the user as active by default
            if (_reusableSqls.UpsertUser(userComplete))
                return Ok("Register successful");
            else
                throw new Exception("Fail to add user");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            if (string.IsNullOrEmpty(userForLogin.Email) || string.IsNullOrEmpty(userForLogin.Password))
                throw new Exception("Email and Password cannot empty");

            if (!_authHelper.CheckPassword(userForLogin))
                return StatusCode(401, "Email or Password is incorrect!");

            // If the password is correct, get the UserId and create a token
            string sqlGetUserId = $"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = @EmailParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);
            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId, sqlParameters);

            return Ok(new Dictionary<string, string>
                {
                    {"token", _authHelper.CreateToken(userId) }
                });
            // We can check the generated token in https://jwt.io/            
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForResetPasswordDto userForResetPassword)
        {
            if (string.IsNullOrEmpty(userForResetPassword.OldPassword) ||
                string.IsNullOrEmpty(userForResetPassword.NewPassword) ||
                string.IsNullOrEmpty(userForResetPassword.NewPasswordConfirm))
                throw new Exception("OldPassword, NewPassword, and NewPasswordConfirm cannot be empty");

            if (userForResetPassword.NewPassword != userForResetPassword.NewPasswordConfirm)
                throw new Exception("Passwords do not match");

            // Check if the OldPassword is correct
            UserForLoginDto userForLogin = new UserForLoginDto
            {
                Email = userForResetPassword.Email,
                Password = userForResetPassword.OldPassword
            };
            if (!_authHelper.CheckPassword(userForLogin))
                return StatusCode(401, "Email or Password is incorrect!");

            UserForLoginDto newUserForLogin = new UserForLoginDto
            {
                Email = userForResetPassword.Email,
                Password = userForResetPassword.NewPassword
            };

            if (_authHelper.SetPassword(newUserForLogin))
                return Ok("Register successful");
            throw new Exception("Fail to register user");

        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            // Get the userId from the claims
            string userId = User.FindFirst("userId")?.Value + "";
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid user");

            // Check if the user exists in the database
            string sqlCheckUserExists = $"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

            int userIdInt = _dapper.LoadDataSingle<int>(sqlCheckUserExists, sqlParameters);
            if (userIdInt == 0)
                return Unauthorized("Invalid user");

            // Create a new token
            string newToken = _authHelper.CreateToken(userIdInt);
            return Ok(new Dictionary<string, string>
            {
                {"token", newToken }
            });
        }
    }
}
