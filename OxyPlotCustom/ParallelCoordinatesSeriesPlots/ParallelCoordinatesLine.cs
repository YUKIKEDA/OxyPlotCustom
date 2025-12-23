using System;
using OxyPlot;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 並行座標プロットの各ラインの情報を保持するデータ型
    /// </summary>
    public class ParallelCoordinatesLine
    {
        public string Id { get; }
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
        /// 正規化済みの値（0.0～1.0）の配列（描画パフォーマンス向上のためのキャッシュ）
        /// </summary>
        public double[]? NormalizedValues { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="values">各次元（軸）での値の配列</param>
        /// <param name="id">ラインID（省略時は一意のIDを自動生成）</param>
        public ParallelCoordinatesLine(double[] values)
            : this(Guid.NewGuid().ToString("N"), values)
        {
        }

        public ParallelCoordinatesLine(string id, double[] values)
        {
            Id = id;
            Values = values;
        }
    }
}

