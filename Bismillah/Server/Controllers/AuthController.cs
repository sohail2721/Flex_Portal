using System.Data.SqlClient;
using Dapper;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Bismillah.Server;
using Microsoft.AspNetCore.Authorization;
using Bismillah.Shared;
using Microsoft.AspNetCore.Mvc;
//using Flexmgmt.Shared;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bismillah.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration configuration;

        public AuthController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }

        private async Task<string> CreateJWT(string Username)
        {
            var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["jwt:Key"]));
            var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
            String Roles;
            if (Char.IsDigit(Username[0]))
            {
                Roles = "student";
            }
            else
            {
                Roles = "faculty";
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Role, Roles),
                new Claim(JwtRegisteredClaimNames.Sub, Username),
                new Claim(JwtRegisteredClaimNames.Jti, Username)
            };

            var token = new JwtSecurityToken(issuer: configuration["jwt:Issuer"], audience: configuration["jwt:Audience"], claims: claims, expires: DateTime.Now.AddMonths(1), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("VerificationStudent/{UserName}")]
        public async Task<ActionResult<List<int>>> VerifyExistenceStudent(string UserName)
        {
            var i = await CreateConnection().QueryAsync<int>($"select count(*) Count from STUDENTS where Roll_no = '{UserName}'");
            return Ok(i);
        }

        [HttpGet("VerificationFaculty/{FacultyUserName}")]
        public async Task<ActionResult<List<int>>> VerifyExistenceFaculty(string FacultyUserName)
        {
            var i = await CreateConnection().QueryAsync<int>($"select count(*) Count from Faculty where FacultyID = '{FacultyUserName}'");
            return Ok(i);
        }
        [HttpGet("VerificationAdmin/{UserName}")]
        public async Task<ActionResult<List<int>>> VerificationAdmin(string UserName)
        {
            var i = await CreateConnection().QueryAsync<int>($"SELECT count(*) FROM ADMIN where AdminID = '{UserName}'");
            return Ok(i);
        }

        [HttpGet("student/{UserName}")]
        public async Task<ActionResult<List<LoginModel>>> VerifyUser(string UserName)
        {

            var i = await CreateConnection().QueryAsync<LoginModel>($"SELECT ROLL_NO,FIRST_NAME,LAST_NAME,FLEXPASSWORD FROM STUDENTS WHERE Roll_no = '{UserName}'");
            i.ToList()[0].jwtbearer = await CreateJWT(UserName);
            return Ok(i);
        }

        [HttpGet("faculty/{FacultyUserName}")]
        public async Task<ActionResult<List<LoginModelFaculty>>> VerifyFacultyUser(string FacultyUserName)
        {
            Console.WriteLine("HELLO");
            var i = await CreateConnection().QueryAsync<LoginModelFaculty>($"SELECT FacultyID,First_Name,Last_Name,FLEXPASSWORD FROM FACULTY WHERE FACULTYID = '{FacultyUserName}'");
            i.ToList()[0].jwtbearer = await CreateJWT(FacultyUserName);
            return Ok(i);
        }
        [HttpGet("Admin/{UserName}")]
        public async Task<ActionResult<List<LoginModelAdmin>>> VerifyAdminUser(string UserName)
        {
            Console.WriteLine("HELLO");
            var i = await CreateConnection().QueryAsync<LoginModelAdmin>($"SELECT * FROM ADMIN WHERE AdminID  = '{UserName}'");
            i.ToList()[0].jwtbearer = await CreateJWT(UserName);
            return Ok(i);
        }

        [HttpPost("ResetStudent")]
        public async Task<SuccessMessageModel> ResetStudent(UpdateModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE STUDENTS\nSET FLEXPASSWORD = '{Model.Program}'\nWHERE Roll_no = '{Model.Roll_No}'");
            return new SuccessMessageModel { message = "success" };
        }
        [HttpPost("ResetFaculty")]
        public async Task<SuccessMessageModel> ResetFaculty(UpdateModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE FACULTY\nSET FLEXPASSWORD = '{Model.Program}'\nWHERE FACULTYID = '{Model.Roll_No}'");
            return new SuccessMessageModel { message = "success" };
        }
        [HttpPost("ResetAdmin")]
        public async Task<SuccessMessageModel> ResetAdmin(UpdateModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE ADMIN\nSET AdminPassword = '{Model.Program}'\nWHERE AdminID = '{Model.Roll_No}'");
            return new SuccessMessageModel { message = "success" };
        }


    }
}

