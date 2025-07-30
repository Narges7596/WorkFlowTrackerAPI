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
    public class ClientsController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public ClientsController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetClients/{clientId}")]
        public IEnumerable<Client> GetClients(int clientId = 0)
        {
            string sql = @"EXEC [WorkFlow].[spClient_Get]";

            DynamicParameters sqlParameters = new DynamicParameters();
            if (clientId > 0)
            {
                sql += " @ClientId = @ClientIdParam";
                sqlParameters.Add("@ClientIdParam", clientId, DbType.Int32);
            }

            IEnumerable<Client> clients = _dapper.LoadData<Client>(sql, sqlParameters);
            return clients;
        }

        [HttpPut("UpsertClient")]
        public IActionResult UpsertClient(Client client)
        {
            string sql = @$"EXEC [WorkFlow].[spClientUpsert]
            @ClientName = @ClientNameParam,
            @ClientId = @ClientIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ClientNameParam", UtilityHelper.EscapeSingleQuotes(client.ClientName), DbType.String);
            sqlParameters.Add("@ClientIdParam", client.ClientId, DbType.Int32);

            _dapper.ExecuteSql(sql, sqlParameters);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to insert or update client, id {client.ClientId}");
        }

        [HttpDelete("DeleteClient/{clientId}")]
        public IActionResult DeleteClient(int clientId)
        {
            string sql = "EXEC [WorkFlow].[spClient_Delete] @ClientId = @ClientIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ClientIdParam", clientId, DbType.Int32);

            if (_dapper.ExecuteSql(sql, sqlParameters))
                return Ok();
            throw new Exception($"Fail to delete client, id {clientId}");
        }
    }
}