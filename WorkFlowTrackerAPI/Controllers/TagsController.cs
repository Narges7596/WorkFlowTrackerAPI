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
    public class TagsController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public TagsController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetTags/{tagId}")]
        public IEnumerable<Tag> GetTags(int tagId = 0, int clientId = 0)
        {
            string sql = @"EXEC [WorkFlow].[spTag_Get]";

            DynamicParameters sqlParameters = new DynamicParameters();
            if (tagId > 0)
            {
                sql += " @TagId = @TagIdParam";
                sqlParameters.Add("@TagIdParam", tagId, DbType.Int32);
            }

            IEnumerable<Tag> tags = _dapper.LoadData<Tag>(sql, sqlParameters);
            return tags;
        }

        [HttpPut("UpsertTag")]
        public IActionResult UpsertTag(Tag tag)
        {
            string sql = @$"EXEC [WorkFlow].[spTagUpsert]
            @TagName = @TagNameParam,
            @TagId = @TagIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@TagNameParam", UtilityHelper.EscapeSingleQuotes(tag.TagName), DbType.String);
            sqlParameters.Add("@TagIdParam", tag.TagId, DbType.Int32);

            _dapper.ExecuteSql(sql, sqlParameters);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to insert or update tag, id {tag.TagId}");
        }

        [HttpDelete("DeleteTag/{tagId}")]
        public IActionResult DeleteTag(int tagId)
        {
            string sql = "EXEC [WorkFlow].[spTag_Delete] @TagId = @TagIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@TagIdParam", tagId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete tag, id {tagId}");
        }
    }
}