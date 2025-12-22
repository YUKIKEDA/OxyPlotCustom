using System.Windows.Controls;

namespace OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// ParallelCoordinatesSeriesView.xaml の相互作用ロジック
    /// </summary>
    public partial class ParallelCoordinatesSeriesView : UserControl
    {
        public ParallelCoordinatesSeriesView()
        {
            InitializeComponent();
            DataContext = new ParallelCoordinatesSeriesViewModel();
        }
    }
}
