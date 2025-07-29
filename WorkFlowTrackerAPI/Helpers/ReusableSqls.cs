using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using System.Data;

namespace DotnetAPI.Helpers
{
    public class ReusableSqls
    {
        private readonly DataContextDapper _dapper;
        public ReusableSqls(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(UserCompleteDto user)
        {
            string sql = @$"EXEC [WorkFlow].spUser_Upsert
            @Email = @EmailParam,
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @Gender = @GenderParam,
            @Department = @DepartmentParam,
            @Role = @RoleParam,
            @Team = @TeamParam,
            @DateJoined = @DateJoinedParam,
            @Salary = @SalaryParam,
            @DaysPerWeek = @DaysPerWeekParam,
            @HoursPerDay = @HoursPerDayParam,
            @UserId = @UserIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@EmailParam", user.Email, DbType.String);
            sqlParameters.Add("@FirstNameParam", UtilityHelper.EscapeSingleQuotes(user.FirstName), DbType.String);
            sqlParameters.Add("@LastNameParam", UtilityHelper.EscapeSingleQuotes(user.LastName), DbType.String);
            sqlParameters.Add("@GenderParam", user.Gender, DbType.String);

            sqlParameters.Add("@DepartmentParam", user.Department, DbType.String);
            sqlParameters.Add("@RoleParam", user.Role, DbType.String);
            sqlParameters.Add("@TeamParam", user.Team, DbType.String);
            sqlParameters.Add("@DateJoinedParam", user.DateJoined, DbType.DateTime);

            sqlParameters.Add("@SalaryParam", user.Salary, DbType.Decimal);
            sqlParameters.Add("@DaysPerWeekParam", user.DaysPerWeek, DbType.Int32);
            sqlParameters.Add("@HoursPerDayParam", user.HoursPerDay, DbType.Int32);

            sqlParameters.Add("@UserIdParam", user.UserId, DbType.Int32);

            return _dapper.ExecuteSql(sql, sqlParameters);
        }
    }
}
