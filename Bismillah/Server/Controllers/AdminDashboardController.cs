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


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bismillah.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : Controller
    {
        public IConfiguration configuration;

        public AdminDashboardController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }

        [HttpGet("GetOfferedCourses")]
        public async Task<ActionResult<List<GetOfferedCourses>>> GetOfferedCourses()
        {
            var i = await CreateConnection().QueryAsync<GetOfferedCourses>($"SELECT * FROM COURSES");
            return Ok(i);
        }
        [HttpGet("AddNewCourseValidation/{selectedCourse}")]
        public async Task<ActionResult<List<int>>> AddNewCourseValidation(string selectedCourse)
        {
            var i = await CreateConnection().QueryAsync<int>($"Select count(*) from COURSES where Course_ID = '{selectedCourse}'");
            return Ok(i);
        }
        [HttpGet("NewAllocationValidation/{selectedFaculty}/{selectedCourse}")]
        public async Task<ActionResult<List<int>>> NewAllocationValidation(string selectedFaculty, string selectedCourse)
        {
            var i = await CreateConnection().QueryAsync<int>($"SELECT COUNT(*) AS ALREADYREG \nFROM COURSEALLOCATIONTABLE\nWHERE FacultyID='{selectedFaculty}' AND Course_ID = '{selectedCourse}'");
            return Ok(i);
        }
        //^
        //|
        [HttpGet("NoMoreThanThreeValidation/{selectedFaculty}")]
        public async Task<ActionResult<List<int>>> NoMoreThanThreeValidation(string selectedFaculty)
        {
            var i = await CreateConnection().QueryAsync<int>($"\nSELECT COUNT(*) AS ALREADYREG \nFROM COURSEALLOCATIONTABLE\nWHERE FacultyID='{selectedFaculty}' ");
            return Ok(i);
        }
        [HttpGet("GetRegisteredStudentsForAdmin/{selectedCourse}")]
        public async Task<ActionResult<List<ManageStudentsAdminModel>>> GetRegisteredStudentsForAdmin(string selectedCourse)
        {
            var i = await CreateConnection().QueryAsync<ManageStudentsAdminModel>($"SELECT STUDENTS.Roll_no,First_Name,Last_Name,Program,SECTION FROM COURSEREG\nINNER JOIN STUDENTS ON COURSEREG.Roll_NO = STUDENTS.Roll_no\nWHERE Course_ID = '{selectedCourse}'");
            return Ok(i);
        }
        [HttpGet("GetFacultyDetails")]
        public async Task<ActionResult<List<FacultyDetails>>> GetFacultyDetails()
        {
            var i = await CreateConnection().QueryAsync<FacultyDetails>($"SELECT DISTINCT FACULTY.FacultyID,First_Name,Last_Name,Course_ID FROM COURSEALLOCATIONTABLE\nINNER JOIN FACULTY ON FACULTY.FacultyID = COURSEALLOCATIONTABLE.FacultyID");
            return Ok(i);
        }
        [HttpGet("GetCourseRegStatus")]
        public async Task<ActionResult<List<CourseRegStatus>>> GetCourseRegStatus()
        {
            var i = await CreateConnection().QueryAsync<CourseRegStatus>($"SELECT DISTINCT RegStatus FROM ADMIN");
            return Ok(i);
        }
        [HttpPost("UpdateCourseReg")]
        public async Task<SuccessMessageModel> UpdateCourseReg(SuccessMessageModel Model)
        {
            await CreateConnection().ExecuteAsync($"\nUPDATE ADMIN\nSET RegStatus = '{Model.message}'");
            return new SuccessMessageModel { message = "success" };


        }
        [HttpPost("AddNewCourse")]
        public async Task<SuccessMessageModel> AddNewCourse(GetOfferedCourses Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO COURSES VALUES ('{Model.Course_ID}','{Model.Course_Name}','{Model.Grading_Type}','{Model.Credit_Hours}');");
            return new SuccessMessageModel { message = "success" };


        }
        [HttpPost("AddNewAllocation")]
        public async Task<SuccessMessageModel> AddNewAllocation(FacultyDetails Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO COURSEALLOCATIONTABLE VALUES ('{Model.First_name}','{Model.Course_ID}','{Model.FacultyID}');");
            return new SuccessMessageModel { message = "success" };


        }
        [HttpPost("UpdateSection")]
        public async Task<SuccessMessageModel> UpdateSection(UpdateModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE STUDENTS SET Section = '{Model.Program}' WHERE Roll_no = '{Model.Roll_No}'");
            return new SuccessMessageModel { message = "success" };


        }
        [HttpPost("UpdateProgram")]
        public async Task<SuccessMessageModel> UpdateProgram(UpdateModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE STUDENTS SET Program = '{Model.Program}' WHERE Roll_no = '{Model.Roll_No}'");
            return new SuccessMessageModel { message = "success" };
        }

        //Admin Reports Queries
        [HttpGet("ReportTwo/{selectedCourse}")]
        public async Task<ActionResult<List<AdminReportTwoModel>>> ReportTwo(string selectedCourse)
        {
            var i = await CreateConnection().QueryAsync<AdminReportTwoModel>($"SELECT DISTINCT STUDENTS.Roll_NO,First_Name,Last_Name,Course_ID FROM COURSEREG\nINNER JOIN STUDENTS ON COURSEREG.Roll_NO = STUDENTS.Roll_no\nWHERE Course_ID = '{selectedCourse}'");
            return Ok(i);
        }
    }


}
