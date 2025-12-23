using OxyPlot;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 編集モードで点を追加して一時的なラインを作成するハンドラー
    /// </summary>
    public class PointAdditionHandler : IParallelCoordinatesInteractionHandler
    {
        /// <summary>
        /// ハンドラーが有効かどうか
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 編集モードが有効かどうか
        /// </summary>
        public bool IsEditMode { get; set; } = false;

        /// <summary>
        /// 追加された点のコレクション（キーは次元のインデックス、値はその次元での値）
        /// </summary>
        private Dictionary<int, double> _addedPoints = new Dictionary<int, double>();

        /// <summary>
        /// 一時的なライン（編集モードで作成されたライン）
        /// </summary>
        private ParallelCoordinatesLine? _temporaryLine;

        /// <summary>
        /// 現在ドラッグ中の点の軸インデックス（nullの場合はドラッグ中ではない）
        /// </summary>
        private int? _draggingPointAxisIndex;

        /// <summary>
        /// 軸をクリックしたときの許容範囲（ピクセル）
        /// </summary>
        public double AxisClickTolerance { get; set; } = 20.0;

        /// <summary>
        /// 点をクリックしたときの許容範囲（ピクセル）
        /// </summary>
        public double PointClickTolerance { get; set; } = 10.0;

        /// <summary>
        /// 追加された点の色
        /// </summary>
        public OxyColor PointColor { get; set; } = OxyColors.Red;

        /// <summary>
        /// 追加された点のサイズ
        /// </summary>
        public double PointSize { get; set; } = 8.0;

        /// <summary>
        /// 一時的なラインの色
        /// </summary>
        public OxyColor TemporaryLineColor { get; set; } = OxyColors.Orange;

        /// <summary>
        /// 一時的なラインの太さ
        /// </summary>
        public double TemporaryLineThickness { get; set; } = 2.0;

        /// <summary>
        /// 点の値ラベルのフォントサイズ
        /// </summary>
        public double PointLabelFontSize { get; set; } = 10.0;

        /// <summary>
        /// 点の値ラベルの色
        /// </summary>
        public OxyColor PointLabelColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// 点の値ラベルの水平オフセット（ピクセル）
        /// </summary>
        public double PointLabelHorizontalOffset { get; set; } = 10.0;

        /// <summary>
        /// 点の値ラベルの表示形式
        /// </summary>
        public string PointLabelFormat { get; set; } = "F2";

        /// <summary>
        /// 点の値ラベルの背景色
        /// </summary>
        public OxyColor PointLabelBackgroundColor { get; set; } = OxyColors.LightGray;

        /// <summary>
        /// 点の値ラベルの背景の余白（ピクセル）
        /// </summary>
        public double PointLabelPadding { get; set; } = 4.0;

        /// <summary>
        /// マウスクリック時の処理を行います
        /// </summary>
        public bool HandleMouseDown(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            if (!IsEnabled || !IsEditMode || series.Dimensions.Length == 0)
            {
                return false;
            }

            // 既存のドラッグをクリア
            _draggingPointAxisIndex = null;

            // まず、既存の点がクリックされたかどうかを確認（点のドラッグを優先）
            int? clickedPointAxisIndex = GetClickedPointAxisIndex(series, point);
            if (clickedPointAxisIndex.HasValue)
            {
                // 既存の点がクリックされた場合、ドラッグを開始
                _draggingPointAxisIndex = clickedPointAxisIndex.Value;
                return true;
            }

            // 既存の点がクリックされなかった場合、軸をクリックしたかどうかを確認
            int? clickedAxisIndex = GetClickedAxisIndex(series, point);
            if (clickedAxisIndex.HasValue)
            {
                // Y座標から値を逆算
                double value = GetValueFromYPosition(series, clickedAxisIndex.Value, point.Y);
                
                // 点を追加または更新
                _addedPoints[clickedAxisIndex.Value] = value;

                // 一時的なラインを更新
                UpdateTemporaryLine(series);

                // プロットを更新
                series.PlotModel?.InvalidatePlot(false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// マウス移動時の処理を行います
        /// </summary>
        public bool HandleMouseMove(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            if (!IsEnabled || !IsEditMode || _draggingPointAxisIndex == null || series.Dimensions.Length == 0)
            {
                return false;
            }

            // ドラッグ中の点の位置を更新
            int axisIndex = _draggingPointAxisIndex.Value;
            
            // Y座標が軸の範囲内かチェック
            double topY = series.GetAxisTopPosition();
            double bottomY = series.GetAxisBottomPosition();
            double clampedY = Math.Max(topY, Math.Min(bottomY, point.Y));

            // Y座標から値を逆算
            double value = GetValueFromYPosition(series, axisIndex, clampedY);
            
            // 点の値を更新
            _addedPoints[axisIndex] = value;

            // 一時的なラインを更新
            UpdateTemporaryLine(series);

            // プロットを更新
            series.PlotModel?.InvalidatePlot(false);

            return true;
        }

        /// <summary>
        /// マウスアップ時の処理を行います
        /// </summary>
        public bool HandleMouseUp(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            if (!IsEnabled || !IsEditMode || _draggingPointAxisIndex == null)
            {
                return false;
            }

            // ドラッグを終了
            _draggingPointAxisIndex = null;
            return true;
        }

        /// <summary>
        /// 追加の描画処理を行います
        /// </summary>
        public void Render(ParallelCoordinatesSeries series, IRenderContext rc)
        {
            if (!IsEnabled || !IsEditMode || series.Dimensions.Length == 0)
            {
                return;
            }

            // まず一時的なラインを描画（下層）
            if (_temporaryLine != null)
            {
                RenderTemporaryLine(series, rc, _temporaryLine);
            }

            // その後、追加された点とラベルを描画（上層）
            RenderAddedPoints(series, rc);
        }

        /// <summary>
        /// クリックされた点の軸インデックスを取得します
        /// </summary>
        private int? GetClickedPointAxisIndex(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            foreach (var pointEntry in _addedPoints)
            {
                int axisIndex = pointEntry.Key;
                double value = pointEntry.Value;

                var dimension = series.Dimensions[axisIndex];
                double x = series.GetAxisXPosition(axisIndex);

                // 値をY座標に変換
                double availableHeight = series.GetAvailableHeight();
                double plotBottom = series.GetAxisBottomPosition();
                double normalizedValue = dimension.NormalizeValue(value);
                normalizedValue = Math.Max(0.0, Math.Min(1.0, normalizedValue));
                double y = plotBottom - normalizedValue * availableHeight;

                var pointPosition = new ScreenPoint(x, y);

                // 点とマウス位置の距離を計算
                double distance = GetDistance(point, pointPosition);

                if (distance <= PointClickTolerance)
                {
                    return axisIndex;
                }
            }

            return null;
        }

        /// <summary>
        /// クリックされた軸のインデックスを取得します
        /// </summary>
        private int? GetClickedAxisIndex(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            for (int i = 0; i < series.Dimensions.Length; i++)
            {
                double axisX = series.GetAxisXPosition(i);
                double distance = Math.Abs(point.X - axisX);

                if (distance <= AxisClickTolerance)
                {
                    // Y座標が軸の範囲内かチェック
                    double topY = series.GetAxisTopPosition();
                    double bottomY = series.GetAxisBottomPosition();
                    if (point.Y >= topY && point.Y <= bottomY)
                    {
                        return i;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Y座標から値を逆算します
        /// </summary>
        private double GetValueFromYPosition(ParallelCoordinatesSeries series, int axisIndex, double y)
        {
            var dimension = series.Dimensions[axisIndex];
            double availableHeight = series.GetAvailableHeight();
            double plotBottom = series.GetAxisBottomPosition();

            // Y座標を正規化された値（0-1）に変換
            double normalizedY = (plotBottom - y) / availableHeight;
            normalizedY = Math.Max(0.0, Math.Min(1.0, normalizedY)); // 0-1の範囲にクランプ

            // 値を計算（元のMinValueとMaxValueの範囲で）
            double value = dimension.MinValue + normalizedY * (dimension.MaxValue - dimension.MinValue);

            // 範囲を制限
            value = Math.Max(dimension.MinValue, Math.Min(dimension.MaxValue, value));

            return value;
        }

        /// <summary>
        /// 一時的なラインを更新します
        /// </summary>
        private void UpdateTemporaryLine(ParallelCoordinatesSeries series)
        {
            if (_addedPoints.Count == 0)
            {
                // 点が1つも追加されていない場合はラインを作成しない
                _temporaryLine = null;
                return;
            }

            // 点が1つ以上追加されている場合、ラインを作成
            // 点が追加されていない軸については、その次元の中央値を使用
            var values = new double[series.Dimensions.Length];
            for (int i = 0; i < series.Dimensions.Length; i++)
            {
                if (_addedPoints.TryGetValue(i, out double value))
                {
                    values[i] = value;
                }
                else
                {
                    // 点が追加されていない軸がある場合は、その次元の中央値を使用
                    var dimension = series.Dimensions[i];
                    values[i] = (dimension.MinValue + dimension.MaxValue) / 2.0;
                }
            }

            // すべての軸に点が追加されているかどうかを判定
            bool isComplete = _addedPoints.Count == series.Dimensions.Length;

            _temporaryLine = new ParallelCoordinatesLine(values)
            {
                Color = TemporaryLineColor,
                StrokeThickness = TemporaryLineThickness,
                LineStyle = isComplete ? LineStyle.Solid : LineStyle.Dash
            };
        }

        /// <summary>
        /// 追加された点を描画します
        /// </summary>
        private void RenderAddedPoints(ParallelCoordinatesSeries series, IRenderContext rc)
        {
            foreach (var pointEntry in _addedPoints)
            {
                int axisIndex = pointEntry.Key;
                double value = pointEntry.Value;

                var dimension = series.Dimensions[axisIndex];
                double x = series.GetAxisXPosition(axisIndex);

                // 値をY座標に変換
                double availableHeight = series.GetAvailableHeight();
                double plotBottom = series.GetAxisBottomPosition();
                double normalizedValue = dimension.NormalizeValue(value);
                normalizedValue = Math.Max(0.0, Math.Min(1.0, normalizedValue));
                double y = plotBottom - normalizedValue * availableHeight;

                var point = new ScreenPoint(x, y);

                // 点を描画（円）
                rc.DrawEllipse(
                    new OxyRect(
                        point.X - PointSize / 2,
                        point.Y - PointSize / 2,
                        PointSize,
                        PointSize
                    ),
                    PointColor,
                    PointColor,
                    1.0,
                    EdgeRenderingMode.Automatic
                );

                // 点の値を右側に表示
                var labelPoint = new ScreenPoint(
                    point.X + PointLabelHorizontalOffset,
                    point.Y
                );
                string labelText = value.ToString(PointLabelFormat);
                
                // テキストのサイズを測定
                var textSize = rc.MeasureText(
                    labelText,
                    null, // フォントファミリー（nullでデフォルト）
                    PointLabelFontSize,
                    FontWeights.Normal
                );

                // 背景の矩形を計算
                double padding = PointLabelPadding;
                var backgroundRect = new OxyRect(
                    labelPoint.X - padding,
                    labelPoint.Y - textSize.Height / 2 - padding,
                    textSize.Width + padding * 2,
                    textSize.Height + padding * 2
                );

                // 背景を描画
                rc.DrawRectangle(
                    backgroundRect,
                    PointLabelBackgroundColor,
                    OxyColors.Undefined,
                    0,
                    EdgeRenderingMode.Automatic
                );

                // テキストを描画
                rc.DrawText(
                    labelPoint,
                    labelText,
                    PointLabelColor,
                    fontSize: PointLabelFontSize,
                    fontWeight: FontWeights.Normal,
                    rotation: 0,
                    horizontalAlignment: HorizontalAlignment.Left,
                    verticalAlignment: VerticalAlignment.Middle
                );
            }
        }

        /// <summary>
        /// 一時的なラインを描画します
        /// </summary>
        private void RenderTemporaryLine(ParallelCoordinatesSeries series, IRenderContext rc, ParallelCoordinatesLine line)
        {
            if (line.Values.Length != series.Dimensions.Length || _addedPoints.Count < 2)
            {
                return;
            }

            double availableHeight = series.GetAvailableHeight();
            double plotBottom = series.GetAxisBottomPosition();

            // すべての軸に点が追加されているかどうかを判定
            bool isComplete = _addedPoints.Count == series.Dimensions.Length;

            // 追加された点の座標を計算（ソート済み）
            var addedPointPositions = new List<ScreenPoint>();
            var sortedAxisIndices = _addedPoints.Keys.OrderBy(i => i).ToList();

            foreach (int axisIndex in sortedAxisIndices)
            {
                var dimension = series.Dimensions[axisIndex];
                double x = series.GetAxisXPosition(axisIndex);
                double value = line.Values[axisIndex];

                // 値を正規化
                double normalizedValue = dimension.NormalizeValue(value);
                normalizedValue = Math.Max(0.0, Math.Min(1.0, normalizedValue));

                // Y座標に変換
                double y = plotBottom - normalizedValue * availableHeight;

                addedPointPositions.Add(new ScreenPoint(x, y));
            }

            // 点が2つ以上ある場合のみ線を描画
            if (addedPointPositions.Count >= 2)
            {
                if (isComplete)
                {
                    // すべての点が追加されている場合：すべての軸を通る実線を描画
                    var allPoints = new List<ScreenPoint>();
                    for (int i = 0; i < series.Dimensions.Length; i++)
                    {
                        var dimension = series.Dimensions[i];
                        double x = series.GetAxisXPosition(i);
                        double value = line.Values[i];

                        double normalizedValue = dimension.NormalizeValue(value);
                        normalizedValue = Math.Max(0.0, Math.Min(1.0, normalizedValue));
                        double y = plotBottom - normalizedValue * availableHeight;

                        allPoints.Add(new ScreenPoint(x, y));
                    }

                    rc.DrawLine(
                        allPoints,
                        line.Color,
                        line.StrokeThickness,
                        EdgeRenderingMode.Automatic
                    );
                }
                else
                {
                    // 一部の点のみ追加されている場合：追加された点の間を点線で繋ぐ
                    DrawDashedLine(rc, addedPointPositions, line.Color, line.StrokeThickness);
                }
            }
        }

        /// <summary>
        /// 点線を描画します
        /// </summary>
        private static void DrawDashedLine(IRenderContext rc, List<ScreenPoint> points, OxyColor color, double thickness)
        {
            if (points.Count < 2)
            {
                return;
            }

            // 点線のパターン（実線部分と空白部分の長さ）
            double dashLength = 5.0;
            double gapLength = 5.0;

            // 各セグメントを点線で描画
            for (int i = 0; i < points.Count - 1; i++)
            {
                var start = points[i];
                var end = points[i + 1];

                double dx = end.X - start.X;
                double dy = end.Y - start.Y;
                double segmentLength = Math.Sqrt(dx * dx + dy * dy);

                if (segmentLength == 0)
                {
                    continue;
                }

                // 単位ベクトルを計算
                double unitX = dx / segmentLength;
                double unitY = dy / segmentLength;

                // 点線を描画
                double currentLength = 0;
                bool isDrawing = true;

                while (currentLength < segmentLength)
                {
                    double remainingLength = segmentLength - currentLength;
                    double currentSegmentLength = isDrawing ? Math.Min(dashLength, remainingLength) : Math.Min(gapLength, remainingLength);

                    if (isDrawing && currentSegmentLength > 0)
                    {
                        var dashStart = new ScreenPoint(
                            start.X + unitX * currentLength,
                            start.Y + unitY * currentLength
                        );
                        var dashEnd = new ScreenPoint(
                            start.X + unitX * (currentLength + currentSegmentLength),
                            start.Y + unitY * (currentLength + currentSegmentLength)
                        );

                        rc.DrawLine(
                            [dashStart, dashEnd],
                            color,
                            thickness,
                            EdgeRenderingMode.Automatic
                        );
                    }

                    currentLength += currentSegmentLength;
                    isDrawing = !isDrawing;
                }
            }
        }

        /// <summary>
        /// 編集モードをリセットします（追加された点と一時的なラインをクリア）
        /// </summary>
        public void ResetEditMode()
        {
            _addedPoints.Clear();
            _temporaryLine = null;
            _draggingPointAxisIndex = null;
        }

        /// <summary>
        /// 2点間の距離を計算します
        /// </summary>
        private static double GetDistance(ScreenPoint p1, ScreenPoint p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 一時的なラインを取得します（nullの場合は未完成）
        /// </summary>
        public ParallelCoordinatesLine? GetTemporaryLine() => _temporaryLine;
    }
}
