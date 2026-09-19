
using Microsoft.Maui.Graphics.Platform;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.Views.Drawable
{
    public class MapDrawable : IDrawable
    {
        private ILogger _logger;
        private IResourceHelper _resourceHelper;

        public MapDrawable(ILogger logger, IResourceHelper resourceHelper)
        {
            logger.Debug(() => $"MapDrawable init");
            _logger = logger;
            _resourceHelper = resourceHelper;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            _logger.Debug(() => $"MapDrawable draw, {dirtyRect.X} {dirtyRect.Y} {dirtyRect.Width} {dirtyRect.Height}");

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
            }

            if (image != null)
            {
                // Draw image at x: 10, y: 10 with specified width and height
                canvas.DrawImage(image, 0, 0, dirtyRect.Width, dirtyRect.Height);
            }

            // Draw stuff over the top
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
        }
    }
}
