using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
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
    public class WorkLogsController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public WorkLogsController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetWorkLogs/{workLogId}")]
        public IEnumerable<WorkLogGetDto> GetWorkLogs(DateTime startDateTime, DateTime endDateTime, int userId = 0, int projectId = 0, string descSearch = "None")
        {
            string sql = @"EXEC [WorkFlow].[spWorkLog_Get]";

            string parameters = string.Empty;
            DynamicParameters sqlParameters = new DynamicParameters();

            parameters += " @StartDateTime = @StartDateTimeParam";
            sqlParameters.Add("@StartDateTimeParam", startDateTime, DbType.DateTime);

            parameters += ", @EndDateTime = @EndDateTimeParam";
            sqlParameters.Add("@EndDateTimeParam", endDateTime, DbType.DateTime);
            if (userId > 0)
            {
                parameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }
            if (projectId > 0)
            {
                parameters += ", @ProjectId = @ProjectIdParam";
                sqlParameters.Add("@ProjectIdParam", projectId, DbType.Int32);
            }
            if (descSearch != "None")
            {
                parameters += ", @DescSearch = @DescSearchParam";
                sqlParameters.Add("@DescSearchParam", descSearch, DbType.String);
            }
            sql += parameters;

            IEnumerable<WorkLogGetDto> workLogs = _dapper.LoadData<WorkLogGetDto>(sql, sqlParameters);

            // Getting tags for each work log
            foreach (var workLog in workLogs)
            {
                string tagSql = @"EXEC [WorkFlow].[spWorkLogTags_Get] @WorkLogId = @WorkLogIdParam";
                DynamicParameters tagParameters = new DynamicParameters();
                tagParameters.Add("@WorkLogIdParam", workLog.WorkLogId, DbType.Int32);
                IEnumerable<Tag> tags = _dapper.LoadData<Tag>(tagSql, tagParameters);
                workLog.Tags = tags.ToList<Tag>();
            }

            return workLogs;
        }

        [HttpPut("UpsertWorkLog")]
        public IActionResult UpsertWorkLog(WorkLog workLog)
        {
            string sql = @$"EXEC [WorkFlow].[spWorkLog_Upsert]
            @WorkLogId = @WorkLogIdParam,
            @UserId = @UserIdParam,
            @WorkDay = @WorkDayParam,
            @StartTime = @StartTimeParam,
            @EndTime = @EndTimeParam,
            @ProjectID = @ProjectIDParam,
            @Description = @DescriptionParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@WorkLogIdParam", workLog.WorkLogId, DbType.Int32);
            sqlParameters.Add("@UserIdParam", workLog.UserId, DbType.Int32);
            sqlParameters.Add("@WorkDayParam", workLog.WorkDay, DbType.Date);
            sqlParameters.Add("@StartTimeParam", workLog.StartTime, DbType.Time);
            sqlParameters.Add("@EndTimeParam", workLog.EndTime, DbType.Time);
            sqlParameters.Add("@ProjectIDParam", workLog.ProjectID, DbType.Int32);
            sqlParameters.Add("@DescriptionParam", UtilityHelper.EscapeSingleQuotes(workLog.Description), DbType.String);

            _dapper.ExecuteSql(sql, sqlParameters);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to insert or update workLog, id {workLog.WorkLogId}");
        }

        [HttpDelete("DeleteWorkLog/{workLogId}")]
        public IActionResult DeleteWorkLog(int workLogId)
        {
            string sql = "EXEC [WorkFlow].[spWorkLog_Delete] @WorkLogId = @WorkLogIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@WorkLogIdParam", workLogId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete workLog, id {workLogId}");
        }
    }
}