
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.AppSettings;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.Views.Drawable
{
    public class MapDrawable : IDrawable
    {
        public bool ShowShadow { get; set; } = true;
        public DateTime UtcTime { get; set; } = DateTime.UtcNow;
        public bool ShowCities { get; set; } = true;

        public bool NeedUpdateBecauseDataChanged
        {
            get
            {
                return _userSettings.HasBeenUpdatedSince(_lastRefreshTime);
            }
        }

        private Distance _nearbyDistance = Distance.FromValues(200, Distance.Units.Kilometers);
        private const int _dotsize = 1;
        private Color _cityColour = Colors.White;
        private Color _homeCityColour = Colors.Red;
        private DateTime _lastRefreshTime = DateTime.MinValue;
        private List<City> _cities = new List<City>();
        private float _currentImageWidth = 2;
        private float _currentImageHeight = 1;

        private ILogger _logger;
        private IResourceProvider _resourceProvider;
        private ICityRepository _citiesRepository;
        private IUserSettings _userSettings;
        private ISystemTimeProvider _systemTimeProvider;

        public MapDrawable(ILogger logger, IResourceProvider resourceProvider, IUserSettings userSettings, ICityRepository citiesRepository, ISystemTimeProvider systemTimeProvider)
        {
            logger.Debug(() => $"MapDrawable init");

            _cityColour = resourceProvider.GetResource<Color>("White", Colors.White);
            _homeCityColour = resourceProvider.GetResource<Color>("Red", Colors.White);

            _logger = logger;
            _resourceProvider = resourceProvider;
            _userSettings = userSettings;
            _citiesRepository = citiesRepository;
            _systemTimeProvider = systemTimeProvider;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            _logger.Debug(() => $"MapDrawable draw, {dirtyRect.X} {dirtyRect.Y} {dirtyRect.Width} {dirtyRect.Height} at {UtcTime}");

            // Background
            canvas.FillColor = _resourceProvider.GetResource<Color>("PrimaryLight", Colors.Red);
            canvas.FillRectangle(dirtyRect);

            // Map Image
            var image = _resourceProvider.LoadImageFromEmbeddedResource("WorldolioMauiPOC.Resources.Images.Embedded.earth_transparent_2048.png");
            if (image != null)
            {
                _currentImageWidth = dirtyRect.Width;
                _currentImageHeight = dirtyRect.Height;
                // Draw image at x: 10, y: 10 with specified width and height
                canvas.DrawImage(image, 0, 0, dirtyRect.Width, dirtyRect.Height);
            }

            // Day Night shadow
            if (ShowShadow)
            {
                DrawShadow(UtcTime, canvas, dirtyRect.Width, dirtyRect.Height);
            }

            // Draw the current cities
            if (ShowCities)
            {
                LoadCitiesIfNeeded();
                DrawCurrentCities(canvas, dirtyRect.Width, dirtyRect.Height);
            }
        }

        #region day night shadow

        private void DrawShadow(DateTime time, ICanvas canvas, float width, float height)
        {
            _logger.Debug(() => $"MapDrawable DrawShadow, UTC time: {time}");

            bool bShadowNorth = true;
            var arrEdge = GeoCalculator.CalcDayNightShadowEdge(time, ref bShadowNorth);

            // translate the lat/long edge of the shadow into xy points on the image and then create a Path from them
            var path = ConvertPositionsToPath(arrEdge, width, height);

            // add on the top or bottom corners
            if (bShadowNorth)
            {
                // top 2 corners
                path.LineTo((float)width, 0);
                path.LineTo(0, 0);
            }
            else
            {
                // bottom 2 corners
                path.LineTo((float)width, (float)height);
                path.LineTo(0, (float)height);
            }
            path.Close(); // Connects the last point back to the first - not sure this is needed
            canvas.FillColor = Color.FromArgb("#7F000000");
            canvas.FillPath(path);
        }

        private PathF ConvertPositionsToPath(Position[] positions, float width, float height)
        {
            // translate the positions into xy points on the image and then create a Path from them
            PathF path = new PathF();
            for (int index = 0; index < positions.Length; index++)
            {
                var point = PositionToMapPoint(positions[index], width, height);
                if (index == 0)
                {
                    path.MoveTo(point.X, point.Y);
                }
                else
                {
                    path.LineTo(point.X, point.Y);
                }
            }
            return path;
        }

        #endregion

        #region drawing cities

        private void LoadCitiesIfNeeded()
        {
            _logger.Debug(() => $"MapDrawable LoadCitiesIfNeeded, last refresh: {_lastRefreshTime}");

            if (NeedUpdateBecauseDataChanged)
            {
                _logger.Debug(() => $"MapDrawable LoadCitiesIfNeeded - refresh needed");

                // we cannot be async
                // TODO start loading and then actually perform the drawing when the data is ready perhaps
                var temp = _citiesRepository.GetByIdsAsync(_userSettings.Cities).Result;
                _cities = new List<City>(temp);

                _lastRefreshTime = _systemTimeProvider.GetUtcNow();
            }

            _logger.Debug(() => $"MapDrawable cities = {_cities.Count}");
        }

        private void DrawCurrentCities(ICanvas canvas, float width, float height)
        {
            var home = _cities.FirstOrDefault();
            foreach (City city in _cities)
            {
                var dot = CalcMapRect(city, width, height);
                var isHome = home != null && home.Id == city.Id;
                DrawOutlinedDot(canvas, dot, isHome);
            }
        }

        private void DrawOutlinedDot(ICanvas canvas, RectF dot, bool isHome)
        {
            canvas.FillColor = isHome ? _homeCityColour : _cityColour;
            canvas.FillEllipse(dot);
            canvas.StrokeSize = 1;
            canvas.StrokeColor = Colors.Black;
            canvas.DrawEllipse(dot.Inflate(1, 1));
        }

        public RectF CalcMapRect(City city, float width, float height)
        {
            return CalcMapRect(city.Position, _dotsize, width, height);
        }

        protected RectF CalcMapRect(Position pos, int dotSize, float width, float height)
        {
            return CalcMapRect(PositionToMapPoint(pos, width, height), dotSize);
        }

        protected RectF CalcMapRect(PointF pt, int DotSize)
        {
            return new RectF(pt.X - DotSize, pt.Y - DotSize, (2 * DotSize) + 1, (2 * DotSize) + 1);
        }

        #endregion

        #region Convert Lat/Long Positions <-> Image Point (needs to take account of the current image size)

        private PointF PositionToMapPoint(Position Pos, float width, float height)
        {
            return PositionToMapPoint_Equirectangular(Pos, width, height);
        }

        private PointF PositionToMapPoint_Equirectangular(Position Pos, float width, float height)
        {
            // Equirectangular projection
            // For some strange reason they only go to +/- 90 degrees latitude

            // west = -ve, east = +ve
            int xPos = (int)(((Pos.Longitude + 180.0) * width) / 360.0);
            // north = +ve, south = -ve
            //int yPos = (int)(((180.0 - Pos.Latitude) * Height) / 360.0);
            int yPos = (int)(((90.0 - Pos.Latitude) * height) / 180.0);
            if (yPos < 0)
                yPos = 0;
            if (yPos > height)
                yPos = (int) height;
            return new PointF(xPos, yPos);
        }

        private Position MapPointToPosition(PointF Pt, float width, float height)
        {
            return MapPointToPosition_Equirectangular(Pt, width, height);
        }

        private Position MapPointToPosition_Equirectangular(PointF Pt, float width, float height)
        {
            double Longitude = (((double)Pt.X * 360.0) / width) - 180.0;
            double Latitude = 90.0 - (((double)Pt.Y * 180.0) / height);

            return new Position(Latitude, Longitude);
        }

        #endregion

        public string GetTooltipText(PointF hoverPosition)
        {
            var pos = MapPointToPosition(hoverPosition, _currentImageWidth, _currentImageHeight);
            var home = _cities.FirstOrDefault();
            string strDist = "";
            if (home != null)
            {
                Distance dist = home.GetDistance(pos);
                // TODO - get units from settings
                strDist = $", Dist: {dist.ToString(Distance.Units.Kilometers)}";
            }

            string strInfo = $"Long: {pos.Longitude.ToString("#,0")} W, Lat: {pos.Latitude.ToString("#,0")} N{strDist}";

            string strCity = "";

            foreach (City thisCity in _cities)
            {
                if (thisCity.GetDistance(pos).Kilometers < _nearbyDistance.Kilometers)
                {
                    strCity = $" ({thisCity.DisplayName})";
                    break;
                }
            }

            return $"{strInfo} {strCity}";
        }
    }
}
