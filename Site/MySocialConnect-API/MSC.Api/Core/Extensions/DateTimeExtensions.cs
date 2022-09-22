using System;

namespace MSC.Api.Core.Extensions;
public static class DateTimeExtensions
{
    public static int CalculateAge(this DateTime dob)
    {
        //todays date
        var today = DateTime.Now;

        //calculate the age
        var age = today.Year - dob.Year;

        //go back to the year in which the person was in case of a leap year
        if(dob.Date > today.AddYears(-age))
            age--;
            
        return age;
    }
}   
