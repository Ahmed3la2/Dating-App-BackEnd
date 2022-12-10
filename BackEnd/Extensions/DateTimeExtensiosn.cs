using System;

namespace BackEnd.Extensions
{
    public static class DateTimeExtensiosn
    {
        // Method To Get The Age Of User
        public static int CalcuateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
