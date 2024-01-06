using System;
namespace Bismillah.Shared
{
    public class LoginModel
    {
        public string Roll_no { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string FLEXPASSWORD { get; set; }  = string.Empty;
        public string jwtbearer { get; set; }  = string.Empty;
    }
}

