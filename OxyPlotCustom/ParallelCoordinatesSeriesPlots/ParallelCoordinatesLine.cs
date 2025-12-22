using OxyPlot;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 並行座標プロットの各ラインの情報を保持するデータ型
    /// </summary>
    public class ParallelCoordinatesLine
    {
        /// <summary>
        /// 各次元（軸）での値の配列
        /// </summary>
        public double[] Values { get; }

        /// <summary>
        /// ラインの色
        /// </summary>
        public OxyColor Color { get; set; } = OxyColors.Blue;

        /// <summary>
        /// ラインの太さ
        /// </summary>
        public double StrokeThickness { get; set; } = 1.0;

        /// <summary>
        /// ラインのスタイル
        /// </summary>
        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        /// <summary>
        /// ツールチップに表示するラベル
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// ツールチップに表示する追加情報
        /// </summary>
        public string? ToolTip { get; set; }

        /// <summary>
        /// ラインの可視性
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// タグ（任意のオブジェクトを保持可能）
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="values">各次元（軸）での値の配列</param>
        public ParallelCoordinatesLine(double[] values)
        {
            Values = values;
        }
    }
}

