using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSqls _reusableSqls;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSqls = new ReusableSqls(config);
        }

        [AllowAnonymous]
        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()", null);
        }


        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<User> GetUsers(int userId = 0, bool isActive = false)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";

            string parameters = string.Empty;
            DynamicParameters sqlParameters = new DynamicParameters();
            if (userId > 0)
            {
                parameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }
            if (isActive)
            {
                parameters += ", @IsActive = @isActiveParam";
                sqlParameters.Add("@isActiveParam", isActive, DbType.Boolean);
            }
            sql += parameters.Substring(1);

            IEnumerable<User> users = _dapper.LoadData<User>(sql, sqlParameters);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(User user)
        {
            if (_reusableSqls.UpsertUser(user))
                return Ok();
            throw new Exception($"Fail to update user, id {user.UserId}");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = "EXEC TutorialAppSchema.spUsers_Delete @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete user, id {userId}");
        }
    }
}