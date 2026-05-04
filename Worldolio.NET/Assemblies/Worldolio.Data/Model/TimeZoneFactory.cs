using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public interface ITimeZoneFactory
    {
        TimeZone GetTimeZoneFromIanaName(string name);
    }

    public class TimeZoneFactory : ITimeZoneFactory
    {
        private IInstantProvider _instantProvider;

        public TimeZoneFactory(IInstantProvider instantProvider)
        {
            _instantProvider = instantProvider;
        }

        public TimeZone GetTimeZoneFromIanaName(string name)
        {
            return new Model.TimeZone(name, _instantProvider);
        }
    }
}
