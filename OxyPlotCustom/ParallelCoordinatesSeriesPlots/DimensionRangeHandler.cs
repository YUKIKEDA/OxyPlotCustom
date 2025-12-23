using OxyPlot;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 軸の表示範囲を調整するハンドラー（ハンドルをドラッグして上下限値を設定）
    /// </summary>
    public class DimensionRangeHandler : IParallelCoordinatesInteractionHandler
    {
        /// <summary>
        /// ハンドラーが有効かどうか
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// ハンドルのサイズ（ピクセル）
        /// </summary>
        public double HandleSize { get; set; } = 6.0;

        /// <summary>
        /// ハンドルの色
        /// </summary>
        public OxyColor HandleColor { get; set; } = OxyColors.DarkSlateGray;

        /// <summary>
        /// ハンドルの太さ
        /// </summary>
        public double HandleThickness { get; set; } = 2.0;

        /// <summary>
        /// ハンドルをクリックしたときの許容範囲（ピクセル）
        /// </summary>
        public double HitTestTolerance { get; set; } = 15.0;

        /// <summary>
        /// 現在ドラッグ中のハンドルの情報（nullの場合はドラッグ中ではない）
        /// </summary>
        private DraggingHandle? _draggingHandle;

        /// <summary>
        /// ドラッグ中のハンドルの情報
        /// </summary>
        private class DraggingHandle
        {
            public int DimensionIndex { get; set; }
            public bool IsUpperHandle { get; set; } // true: 上限ハンドル, false: 下限ハンドル
            public double InitialValue { get; set; } // ドラッグ開始時の値
        }

        /// <summary>
        /// マウスクリック時の処理を行います
        /// </summary>
        public bool HandleMouseDown(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            if (!IsEnabled || series.Dimensions.Count == 0)
            {
                return false;
            }

            // 既存のドラッグをクリア
            _draggingHandle = null;

            // 各軸のハンドルをチェック
            for (int i = 0; i < series.Dimensions.Count; i++)
            {
                var dimension = series.Dimensions.ElementAt(i).Value;
                double x = series.GetAxisXPosition(i);

                // 上限ハンドルの位置を計算
                double upperY = GetHandleYPosition(series, dimension, true);
                double distanceToUpper = GetDistance(point, new ScreenPoint(x, upperY));
                if (distanceToUpper <= HitTestTolerance)
                {
                    _draggingHandle = new DraggingHandle
                    {
                        DimensionIndex = i,
                        IsUpperHandle = true,
                        InitialValue = dimension.DisplayMaxValue
                    };
                    return true;
                }

                // 下限ハンドルの位置を計算
                double lowerY = GetHandleYPosition(series, dimension, false);
                double distanceToLower = GetDistance(point, new ScreenPoint(x, lowerY));
                if (distanceToLower <= HitTestTolerance)
                {
                    _draggingHandle = new DraggingHandle
                    {
                        DimensionIndex = i,
                        IsUpperHandle = false,
                        InitialValue = dimension.DisplayMinValue
                    };
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// マウス移動時の処理を行います
        /// </summary>
        public bool HandleMouseMove(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            if (!IsEnabled || _draggingHandle == null || series.Dimensions.Count == 0)
            {
                return false;
            }

            var dimension = series.Dimensions.ElementAt(_draggingHandle.DimensionIndex).Value;

            // Y座標から値を逆算
            double availableHeight = series.GetAvailableHeight();
            double plotBottom = series.GetAxisBottomPosition();
            double normalizedY = (plotBottom - point.Y) / availableHeight;
            normalizedY = Math.Max(0.0, Math.Min(1.0, normalizedY)); // 0-1の範囲にクランプ

            // 値を計算（元のMinValueとMaxValueの範囲で）
            double value = dimension.MinValue + normalizedY * (dimension.MaxValue - dimension.MinValue);

            // 範囲の制約を適用
            if (_draggingHandle.IsUpperHandle)
            {
                // 上限ハンドル：下限値より大きく、元の最大値以下
                value = Math.Max(dimension.DisplayMinValue + (dimension.MaxValue - dimension.MinValue) * 0.01, value);
                value = Math.Min(dimension.MaxValue, value);
                dimension.DisplayMaxValue = value;
            }
            else
            {
                // 下限ハンドル：上限値より小さく、元の最小値以上
                value = Math.Min(dimension.DisplayMaxValue - (dimension.MaxValue - dimension.MinValue) * 0.01, value);
                value = Math.Max(dimension.MinValue, value);
                dimension.DisplayMinValue = value;
            }

            // プロットを更新
            series.PlotModel?.InvalidatePlot(false);

            return true;
        }

        /// <summary>
        /// マウスアップ時の処理を行います
        /// </summary>
        public bool HandleMouseUp(ParallelCoordinatesSeries series, ScreenPoint point)
        {
            if (!IsEnabled || _draggingHandle == null)
            {
                return false;
            }

            // ドラッグを終了
            _draggingHandle = null;
            return true;
        }

        /// <summary>
        /// 追加の描画処理を行います
        /// </summary>
        public void Render(ParallelCoordinatesSeries series, IRenderContext rc)
        {
            if (!IsEnabled || series.Dimensions.Count == 0)
            {
                return;
            }

            // 各軸のハンドルを描画
            for (int i = 0; i < series.Dimensions.Count; i++)
            {
                var dimension = series.Dimensions.ElementAt(i).Value;
                double x = series.GetAxisXPosition(i);

                // 上限ハンドルを描画
                double upperY = GetHandleYPosition(series, dimension, true);
                RenderHandle(rc, new ScreenPoint(x, upperY));

                // 下限ハンドルを描画
                double lowerY = GetHandleYPosition(series, dimension, false);
                RenderHandle(rc, new ScreenPoint(x, lowerY));
            }
        }

        /// <summary>
        /// ハンドルを描画します
        /// </summary>
        private void RenderHandle(IRenderContext rc, ScreenPoint center)
        {
            // ハンドルのサイズを計算
            double halfWidth = HandleSize * 1.25;
            double halfHeight = HandleSize * 0.3;

            // ハンドルを横長の長方形として描画（塗りつぶしと輪郭線）
            var rect = new OxyRect(
                center.X - halfWidth,
                center.Y - halfHeight,
                halfWidth * 2,
                halfHeight * 2
            );

            rc.DrawRectangle(rect, HandleColor, HandleColor, HandleThickness, EdgeRenderingMode.Automatic);
        }

        /// <summary>
        /// ハンドルのY座標を取得します
        /// </summary>
        private static double GetHandleYPosition(ParallelCoordinatesSeries series, ParallelCoordinatesDimension dimension, bool isUpper)
        {
            double availableHeight = series.GetAvailableHeight();
            double plotBottom = series.GetAxisBottomPosition();

            double value = isUpper ? dimension.DisplayMaxValue : dimension.DisplayMinValue;
            double normalizedValue = (value - dimension.MinValue) / (dimension.MaxValue - dimension.MinValue);
            normalizedValue = Math.Max(0.0, Math.Min(1.0, normalizedValue));

            return plotBottom - normalizedValue * availableHeight;
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
    }
}
