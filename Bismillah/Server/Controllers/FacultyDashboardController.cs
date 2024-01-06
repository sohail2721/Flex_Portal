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
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Flexmgmt.Shared;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bismillah.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultyDashboardController : Controller
    {
        public IConfiguration configuration;

        public FacultyDashboardController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }
        [HttpGet("GetEvalType")]
        public async Task<ActionResult<List<EvalTypeModel>>> GetEvalType()
        {
            var i = await CreateConnection().QueryAsync<EvalTypeModel>($"SELECT DISTINCT EvalType FROM EVALUATIONS");
            return Ok(i);
        }
        [HttpGet("GetFacultyInfo/{currentUserName}")]
        public async Task<ActionResult<List<FacultyInformation>>> GetFacultyInfo(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<FacultyInformation>($"SELECT * FROM FACULTY\nWHERE FacultyID = '{currentUserName}'");
            return Ok(i);
        }
        [HttpGet("GetCurrentEvaluations/{currentUserName}/{EvalType}")]
        public async Task<ActionResult<List<GetCurrentEvaluationModel>>> GetCurrentEvaluations(string currentUserName, string EvalType)
        {
            var i = await CreateConnection().QueryAsync<GetCurrentEvaluationModel>($" SELECT DISTINCT EvalDATE,Roll_NO,SCORE,MaxScore,EvalType,EValuations.FacultyID,EVALUATIONS.EvaluationID FROM STUDENTEVALUATIONS\nINNER JOIN EVALUATIONS ON EVALUATIONS.EvaluationID = STUDENTEVALUATIONS.EvaluationID\nINNER JOIN COURSEREG ON COURSEREG.Roll_NO = STUDENTEVALUATIONS.RollNO\nWHERE  EValuations.FacultyID='{currentUserName}' and EvalType = '{EvalType}'");
            return Ok(i);
        }
        [HttpGet("GetRegisteredCourses/{currentUserName}")]
        public async Task<ActionResult<List<GetCoursesInfoModel>>> GetRegisteredCourses(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<GetCoursesInfoModel>($"SELECT COURSEALLOCATIONTABLE.Course_ID,Course_Name,FacultyID FROM COURSEALLOCATIONTABLE\nINNER JOIN COURSES ON COURSES.Course_ID = COURSEALLOCATIONTABLE.Course_ID WHERE FacultyID='{currentUserName}'");
            return Ok(i);
        }

        [HttpGet("GetStudentList/{SelectedCourse}/{CurrentUsername}")]
        public async Task<ActionResult<List<GetCoursesInfoModel>>> GetStudentList(string SelectedCourse, string CurrentUsername)
        {
            var i = await CreateConnection().QueryAsync<StudentListForAttendanceModel>($"SELECT DISTINCT COURSEREG.CourseREGID,STUDENTS.Roll_NO,FIRST_NAME,LAST_NAME FROM COURSEREG\nINNER JOIN ATTENDANCE ON COURSEREG.CourseREGID = ATTENDANCE.CourseREGID\nINNER JOIN STUDENTS ON STUDENTS.Roll_no = COURSEREG.Roll_NO\nWHERE FACULTYID = '{CurrentUsername}' AND Course_ID = '{SelectedCourse}'");
            return Ok(i);
        }
        [HttpGet("GetUploadedAttendanceDates/{SelectedCourse}/{CurrentUsername}")]
        public async Task<ActionResult<List<AttendanceDateListModel>>> GetUploadedAttendanceDates(string SelectedCourse, string CurrentUsername)
        {
            var i = await CreateConnection().QueryAsync<AttendanceDateListModel>($"SELECT Distinct [Date] FROM COURSEREG\nINNER JOIN ATTENDANCE ON COURSEREG.CourseREGID = ATTENDANCE.CourseREGID\nINNER JOIN STUDENTS ON STUDENTS.Roll_no = COURSEREG.Roll_NO\nWHERE FACULTYID = '{CurrentUsername}' AND Course_ID = '{SelectedCourse}' ");
            return Ok(i);
        }
        [HttpGet("GetLastAid/{UserName}")]
        public async Task<ActionResult<List<string>>> GetLastAid(string UserName)
        {
            var i = await CreateConnection().QueryAsync<string>($"SELECT TOP 1 AttendanceID FROM ATTENDANCE ORDER BY AttendanceID DESC");
            return Ok(i);
        }
        [HttpGet("GetNumFeedbacks/{UserName}")]
        public async Task<ActionResult<List<string>>> GetNumFeedbacks(string UserName)
        {
            var i = await CreateConnection().QueryAsync<string>($"SELECT Count(*) as NumFeedbacks FROM FeedbackResponses\nWHERE FACULTYID = '{UserName}'");
            return Ok(i);
        }
        [HttpGet("GetLastEid/{UserName}")]
        public async Task<ActionResult<List<string>>> GetLastEid(string UserName)
        {

            var i = await CreateConnection().QueryAsync<string>($"SELECT TOP 1 EvaluationID FROM EVALUATIONS ORDER BY CAST(SUBSTRING(evaluationId, 2, LEN(evaluationId)) AS INT) DESC;");
            return Ok(i);
        }
        [HttpGet("GetLastSEid/{UserName}")]
        public async Task<ActionResult<List<string>>> GetLastSEid(string UserName)
        {

            var i = await CreateConnection().QueryAsync<string>($"SELECT TOP 1 STUDENTEVALID FROM STUDENTEVALUATIONS ORDER BY StudentEvalID DESC;");
            return Ok(i);
        }
        [HttpGet("GetAttendanceForDate/{currentUserName}/{selected}/{selectedDate}")]
        public async Task<ActionResult<List<AttendanceModelForUploaded>>> GetAttendanceForDate(string currentUserName, string selected, string selectedDate)
        {

            var i = await CreateConnection().QueryAsync<AttendanceModelForUploaded>($"SELECT STUDENTS.Roll_NO,FIRST_NAME,LAST_NAME,[Status] FROM COURSEREG INNER JOIN ATTENDANCE ON COURSEREG.CourseREGID = ATTENDANCE.CourseREGID INNER JOIN STUDENTS ON STUDENTS.Roll_no = COURSEREG.Roll_NO WHERE FACULTYID = '{currentUserName}' AND Course_ID = '{selected}' AND [Date] = '{selectedDate}'");
            return Ok(i);
        }
        [HttpGet("GetRegisteredStudents")]
        public async Task<ActionResult<List<LoginModel>>> GetRegisteredStudents()
        {
            var i = await CreateConnection().QueryAsync<LoginModel>($"SELECT DISTINCT Students.Roll_NO,First_Name,Last_Name,Phone,Phone FROM STUDENTS\nINNER JOIN COURSEREG ON STUDENTS.Roll_no = COURSEREG.Roll_NO");
            return Ok(i);
        }
        [HttpGet("GetRecievedFeedbackResponses/{currentUserName}")]
        public async Task<ActionResult<List<FeedbackModelFaculty>>> GetRecievedFeedbackResponses(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<FeedbackModelFaculty>($"SELECT * FROM FeedbackResponses WHERE FACULTYID = '{currentUserName}'");
            return Ok(i);
        }
        [HttpPost("UpdateAttendance")]
        public async Task<SuccessMessageModel> UpdateAttendance(AttendanceModelForUpdate Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE ATTENDANCE\nSET [Status]='{Model.Status}'\nFROM ATTENDANCE\nINNER JOIN COURSEREG ON COURSEREG.CourseREGID = ATTENDANCE.CourseREGID\nINNER JOIN STUDENTS ON STUDENTS.Roll_no = COURSEREG.Roll_NO\nWHERE FACULTYID = '{Model.FacultyID}' AND COURSEREG.Roll_NO = '{Model.Roll_No}' AND [Date] = '{Model.Date}' AND Course_ID = '{Model.CourseID}'");
            return new SuccessMessageModel { message = "success" };

        }
        [HttpPost("AddNewEvaluation")]
        public async Task<SuccessMessageModel> AddNewEvaluation(EvaluationModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO EVALUATIONS VALUES ('{Model.EvaluationID}','{Model.FacultyID}','{Model.EvalType}','{Model.MaxScore}',CONVERT(date, '{Model.EvalDate}'));");
            return new SuccessMessageModel { message = "success" };

        }
        [HttpPost("PostNewStudentEval")]
        public async Task<SuccessMessageModel> PostNewStudentEval(EvaluationModel Model)
        {   //Wrong Model used, Dont get confused with names
            await CreateConnection().ExecuteAsync($"INSERT INTO STUDENTEVALUATIONS VALUES ('{Model.FacultyID}','{Model.EvalType}','{Model.EvaluationID}','{Model.EvalDate}');");
            return new SuccessMessageModel { message = "success" };

        }
        [HttpPost("UpdateEvaluations")]
        public async Task<SuccessMessageModel> UpdateEvaluations(GetCurrentEvaluationModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE STUDENTEVALUATIONS\nSET SCORE = {Model.SCORE}\nFROM STUDENTEVALUATIONS\nINNER JOIN EVALUATIONS ON EVALUATIONS.EvaluationID = STUDENTEVALUATIONS.EvaluationID\nINNER JOIN COURSEREG ON COURSEREG.Roll_NO = STUDENTEVALUATIONS.RollNO\nWHERE  EValuations.FacultyID ='{Model.FacultyID}' AND EvalType = '{Model.EvalType}' AND Roll_NO = '{Model.Roll_No}'");
            return new SuccessMessageModel { message = "success" };

        }
        [HttpPost("SubmitAttendance")]
        public async Task<SuccessMessageModel> SubmitAttendance(AttendanceModelForSubmission Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO ATTENDANCE VALUES ('{Model.AttendanceID}','{Model.CourseREGID}',CONVERT(date,'{Model.Date}'),'{Model.Status}');");
            return new SuccessMessageModel { message = "success" };
        }

        //Reports
        [HttpGet("AttendanceSheetReport/{currentUserName}")]
        public async Task<ActionResult<List<FacultyReportOne>>> AttendanceSheetReport(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<FacultyReportOne>($"\nSELECT COURSEREG.Roll_NO,First_Name,Last_Name,CAST(ROUND(CAST(COUNT(CASE WHEN Status = 'P' THEN 1 END) AS DECIMAL(5,2)) / COUNT(*) * 100, 1) AS VARCHAR(50))  AS Attendance_Percentage\nFROM ATTENDANCE \nINNER JOIN COURSEREG ON COURSEREG.CourseREGID = ATTENDANCE.CourseREGID\nINNER JOIN STUDENTS ON STUDENTS.Roll_no = COURSEREG.Roll_NO\nWHERE COURSEREG.FACULTYID = '{currentUserName}' \nGROUP BY COURSEREG.Roll_NO,First_Name,Last_Name;");
            return Ok(i);
        }

        [HttpGet("EvaluationReport/{currentUserName}/{studentUserName}")]
        public async Task<ActionResult<List<FacultyReportTwo>>> EvaluationReport(string currentUserName, string studentUserName)
        {
            var i = await CreateConnection().QueryAsync<FacultyReportTwo>($"SELECT EvalType,SCORE, MaxScore, SUM(SCORE) OVER() AS TotalScore\nFROM STUDENTEVALUATIONS\nINNER JOIN EVALUATIONS ON EVALUATIONS.EvaluationID = STUDENTEVALUATIONS.EvaluationID\nWHERE FACULTYID = '{currentUserName}' AND RollNO = '{studentUserName}'");
            return Ok(i);
        }
        [HttpGet("GradeReport/{currentUserName}")]
        public async Task<ActionResult<List<FacultyReportThree>>> GradeReport(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<FacultyReportThree>($"SELECT Roll_NO,Course_ID,Grade FROM GRADES\nINNER JOIN COURSEREG ON COURSEREG.CourseREGID = GRADES.CourseREGID\nWHERE FACULTYID = '{currentUserName}'");
            return Ok(i);
        }
        [HttpGet("CountOfGradesReport/{currentUserName}")]
        public async Task<ActionResult<List<FacultyReportFour>>> CountOfGradesReport(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<FacultyReportFour>($"SELECT GRADE, COUNT(*) AS GradeCount\nFROM GRADES\nINNER JOIN COURSEREG ON COURSEREG.CourseREGID = GRADES.CourseREGID\nWHERE FACULTYID = '{currentUserName}'\nGROUP BY GRADE");
            return Ok(i);
        }
        [HttpGet("GetStudentList/{currentUserName}")]
        public async Task<ActionResult<List<GetStudentList>>> GetStudentList(string currentUserName)
        {
            var i = await CreateConnection().QueryAsync<GetStudentList>($"SELECT STUDENTS.Roll_NO,First_Name,Last_Name from STUDENTS\nINNER JOIN COURSEREG ON COURSEREG.Roll_NO = STUDENTS.Roll_no\nWHERE FACULTYID = '{currentUserName}'");
            return Ok(i);
        }
    }
}

