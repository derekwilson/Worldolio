namespace Worldolio.Data.Model
{
    public class Distance
    {
        /// <summary>
        /// The units the distance is measured in
        /// </summary>
        public enum Units
        {
            /// <summary>
            /// metric
            /// </summary>
            Kilometers = 0,
            /// <summary>
            /// Imperial
            /// </summary>
            Miles = 1,
            /// <summary>
            /// Bizare
            /// </summary>
            NauticalMiles = 2
        }

        /// <summary>
        /// Inflate the enumerated type from supplied int
        /// </summary>
        public static Units LoadFromInt(int val)
        {
            return (Units)Enum.ToObject(typeof(Units), (object)val);
        }

        /// <summary>
        /// This is always held in kilometers
        /// </summary>
        private double m_val;

        /// <summary>
        /// A measure of distance that can be access in a variety of units
        /// </summary>
        /// <param name="val">the distance in KM</param>
        public Distance(double val)
        {
            m_val = val;
        }
        /// <summary>
        /// A measure of distance that can be access in a variety of units
        /// </summary>
        /// <param name="val">the distance</param>
        /// <param name="units">the units the distance is measured in</param>
        public Distance(double val, Units units)
        {
            m_val = 0;
            SetValue(val, units);
        }

        /// <summary>
        /// Set the distance using the units provided
        /// </summary>
        /// <param name="val">the distance</param>
        /// <param name="units">the units the distance is measured in</param>
        private void SetValue(double val, Units units)
        {
            switch (units)
            {
                case Units.Kilometers:
                    m_val = val;
                    break;
                case Units.NauticalMiles:
                    m_val = val * 1.852;
                    break;
                case Units.Miles:
                    m_val = val * 1.61;
                    break;
            }
        }

        /// <summary>
        /// the distance in KM
        /// </summary>
        public double Kilometers
        {
            get { return m_val; }
            set { SetValue(value, Units.Kilometers); }
        }

        /// <summary>
        /// the distance in KM as a string
        /// </summary>
        public string KilometersStr
        {
            get { return m_val.ToString("#,0"); }
        }

        /// <summary>
        /// the distance in Miles
        /// </summary>
        public double Miles
        {
            get { return m_val * 0.62; }
            set { SetValue(value, Units.Miles); }
        }

        /// <summary>
        /// the distance in Miles as a string
        /// </summary>
        public string MilesStr
        {
            get { return Miles.ToString("#,0"); }
        }

        /// <summary>
        /// the distance in NauticalMiles
        /// </summary>
        public double NauticalMiles
        {
            get { return (m_val / 1.852); }
            set { SetValue(value, Units.NauticalMiles); }
        }

        /// <summary>
        /// the distance in NauticalMiles as a string
        /// </summary>
        public string NauticalMilesStr
        {
            get { return NauticalMiles.ToString("#,0"); }
        }

        /// <summary>
        /// return a string containing the formatted value and units
        /// </summary>
        /// <returns></returns>
        public string ToString(Units units)
        {
            switch (units)
            {
                case Units.Kilometers:
                    return KilometersStr + " KM";
                case Units.NauticalMiles:
                    return NauticalMilesStr + " NM";
                case Units.Miles:
                    return MilesStr + " Miles";
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// return the negated distance
        /// </summary>
        /// <returns>a new distance equal to the negative of the distance</returns>
        public Distance Negate()
        {
            return new Distance(-m_val);
        }
    }
}
