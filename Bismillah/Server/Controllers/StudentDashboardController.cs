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
    public class StudentDashboardController : Controller
    {
        public IConfiguration configuration;

        public StudentDashboardController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }

        [HttpGet("GetAttendance/{UserName}")]
        public async Task<ActionResult<List<AttendanceModel>>> GetAttendance(string UserName)
        {
            var i = await CreateConnection().QueryAsync<AttendanceModel>($"SELECT Course_ID,[Date],[Status] FROM COURSEREG INNER JOIN ATTENDANCE ON COURSEREG.CourseREGID = ATTENDANCE.CourseREGID WHERE Roll_NO = '{UserName}'");
            return Ok(i);
        }

        [HttpGet("GetStudentMarks/{UserName}")]
        public async Task<ActionResult<List<GetMarksModel>>> GetStudentMarks(string UserName)
        {
            var i = await CreateConnection().QueryAsync<GetMarksModel>($"SELECT EvalType,MaxScore,EvalDATE,SCORE,Course_ID FROM EVALUATIONS INNER JOIN STUDENTEVALUATIONS ON EVALUATIONS.EvaluationID = STUDENTEVALUATIONS.EvaluationID INNER JOIN COURSEALLOCATIONTABLE ON EVALUATIONS.FacultyID = COURSEALLOCATIONTABLE.FacultyID WHERE RollNO = '{UserName}'");
            return Ok(i);
        }
        [HttpGet("GetStudentInfo/{currentUserName}")]
        public async Task<ActionResult<List<StudentInformation>>> GetStudentInfo(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<StudentInformation>($"SELECT * FROM STUDENTS\nWHERE Roll_no = '{currentUserName}'");
            return Ok(i);
        }

        [HttpGet("GetStudentGrades/{UserName}")]
        public async Task<ActionResult<List<GetGradesModel>>> GetStudentGrades(string UserName)
        {
            var i = await CreateConnection().QueryAsync<GetGradesModel>($"SELECT GRADE,Course_Name,COURSEREG.Course_ID FROM GRADES INNER JOIN COURSEREG ON COURSEREG.CourseREGID = GRADES.CourseRegID INNER JOIN COURSES ON COURSES.Course_ID = COURSEREG.Course_ID WHERE Roll_NO = '{UserName}'");
            return Ok(i);
        }
        [HttpGet("GetCourseInfo/{UserName}")]
        public async Task<ActionResult<List<GetCoursesInfoModel>>> GetCourseInfo(string UserName)
        {
            var i = await CreateConnection().QueryAsync<GetCoursesInfoModel>($"SELECT COURSES.Course_ID,Course_Name,FACULTYID FROM COURSEREG INNER JOIN COURSES ON COURSES.Course_ID = COURSEREG.Course_ID WHERE Roll_NO = '{UserName}'");
            return Ok(i);
        }
        [HttpGet("GetFacultyID/{currentUserName}/{SelectedOptionFacultyID}")]
        public async Task<ActionResult<List<string>>> GetFacultyID(string currentUserName, string SelectedOptionFacultyID)
        {

            var i = await CreateConnection().QueryAsync<string>($"SELECT FACULTY.FACULTYID FROM FACULTY\nINNER JOIN COURSEREG ON COURSEREG.FACULTYID = FACULTY.FacultyID\nWHERE Roll_NO = '{currentUserName}' AND Course_ID = '{SelectedOptionFacultyID}'");
            return Ok(i);
        }
        [HttpGet("GetRegisteredCourses/{currentUserName}")]
        public async Task<ActionResult<List<GetOfferedCourses>>> GetRegisteredCourses(string currentUserName)
        {

            var i = await CreateConnection().QueryAsync<GetOfferedCourses>($"SELECT COURSES.Course_ID,Course_Name,Grading_Type,Credit_Hours FROM COURSEREG\nINNER JOIN COURSES ON COURSES.Course_ID = COURSEREG.Course_ID\nWHERE Roll_NO = '{currentUserName}'");
            return Ok(i);
        }
        [HttpGet("GetAvailFaculty/{selectedCourse}")]
        public async Task<ActionResult<List<FacultyDetailsOne>>> GetAvailableFaculty(string selectedCourse)
        {

            var i = await CreateConnection().QueryAsync<FacultyDetailsOne>($"SELECT FACULTY.FacultyID,First_Name,Last_Name,Course_ID FROM COURSEALLOCATIONTABLE\nINNER JOIN FACULTY ON FACULTY.FacultyID = COURSEALLOCATIONTABLE.FacultyID\nWHERE Course_ID = '{selectedCourse}'");
            return Ok(i);
        }
        [HttpGet("GetLastCRID/{currentUserName}")]
        public async Task<ActionResult<List<string>>> GetLastCRID(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<string>($"SELECT TOP 1 COURSEREGID\nFROM COURSEREG\nORDER BY CAST(SUBSTRING(CourseREGID, 2, LEN(COURSEREGID)-1) AS BIGINT) DESC");
            return Ok(i);
        }
        [HttpGet("FeedbackValidation/{UserName}/{FacultyUserName}")]
        public async Task<ActionResult<List<int>>> VerifyExistenceStudent(string UserName,string FacultyUserName)
        {
            var i = await CreateConnection().QueryAsync<int>($"SELECT COUNT(*) AS FEEDBACKCOUNT FROM FeedbackResponses\nWHERE Roll_No = '{UserName}' AND FACULTYID = '{FacultyUserName}'");
            return Ok(i);
        }
        [HttpPost("RegisterCourse")]
        public async Task<SuccessMessageModel> RegisterCourse(CourseRegModel Model)
        {
            await CreateConnection().ExecuteAsync($"\nINSERT INTO COURSEREG VALUES\n('{Model.CourseREGID}','{Model.Roll_No}','{Model.Course_ID}','{Model.FacultyID}');");
            return new SuccessMessageModel { message = "success" };


        }
        [HttpPost("SubmitFeedback")]
        public async Task<SuccessMessageModel> SubmitFeedback(FeedbackModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO FEEDBACKRESPONSES VALUES('{Model.ResponseId}','{Model.FACULTYID}','{Model.QA[0]}','{Model.QA[1]}','{Model.QA[2]}','{Model.QA[3]}','{Model.QA[4]}','{Model.QA[5]}','{Model.QA[6]}','{Model.QA[7]}','{Model.QA[8]}','{Model.QA[9]}','{Model.QA[10]}','{Model.QA[11]}','{Model.QA[12]}','{Model.QA[13]}','{Model.QA[14]}','{Model.QA[15]}','{Model.QA[16]}','{Model.QA[17]}','{Model.QA[18]}','{Model.comments}','{Model.Roll_no}');");
            return new SuccessMessageModel { message = "success" };


        }
    }
}

