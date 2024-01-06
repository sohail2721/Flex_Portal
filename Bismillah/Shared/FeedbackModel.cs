using System;
namespace Bismillah.Shared
{
    public class FeedbackModel
    {
        public string ResponseId { get; set; } = string.Empty;
        public string FACULTYID { get; set; } = string.Empty;
        public string[] QA { get; set; } = new string[19];
        public string comments { get; set; } = string.Empty;
        public string Roll_no { get; set; } = string.Empty;


    }

}

