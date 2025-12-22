using System.Windows.Controls;
using Reactive.Bindings;
using OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots;

namespace OxyPlotCustom.Examples
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            AvailableViews = new ReactiveCollection<PlotViewItem>
            {
                new PlotViewItem("Parallel Coordinates Series", new ParallelCoordinatesSeriesView())
            };

            SelectedViewItem = new ReactiveProperty<PlotViewItem?>(AvailableViews[0]);
            SelectedView = new ReactiveProperty<UserControl?>(AvailableViews[0].View);

            // SelectedViewItemが変更されたときにSelectedViewを更新
            SelectedViewItem.Subscribe(item =>
            {
                SelectedView.Value = item?.View;
            });
        }

        public ReactiveCollection<PlotViewItem> AvailableViews { get; }

        public ReactiveProperty<PlotViewItem?> SelectedViewItem { get; }

        public ReactiveProperty<UserControl?> SelectedView { get; }
    }

    public class PlotViewItem
    {
        public PlotViewItem(string name, UserControl view)
        {
            Name = name;
            View = view;
        }

        public string Name { get; }
        public UserControl View { get; }
    }
}

