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
    public class ProjectsController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public ProjectsController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetProjects/{projectId}/{clientId}")]
        public IEnumerable<Project> GetProjects(int projectId = 0, int clientId = 0)
        {
            string sql = @"EXEC [WorkFlow].[spProject_Get]";

            string parameters = string.Empty;
            DynamicParameters sqlParameters = new DynamicParameters();
            if (projectId > 0)
            {
                parameters += ", @ProjectId = @ProjectIdParam";
                sqlParameters.Add("@ProjectIdParam", projectId, DbType.Int32);
            }
            if (clientId > 0)
            {
                parameters += ", @ClientId = @ClientIdParam";
                sqlParameters.Add("@ClientIdParam", clientId, DbType.Int32);
            }
            if (!string.IsNullOrEmpty(parameters))
                sql += parameters.Substring(1);

            IEnumerable<Project> projects = _dapper.LoadData<Project>(sql, sqlParameters);
            return projects;
        }

        [HttpPut("UpsertProject")]
        public IActionResult UpsertProject(Project project)
        {
            string sql = @$"EXEC [WorkFlow].[spProjectUpsert]
            @ProjectName = @ProjectNameParam,
            @ClientId = @ClientIdParam,
            @ProjectId = @ProjectIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ProjectNameParam", UtilityHelper.EscapeSingleQuotes(project.ProjectName), DbType.String);
            sqlParameters.Add("@ClientIdParam", project.ClientId, DbType.Int32);
            sqlParameters.Add("@ProjectIdParam", project.ProjectId, DbType.Int32);

            _dapper.ExecuteSql(sql, sqlParameters);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to insert or update project, id {project.ProjectId}");
        }

        [HttpDelete("DeleteProject/{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            string sql = "EXEC [WorkFlow].[spProject_Delete] @ProjectId = @ProjectIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ProjectIdParam", projectId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete project, id {projectId}");
        }
    }
}