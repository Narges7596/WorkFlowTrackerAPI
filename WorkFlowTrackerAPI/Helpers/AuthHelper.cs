using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthHelper(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusKey = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusKey),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8);

            return passwordHash;
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : ""));
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }

        public bool SetPassword(UserForLoginDto userForLogin)
        {
            string sqlCheckEmailExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = @EmailParam";
            DynamicParameters sqlCheckParameters = new DynamicParameters();
            sqlCheckParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            IEnumerable<string> existingEmail = _dapper.LoadData<string>(sqlCheckEmailExists, sqlCheckParameters);
            if (existingEmail.Count() != 0)
                throw new Exception("User with this email already exists");

            // Generate a secure random salt for the password hashing.
            // 128 bits = 16 bytes (ensures the salt is 16 bytes, which is a secure and standard length for password salting.
            byte[] passwordSalt = new byte[128 / 8];
            // We use "using" with RandomNumberGenerator to ensure it is disposed of correctly and
            // its resources are freed as soon as you are done with it.
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForLogin.Password, passwordSalt);

            // Insert into the database
            string sqlInsertAuth = @"
            EXEC TutorialAppSchema.spRegistrationUpsert
                @Email = @EmailParam,
                @PasswordHash = @PasswordHashParam,
                @PasswordSalt = @PasswordSaltParam";

            // we have parameters to prevent SQL injection attacks.
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);
            sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

            return _dapper.ExecuteSql(sqlInsertAuth, sqlParameters);
        }

        public bool CheckPassword(UserForLoginDto userForLogin)
        {
            // Check if the email exists in the database
            string sqlCheckEmailExists = "EXEC TutorialAppSchema.spLoginConfirmation_Get @Email=@EmailParam";

            // we have parameters to prevent SQL injection attacks.
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlCheckEmailExists, sqlParameters);
            if (userForConfirmation == null)
                return false;

            // Check if the password matches
            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            //if (userForConfirmation.PasswordHash == passwordHash) // This line is incorrect because it compares references, not values.
            // Use SequenceEqual to compare the byte arrays
            return userForConfirmation.PasswordHash.SequenceEqual(passwordHash);
        }
    }
}
