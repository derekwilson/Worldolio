using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Views.Drawable
{
    public class RandomLinesDrawable : IDrawable
    {
        private ILogger _logger;

        public RandomLinesDrawable(ILogger logger)
        {
            logger.Debug(() => $"RandomLinesDrawable init");
            _logger = logger;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            _logger.Debug(() => $"RandomLinesDrawable draw, {dirtyRect.X} {dirtyRect.Y} {dirtyRect.Width} {dirtyRect.Height}");

            //canvas.FillColor = Color.FromArgb("#003366");
            //canvas.FillRectangle(dirtyRect);

            canvas.StrokeSize = 1;
            canvas.StrokeColor = Color.FromArgb("#FF0000");
            canvas.DrawRectangle(dirtyRect);

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
