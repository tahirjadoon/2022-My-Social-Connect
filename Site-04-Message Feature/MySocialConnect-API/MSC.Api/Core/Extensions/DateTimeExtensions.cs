using System;
using MSC.Api.Core.Constants;

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
        if (dob.Date > today.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// The oldest the person can be
    /// </summary>
    /// <param name="maxAge"></param>
    /// <returns></returns>
    public static DateTime CalculateMinDob(this int maxAge)
    {
        if (maxAge <= 0) maxAge = DataConstants.MaxAge;
        var dob = DateTime.Today.AddYears(-maxAge - 1);
        return dob;
    }

    /// <summary>
    /// The youngest the person can be
    /// </summary>
    /// <param name="minAge"></param>
    /// <returns></returns>
    public static DateTime CalculateMaxDob(this int minAge)
    {
        if (minAge <= 0) minAge = DataConstants.MinAge;
        var dob = DateTime.Today.AddYears(-minAge);
        return dob;
    }
}
