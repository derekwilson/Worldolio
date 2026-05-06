namespace Worldolio.Data.Model
{
    /// <summary>
    /// A position is a point on the planet
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Position's latitude, north = +ve, south = -ve
        /// </summary>
        double m_latitude;
        /// <summary>
        /// Position's longitude, west = -ve, east = +ve
        /// </summary>
        double m_longitude;

        /// <summary>
        /// Position's latitude, north = +ve, south = -ve
        /// </summary>
        public double Latitude
        {
            get { return m_latitude; }
            set { m_latitude = value; CheckPos(); }
        }
        /// <summary>
        /// Position's longitude, west = -ve, east = +ve
        /// </summary>
        public double Longitude
        {
            get { return m_longitude; }
            set { m_longitude = value; CheckPos(); }
        }

        /// <summary>
        /// Construct a position on the planet
        /// </summary>
        /// <param name="latitude">latitude, north = +ve, south = -ve</param>
        /// <param name="longitude">longitude, west = -ve, east = +ve</param>
        public Position(double latitude, double longitude)
        {
            m_latitude = latitude;
            m_longitude = longitude;
            CheckPos();
        }

        /// <summary>
        /// stop a position from having illegal lat/long
        /// </summary>
        private void CheckPos()
        {
            if (m_latitude < -90)
                m_latitude = -90;
            if (m_latitude > 90)
                m_latitude = 90;

            if (m_longitude < -180)
                m_longitude = -180;
            if (m_longitude > 180)
                m_longitude = 180;
        }

        /// <summary>
        /// the latitude formatted as decimal degrees
        /// </summary>
        public string LatitudeDecimalStr
        {
            get
            {
                return string.Format("{0}{1}", System.Math.Abs(m_latitude).ToString("0.00"), (m_latitude >= 0 ? "N" : "S"));
            }
        }

        /// <summary>
        /// the longitude formatted as decimal degrees
        /// </summary>
        public string LongitudeDecimalStr
        {
            get
            {
                return string.Format("{0}{1}", System.Math.Abs(m_longitude).ToString("0.00"), (m_longitude >= 0 ? "E" : "W"));
            }
        }

        /// <summary>
        /// the latitude formatted in deg min sec
        /// </summary>
        public string LatitudeStr
        {
            get
            {
                return DecimalToDegreesString(m_latitude, "N", "S");
            }
        }

        /// <summary>
        /// the longitude formatted in deg min sec
        /// </summary>
        public string LongitudeStr
        {
            get
            {
                return DecimalToDegreesString(m_longitude, "E", "W");
            }
        }

        /// <summary>
        /// format the position into a string
        /// </summary>
        /// <param name="bDecimal">true to use decimal degrees, false for deg min sec</param>
        /// <returns>formatted string</returns>
        public string ToString(bool bDecimal)
        {
            if (bDecimal)
                return string.Format("{0}, {1}", LongitudeDecimalStr, LatitudeDecimalStr);

            return string.Format("{0}, {1}", LongitudeStr, LatitudeStr);
        }

        /// <summary>
        /// convert a decimal deg into deg min sec string
        /// </summary>
        /// <param name="deg">decimal degrees</param>
        /// <param name="posSuffix">suffix to use for positive degrees</param>
        /// <param name="negSuffix">suffix to use for negative degrees</param>
        /// <returns>formatted string</returns>
        public string DecimalToDegreesString(double deg, string posSuffix, string negSuffix)
        {
            int WholeDegrees = (int)deg;
            int WholeMinutes = (int)((deg - (double)WholeDegrees) * 60.0);

            return string.Format("{0} {1}' {2}", Math.Abs(WholeDegrees), Math.Abs(WholeMinutes), (deg < 0 ? negSuffix : posSuffix));
        }

        /// <summary>
        /// The difference between two positions is the distance between them
        /// </summary>
        /// <param name="p1">pos1</param>
        /// <param name="p2">pos2</param>
        /// <returns>distance between the points</returns>
        public static Distance operator -(Position p1, Position p2)
        {
            return GeoCalculator.GetDistance(p1, p2);
        }

        /// <summary>
        /// Move the position by the specified distance
        /// </summary>
        /// <param name="northSouth">north = +ve, south = -ve</param>
        /// <param name="eastWest">west = -ve, east = +ve</param>
        /// <returns>the moved position</returns>
        public Position Move(Distance northSouth, Distance eastWest)
        {
            // a NM = 1 min, 60 NM = 1 deg for movements in latitude and for movements of longitude at the equator
            // a degree movement of longitude away from the equator is smaller as the meridians approach each other

            double newLat = m_latitude + northSouth.NauticalMiles / 60;

            double latRad = Math.PI * m_latitude / 180;
            double nmPerDeg = Math.Cos(latRad) * 60;
            double newLong = m_longitude + eastWest.NauticalMiles / nmPerDeg;

            return new Position(newLat, newLong);
        }
    }
}
