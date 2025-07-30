using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AbsentDaysController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public AbsentDaysController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetAbsentDays/{userId}/{type}/{status}/{startDateTime}/{endDateTime}")]
        public IEnumerable<AbsentDay> GetAbsentDays(DateTime startDateTime, DateTime endDateTime, int userId = 0, string type = "None", string status = "None")
        {
            string sql = @"EXEC [WorkFlow].[spAbsentDay_Get]";

            string parameters = string.Empty;
            DynamicParameters sqlParameters = new DynamicParameters();

            parameters += " @StartDate = @StartDateParam";
            sqlParameters.Add("@StartDateParam", startDateTime, DbType.DateTime);

            parameters += ", @EndDate = @EndDateParam";
            sqlParameters.Add("@EndDateParam", endDateTime, DbType.DateTime);
            if (userId > 0)
            {
                parameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }
            if (type != "None")
            {
                parameters += ", @Type = @TypeParam";
                sqlParameters.Add("@TypeParam", type, DbType.String);
            }
            if (type != "Status")
            {
                parameters += ", @Status = @StatusParam";
                sqlParameters.Add("@StatusParam", status, DbType.String);
            }
            sql += parameters;

            IEnumerable<AbsentDay> absentDays = _dapper.LoadData<AbsentDay>(sql, sqlParameters);
            return absentDays;
        }

        [HttpPut("UpsertAbsentDay")]
        public IActionResult UpsertAbsentDay(AbsentDay absentDay)
        {
            string sql = @$"EXEC [WorkFlow].[spAbsentDay_Upsert]
                @AbsentDayId = @AbsentDayIdParam,
                @UserId = @UserIdParam,
                @StartDate = @StartDateParam,
                @EndDate = @EndDateParam,
                @Type = @TypeParam,
                @Status = @StatusParam,
                @Description = @DescriptionParam,
                @RequestedAt = @RequestedAtParam,
                @ApprovedBy = @ApprovedByParam,
                @ApprovedAt = @ApprovedAtParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@AbsentDayIdParam", absentDay.AbsentDayId, DbType.Int32);
            sqlParameters.Add("@UserIdParam", absentDay.UserId, DbType.Int32);
            sqlParameters.Add("@StartDateParam", absentDay.StartDate, DbType.Date);
            sqlParameters.Add("@EndDateParam", absentDay.EndDate, DbType.Date);
            sqlParameters.Add("@TypeParam", absentDay.Type, DbType.String);
            sqlParameters.Add("@StatusParam", absentDay.Status, DbType.String);
            sqlParameters.Add("@DescriptionParam", UtilityHelper.EscapeSingleQuotes(absentDay.Description), DbType.String);
            sqlParameters.Add("@RequestedAtParam", absentDay.RequestedAt, DbType.Time);
            sqlParameters.Add("@ApprovedByParam", absentDay.ApprovedBy, DbType.Int32);
            sqlParameters.Add("@ApprovedAtParam", absentDay.ApprovedAt, DbType.Time);

            _dapper.ExecuteSql(sql, sqlParameters);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to insert or update absentDay, id {absentDay.AbsentDayId}");
        }

        [HttpDelete("DeleteAbsentDay/{absentDayId}")]
        public IActionResult DeleteAbsentDay(int absentDayId)
        {
            string sql = "EXEC [WorkFlow].[spAbsentDay_Delete] @AbsentDayId = @AbsentDayIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@AbsentDayIdParam", absentDayId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete absentDay, id {absentDayId}");
        }
    }
}