using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 並行座標プロット
    /// </summary>
    public class ParallelCoordinatesSeries : ItemsSeries
    {
        /// <summary>
        /// 次元（軸）のコレクション（キーは次元名、値は次元情報）
        /// </summary>
        public Dictionary<string, ParallelCoordinatesDimension> Dimensions { get; } = [];

        /// <summary>
        /// ラインのコレクション（キーはラインID、値はライン情報）
        /// </summary>
        public Dictionary<string, ParallelCoordinatesLine> Lines { get; } = [];

        /// <summary>
        /// プロットエリアの上下の余白（ピクセル）
        /// </summary>
        public double VerticalMargin { get; set; }

        /// <summary>
        /// プロットエリアの左右の余白（ピクセル）
        /// </summary>
        public double HorizontalMargin { get; set; }

        #region Axis Appearance

        /// <summary>
        /// 軸の色
        /// </summary>
        public OxyColor AxisColor { get; set; }

        /// <summary>
        /// 軸の太さ
        /// </summary>
        public double AxisThickness { get; set; }

        /// <summary>
        /// 軸ラベルを上部に表示するかどうか
        /// </summary>
        public bool ShowAxisLabelsTop { get; set; }

        /// <summary>
        /// 軸ラベルを下部に表示するかどうか
        /// </summary>
        public bool ShowAxisLabelsBottom { get; set; }

        /// <summary>
        /// 軸ラベルのフォントサイズ
        /// </summary>
        public double AxisLabelFontSize { get; set; }

        /// <summary>
        /// 軸ラベルのフォントカラー
        /// </summary>
        public OxyColor AxisLabelFontColor { get; set; }

        /// <summary>
        /// 軸ラベルの縦オフセット（ピクセル）
        /// </summary>
        public double AxisLabelVerticalOffset { get; set; }

        /// <summary>
        /// 軸の目盛りカラー
        /// </summary>
        public OxyColor AxisTickColor { get; set; }

        /// <summary>
        /// 軸の目盛りの太さ
        /// </summary>
        public double AxisTickThickness { get; set; }

        /// <summary>
        /// 軸の目盛ラベルの色
        /// </summary>
        public OxyColor AxisTickLabelColor { get; set; }

        /// <summary>
        /// 軸の目盛ラベルのフォントサイズ
        /// </summary>
        public double AxisTickLabelFontSize { get; set; }

        /// <summary>
        /// 軸の目盛り数
        /// </summary>
        public int AxisTickCount { get; set; }

        /// <summary>
        /// 軸の目盛の長さ
        /// </summary>
        public double AxisTickLength { get; set; }

        /// <summary>
        /// 軸の目盛ラベルの水平オフセット
        /// </summary>
        public double AxisTickLabelHorizontalOffset { get; set; }

        #endregion

        public ParallelCoordinatesSeries(Dictionary<string, ParallelCoordinatesDimension> dimensions)
        {
            Dimensions = dimensions;
            Lines = CreateLinesFromDimensions(dimensions, dimensions.Keys);

            #region Default Appearance

            // 余白のデフォルト値
            VerticalMargin = 30.0;
            HorizontalMargin = 40.0;

            // 軸の外観のデフォルト値
            AxisColor = OxyColors.Black;
            AxisThickness = 1.0;
            ShowAxisLabelsTop = true;
            ShowAxisLabelsBottom = true;
            AxisLabelFontSize = 10.0;
            AxisLabelFontColor = OxyColors.Black;
            AxisLabelVerticalOffset = 15.0;
            AxisTickColor = OxyColors.Black;
            AxisTickThickness = 1.0;
            AxisTickLabelColor = OxyColors.Black;
            AxisTickLabelFontSize = 10.0;
            AxisTickCount = 5;
            AxisTickLength = 5.0;
            AxisTickLabelHorizontalOffset = 10.0;

            #endregion
        }

        #region Initialization

        /// <summary>
        /// ParallelCoordinatesDimensionのコレクションから、ParallelCoordinatesLineのディクショナリを作成します
        /// </summary>
        /// <param name="dimensions">次元のコレクション（キーは次元名、値は次元情報）</param>
        /// <param name="dimensionOrder">次元の順序（この順序で値が並べられます）</param>
        /// <returns>作成されたParallelCoordinatesLineのディクショナリ</returns>
        private static Dictionary<string, ParallelCoordinatesLine> CreateLinesFromDimensions(
            Dictionary<string, ParallelCoordinatesDimension> dimensions,
            IEnumerable<string> dimensionOrder)
        {
            var lines = new Dictionary<string, ParallelCoordinatesLine>();

            if (dimensions.Count == 0)
            {
                return lines;
            }

            // 次元の順序リストを取得
            var orderList = dimensionOrder.ToList();
            if (orderList.Count == 0)
            {
                orderList = dimensions.Keys.ToList();
            }

            // 各次元の値の配列を取得
            var dimensionArrays = new List<double[]>();
            foreach (var key in orderList)
            {
                if (dimensions.TryGetValue(key, out var dimension))
                {
                    dimensionArrays.Add(dimension.Values);
                }
            }

            if (dimensionArrays.Count == 0)
            {
                return lines;
            }

            // すべての次元で同じ長さの値配列を持つことを確認
            var lineCount = dimensionArrays[0].Length;
            if (dimensionArrays.Any(arr => arr.Length != lineCount))
            {
                throw new ArgumentException("すべての次元の値配列は同じ長さである必要があります。", nameof(dimensions));
            }

            // 各インデックスでラインを作成
            for (int i = 0; i < lineCount; i++)
            {
                var values = new double[dimensionArrays.Count];
                for (int j = 0; j < dimensionArrays.Count; j++)
                {
                    values[j] = dimensionArrays[j][i];
                }

                var line = new ParallelCoordinatesLine(values);
                lines[$"Line_{i}"] = line;
            }

            return lines;
        }

        #endregion

        #region overrides

        protected override void UpdateData() { }
        protected override void EnsureAxes() { }
        protected override void SetDefaultValues() { }
        protected override void UpdateMaxMin() { }
        protected override void UpdateAxisMaxMin() { }
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox) { }

        /// <summary>
        /// 軸が必要かどうかを判定します
        /// </summary>
        /// <returns>常にfalse（独自の軸描画を使用するため）</returns>
        protected override bool AreAxesRequired() => false;

        /// <summary>
        /// 指定した軸を使用しているかどうかを判定します
        /// </summary>
        /// <param name="axis">確認する軸</param>
        /// <returns>常にfalse（独自の軸描画を使用するため）</returns>
        protected override bool IsUsing(Axis axis) => false;

        /// <summary>
        /// シリーズをレンダリングします
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        public override void Render(IRenderContext rc)
        {
            if (Dimensions.Count == 0 || Lines.Count == 0)
            {
                return;
            }

            RenderAxes(rc);
            RenderDataLines(rc);
        }

        #endregion

        #region Rendering Axes

        /// <summary>
        /// 軸を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderAxes(IRenderContext rc)
        {
            if (Dimensions.Count == 0)
            {
                return;
            }

            for (int i = 0; i < Dimensions.Count; i++)
            {
                var dimension = Dimensions.ElementAt(i).Value;

                // 各軸のX座標を計算（軸間隔を等分に配置）
                double x = GetAxisXPosition(i);

                // 軸の垂直線を描画（余白を考慮）
                var topPoint = new ScreenPoint(x, GetAxisTopPosition());
                var bottomPoint = new ScreenPoint(x, GetAxisBottomPosition());

                rc.DrawLine([topPoint, bottomPoint], AxisColor, AxisThickness, EdgeRenderingMode.Automatic);

                // 軸のラベルを描画
                RenderAxisLabels(rc, dimension.Label, x);

                // 軸の目盛りを描画
                RenderAxisTicks(rc, dimension, x);
            }
        }

        /// <summary>
        /// 軸のラベルを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="lebel">ラベル文字列</param>
        /// <param name="x">軸のX座標(スクリーン位置)</param>
        private void RenderAxisLabels(IRenderContext rc, string lebel, double x)
        {
            // 軸のタイトルを上部に描画
            if (ShowAxisLabelsTop)
            {
                var titleTopPoint = new ScreenPoint(x, GetAxisTopPosition() - AxisLabelVerticalOffset);
                rc.DrawText(
                    titleTopPoint,
                    lebel,
                    AxisLabelFontColor,
                    fontSize: AxisLabelFontSize,
                    fontWeight: FontWeights.Bold,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Bottom
                );
            }

            // 軸のタイトルを下部に描画
            if (ShowAxisLabelsBottom)
            {
               var titleBottomPoint = new ScreenPoint(x, GetAxisBottomPosition() + AxisLabelVerticalOffset);
                rc.DrawText(
                    titleBottomPoint,
                    lebel,
                    AxisLabelFontColor,
                    fontSize: AxisLabelFontSize,
                    fontWeight: FontWeights.Bold,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Top);
            }
        }

        /// <summary>
        /// 軸の目盛りを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="dimension">次元情報</param>
        /// <param name="x">軸のX座標(スクリーン位置)</param>
        private void RenderAxisTicks(IRenderContext rc, ParallelCoordinatesDimension dimension, double x)
        {
            // 垂直方向の利用可能な高さを計算（上下の余白を除く）
            double availableHeight = GetAvailableHeight();

            // プロット領域の上下端の座標を計算
            double plotBottom = GetAxisBottomPosition();

            // 元のRangeに基づいて目盛りを描画
            for (int t = 0; t <= AxisTickCount; t++)
            {
                // 目盛りの値を計算（元のRangeの最小値から最大値まで等間隔で分割）
                double value = dimension.MinValue + (dimension.MaxValue - dimension.MinValue) * t / AxisTickCount;

                // 値を元のRangeに基づいて0-1の範囲に正規化
                double normalizedValue = (value - dimension.MinValue) / (dimension.MaxValue - dimension.MinValue);

                // 正規化された値をY座標に変換（下から上に向かって配置）
                double tickYPosition = plotBottom - normalizedValue * availableHeight;

                // 目盛り線
                rc.DrawLine(
                    [new ScreenPoint(x - AxisTickLength, tickYPosition), new ScreenPoint(x + AxisTickLength, tickYPosition)],
                    AxisTickColor,
                    AxisTickThickness,
                    EdgeRenderingMode.Automatic
                );

                // 目盛りラベル
                rc.DrawText(
                    new ScreenPoint(x + AxisTickLabelHorizontalOffset, tickYPosition),
                    value.ToString("F1"),
                    AxisTickLabelColor,
                    fontSize: AxisLabelFontSize,
                    fontWeight: FontWeights.Normal,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Left,
                    verticalAlignment: VerticalAlignment.Middle
                );
            }
        }

        #endregion

        #region Rendering Data Lines

        /// <summary>
        /// 全てのデータ線を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderDataLines(IRenderContext rc)
        {
            if (Lines.Count == 0 || Dimensions.Count == 0)
            {
                return;
            }

            // 各ラインを描画
            foreach (var lineEntry in Lines)
            {
                var line = lineEntry.Value;
                if (line.IsVisible)
                {
                    RenderSingleDataLine(rc, line);
                }
            }
        }

        /// <summary>
        /// 単一のデータ線を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="line">描画するライン</param>
        private void RenderSingleDataLine(IRenderContext rc, ParallelCoordinatesLine line)
        {
            if (line.Values.Length != Dimensions.Count)
            {
                return;
            }

            // 利用可能な高さを取得
            double availableHeight = GetAvailableHeight();
            double plotBottom = GetAxisBottomPosition();

            // 各軸での座標点を計算
            var points = new List<ScreenPoint>();
            for (int i = 0; i < Dimensions.Count; i++)
            {
                var dimension = Dimensions.ElementAt(i).Value;

                // 軸のX座標を取得
                double x = GetAxisXPosition(i);

                // ラインの値を取得
                double value = line.Values[i];

                // 値を正規化
                double normalizedValue = (value - dimension.MinValue) / (dimension.MaxValue - dimension.MinValue);
                
                // 正規化された値をY座標に変換（下から上に向かって配置）
                double y = plotBottom - normalizedValue * availableHeight;

                points.Add(new ScreenPoint(x, y));
            }

            // 点が2つ以上ある場合のみ線を描画
            if (points.Count >= 2)
            {
                rc.DrawLine(
                    points,
                    line.Color,
                    line.StrokeThickness,
                    EdgeRenderingMode.Automatic
                );
            }
        }

        #endregion

        #region Rendering Tooltips

        /// <summary>
        /// 固定ツールチップを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderTooltip(IRenderContext rc)
        {

        }

        #endregion

        #region Private Shared Methods

        private double GetAxisTopPosition() => PlotModel.PlotArea.Top + VerticalMargin;
        private double GetAxisBottomPosition() => PlotModel.PlotArea.Bottom - VerticalMargin;
        private double GetAvailableWidth() => PlotModel.PlotArea.Width - 2 * HorizontalMargin;
        private double GetAxisXPosition(int axisIndex)
            => PlotModel.PlotArea.Left + HorizontalMargin + GetAvailableWidth() * axisIndex / (Dimensions.Count - 1);

        private double GetAvailableHeight() => PlotModel.PlotArea.Height - 2 * VerticalMargin;

        #endregion
    }
}
