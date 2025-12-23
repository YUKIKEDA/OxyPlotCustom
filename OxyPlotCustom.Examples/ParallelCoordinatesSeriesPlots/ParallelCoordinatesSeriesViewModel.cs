using OxyPlot;
using Reactive.Bindings;
using OxyPlotCustom.ParallelCoordinatesSeriesPlots;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots
{
    public class ParallelCoordinatesSeriesViewModel
    {
        public PlotModel? PlotModel { get; private set; }

        public ReactiveCommand<ScreenPoint> MouseMoveCommand { get; }
        public ReactiveCommand MouseLeaveCommand { get; }
        public ReactiveCommand<ScreenPoint> MouseDownCommand { get; }
        public ReactiveCommand<ScreenPoint> MouseUpCommand { get; }

        /// <summary>
        /// 編集モードが有効かどうか
        /// </summary>
        public ReactiveProperty<bool> IsEditMode { get; }

        /// <summary>
        /// カラーマップを表示するかどうか
        /// </summary>
        public ReactiveProperty<bool> ShowColorMap { get; }

        private ParallelCoordinatesSeries? Series { get; set; }
        private PointAdditionHandler? PointAdditionHandler { get; set; }

        public ParallelCoordinatesSeriesViewModel()
        {
            PlotModel = new PlotModel 
            { 
                Title = "Parallel Coordinates Plot Demo" 
            };

            // CSVファイルからデータを読み込む
            var dimensions = LoadDataFromCsv();

            // ParallelCoordinatesSeriesを作成してPlotModelに追加
            Series = new ParallelCoordinatesSeries(dimensions);
            
            // カラーマップを設定
            Series.ColorMapDimensionName = "colorVal";
            Series.ColorMap = OxyPalettes.Jet(256);
            
            // 範囲調整ハンドラーを追加
            var rangeHandler = new DimensionRangeHandler();
            Series.InteractionHandlers.Add(rangeHandler);
            
            // 点追加ハンドラーを追加
            PointAdditionHandler = new PointAdditionHandler();
            Series.InteractionHandlers.Add(PointAdditionHandler);
            
            PlotModel.Series.Add(Series);

            // 編集モードのプロパティを初期化
            IsEditMode = new ReactiveProperty<bool>(false);
            IsEditMode.Subscribe(isEditMode =>
            {
                if (PointAdditionHandler != null && Series != null)
                {
                    PointAdditionHandler.IsEditMode = isEditMode;
                    Series.IsEditMode = isEditMode;
                    if (isEditMode)
                    {
                        // 編集モードを有効にしたときにハイライトをクリア
                        if (Series.HighlightedLineId != null)
                        {
                            Series.HighlightedLineId = null;
                        }
                    }
                    else
                    {
                        // 編集モードを無効にしたときにリセット
                        PointAdditionHandler.ResetEditMode();
                    }
                    PlotModel?.InvalidatePlot(false);
                }
            });

            // カラーマップ表示のプロパティを初期化
            ShowColorMap = new ReactiveProperty<bool>(false);
            ShowColorMap.Subscribe(showColorMap =>
            {
                if (Series != null)
                {
                    Series.ShowColorMap = showColorMap;
                    PlotModel?.InvalidatePlot(false);
                }
            });

            // コマンドを初期化
            MouseMoveCommand = new ReactiveCommand<ScreenPoint>().WithSubscribe(OnMouseMove);
            MouseLeaveCommand = new ReactiveCommand().WithSubscribe(OnMouseLeave);
            MouseDownCommand = new ReactiveCommand<ScreenPoint>().WithSubscribe(OnMouseDown);
            MouseUpCommand = new ReactiveCommand<ScreenPoint>().WithSubscribe(OnMouseUp);
        }

        /// <summary>
        /// CSVファイルからデータを読み込みます
        /// </summary>
        /// <returns>次元データのディクショナリ</returns>
        private static Dictionary<string, ParallelCoordinatesDimension> LoadDataFromCsv()
        {
            // CSVファイルのパスを取得（実行ファイルの場所からの相対パス）
            var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParallelCoordinatesSeriesPlots", "data.csv");

            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException($"CSVファイルが見つかりません: {csvPath}");
            }

            // CsvHelperの設定
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim
            };

            // 各列のデータを格納するディクショナリ
            var columnData = new Dictionary<string, List<double>>();
            var headers = new List<string>();

            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, config))
            {
                // ヘッダーを読み込む
                if (csv.Read())
                {
                    csv.ReadHeader();
                    headers = csv.HeaderRecord?.ToList() ?? new List<string>();
                    
                    // 各列のリストを初期化
                    foreach (var header in headers)
                    {
                        columnData[header] = new List<double>();
                    }
                }

                // データ行を読み込む
                while (csv.Read())
                {
                    foreach (var header in headers)
                    {
                        var valueStr = csv.GetField(header);
                        if (string.IsNullOrWhiteSpace(valueStr))
                        {
                            columnData[header].Add(0);
                        }
                        else
                        {
                            // 科学記法（6.00E+05など）を含む数値を解析
                            if (double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                            {
                                columnData[header].Add(value);
                            }
                            else
                            {
                                columnData[header].Add(0);
                            }
                        }
                    }
                }
            }

            // 次元データを作成
            var dimensions = new Dictionary<string, ParallelCoordinatesDimension>();
            foreach (var header in headers)
            {
                var dataArray = columnData[header].ToArray();
                dimensions[header] = new ParallelCoordinatesDimension(header, dataArray);
            }

            return dimensions;
        }

        /// <summary>
        /// マウス移動時のハイライト処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseMove(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理（ドラッグなど）
            bool handled = false;
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseMove(Series, screenPoint))
                {
                    handled = true;
                    break;
                }
            }

            // 編集モード時はハイライト処理をスキップ
            if (Series.IsEditMode)
            {
                // ハイライトをクリア
                if (Series.HighlightedLineId != null)
                {
                    Series.HighlightedLineId = null;
                    PlotModel.InvalidatePlot(false);
                }
                return;
            }

            // ハンドラーで処理されなかった場合のみハイライト処理
            if (!handled)
            {
                // 最も近いラインを取得
                var nearestLineId = Series.GetNearestLineId(screenPoint);

                // ハイライトを更新
                if (Series.HighlightedLineId != nearestLineId)
                {
                    Series.HighlightedLineId = nearestLineId;
                    PlotModel.InvalidatePlot(false);
                }
            }
        }

        /// <summary>
        /// マウスが離れたときのハイライト解除処理
        /// </summary>
        private void OnMouseLeave()
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // 編集モード時はハイライト処理をスキップ
            if (Series.IsEditMode)
            {
                return;
            }

            // ハイライトを解除
            if (Series.HighlightedLineId != null)
            {
                Series.HighlightedLineId = null;
                PlotModel.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// マウスダウン時の処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseDown(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseDown(Series, screenPoint))
                {
                    PlotModel.InvalidatePlot(false);
                    return;
                }
            }
        }

        /// <summary>
        /// マウスアップ時の処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseUp(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseUp(Series, screenPoint))
                {
                    PlotModel.InvalidatePlot(false);
                    return;
                }
            }
        }
    }
}
