using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSqls _reusableSqls;

        public UsersController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSqls = new ReusableSqls(config);
        }

        [HttpGet("GetUsers/{userId}/{department}/{team}")]
        public IEnumerable<UserCompleteDto> GetUsers(int userId = 0, string department = "None", string team = "None")
        {
            string sql = @"EXEC WorkFlow.spUser_Get";

            string parameters = string.Empty;
            DynamicParameters sqlParameters = new DynamicParameters();
            if (userId > 0)
            {
                parameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }
            if (department != "None")
            {
                parameters += ", @Department = @DepartmentParam";
                sqlParameters.Add("@DepartmentParam", department, DbType.String);
            }
            if (team != "None")
            {
                parameters += ", @Team = @TeamParam";
                sqlParameters.Add("@TeamParam", team, DbType.String);
            }
            if(!string.IsNullOrEmpty(parameters))
                sql += parameters.Substring(1);

            IEnumerable<UserCompleteDto> users = _dapper.LoadData<UserCompleteDto>(sql, sqlParameters);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserCompleteDto user)
        {
            if (_reusableSqls.UpsertUser(user))
                return Ok();
            throw new Exception($"Fail to insert or update user, id {user.UserId}");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = "EXEC WorkFlow.spUser_Delete @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete user, id {userId}");
        }
    }
}