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
        /// 編集モード時の既存プロットの透明度（0.0～1.0、デフォルトは0.3）
        /// </summary>
        public ReactiveProperty<double> EditModeOpacity { get; }

        /// <summary>
        /// 編集モード時の既存プロットの色の薄さ（0.0=元の色、1.0=白、デフォルトは0.7）
        /// </summary>
        public ReactiveProperty<double> EditModeLightness { get; }

        /// <summary>
        /// カラーマップを表示するかどうか
        /// </summary>
        public ReactiveProperty<bool> ShowColorMap { get; }

        private ParallelCoordinatesSeries? Series { get; set; }
        private PointAdditionHandler? PointAdditionHandler { get; set; }

        /// <summary>
        /// ラインの元の色を保持するディクショナリ（編集モード解除時に元に戻すため）
        /// </summary>
        private Dictionary<string, OxyColor> _originalLineColors = [];

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
            EditModeOpacity = new ReactiveProperty<double>(0.3);
            EditModeLightness = new ReactiveProperty<double>(0.7);

            IsEditMode.Subscribe(isEditMode =>
            {
                if (PointAdditionHandler != null && Series != null)
                {
                    PointAdditionHandler.IsEditMode = isEditMode;
                    if (isEditMode)
                    {
                        // 既存のラインの色を調整（透明度と薄さを適用）
                        ApplyEditModeToLines();
                    }
                    else
                    {
                        // 編集モードを無効にしたときにリセット
                        PointAdditionHandler.ResetEditMode();
                        // ラインの色を元に戻す
                        RestoreOriginalLineColors();
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
        /// <returns>次元データの配列（ヘッダー順）</returns>
        private static ParallelCoordinatesDimension[] LoadDataFromCsv()
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

            // 次元データを作成（ヘッダー順を保持）
            return headers
                .Select(header => new ParallelCoordinatesDimension(header, columnData[header].ToArray()))
                .ToArray();
        }

        /// <summary>
        /// マウス移動時の処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseMove(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理（ドラッグなど）
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseMove(Series, screenPoint))
                {
                    PlotModel.InvalidatePlot(false);
                    break;
                }
            }
        }

        /// <summary>
        /// マウスが離れたときの処理
        /// </summary>
        private void OnMouseLeave()
        {
            
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

        /// <summary>
        /// 編集モード時に既存のラインの色に透明度と薄さを適用します
        /// </summary>
        private void ApplyEditModeToLines()
        {
            if (Series == null || !Series.HasLines)
            {
                return;
            }

            _originalLineColors.Clear();
            double opacity = EditModeOpacity.Value;
            double lightness = EditModeLightness.Value;

            foreach (var line in Series.Lines)
            {
                // 元の色を保存
                _originalLineColors[line.Id] = line.Color;

                // 透明度と薄さを適用
                line.Color = ApplyOpacityAndLightness(line.Color, opacity, lightness);
            }
        }

        /// <summary>
        /// 編集モード解除時にラインの色を元に戻します
        /// </summary>
        private void RestoreOriginalLineColors()
        {
            if (Series == null || !Series.HasLines)
            {
                return;
            }

            foreach (var line in Series.Lines)
            {
                if (_originalLineColors.TryGetValue(line.Id, out OxyColor originalColor))
                {
                    line.Color = originalColor;
                }
            }

            _originalLineColors.Clear();
        }

        /// <summary>
        /// 色に透明度と明度を適用します
        /// </summary>
        /// <param name="color">元の色</param>
        /// <param name="opacity">透明度（0.0～1.0）</param>
        /// <param name="lightness">色の薄さ（0.0=元の色、1.0=白）</param>
        /// <returns>透明度と薄さが適用された色</returns>
        private static OxyColor ApplyOpacityAndLightness(OxyColor color, double opacity, double lightness)
        {
            byte alpha = (byte)(255 * opacity);
            byte r = (byte)(color.R * (1 - lightness) + 255 * lightness);
            byte g = (byte)(color.G * (1 - lightness) + 255 * lightness);
            byte b = (byte)(color.B * (1 - lightness) + 255 * lightness);
            return OxyColor.FromArgb(alpha, r, g, b);
        }

    }
}
