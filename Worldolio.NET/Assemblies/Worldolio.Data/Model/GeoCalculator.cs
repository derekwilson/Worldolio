using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldolio.Data.Repository;

namespace Worldolio.Data.Model
{
    public class GeoCalculator
    {
        #region Day Night Shadow

        private static double ComputeDeclination(int T, int M, int J, double STD)
        {
            double K = Math.PI / 180.0;

            long N;
            double X;
            double Ekliptik, J2000;

            N = 365 * J + T + 31 * M - 46;
            if (M < 3)
                N = N + (int)((J - 1) / 4);
            else
                N = N - (int)(0.4 * M + 2.3) + (int)(J / 4.0);

            X = (N - 693960) / 1461.0;
            X = (X - (int)X) * 1440.02509 + (int)X * 0.0307572;
            X = X + STD / 24.0 * 0.9856645 + 356.6498973;
            X = X + 1.91233 * Math.Sin(0.9999825 * X * K);
            X = (X + Math.Sin(1.999965 * X * K) / 50.0 + 282.55462) / 360.0;
            X = (X - (int)X) * 360.0;

            J2000 = (J - 2000) / 100.0;
            Ekliptik = 23.43929111 - (46.8150 + (0.00059 - 0.001813 * J2000) * J2000) * J2000 / 3600.0;

            X = Math.Sin(X * K) * Math.Sin(K * Ekliptik);

            return Math.Atan(X / Math.Sqrt(1.0 - X * X)) / K + 0.00075;
        }

        private static double ComputeGHA(int T, int M, int J, double STD)
        {
            double K = Math.PI / 180.0;

            long N;
            double X, XX, P;

            N = 365 * J + T + 31 * M - 46;
            if (M < 3)
                N = N + (int)((J - 1) / 4);
            else
                N = N - (int)(0.4 * M + 2.3) + (int)(J / 4.0);

            P = STD / 24.0;
            X = (P + N - 7.22449E5) * 0.98564734 + 279.306;
            X = X * K;
            XX = -104.55 * Math.Sin(X) - 429.266 * Math.Cos(X) + 595.63 * Math.Sin(2.0 * X) - 2.283 * Math.Cos(2.0 * X);
            XX = XX + 4.6 * Math.Sin(3.0 * X) + 18.7333 * Math.Cos(3.0 * X);
            XX = XX - 13.2 * Math.Sin(4.0 * X) - Math.Cos(5.0 * X) - Math.Sin(5.0 * X) / 3.0 + 0.5 * Math.Sin(6.0 * X) + 0.231;
            XX = XX / 240.0 + 360.0 * (P + 0.5);
            if (XX > 360)
                XX = XX - 360.0;
            return XX;
        }

        private static double ComputeLat(int longitude, double dec)
        {
            double K = Math.PI / 180.0;

            double tan, itan;

            tan = -Math.Cos(longitude * K) / Math.Tan(dec * K);
            itan = Math.Atan(tan);
            return itan / K;

            //			return (int)Math.Round(itan);
        }

        /// <summary>
        /// The number of points that will be generated in the position array for the edge of the shadow
        /// </summary>
        public static int SHADOW_EDGE_POINTS = 360;
        private static Position[] _arrShadowEdge = new Position[SHADOW_EDGE_POINTS];

        /// <summary>
        /// Calculates the edge of the DayNight shadow
        /// </summary>
        /// <param name="utcNow">utc time to calc the shadow for</param>
        /// <param name="bShadowNorth">true if the shadow is north of the line else south</param>
        /// <returns>array of lat/long points for the edge of the shadow</returns>
        public static Position[] CalcDayNightShadowEdge(System.DateTime utcNow, ref bool bShadowNorth)
        {
            int x0 = 180;

            int year = utcNow.Year;
            int month = utcNow.Month;
            int day = utcNow.Day;
            int hours = utcNow.Hour;
            int minutes = utcNow.Minute;
            int seconds = utcNow.Second;

            // compute Declination
            double STD = 1.0 * (hours) + minutes / 60.0 + seconds / 3600.0;
            double dec = ComputeDeclination(day, month, year, STD);

            // compute Greenwich Hour Angle
            double GHA = ComputeGHA(day, month, year, STD);

            // compute equation of time
            double GHA12 = ComputeGHA(day, month, year, 12.0);
            if (GHA12 > 5.0)
                GHA12 = GHA12 - 360.0;
            double equation = GHA12 * 4.0;  // Minuten

            int x = x0 - (int)Math.Round(GHA);
            if (x < 0) x = x + 360;
            if (x > 360) x = x - 360;

            bShadowNorth = (dec < 0);

            int index = 0;
            for (int i = 0; i < 360; i++)
            {
                double yy = ComputeLat(i - x, dec);

                _arrShadowEdge[index++] = new Position(yy, i - 180);
            }
            return _arrShadowEdge;
        }

