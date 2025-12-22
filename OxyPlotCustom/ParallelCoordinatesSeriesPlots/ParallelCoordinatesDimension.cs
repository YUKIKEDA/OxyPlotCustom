namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 並行座標プロットの各軸情報
    /// </summary>
    public class ParallelCoordinatesDimension
    {
        /// <summary>
        /// 軸のラベル
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// 値の配列
        /// </summary>
        public double[] Values { get; }

        /// <summary>
        /// 値の数
        /// </summary>
        public int NumValues => Values.Length;

        /// <summary>
        /// 最大値
        /// </summary>
        public double MaxValue { get; private set; }

        /// <summary>
        /// 最小値
        /// </summary>
        public double MinValue { get; private set; }

        /// <summary>
        /// 表示範囲の最大値
        /// </summary>
        public double DisplayMaxValue { get; set; }

        /// <summary>
        /// 表示範囲の最小値
        /// </summary>
        public double DisplayMinValue { get; set; }

        /// <summary>
        /// 軸のの最大値・最小値のマージン比率
        /// </summary>
        public double AxisMarginRatio { get; set; } = 0.0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">軸のラベル</param>
        /// <param name="values">値の配列</param>
        public ParallelCoordinatesDimension(string label, double[] values)
        {
            Label = label;
            Values = values;

            InitializeMinMax(values);
        }

        private void InitializeMinMax(double[] values)
        {
            if (values.Length > 0)
            {
                double min = values.Min();
                double max = values.Max();
                double range = max - min;

                MinValue = min - range * AxisMarginRatio;
                MaxValue = max + range * AxisMarginRatio;
            }
            else
            {
                MinValue = 0.0;
                MaxValue = 1.0;
            }

            DisplayMinValue = MinValue;
            DisplayMaxValue = MaxValue;
        }
    }
}
