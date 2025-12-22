using OxyPlot;
using OxyPlotCustom.ParallelCoordinatesSeriesPlots;

namespace OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots
{
    public class ParallelCoordinatesSeriesViewModel
    {
        public PlotModel? PlotModel { get; private set; }

        public ParallelCoordinatesSeriesViewModel()
        {
            PlotModel = new PlotModel 
            { 
                Title = "Parallel Coordinates Plot Demo" 
            };

            // サンプルデータを作成
            var dimensions = new Dictionary<string, ParallelCoordinatesDimension>
            {
                { "Dimension1", new ParallelCoordinatesDimension("Dimension 1", [1.0, 2.0, 3.0, 4.0, 5.0]) },
                { "Dimension2", new ParallelCoordinatesDimension("Dimension 2", [2.0, 3.0, 4.0, 5.0, 6.0]) },
                { "Dimension3", new ParallelCoordinatesDimension("Dimension 3", [3.0, 4.0, 5.0, 6.0, 7.0]) },
                { "Dimension4", new ParallelCoordinatesDimension("Dimension 4", [4.0, 5.0, 6.0, 7.0, 8.0]) }
            };

            // ParallelCoordinatesSeriesを作成してPlotModelに追加
            var series = new ParallelCoordinatesSeries(dimensions);
            PlotModel.Series.Add(series);
        }
    }
}
