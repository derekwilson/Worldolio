using NodaTime;
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
        public static Position[] CalcDayNightShadowEdge(ZonedDateTime utcNow, ref bool bShadowNorth)
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

        private static int CalcJulianDay(ZonedDateTime dt)
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
        private static double calcSunsetUTC(int iJulianDay, double latitude, double longitude)
        {
            // First calculates sunrise and approx length of day
            double dGamma = CalcGamma(iJulianDay + 1);
            double eqTime = CalcEqofTime(dGamma);
            double solarDec = CalcSolarDec(dGamma);
            double hourAngle = CalcHourAngle(latitude, solarDec, false);
            double delta = longitude - dRadToDeg(hourAngle);
            double timeDiff = 4 * delta;
            double setTimeUTC = 720 + timeDiff - eqTime;

            // first pass used to include fractional day in gamma calc
            double gamma_sunset = CalcGamma2(iJulianDay, (int)setTimeUTC / 60);
            eqTime = CalcEqofTime(gamma_sunset);
            solarDec = CalcSolarDec(gamma_sunset);
            hourAngle = CalcHourAngle(latitude, solarDec, false);
            delta = longitude - dRadToDeg(hourAngle);
            timeDiff = 4 * delta;
            setTimeUTC = 720 + timeDiff - eqTime; // in minutes

            return setTimeUTC;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double calcSunriseUTC(int iJulianDay, double latitude, double longitude)
        {
            // *** First pass to approximate sunrise
            double gamma = CalcGamma(iJulianDay);
            double eqTime = CalcEqofTime(gamma);
            double solarDec = CalcSolarDec(gamma);
            double hourAngle = CalcHourAngle(latitude, solarDec, true);
            double delta = longitude - dRadToDeg(hourAngle);
            double timeDiff = 4 * delta;
            double timeUTC = 720 + timeDiff - eqTime;

            // *** Second pass includes fractional jday in gamma calc
            double gamma_sunrise = CalcGamma2(iJulianDay, (int)timeUTC / 60);
            eqTime = CalcEqofTime(gamma_sunrise);
            solarDec = CalcSolarDec(gamma_sunrise);
            hourAngle = CalcHourAngle(latitude, solarDec, true);
            delta = longitude - dRadToDeg(hourAngle);
            timeDiff = 4 * delta;
            timeUTC = 720 + timeDiff - eqTime; // in minutes

            return timeUTC;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double calcSolNoonUTC(int iJulianDay, double longitude)
        {
            // Adds approximate fractional day to julday before calc gamma
            double gamma_solnoon = CalcGamma2(iJulianDay, 12 + (int)(longitude / 15));
            double eqTime = CalcEqofTime(gamma_solnoon);
            double solarNoonDec = CalcSolarDec(gamma_solnoon);
            double solNoonUTC = 720 + (longitude * 4) - eqTime; // min
            return solNoonUTC;
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
        private static double findRecentSunrise(ZonedDateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunriseUTC(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday--;
            //				if (jday < 1) 
            //					jday = 365;
            //				dTime = calcSunriseUTC(jday,latitude,longitude);
            //			}
            return dTime;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findRecentSunset(ZonedDateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunsetUTC(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday--;
            //				if (jday < 1) 
            //					jday = 365;
            //				dTime = calcSunsetUTC(jday,latitude,longitude);
            //			}
            return dTime;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findNextSunrise(ZonedDateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunriseUTC(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday++;
            //				if (jday > 366) 
            //					jday = 1;
            //				dTime = calcSunriseUTC(jday,latitude,longitude);
            //			}
            return dTime;
        }

        // please note that the logitude for this calc is east = -ve, west =+ve
        private static double findNextSunset(ZonedDateTime today, double latitude, double longitude)
        {
            int jday = today.DayOfYear;
            double dTime = calcSunsetUTC(jday, latitude, longitude);

            //			while(!IsInteger(dTime) )
            //			{
            //				jday++;
            //				if (jday > 366) 
            //					jday = 1;
            //				dTime = calcSunsetUTC(jday,latitude,longitude);
            //			}
            return dTime;
        }

        private static ZonedDateTime ConvertUTCMinutesToZonedDateTime(ZonedDateTime today, double timeUTC)
        {
            // trap any wrapping of the day eg. Tonga
            if (timeUTC < 0)
                timeUTC += 1440;
            if (timeUTC > 1440)
                timeUTC -= 1440;

            double dHour = timeUTC / 60;
            int iHour = (int)dHour;
            double dMinute = 60 * (dHour - iHour);
            int iMinute = (int)dMinute;
            double dSecond = 60 * (dMinute - iMinute);
            int iSecond = (int)dSecond;

            LocalDateTime local = new LocalDateTime(today.Year, today.Month, today.Day, iHour, iMinute, iSecond);
            // no duplicate times in UTC so strictly will work
            return local.InZoneStrictly(DateTimeZone.Utc);
        }

        /// <summary>
        /// Get the time of apparent sunrse for the specified place
        /// </summary>
        /// <param name="today">the day to calc the sunrise for, time is ignored</param>
        /// <param name="pos">place to calculate for</param>
        /// <returns>the local time of sunrise</returns>
        public static ZonedDateTime GetSunriseInUtc(ZonedDateTime today, Position pos)
        {
            // the rest of the app uses +ve to mean east this calc uses +ve to mean west
            double longitude = -pos.Longitude;
            double timeUTC = calcSunriseUTC(today.DayOfYear, pos.Latitude, longitude);

            int iJulianDay = today.DayOfYear;
            // if Northern hemisphere and spring or summer, use last sunrise and next sunset
            if ((pos.Latitude > 66.4) && (iJulianDay > 79) && (iJulianDay < 267))
                timeUTC = findRecentSunrise(today, pos.Latitude, longitude);
            // if Northern hemisphere and fall or winter, use next sunrise and last sunset
            else if ((pos.Latitude > 66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
                timeUTC = findNextSunrise(today, pos.Latitude, longitude);
            // if Southern hemisphere and fall or winter, use last sunrise and next sunset
            else if ((pos.Latitude < -66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
                timeUTC = findRecentSunrise(today, pos.Latitude, longitude);
            // if Southern hemisphere and spring or summer, use next sunrise and last sunset
            else if ((pos.Latitude < -66.4) && (iJulianDay > 79) && (iJulianDay < 267))
                timeUTC = findNextSunrise(today, pos.Latitude, longitude);

            return ConvertUTCMinutesToZonedDateTime(today, timeUTC);
        }

        /// <summary>
        /// Get the time of apparent sunset for the specified place
        /// </summary>
        /// <param name="today">the day to calc the sunset for, time is ignored</param>
        /// <param name="pos">place to calculate for</param>
        /// <param name="timeZone">the timezone that applies for this place</param>
        /// <returns>the local time of sunset</returns>
        public static ZonedDateTime GetSunsetInUtc(ZonedDateTime today, Position pos)
        {
            // the rest of the app uses +ve to mean east this calc uses +ve to mean west
            double longitude = -pos.Longitude;
            double timeUTC = calcSunsetUTC(today.DayOfYear, pos.Latitude, longitude);

            int iJulianDay = today.DayOfYear;
            // if Northern hemisphere and spring or summer, use last sunrise and next sunset
            if ((pos.Latitude > 66.4) && (iJulianDay > 79) && (iJulianDay < 267))
                timeUTC = findRecentSunset(today, pos.Latitude, longitude);
            // if Northern hemisphere and fall or winter, use next sunrise and last sunset
            else if ((pos.Latitude > 66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
                timeUTC = findNextSunset(today, pos.Latitude, longitude);
            // if Southern hemisphere and fall or winter, use last sunrise and next sunset
            else if ((pos.Latitude < -66.4) && ((iJulianDay < 83) || (iJulianDay > 263)))
                timeUTC = findRecentSunset(today, pos.Latitude, longitude);
            // if Southern hemisphere and spring or summer, use next sunrise and last sunset
            else if ((pos.Latitude < -66.4) && (iJulianDay > 79) && (iJulianDay < 267))
                timeUTC = findNextSunset(today, pos.Latitude, longitude);

            return ConvertUTCMinutesToZonedDateTime(today, timeUTC);
        }

        /// <summary>
        /// Get the time of solar noon
        /// </summary>
        /// <param name="today">the day to calc the noon for, time is ignored</param>
        /// <param name="longitude">logitude of place, west = -ve, east = +ve</param>
        /// <param name="timeZone">the timezone that applies for this place</param>
        /// <returns>the local time of noon</returns>
        public static ZonedDateTime GetSolarNoonInUtc(ZonedDateTime today, double longitude)
        {
            // the rest of the app uses +ve to mean east this calc uses +ve to mean west
            longitude = -longitude;

            int jday = today.DayOfYear;
            double timeUTC = calcSolNoonUTC(jday, longitude);
            return ConvertUTCMinutesToZonedDateTime(today, timeUTC);
        }

        #endregion

        #region Moon

        public static string GetFormattedMoonPhase(int year, int month, int day)
        {
            var phase = GetMoonPhase(year, month, day);
            if (phase < 0 || phase >= MoonPhanseNames.Length)
            {
                return "Unknown";
            }
            return MoonPhanseNames[phase];
        }

        // this is "Trig2" from here
        // https://fcds.cs.put.poznan.pl/MyWeb/Praca/Ubiquitous/LunarPhases.pdf

        public static int GetMoonPhase(int year, int month, int day)
        {
            var n = Math.Floor(12.37 * (year - 1900 + ((1.0 * month - 0.5) / 12.0)));
            var RAD = 3.14159265 / 180.0;
            var t = n / 1236.85;
            var t2 = t * t;
            var as1 = 359.2242 + 29.105356 * n;
            var am = 306.0253 + 385.816918 * n + 0.010730 * t2;
            var xtra = 0.75933 + 1.53058868 * n + ((1.178e-4) - (1.55e-7) * t) * t2;
            xtra += (0.1734 - 3.93e-4 * t) * Math.Sin(RAD * as1) - 0.4068 * Math.Sin(RAD * am);
            var i = (xtra > 0.0 ? Math.Floor(xtra) : Math.Ceiling(xtra - 1.0));
            var j1 = julday(year, month, day);
            var jd = (2415020 + 28 * n) + i;
            var doubleRetval = (j1 - jd + 30) % 30;
            return (int) doubleRetval;
        }

        private static double julday(int year, int month, int day) {
            if (year < 0) { year++; }
            var jy = year;
            var jm = month + 1;
            if (month <= 2) { jy--; jm += 12; }
            var jul = Math.Floor(365.25 * jy) + Math.Floor(30.6001 * jm) + day + 1720995;
            if (day + 31 * (month + 12 * year) >= (15 + 31 * (10 + 12 * 1582)))
            {
                var ja = Math.Floor(0.01 * jy);
                jul = jul + 2 - ja + Math.Floor(0.25 * ja);
            }
            return jul;
        }

        static string[] MoonPhanseNames = [
            "New Moon",     // 0
            "Waxing Cresent",     // 1
            "Waxing Cresent",     // 2
            "Waxing Cresent",     // 3
            "Waxing Cresent",     // 4
            "Waxing Cresent",     // 5
            "Waxing Cresent",     // 6
            "First Quater",     // 7
            "Waxing Gibbous",     // 8
            "Waxing Gibbous",     // 9
            "Waxing Gibbous",     // 10
            "Waxing Gibbous",     // 11
            "Waxing Gibbous",     // 12
            "Waxing Gibbous",     // 13
            "Waxing Gibbous",     // 14
            "Full Moon",     // 15
            "Waning Gibbous",     // 16
            "Waning Gibbous",     // 17
            "Waning Gibbous",     // 18
            "Waning Gibbous",     // 19
            "Waning Gibbous",     // 20
            "Waning Gibbous",     // 21
            "Waning Gibbous",     // 22
            "Waning Gibbous",     // 23
            "Third Quater",     // 24
            "Waning Cresent",     // 25
            "Waning Cresent",     // 26
            "Waning Cresent",     // 27
            "Waning Cresent",     // 28
            "Waning Cresent",     // 29
            ];

        public static Tuple<ZonedDateTime?, ZonedDateTime?, bool, bool> GetMoonRiseAndSetInUtc(ZonedDateTime today, Position pos)
        {
            // Decimal degrees west longitudes must be negative
            double longitude = pos.Longitude;
            // Decimal degrees south negative
            double latitude = pos.Latitude;

            //
            // main loop. All the work is done in the functions with the long names
            // find_sun_and_twi_events_for_date() and find_moonrise_set()
            //
            var mj = mjd(today.Day, today.Month, today.Year, 0);
            return find_moonrise_set_utc(today, mj, 0, longitude, latitude);
        }

        private static double mjd(int day, int month, int year, int hour)
        {
            //
            //	Takes the day, month, year and hours in the day and returns the
            //  modified julian day number defined as mjd = jd - 2400000.5
            //  checked OK for Greg era dates - 26th Dec 02
            //
            double b;
            if (month <= 2)
            {
                month = month + 12;
                year = year - 1;
            }
            var a = 10000.0 * year + 100.0 * month + day;
            if (a <= 15821004.1)
            {
                b = (-2 * Math.Floor((year + 4716D) / 4)) - 1179;
            }
            else
            {
                b = Math.Floor(year / 400D) - Math.Floor(year / 100D) + Math.Floor(year / 4D);
            }
            a = 365.0 * year - 679004.0;
            return (a + b + Math.Floor(30.6001 * (month + 1)) + day + hour / 24.0);
        }

        private static double frac(double x)
        {
            //
            //	returns the fractional part of x as used in minimoon and minisun
            //
            var a = x - Math.Floor(x);
            if (a < 0) a += 1;
            return a;
        }

        private static int ipart(double x)
        {
            //
            //	returns the integer part - like int() in basic
            //
            int a;
            if (x > 0)
            {
                a = (int) Math.Floor(x);
            }
            else
            {
                a = (int) Math.Ceiling(x);
            }
            return a;
        }

        private static Tuple<double,double> minimoon(double t)
        {
            //
            // takes t and returns the geocentric ra and dec in an array mooneq
            // claimed good to 5' (angle) in ra and 1' in dec
            // tallies with another approximate method and with ICE for a couple of dates
            //
            var p2 = 6.283185307;
            var arc = 206264.8062;
            var coseps = 0.91748;
            var sineps = 0.39778;

            var L0 = frac(0.606433 + 1336.855225 * t);  // mean longitude of moon
            var L = p2 * frac(0.374897 + 1325.552410 * t); //mean anomaly of Moon

            var LS = p2 * frac(0.993133 + 99.997361 * t); //mean anomaly of Sun
            var D = p2 * frac(0.827361 + 1236.853086 * t); //difference in longitude of moon and sun
            var F = p2 * frac(0.259086 + 1342.227825 * t); //mean argument of latitude

            // corrections to mean longitude in arcsec
            var DL = 22640 * Math.Sin(L);

            DL += -4586 * Math.Sin(L - 2 * D);
            DL += +2370 * Math.Sin(2 * D);
            DL += +769 * Math.Sin(2 * L);
            DL += -668 * Math.Sin(LS);
            DL += -412 * Math.Sin(2 * F);
            DL += -212 * Math.Sin(2 * L - 2 * D);
            DL += -206 * Math.Sin(L + LS - 2 * D);
            DL += +192 * Math.Sin(L + 2 * D);
            DL += -165 * Math.Sin(LS - 2 * D);
            DL += -125 * Math.Sin(D);
            DL += -110 * Math.Sin(L + LS);
            DL += +148 * Math.Sin(L - LS);
            DL += -55 * Math.Sin(2 * F - 2 * D);

            // simplified form of the latitude terms
            var S = F + (DL + 412 * Math.Sin(2 * F) + 541 * Math.Sin(LS)) / arc;
            var H = F - 2 * D;
            var N = -526 * Math.Sin(H);
            N += +44 * Math.Sin(L + H);
            N += -31 * Math.Sin(-L + H);
            N += -23 * Math.Sin(LS + H);
            N += +11 * Math.Sin(-LS + H);
            N += -25 * Math.Sin(-2 * L + F);
            N += +21 * Math.Sin(-L + F);

            // ecliptic long and lat of Moon in rads
            var L_moon = p2 * frac(L0 + DL / 1296000);
            var B_moon = (18520.0 * Math.Sin(S) + N) / arc;

            // equatorial coord conversion - note fixed obliquity
            var CB = Math.Cos(B_moon);
            var X = CB * Math.Cos(L_moon);
            var V = CB * Math.Sin(L_moon);
            var W = Math.Sin(B_moon);
            var Y = coseps * V - sineps * W;
            var Z = sineps * V + coseps * W;

            var RHO = Math.Sqrt(1.0 - Z * Z);
            var dec = (360.0 / p2) * Math.Atan(Z / RHO);
            var ra = (48.0 / p2) * Math.Atan(Y / (X + RHO));
            if (ra < 0) ra += 24;

            return Tuple.Create(dec, ra);
        }

        private static double range(double x)
        {
            //
            //	returns an angle in degrees in the range 0 to 360
            //
            var b = x / 360;
            var a = 360 * (b - ipart(b));
            if (a < 0)
            {
                a = a + 360;
            }
            return a;
        }

        private static double lmst(double mjd, double glong)
        {
            //
            //	Takes the mjd and the longitude (west negative) and then returns
            //  the local sidereal time in hours. Im using Meeus formula 11.4
            //  instead of messing about with UTo and so on
            //
            var d = mjd - 51544.5;

            var t = d / 36525.0;
            var lst = range(280.46061837 + 360.98564736629 * d + 0.000387933 * t * t - t * t * t / 38710000);
            return (lst / 15.0 + glong / 15);
        }


        private static double sin_alt(int iobj, double mjd0, double hour, double glong, double cglat, double sglat)
        {
            //
            //	this rather mickey mouse function takes a lot of
            //  arguments and then returns the sine of the altitude of
            //  the object labelled by iobj. iobj = 1 is moon, iobj = 2 is sun
            //
            var rads = 0.0174532925;
            Tuple<double, double> objpos;
            var mjd = mjd0 + hour / 24.0;
            var t = (mjd - 51544.5) / 36525.0;
            if (iobj == 1)
            {
                objpos = minimoon(t);
            }
            else
            {
                //objpos = minisun(t);
                objpos = Tuple.Create(0D,0D);
            }
            var dec = objpos.Item1;
            var ra = objpos.Item2;
            // hour angle of object
            var tau = 15.0 * (lmst(mjd, glong) - ra);
            // sin(alt) of object using the conversion formulas
            var salt = sglat * Math.Sin(rads * dec) + cglat * Math.Cos(rads * dec) * Math.Cos(rads * tau);
            return salt;
        }

        private static double[] quad(double ym, double yz, double yp)
        {
            //
            //	finds the parabola throuh the three points (-1,ym), (0,yz), (1, yp)
            //  and returns the coordinates of the max/min (if any) xe, ye
            //  the values of x where the parabola crosses zero (roots of the quadratic)
            //  and the number of roots (0, 1 or 2) within the interval [-1, 1]
            //
            //	well, this routine is producing sensible answers
            //
            //  results passed as array [nz, z1, z2, xe, ye]
            //
            double[] quadout = [0,0,0,0,0];

            var nz = 0;
            var a = 0.5 * (ym + yp) - yz;
            var b = 0.5 * (yp - ym);
            var c = yz;
            var xe = -b / (2 * a);
            var ye = (a * xe + b) * xe + c;
            var dis = b * b - 4.0 * a * c;
            double dx, z1 = 0, z2 = 0;
            if (dis > 0)
            {
                dx = 0.5 * Math.Sqrt(dis) / Math.Abs(a);
                z1 = xe - dx;
                z2 = xe + dx;
                if (Math.Abs(z1) <= 1.0) nz += 1;
                if (Math.Abs(z2) <= 1.0) nz += 1;
                if (z1 < -1.0) z1 = z2;
            }
            quadout[0] = nz;
            quadout[1] = z1;
            quadout[2] = z2;
            quadout[3] = xe;
            quadout[4] = ye;
            return quadout;
        }

        private static ZonedDateTime hrsminInUtc(ZonedDateTime today, double hours)
        {
            //
            //	takes decimal hours and returns a string in hhmm format
            //
            var hrs = Math.Floor(hours * 60 + 0.5) / 60.0;
            var h = (int) Math.Floor(hrs);
            var m = (int) Math.Floor(60 * (hrs - h) + 0.5);
            return ConvertUTCMinutesToZonedDateTime(today, h * 60 + m);
        }

        private static Tuple<ZonedDateTime?, ZonedDateTime?, bool, bool> find_moonrise_set_utc(ZonedDateTime today, double mjd, int tz, double glong, double glat)
        {
            //
            //	Im using a separate function for moonrise/set to allow for different tabulations
            //  of moonrise and sun events ie weekly for sun and daily for moon. The logic of
            //  the function is identical to find_sun_and_twi_events_for_date()
            //
            var rads = 0.0174532925;
            ZonedDateTime? moonRise = null;
            ZonedDateTime? moonSet = null;
            bool alwaysUp = false;
            bool alwaysDown = false;

            var sinho = Math.Sin(rads * 8 / 60);        //moonrise taken as centre of moon at +8 arcmin
            var sglat = Math.Sin(rads * glat);
            var cglat = Math.Cos(rads * glat);
            var date = mjd - tz / 24;
            bool rise = false;
            bool sett = false;
            bool above = false;
            var hour = 1.0;
            var ym = sin_alt(1, date, hour - 1.0, glong, cglat, sglat) - sinho;
            if (ym > 0.0) above = true;
            double utrise = 0, utset = 0;
            while (hour < 25 && (sett == false || rise == false))
            {
                var yz = sin_alt(1, date, hour, glong, cglat, sglat) - sinho;
                var yp = sin_alt(1, date, hour + 1.0, glong, cglat, sglat) - sinho;
                var quadout = quad(ym, yz, yp);
                var nz = quadout[0];
                var z1 = quadout[1];
                var z2 = quadout[2];
                var xe = quadout[3];
                var ye = quadout[4];

                // case when one event is found in the interval
                if (nz == 1)
                {
                    if (ym < 0.0)
                    {
                        utrise = hour + z1;
                        rise = true;
                    }
                    else
                    {
                        utset = hour + z1;
                        sett = true;
                    }
                } // end of nz = 1 case

                // case where two events are found in this interval
                // (rare but whole reason we are not using simple iteration)
                if (nz == 2)
                {
                    if (ye < 0.0)
                    {
                        utrise = hour + z2;
                        utset = hour + z1;
                    }
                    else
                    {
                        utrise = hour + z1;
                        utset = hour + z2;
                    }
                }

                // set up the next search interval
                ym = yp;
                hour += 2.0;

            } // end of while loop

            if (rise == true || sett == true)
            {
                if (rise == true) moonRise = hrsminInUtc(today, utrise);
                if (sett == true) moonSet = hrsminInUtc(today, utset);
            }
            else
            {
                if (above == true) alwaysUp = true;
                else alwaysDown = true;
            }

            return Tuple.Create(moonRise, moonSet, alwaysUp, alwaysDown);
        }

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
