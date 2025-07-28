using Dapper;
using DotnetAPI.Data;
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

        public bool UpsertUser(User user)
        {
            string sql = @$"EXEC TutorialAppSchema.spUsers_Upsert
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @Email = @EmailParam,
            @Gender = @GenderParam,
            @JobTitle = @JobTitleParam,
            @Department = @DepartmentParam,
            @Salary = @SalaryParam,
            @Active = @ActiveParam,
            @UserId = @UserIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParam", UtilityHelper.EscapeSingleQuotes(user.FirstName), DbType.String);
            sqlParameters.Add("@LastNameParam", UtilityHelper.EscapeSingleQuotes(user.LastName), DbType.String);
            sqlParameters.Add("@EmailParam", user.Email, DbType.String);
            sqlParameters.Add("@GenderParam", user.Gender, DbType.String);
            sqlParameters.Add("@JobTitleParam", user.JobTitle, DbType.String);
            sqlParameters.Add("@DepartmentParam", user.Department, DbType.String);
            sqlParameters.Add("@SalaryParam", user.Salary, DbType.Decimal);
            sqlParameters.Add("@ActiveParam", user.Active, DbType.Boolean);
            sqlParameters.Add("@UserIdParam", user.UserId, DbType.Int32);


            return _dapper.ExecuteSql(sql, sqlParameters);
        }
    }
}
