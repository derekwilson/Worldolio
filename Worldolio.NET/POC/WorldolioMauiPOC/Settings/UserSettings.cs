using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldolioMauiPOC.Settings
{
    public interface IUserSettings
    {
        long[] Cities { get; set; }
    }
    public class UserSettings : IUserSettings
    {
        private long[] _cities = [429, 458, 252, 477, 324, 79, 320, 279, 382, 180, 351];

        public long[] Cities
        {
            get
            {
                return _cities;
            }
            set
            {
                _cities = value;
            }
        }
    }
}
