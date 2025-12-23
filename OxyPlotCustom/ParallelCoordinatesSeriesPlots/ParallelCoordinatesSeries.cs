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

        #region ColorMap

        /// <summary>
        /// カラーマップに使用する軸の名前（nullの場合はカラーマップを使用しない）
        /// </summary>
        public string? ColorMapDimensionName { get; set; }

        /// <summary>
        /// カラーマップのパレット
        /// </summary>
        public OxyPalette ColorMap { get; set; }

        #endregion

        #region Highlight

        /// <summary>
        /// ハイライトされているラインのID（nullの場合はハイライトなし）
        /// </summary>
        public string? HighlightedLineId { get; set; }

        /// <summary>
        /// ハイライト時のラインの太さ
        /// </summary>
        public double HighlightStrokeThickness { get; set; }

        /// <summary>
        /// ヒットテストの許容範囲（ピクセル）
        /// </summary>
        public double HitTestTolerance { get; set; }

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

            // カラーマップのデフォルト値
            ColorMapDimensionName = null;
            ColorMap = OxyPalettes.Jet(256);

            // ハイライトのデフォルト値
            HighlightedLineId = null;
            HighlightStrokeThickness = 3.0;
            HitTestTolerance = 10.0;

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

            // ラインの色をカラーマップから取得して設定
            UpdateLineColorsFromColorMap();

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

            // まずハイライトされていないラインを描画
            foreach (var lineEntry in Lines)
            {
                var line = lineEntry.Value;
                if (line.IsVisible && lineEntry.Key != HighlightedLineId)
                {
                    RenderSingleDataLine(rc, line);
                }
            }

            // 最後にハイライトされたラインを描画（上に重ねる）
            if (!string.IsNullOrEmpty(HighlightedLineId) 
                && Lines.TryGetValue(HighlightedLineId, out var highlightedLine) 
                && highlightedLine.IsVisible)
            {
                RenderSingleDataLine(rc, highlightedLine, isHighlighted: true);
            }
        }

        /// <summary>
        /// 単一のデータ線を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="line">描画するライン</param>
        /// <param name="isHighlighted">ハイライトされているかどうか</param>
        private void RenderSingleDataLine(IRenderContext rc, ParallelCoordinatesLine line, bool isHighlighted = false)
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
                // ハイライト時の色と太さを決定
                var color = line.Color;
                var thickness = isHighlighted ? HighlightStrokeThickness : line.StrokeThickness;

                rc.DrawLine(
                    points,
                    color,
                    thickness,
                    EdgeRenderingMode.Automatic
                );
            }
        }

        #endregion

        #region Rendering Colormap

        /// <summary>
        /// カラーマップからラインの色を更新します
        /// </summary>
        private void UpdateLineColorsFromColorMap()
        {
            if (string.IsNullOrEmpty(ColorMapDimensionName) || !Dimensions.TryGetValue(ColorMapDimensionName, out var colorMapDimension))
            {
                return;
            }

            // カラーマップに使用する軸のインデックスを取得
            var colorMapDimensionIndex = Array.IndexOf(Dimensions.Keys.ToArray(), ColorMapDimensionName);

            if (colorMapDimensionIndex < 0)
            {
                return;
            }

            // 各ラインの色をカラーマップから取得
            foreach (var lineEntry in Lines)
            {
                var line = lineEntry.Value;
                if (line.Values.Length > colorMapDimensionIndex)
                {
                    double value = line.Values[colorMapDimensionIndex];
                    // 値を0-1の範囲に正規化
                    double normalizedValue = (value - colorMapDimension.MinValue) / (colorMapDimension.MaxValue - colorMapDimension.MinValue);
                    normalizedValue = Math.Max(0.0, Math.Min(1.0, normalizedValue)); // 0-1の範囲にクランプ
                    
                    // カラーマップから色を取得（インデックスを計算）
                    int colorIndex = (int)(normalizedValue * (ColorMap.Colors.Count - 1));
                    colorIndex = Math.Max(0, Math.Min(ColorMap.Colors.Count - 1, colorIndex));
                    line.Color = ColorMap.Colors[colorIndex];
                }
            }
        }

        /// <summary>
        /// カラーマップを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderColormap(IRenderContext rc)
        {
            // カラーバーの描画は一旦無効化
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

        #region Hit Test

        /// <summary>
        /// 指定したスクリーン座標から最も近いラインのIDを取得します
        /// </summary>
        /// <param name="point">スクリーン座標</param>
        /// <returns>最も近いラインのID。見つからない場合はnull</returns>
        public string? GetNearestLineId(ScreenPoint point)
        {
            if (Lines.Count == 0 || Dimensions.Count == 0)
            {
                return null;
            }

            string? nearestLineId = null;
            double minDistanceSquared = double.MaxValue;
            double toleranceSquared = HitTestTolerance * HitTestTolerance; // 平方距離で比較

            foreach (var lineEntry in Lines)
            {
                var line = lineEntry.Value;
                if (!line.IsVisible)
                {
                    continue;
                }

                // 平方距離で比較して平方根計算を削減
                double distance = GetDistanceToLine(point, line, minDistanceSquared);
                
                if (distance != double.MaxValue)
                {
                    double distanceSquared = distance * distance;
                    if (distanceSquared < minDistanceSquared && distanceSquared <= toleranceSquared)
                    {
                        minDistanceSquared = distanceSquared;
                        nearestLineId = lineEntry.Key;
                    }
                }
            }

            return nearestLineId;
        }

        /// <summary>
        /// 指定したポイントからラインまでの最短距離を計算します
        /// </summary>
        /// <param name="point">スクリーン座標</param>
        /// <param name="line">ライン</param>
        /// <param name="currentMinDistanceSquared">現在の最小距離の2乗（早期終了用、負の場合は無視）</param>
        /// <returns>ポイントからラインまでの距離（ピクセル）。currentMinDistanceSquaredが指定され、それより大きい場合はdouble.MaxValue</returns>
        private double GetDistanceToLine(ScreenPoint point, ParallelCoordinatesLine line, double currentMinDistanceSquared = -1)
        {
            if (line.Values.Length != Dimensions.Count)
            {
                return double.MaxValue;
            }

            // 利用可能な高さを取得
            double availableHeight = GetAvailableHeight();
            double plotBottom = GetAxisBottomPosition();

            // 各軸での座標点を計算（配列を使用してGC負荷を削減）
            int dimensionCount = Dimensions.Count;
            if (dimensionCount < 2)
            {
                return double.MaxValue;
            }

            // 各セグメントに対する距離を計算し、最小値を返す
            // 平方距離で比較して平方根計算を削減
            double minDistanceSquared = double.MaxValue;
            
            // 前のポイントを保持して配列割り当てを回避
            ScreenPoint? prevPoint = null;
            int dimensionIndex = 0;
            
            foreach (var dimensionEntry in Dimensions)
            {
                var dimension = dimensionEntry.Value;
                double x = GetAxisXPosition(dimensionIndex);
                double value = line.Values[dimensionIndex];
                double normalizedValue = (value - dimension.MinValue) / (dimension.MaxValue - dimension.MinValue);
                double y = plotBottom - normalizedValue * availableHeight;
                
                var currentPoint = new ScreenPoint(x, y);
                
                if (prevPoint.HasValue)
                {
                    double distanceSquared = GetDistanceSquaredToLineSegment(point, prevPoint.Value, currentPoint);
                    
                    // 早期終了：既に現在の最小距離より大きい場合
                    if (currentMinDistanceSquared > 0 && distanceSquared >= currentMinDistanceSquared)
                    {
                        return double.MaxValue;
                    }
                    
                    if (distanceSquared < minDistanceSquared)
                    {
                        minDistanceSquared = distanceSquared;
                    }
                }
                
                prevPoint = currentPoint;
                dimensionIndex++;
            }

            return Math.Sqrt(minDistanceSquared);
        }

        /// <summary>
        /// ポイントから線分までの最短距離の2乗を計算します（平方根計算を避けるため）
        /// </summary>
        /// <param name="point">ポイント</param>
        /// <param name="lineStart">線分の開始点</param>
        /// <param name="lineEnd">線分の終了点</param>
        /// <returns>ポイントから線分までの距離の2乗（ピクセル^2）</returns>
        private static double GetDistanceSquaredToLineSegment(ScreenPoint point, ScreenPoint lineStart, ScreenPoint lineEnd)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;
            double lengthSquared = dx * dx + dy * dy;

            if (lengthSquared == 0)
            {
                // 線分が点の場合
                double px = point.X - lineStart.X;
                double py = point.Y - lineStart.Y;
                return px * px + py * py;
            }

            // 線分上の最も近い点の位置を計算
            double t = Math.Max(0, Math.Min(1, ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / lengthSquared));

            // 最も近い点
            double closestX = lineStart.X + t * dx;
            double closestY = lineStart.Y + t * dy;

            // 距離の2乗を計算（平方根を避ける）
            double distanceX = point.X - closestX;
            double distanceY = point.Y - closestY;
            return distanceX * distanceX + distanceY * distanceY;
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
