
using Microsoft.Maui.Graphics.Platform;
using System.Reflection;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.Views.Drawable
{
    public class MapDrawable : IDrawable
    {
        public DateTime UtcTime { get; set; } = DateTime.UtcNow;

        private ILogger _logger;
        private IResourceHelper _resourceHelper;

        private float _mapHeight;
        private float _mapWidth;

        public MapDrawable(ILogger logger, IResourceHelper resourceHelper)
        {
            logger.Debug(() => $"MapDrawable init");
            _logger = logger;
            _resourceHelper = resourceHelper;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            _logger.Debug(() => $"MapDrawable draw, {dirtyRect.X} {dirtyRect.Y} {dirtyRect.Width} {dirtyRect.Height} at {UtcTime}");

            // Background
            canvas.FillColor = _resourceHelper.GetResource<Color>("PrimaryLight", Colors.Red);
            canvas.FillRectangle(dirtyRect);

            Microsoft.Maui.Graphics.IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            // TODO - move to the resource helper
            // Load image from Embedded Resources
            using (Stream stream = assembly.GetManifestResourceStream("WorldolioMauiPOC.Resources.Images.Embedded.earth_transparent_2048.png"))
            {
                image = PlatformImage.FromStream(stream);
                _mapWidth = image.Width;
                _mapHeight = image.Height;
            }

            if (image != null)
            {
                // Draw image at x: 10, y: 10 with specified width and height
                canvas.DrawImage(image, 0, 0, dirtyRect.Width, dirtyRect.Height);
            }

            DrawShadow(UtcTime, canvas, dirtyRect.Width, dirtyRect.Height);

            // Draw stuff over the top
            /*
            canvas.StrokeSize = 1;
            canvas.StrokeColor = Color.FromArgb("#0033FF");
            Random Rand = new();
            for (int i = 0; i < 10; i++)
            {
                canvas.DrawLine(
                    x1: (float)Rand.NextDouble() * dirtyRect.Width,
                    y1: (float)Rand.NextDouble() * dirtyRect.Height,
                    x2: (float)Rand.NextDouble() * dirtyRect.Width,
                    y2: (float)Rand.NextDouble() * dirtyRect.Height);
            }
            */
        }

        Point[] _shadow = new Point[GeoCalculator.SHADOW_EDGE_POINTS + 2];

        private void DrawShadow(System.DateTime time, ICanvas canvas, float width, float height)
        {
            bool bShadowNorth = true;
            Position[] arrEdge = GeoCalculator.CalcDayNightShadowEdge(time, ref bShadowNorth);

            // translate the lat/long edge of the shadow into xy points on the image
            for (int index = 0; index < arrEdge.Length; index++)
            {
                _shadow[index] = PositionToMapPoint(arrEdge[index], width, height);
            }

            // add on the top or bottom corners
            if (bShadowNorth)
            {
                // top 2 corners
                _shadow[arrEdge.Length] = new Point(width, 0);
                _shadow[arrEdge.Length + 1] = new Point(0, 0);
            }
            else
            {
                // bottom 2 corners
                _shadow[arrEdge.Length] = new Point(width, height);
                _shadow[arrEdge.Length + 1] = new Point(0, height);
            }

            var path = ConvertPointsToPath(_shadow);
            canvas.FillColor = Color.FromArgb("#7F000000");
            canvas.FillPath(path);

            // fill the whole thing with alpha blending
            /* - translate into MAUI
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            DrawSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            DrawSurface.FillPolygon(semiTransBrush, _shadow);
            DrawSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            */
        }

        private PathF ConvertPointsToPath(Point[] points)
        {
            bool doneFirst = false;
            PathF path = new PathF();
            foreach (Point p in points)
            {
                if (doneFirst)
                {
                    path.LineTo((float)p.X, (float)p.Y);
                }
                else
                {
                    path.MoveTo((float)p.X, (float)p.Y);
                    doneFirst = true;
                }
            }
            path.Close(); // Connects the last point back to the first
            return path;
        }

        private Point PositionToMapPoint(Position Pos, float width, float height)
        {
            return PositionToMapPoint_Equirectangular(Pos, width, height);
        }

        private Point PositionToMapPoint_Equirectangular(Position Pos, float width, float height)
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
            return new Point(xPos, yPos);
        }

        private Position MapPointToPosition(Point Pt)
        {
            return MapPointToPosition_Equirectangular(Pt);
        }

        private Position MapPointToPosition_Equirectangular(Point Pt)
        {
            double Longitude = (((double)Pt.X * 360.0) / _mapWidth) - 180.0;
            double Latitude = 90.0 - (((double)Pt.Y * 180.0) / _mapHeight);

            return new Position(Latitude, Longitude);
        }
    }
}