        #endregion

        #region Sunrise and Sunset

        private static int CalcJulianDay(System.DateTime dt)
        {
            return dt.DayOfYear;
        }

        //	Convert radian angle to degrees
        private static double dRadToDeg(double dAngleRad)
        {
            return (180 * dAngleRad / Math.PI);
        }

        //	Convert degree angle to radians
        private static double dDegToRad(double dAngleDeg)
        {
            return (Math.PI * dAngleDeg / 180);
        }

        private static double CalcGamma(int iJulianDay)
        {
            return (2 * Math.PI / 365) * (iJulianDay - 1);
        }

        private static double CalcGamma2(int iJulianDay, int iHour)
        {
            return (2 * Math.PI / 365) * (iJulianDay - 1 + (iHour / 24));
        }

        private static double CalcEqofTime(double dGamma)
        {
            return (229.18 * (0.000075 + 0.001868 * Math.Cos(dGamma) - 0.032077 * Math.Sin(dGamma) - 0.014615 * Math.Cos(2 * dGamma) - 0.040849 * Math.Sin(2 * dGamma)));
        }

        private static double CalcSolarDec(double dGamma)
        {
            return (0.006918 - 0.399912 * Math.Cos(dGamma) + 0.070257 * Math.Sin(dGamma) - 0.006758 * Math.Cos(2 * dGamma) + 0.000907 * Math.Sin(2 * dGamma));
        }

        private static int CalcDayLength(double dHourAngle)
        {
            //	Return the length of the day in minutes.
            return (int)((2 * Math.Abs(dRadToDeg(dHourAngle))) / 15);
        }

