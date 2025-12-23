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
        /// 次元（軸）の配列（順序は入力の順を保持）
        /// </summary>
        public ParallelCoordinatesDimension[] Dimensions { get; }

        /// <summary>
        /// 次元ラベルの配列（順序はDimensionsと同じ）
        /// </summary>
        public string[] DimensionLabels { get; }

        /// <summary>
        /// ラインの配列
        /// </summary>
        public ParallelCoordinatesLine[] Lines { get; private set; }

        /// <summary>
        /// 次元（軸）が存在するかどうか
        /// </summary>
        public bool HasDimensions => Dimensions != null && Dimensions.Length > 0;

        /// <summary>
        /// ラインが存在するかどうか
        /// </summary>
        public bool HasLines => Lines != null && Lines.Length > 0;

        /// <summary>
        /// インタラクションハンドラーのリスト
        /// </summary>
        public List<IParallelCoordinatesInteractionHandler> InteractionHandlers { get; set; }

        #region Plot Area Margins

        /// <summary>
        /// プロットエリアの上下の余白（ピクセル）
        /// </summary>
        public double VerticalMargin { get; set; } = 30.0;

        /// <summary>
        /// プロットエリアの左右の余白（ピクセル）
        /// </summary>
        public double HorizontalMargin { get; set; } = 40.0;

        #endregion

        #region Axis Appearance

        /// <summary>
        /// 軸の色
        /// </summary>
        public OxyColor AxisColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// 軸の太さ
        /// </summary>
        public double AxisThickness { get; set; } = 1.0;

        /// <summary>
        /// 軸ラベルを上部に表示するかどうか
        /// </summary>
        public bool ShowAxisLabelsTop { get; set; } = true;

        /// <summary>
        /// 軸ラベルを下部に表示するかどうか
        /// </summary>
        public bool ShowAxisLabelsBottom { get; set; } = true;

        /// <summary>
        /// 軸ラベルのフォントサイズ
        /// </summary>
        public double AxisLabelFontSize { get; set; } = 10.0;

        /// <summary>
        /// 軸ラベルのフォントカラー
        /// </summary>
        public OxyColor AxisLabelFontColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// 軸ラベルの縦オフセット（ピクセル）
        /// </summary>
        public double AxisLabelVerticalOffset { get; set; } = 15.0;

        /// <summary>
        /// 軸の目盛りカラー
        /// </summary>
        public OxyColor AxisTickColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// 軸の目盛りの太さ
        /// </summary>
        public double AxisTickThickness { get; set; } = 1.0;

        /// <summary>
        /// 軸の目盛ラベルの色
        /// </summary>
        public OxyColor AxisTickLabelColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// 軸の目盛ラベルのフォントサイズ
        /// </summary>
        public double AxisTickLabelFontSize { get; set; } = 10.0;

        /// <summary>
        /// 軸の目盛り数
        /// </summary>
        public int AxisTickCount { get; set; } = 5;

        /// <summary>
        /// 軸の目盛の長さ
        /// </summary>
        public double AxisTickLength { get; set; } = 5.0;

        /// <summary>
        /// 軸の目盛ラベルの水平オフセット
        /// </summary>
        public double AxisTickLabelHorizontalOffset { get; set; } = 10.0;

        #endregion

        #region ColorMap

        /// <summary>
        /// カラーマップに使用する軸の名前（nullの場合はカラーマップを使用しない）
        /// </summary>
        public string? ColorMapDimensionName { get; set; }

        /// <summary>
        /// カラーマップのパレット
        /// </summary>
        public OxyPalette ColorMap { get; set; } = OxyPalettes.Jet(256);

        /// <summary>
        /// カラーマップを表示するかどうか
        /// </summary>
        public bool ShowColorMap { get; set; } = false;

        /// <summary>
        /// カラーマップの幅（ピクセル）
        /// </summary>
        public double ColorMapWidth { get; set; } = 30.0;

        /// <summary>
        /// カラーマップと軸の間の余白（ピクセル）
        /// </summary>
        public double ColorMapMargin { get; set; } = 40.0;

        /// <summary>
        /// カラーマップのラベルのフォントサイズ
        /// </summary>
        public double ColorMapLabelFontSize { get; set; } = 10.0;

        /// <summary>
        /// カラーマップのラベルのフォントカラー
        /// </summary>
        public OxyColor ColorMapLabelFontColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// カラーマップの目盛り数
        /// </summary>
        public int ColorMapTickCount { get; set; } = 5;

        /// <summary>
        /// カラーマップの目盛りラベルの水平オフセット
        /// </summary>
        public double ColorMapTickLabelHorizontalOffset { get; set; } = 5.0;

        #endregion

        // TODO ここにハイライトのプロパティを追加するのはおかしい。

        #region Highlight

        /// <summary>
        /// ハイライトされているラインのID（nullの場合はハイライトなし）
        /// </summary>
        public string? HighlightedLineId { get; set; }

        /// <summary>
        /// ハイライト時のラインの太さ
        /// </summary>
        public double HighlightStrokeThickness { get; set; } = 3.0;

        /// <summary>
        /// ヒットテストの許容範囲（ピクセル）
        /// </summary>
        public double HitTestTolerance { get; set; } = 10.0;

        #endregion

        #region Edit Mode

        // TODO ここに編集モードのプロパティを追加するのはおかしい。

        /// <summary>
        /// 編集モードが有効かどうか（編集モード時は既存のプロットに透明度を適用）
        /// </summary>
        public bool IsEditMode { get; set; } = false;

        /// <summary>
        /// 編集モード時の既存プロットの透明度（0.0～1.0、デフォルトは0.3）
        /// </summary>
        public double EditModeOpacity { get; set; } = 0.3;

        #endregion

        public ParallelCoordinatesSeries(IEnumerable<ParallelCoordinatesDimension> dimensions)
        {
            Dimensions = dimensions?.ToArray() ?? Array.Empty<ParallelCoordinatesDimension>();
            DimensionLabels = Dimensions.Select(d => d.Label).ToArray();
            Lines = CreateLinesFromDimensions(Dimensions);

            InteractionHandlers = new List<IParallelCoordinatesInteractionHandler>();
        }

        #region Initialization

        /// <summary>
        /// ParallelCoordinatesDimensionの配列から、ParallelCoordinatesLineの配列を作成します
        /// </summary>
        /// <param name="dimensions">次元の配列（順序は入力順）</param>
        /// <returns>作成されたParallelCoordinatesLineの配列</returns>
        private static ParallelCoordinatesLine[] CreateLinesFromDimensions(IEnumerable<ParallelCoordinatesDimension> dimensions)
        {
            var dimensionsList = dimensions?.ToList() ?? new List<ParallelCoordinatesDimension>();
            
            if (dimensionsList.Count == 0)
            {
                return Array.Empty<ParallelCoordinatesLine>();
            }

            // 各次元の値配列を順序通りに取得
            var dimensionArrays = new List<double[]>(dimensionsList.Count);
            for (int i = 0; i < dimensionsList.Count; i++)
            {
                dimensionArrays.Add(dimensionsList[i].Values);
            }

            // すべての次元で同じ長さの値配列を持つことを確認
            var lineCount = dimensionArrays[0].Length;
            if (dimensionArrays.Any(arr => arr.Length != lineCount))
            {
                throw new ArgumentException("すべての次元の値配列は同じ長さである必要があります。", nameof(dimensions));
            }

            // 各インデックスでラインを作成
            var lines = new ParallelCoordinatesLine[lineCount];
            for (int i = 0; i < lineCount; i++)
            {
                var values = new double[dimensionArrays.Count];
                for (int j = 0; j < dimensionArrays.Count; j++)
                {
                    values[j] = dimensionArrays[j][i];
                }

                lines[i] = new ParallelCoordinatesLine($"Line_{i}", values);
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
            if (!HasDimensions || !HasLines)
            {
                return;
            }

            // ラインの色をカラーマップから取得して設定
            UpdateLineColorsFromColorMap();

            RenderDataLines(rc);
            RenderAxes(rc);

            // カラーマップを描画
            if (ShowColorMap && !string.IsNullOrEmpty(ColorMapDimensionName))
            {
                RenderColormap(rc);
            }

            // インタラクションハンドラーで定義された描画の追加処理
            foreach (var handler in InteractionHandlers)
            {
                if (handler.IsEnabled)
                {
                    handler.Render(this, rc);
                }
            }
        }

        #endregion

        #region Rendering Axes

        /// <summary>
        /// 軸を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderAxes(IRenderContext rc)
        {
            if (!HasDimensions)
            {
                return;
            }

            for (int i = 0; i < Dimensions.Length; i++)
            {
                var dimension = Dimensions[i];

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
                double normalizedValue = dimension.NormalizeValue(value);

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
                    FormatTickLabel(value),
                    AxisTickLabelColor,
                    fontSize: AxisTickLabelFontSize,
                    fontWeight: FontWeights.Normal,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Left,
                    verticalAlignment: VerticalAlignment.Middle
                );
            }
        }

        /// <summary>
        /// 数値の大きさに応じて適切なフォーマットを選択して文字列に変換
        /// 大きな数値や小さな数値は指数表記を使用し、それ以外は固定小数点表記を使用
        /// </summary>
        /// <param name="value">フォーマットする数値</param>
        /// <returns>フォーマットされた文字列</returns>
        private static string FormatTickLabel(double value)
        {
            double absValue = Math.Abs(value);
            
            // 絶対値が1000以上または0.001未満の場合は指数表記を使用
            if (absValue >= 1000.0 || (absValue > 0.0 && absValue < 0.001))
            {
                return value.ToString("G2");
            }
            else
            {
                // それ以外は固定小数点表記（小数点以下1桁）
                return value.ToString("F1");
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
            if (!HasLines || !HasDimensions)
            {
                return;
            }

            // まずハイライトされていないラインを描画
            foreach (var line in Lines)
            {
                if (line.IsVisible && line.Id != HighlightedLineId)
                {
                    RenderSingleDataLine(rc, line);
                }
            }

            // 最後にハイライトされたラインを描画（上に重ねる）
            if (!string.IsNullOrEmpty(HighlightedLineId))
            {
                var highlightedLine = FindLineById(HighlightedLineId);
                if (highlightedLine?.IsVisible == true)
                {
                    RenderSingleDataLine(rc, highlightedLine, isHighlighted: true);
                }
            }
        }

        private ParallelCoordinatesLine? FindLineById(string lineId)
        {
            foreach (var line in Lines)
            {
                if (line.Id == lineId)
                {
                    return line;
                }
            }

            return null;
        }

        /// <summary>
        /// 単一のデータ線を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="line">描画するライン</param>
        /// <param name="isHighlighted">ハイライトされているかどうか</param>
        private void RenderSingleDataLine(IRenderContext rc, ParallelCoordinatesLine line, bool isHighlighted = false)
        {
            if (line.Values.Length != Dimensions.Length)
            {
                return;
            }

            // フィルタリング：すべての次元で範囲内かどうかをチェック
            bool isFiltered = IsLineFiltered(line);

            // 利用可能な高さを取得
            double availableHeight = GetAvailableHeight();
            double plotBottom = GetAxisBottomPosition();

            // 各軸での座標点を計算
            var points = new List<ScreenPoint>();
            for (int i = 0; i < Dimensions.Length; i++)
            {
                var dimension = Dimensions[i];

                // 軸のX座標を取得
                double x = GetAxisXPosition(i);

                // ラインの値を取得
                double value = line.Values[i];

                // 値を元のRange（MinValue～MaxValue）に基づいて正規化
                // 目盛りは常に元の範囲を表示するため、ラインも同じ範囲で計算
                double normalizedValue = dimension.NormalizeValue(value);
                
                // 正規化された値をY座標に変換（下から上に向かって配置）
                double y = plotBottom - normalizedValue * availableHeight;

                points.Add(new ScreenPoint(x, y));
            }

            // 点が2つ以上ある場合のみ線を描画
            if (points.Count >= 2)
            {
                // ハイライト時の色と太さを決定
                var color = line.Color;
                
                // フィルタ外の場合は透明度を適用
                if (isFiltered)
                {
                    // 透明度設定
                    byte alpha = (byte)(255 * 0.1);
                    color = OxyColor.FromArgb(alpha, color.R, color.G, color.B);
                }
                // 編集モード時で、ハイライトされていないラインには透明度を適用
                else if (IsEditMode && !isHighlighted)
                {
                    byte alpha = (byte)(255 * EditModeOpacity);
                    color = OxyColor.FromArgb(alpha, color.R, color.G, color.B);
                }
                
                var thickness = isHighlighted ? HighlightStrokeThickness : line.StrokeThickness;

                rc.DrawLine(
                    points,
                    color,
                    thickness,
                    EdgeRenderingMode.Automatic
                );
            }
        }

        /// <summary>
        /// ラインがフィルタリング条件を満たしているかどうかを判定します
        /// </summary>
        /// <param name="line">判定するライン</param>
        /// <returns>フィルタ外（範囲外）の場合はtrue、範囲内の場合はfalse</returns>
        private bool IsLineFiltered(ParallelCoordinatesLine line)
        {
            if (line.Values.Length != Dimensions.Length)
            {
                return true;
            }

            // すべての次元で範囲内かどうかをチェック
            for (int i = 0; i < Dimensions.Length; i++)
            {
                var dimension = Dimensions[i];
                double value = line.Values[i];

                // DisplayMinValueとDisplayMaxValueの範囲外の場合はフィルタ外
                if (value < dimension.DisplayMinValue || value > dimension.DisplayMaxValue)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Rendering Colormap

        /// <summary>
        /// カラーマップからラインの色を更新します
        /// </summary>
        private void UpdateLineColorsFromColorMap()
        {
            var colorMapDimensionIndex = GetDimensionIndexByLabel(ColorMapDimensionName);
            if (!colorMapDimensionIndex.HasValue)
            {
                return;
            }

            var colorMapDimension = Dimensions[colorMapDimensionIndex.Value];

            // 各ラインの色をカラーマップから取得
            int index = colorMapDimensionIndex.Value;
            foreach (var line in Lines)
            {
                if (line.Values.Length > index)
                {
                    double value = line.Values[index];
                    // 値を0-1の範囲に正規化
                    double normalizedValue = colorMapDimension.NormalizeValue(value);
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
            var colorMapDimensionIndex = GetDimensionIndexByLabel(ColorMapDimensionName);
            if (!colorMapDimensionIndex.HasValue)
            {
                return;
            }

            var colorMapDimension = Dimensions[colorMapDimensionIndex.Value];

            // カラーマップの位置を計算（一番右側）
            double colorMapLeft = GetColorMapLeftPosition();
            double colorMapRight = colorMapLeft + ColorMapWidth;
            double colorMapTop = GetAxisTopPosition();
            double colorMapBottom = GetAxisBottomPosition();
            double availableHeight = GetAvailableHeight();

            // カラーマップのバーを描画
            int colorCount = ColorMap.Colors.Count;
            double segmentHeight = availableHeight / colorCount;

            for (int i = 0; i < colorCount; i++)
            {
                double yTop = colorMapBottom - (i + 1) * segmentHeight;
                double yBottom = colorMapBottom - i * segmentHeight;

                // 各色のセグメントを描画
                var rect = new OxyRect(colorMapLeft, yTop, ColorMapWidth, yBottom - yTop);
                rc.DrawRectangle(rect, ColorMap.Colors[i], OxyColors.Undefined, 0, EdgeRenderingMode.Automatic);
            }

            // カラーマップの枠線を描画
            var framePoints = new[]
            {
                new ScreenPoint(colorMapLeft, colorMapTop),
                new ScreenPoint(colorMapRight, colorMapTop),
                new ScreenPoint(colorMapRight, colorMapBottom),
                new ScreenPoint(colorMapLeft, colorMapBottom),
                new ScreenPoint(colorMapLeft, colorMapTop)
            };
            rc.DrawLine(framePoints, AxisColor, AxisThickness, EdgeRenderingMode.Automatic);

            // カラーマップのラベルを描画（上部）
            if (ShowAxisLabelsTop)
            {
                var labelPoint = new ScreenPoint(
                    colorMapLeft + ColorMapWidth / 2,
                    colorMapTop - AxisLabelVerticalOffset
                );
                rc.DrawText(
                    labelPoint,
                    colorMapDimension.Label,
                    ColorMapLabelFontColor,
                    fontSize: ColorMapLabelFontSize,
                    fontWeight: FontWeights.Bold,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Bottom
                );
            }

            // カラーマップのラベルを描画（下部）
            if (ShowAxisLabelsBottom)
            {
                var labelPoint = new ScreenPoint(
                    colorMapLeft + ColorMapWidth / 2,
                    colorMapBottom + AxisLabelVerticalOffset
                );
                rc.DrawText(
                    labelPoint,
                    colorMapDimension.Label,
                    ColorMapLabelFontColor,
                    fontSize: ColorMapLabelFontSize,
                    fontWeight: FontWeights.Bold,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Top
                );
            }

            // カラーマップの目盛りを描画
            double plotBottom = GetAxisBottomPosition();
            for (int t = 0; t <= ColorMapTickCount; t++)
            {
                // 目盛りの値を計算
                double value = colorMapDimension.MinValue + (colorMapDimension.MaxValue - colorMapDimension.MinValue) * t / ColorMapTickCount;

                // 値を0-1の範囲に正規化
                double normalizedValue = colorMapDimension.NormalizeValue(value);

                // 正規化された値をY座標に変換（下から上に向かって配置）
                double tickYPosition = plotBottom - normalizedValue * availableHeight;

                // 目盛り線
                rc.DrawLine(
                    [new ScreenPoint(colorMapRight, tickYPosition), new ScreenPoint(colorMapRight + AxisTickLength, tickYPosition)],
                    AxisTickColor,
                    AxisTickThickness,
                    EdgeRenderingMode.Automatic
                );

                // 目盛りラベル
                rc.DrawText(
                    new ScreenPoint(colorMapRight + AxisTickLength + ColorMapTickLabelHorizontalOffset, tickYPosition),
                    value.ToString("F1"),
                    AxisTickLabelColor,
                    fontSize: AxisTickLabelFontSize,
                    fontWeight: FontWeights.Normal,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Left,
                    verticalAlignment: VerticalAlignment.Middle
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

        #region Hit Test

        /// <summary>
        /// 指定したスクリーン座標から最も近いラインのIDを取得します
        /// </summary>
        /// <param name="point">スクリーン座標</param>
        /// <returns>最も近いラインのID。見つからない場合はnull</returns>
        public string? GetNearestLineId(ScreenPoint point)
        {
            if (!HasLines || !HasDimensions)
            {
                return null;
            }

            string? nearestLineId = null;
            double minDistanceSquared = double.MaxValue;
            double toleranceSquared = HitTestTolerance * HitTestTolerance; // 平方距離で比較

            foreach (var line in Lines)
            {
                if (!line.IsVisible)
                {
                    continue;
                }

                // フィルタリングされたラインは除外
                if (IsLineFiltered(line))
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
                        nearestLineId = line.Id;
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
            if (line.Values.Length != Dimensions.Length)
            {
                return double.MaxValue;
            }

            // 利用可能な高さを取得
            double availableHeight = GetAvailableHeight();
            double plotBottom = GetAxisBottomPosition();

            // 各軸での座標点を計算（配列を使用してGC負荷を削減）
            int dimensionCount = Dimensions.Length;
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
            
            foreach (var dimension in Dimensions)
            {
                double x = GetAxisXPosition(dimensionIndex);
                double value = line.Values[dimensionIndex];
                double normalizedValue = dimension.NormalizeValue(value);
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

        #region Public Helper Methods

        /// <summary>
        /// 軸の上端位置を取得します
        /// </summary>
        public double GetAxisTopPosition() => PlotModel.PlotArea.Top + VerticalMargin;

        /// <summary>
        /// 軸の下端位置を取得します
        /// </summary>
        public double GetAxisBottomPosition() => PlotModel.PlotArea.Bottom - VerticalMargin;

        /// <summary>
        /// 利用可能な幅を取得します（カラーマップがある場合はその分を差し引く）
        /// </summary>
        public double GetAvailableWidth()
        {
            double baseWidth = PlotModel.PlotArea.Width - 2 * HorizontalMargin;
            if (ShowColorMap && !string.IsNullOrEmpty(ColorMapDimensionName))
            {
                baseWidth -= ColorMapWidth + ColorMapMargin;
            }
            return baseWidth;
        }

        /// <summary>
        /// 軸のX座標を取得します
        /// </summary>
        /// <param name="axisIndex">軸のインデックス</param>
        /// <returns>軸のX座標</returns>
        public double GetAxisXPosition(int axisIndex)
        {
            int denominator = Math.Max(1, Dimensions.Length - 1);
            return PlotModel.PlotArea.Left + HorizontalMargin + GetAvailableWidth() * axisIndex / denominator;
        }

        /// <summary>
        /// 利用可能な高さを取得します
        /// </summary>
        public double GetAvailableHeight() => PlotModel.PlotArea.Height - 2 * VerticalMargin;

        /// <summary>
        /// カラーマップの左端位置を取得します
        /// </summary>
        public double GetColorMapLeftPosition()
        {
            double baseRight = PlotModel.PlotArea.Left + PlotModel.PlotArea.Width - HorizontalMargin;
            return baseRight - ColorMapWidth;
        }

        /// <summary>
        /// ラベル名から次元（軸）のインデックスを取得します
        /// </summary>
        /// <param name="label">次元のラベル名</param>
        /// <returns>見つかった場合はインデックス、見つからない場合はnull</returns>
        public int? GetDimensionIndexByLabel(string? label)
        {
            if (string.IsNullOrEmpty(label))
            {
                return null;
            }

            int index = Array.FindIndex(DimensionLabels, l => l == label);
            return index >= 0 ? index : null;
        }

        #endregion
    }
}