        private static double CalcHourAngle(double latitude, double dSolarDec, bool bSunrise)
        {
            double dLatRad = dDegToRad(latitude);
            if (bSunrise)  //Sunrise
                return (Math.Acos(Math.Cos(dDegToRad(90.833)) / (Math.Cos(dLatRad) * Math.Cos(dSolarDec)) - Math.Tan(dLatRad) * Math.Tan(dSolarDec)));
            return -(Math.Acos(Math.Cos(dDegToRad(90.833)) / (Math.Cos(dLatRad) * Math.Cos(dSolarDec)) - Math.Tan(dLatRad) * Math.Tan(dSolarDec)));
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double calcSunsetGMT(int iJulianDay, double latitude, double longitude)
        {
            // First calculates sunrise and approx length of day
            double dGamma = CalcGamma(iJulianDay + 1);
            double eqTime = CalcEqofTime(dGamma);
            double solarDec = CalcSolarDec(dGamma);
            double hourAngle = CalcHourAngle(latitude, solarDec, false);
            double delta = longitude - dRadToDeg(hourAngle);
            double timeDiff = 4 * delta;
            double setTimeGMT = 720 + timeDiff - eqTime;

            // first pass used to include fractional day in gamma calc
            double gamma_sunset = CalcGamma2(iJulianDay, (int)setTimeGMT / 60);
            eqTime = CalcEqofTime(gamma_sunset);
            solarDec = CalcSolarDec(gamma_sunset);
            hourAngle = CalcHourAngle(latitude, solarDec, false);
            delta = longitude - dRadToDeg(hourAngle);
            timeDiff = 4 * delta;
            setTimeGMT = 720 + timeDiff - eqTime; // in minutes

            return setTimeGMT;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double calcSunriseGMT(int iJulianDay, double latitude, double longitude)
        {
            // *** First pass to approximate sunrise
            double gamma = CalcGamma(iJulianDay);
            double eqTime = CalcEqofTime(gamma);
            double solarDec = CalcSolarDec(gamma);
            double hourAngle = CalcHourAngle(latitude, solarDec, true);
            double delta = longitude - dRadToDeg(hourAngle);
            double timeDiff = 4 * delta;
            double timeGMT = 720 + timeDiff - eqTime;

            // *** Second pass includes fractional jday in gamma calc
            double gamma_sunrise = CalcGamma2(iJulianDay, (int)timeGMT / 60);
            eqTime = CalcEqofTime(gamma_sunrise);
            solarDec = CalcSolarDec(gamma_sunrise);
            hourAngle = CalcHourAngle(latitude, solarDec, true);
            delta = longitude - dRadToDeg(hourAngle);
            timeDiff = 4 * delta;
            timeGMT = 720 + timeDiff - eqTime; // in minutes

            return timeGMT;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double calcSolNoonGMT(System.DateTime today, double longitude)
        {
            // Adds approximate fractional day to julday before calc gamma
            double gamma_solnoon = CalcGamma2(today.DayOfYear, 12 + (int)(longitude / 15));
            double eqTime = CalcEqofTime(gamma_solnoon);
            double solarNoonDec = CalcSolarDec(gamma_solnoon);
            double solNoonGMT = 720 + (longitude * 4) - eqTime; // min
            return solNoonGMT;
        }

        private static bool IsInteger(double dValue)
        {
            int iTemp = (int)dValue;
            double dTemp = dValue - iTemp;
            if (dTemp == 0)
                return true;
            return false;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findRecentSunrise(System.DateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunriseGMT(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday--;
            //				if (jday < 1) 
            //					jday = 365;
            //				dTime = calcSunriseGMT(jday,latitude,longitude);
            //			}
            return dTime;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findRecentSunset(System.DateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunsetGMT(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday--;
            //				if (jday < 1) 
            //					jday = 365;
            //				dTime = calcSunsetGMT(jday,latitude,longitude);
            //			}
            return dTime;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findNextSunrise(System.DateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunriseGMT(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday++;
            //				if (jday > 366) 
            //					jday = 1;
            //				dTime = calcSunriseGMT(jday,latitude,longitude);
            //			}
            return dTime;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findNextSunset(System.DateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunsetGMT(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday++;
            //				if (jday > 366) 
            //					jday = 1;
            //				dTime = calcSunsetGMT(jday,latitude,longitude);
            //			}
            return dTime;
        }

        /*


private static System.DateTime ConvertToLocalTime(System.DateTime today, double timeGMT, Worldolio.Data.TimeZone timeZone)
{
    System.TimeSpan utcOffset = timeZone.GetUtcOffset(today);
    double dUtcOffset = (utcOffset.Hours * 60) + utcOffset.Minutes;     // god help us all if we get TZ offsets in secs
    double timeLocal = timeGMT + dUtcOffset;

    // trap any wrapping of the day eg. Tonga
    if (timeLocal < 0)
        timeLocal += 1440;
    if (timeLocal > 1440)
        timeLocal -= 1440;

    double dHour = timeLocal / 60;
    int iHour = (int)dHour;
    double dMinute = 60 * (dHour - iHour);
    int iMinute = (int)dMinute;
    double dSecond = 60 * (dMinute - iMinute);
    int iSecond = (int)dSecond;

    return new System.DateTime(today.Year, today.Month, today.Day, iHour, iMinute, iSecond);

    //			System.DateTime gmtSunrise = new System.DateTime(today.Year,today.Month,today.Day,iHour,iMinute,iSecond);
    //			return this.m_timeZone.ToLocalTime(gmtSunrise);
}

/// <summary>
/// Get the time of apparent sunrse for the specified place
/// </summary>
/// <param name="today">the day to calc the sunrise for, time is ignored</param>
/// <param name="pos">place to calculate for</param>
/// <param name="timeZone">the timezone that applies for this place</param>
/// <returns>the local time of sunrise</returns>
public static System.DateTime GetSunrise(System.DateTime today, Worldolio.Data.Position pos, Worldolio.Data.TimeZone timeZone)
{
    // the rest of the app uses +ve to mean east this calc uses +ve to mean west
    double longitude = -pos.Longitude;
    double timeGMT = calcSunriseGMT(today.DayOfYear, pos.Latitude, longitude);

    int iJulianDay = today.DayOfYear;
    // if Northern hemisphere and spring or summer, use last sunrise and next sunset
    if ((pos.Latitude > 66.4) && (iJulianDay > 79) && (iJulianDay < 267))
        timeGMT = findRecentSunrise(today, pos.Latitude, longitude);
    // if Northern hemisphere and fall or winter, use next sunrise and last sunset
    else if ((pos.Latitude > 66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
        timeGMT = findNextSunrise(today, pos.Latitude, longitude);
    // if Southern hemisphere and fall or winter, use last sunrise and next sunset
    else if ((pos.Latitude < -66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
        timeGMT = findRecentSunrise(today, pos.Latitude, longitude);
    // if Southern hemisphere and spring or summer, use next sunrise and last sunset
    else if ((pos.Latitude < -66.4) && (iJulianDay > 79) && (iJulianDay < 267))
        timeGMT = findNextSunrise(today, pos.Latitude, longitude);

    return ConvertToLocalTime(today, timeGMT, timeZone);
}

/// <summary>
/// Get the time of apparent sunset for the specified place
/// </summary>
/// <param name="today">the day to calc the sunset for, time is ignored</param>
/// <param name="pos">place to calculate for</param>
/// <param name="timeZone">the timezone that applies for this place</param>
/// <returns>the local time of sunset</returns>
public static System.DateTime GetSunset(System.DateTime today, Worldolio.Data.Position pos, Worldolio.Data.TimeZone timeZone)
{
    // the rest of the app uses +ve to mean east this calc uses +ve to mean west
    double longitude = -pos.Longitude;
    double timeGMT = calcSunsetGMT(today.DayOfYear, pos.Latitude, longitude);

    int iJulianDay = today.DayOfYear;
    // if Northern hemisphere and spring or summer, use last sunrise and next sunset
    if ((pos.Latitude > 66.4) && (iJulianDay > 79) && (iJulianDay < 267))
        timeGMT = findRecentSunset(today, pos.Latitude, longitude);
    // if Northern hemisphere and fall or winter, use next sunrise and last sunset
    else if ((pos.Latitude > 66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
        timeGMT = findNextSunset(today, pos.Latitude, longitude);
    // if Southern hemisphere and fall or winter, use last sunrise and next sunset
    else if ((pos.Latitude < -66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
        timeGMT = findRecentSunset(today, pos.Latitude, longitude);
    // if Southern hemisphere and spring or summer, use next sunrise and last sunset
    else if ((pos.Latitude < -66.4) && (iJulianDay > 79) && (iJulianDay < 267))
        timeGMT = findNextSunset(today, pos.Latitude, longitude);

    return ConvertToLocalTime(today, timeGMT, timeZone);
}

/// <summary>
/// Get the time of solar noon
/// </summary>
/// <param name="today">the day to calc the noon for, time is ignored</param>
/// <param name="longitude">logitude of place, west = -ve, east = +ve</param>
/// <param name="timeZone">the timezone that applies for this place</param>
/// <returns>the local time of noon</returns>
public static System.DateTime GetSolarNoon(System.DateTime today, double longitude, Worldolio.Data.TimeZone timeZone)
{
    // the rest of the app uses +ve to mean east this calc uses +ve to mean west
    longitude = -longitude;

    double timeGMT = calcSolNoonGMT(today, longitude);
    return ConvertToLocalTime(today, timeGMT, timeZone);
}
*/
        #endregion

        #region Distances

        /// <summary>
        /// Calculate the distance between two points on the planet
        /// </summary>
        /// <param name="posA">first positoon</param>
        /// <param name="posB">second position</param>
        /// <returns>distance between the points</returns>
        public static Distance GetDistance(Position posA, Position posB)
        {
            // Math functions use radians
            double latA = dDegToRad(posA.Latitude);
            double lonA = dDegToRad(posA.Longitude);
            double latB = dDegToRad(posB.Latitude);
            double lonB = dDegToRad(posB.Longitude);

            double Dist = Math.Acos(Math.Sin(latA) * Math.Sin(latB) + Math.Cos(latA) * Math.Cos(latB) * Math.Cos(lonB - lonA));
            /* Now apply some magic numbers: (180/pi) is to go from radians to
			degrees, *60 gives nautical miles, 1.852 converts to kilometres.
			*/

            return new Distance(dRadToDeg(Dist) * 60, Distance.Units.NauticalMiles);
        }

        /// <summary>
        /// Get all the cities delimited by the supplied points
        /// </summary>
        /// <param name="repo">city rpository</param>
        /// <param name="topLeft">top left corner of the box</param>
        /// <param name="bottomRight">bottom right corner of the box</param>
        /// <returns>a view onto the collection data</returns>
        public static ICollection<City> GetCitiesInArea(ICityRepository repo, Position topLeft, Position bottomRight)
        {
            var allCities = repo.GetAll();

            return allCities.Where(c => IsInArea(c.Position, topLeft, bottomRight)).ToList();
        }

        private static bool IsInArea(Position position, Position topLeft, Position bottomRight)
        {
            return position.Latitude >= bottomRight.Latitude &&
                    position.Latitude <= topLeft.Latitude &&
                    position.Longitude >= topLeft.Longitude &&
                    position.Longitude <= bottomRight.Longitude;
        }

        /// <summary>
        /// Get all the cities delimited by the supplied points
        /// </summary>
        /// <param name="repo">city rpository</param>
        /// <param name="centre">centre point of the area</param>
        /// <param name="halfHeight">distance to top and bottom edge of the area</param>
        /// <param name="halfWidth">distance to left and right edge of the area</param>
        /// <returns></returns>
        public static ICollection<City> GetCitiesInArea(ICityRepository repo, Position centre, Distance halfHeight, Distance halfWidth)
        {
            Position topLeft = centre.Move(halfHeight, halfWidth.Negate());
            Position bottomRight = centre.Move(halfHeight.Negate(), halfWidth);
            return GetCitiesInArea(repo, topLeft, bottomRight);
        }

        #endregion
    }
}
