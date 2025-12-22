using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    public class ParallelCoordinatesSeries : ItemsSeries
    {
        public ParallelCoordinatesSeries()
        {

        }

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

        }

        #endregion

        #region Rendering Axes

        /// <summary>
        /// 軸を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderAxes(IRenderContext rc)
        {

        }

        /// <summary>
        /// 軸のラベルを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderAxisLabels(IRenderContext rc)
        {

        }

        /// <summary>
        /// 軸の目盛りを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderAxisTicks(IRenderContext rc)
        {

        }

        #endregion

        #region Rendering Data Lines

        /// <summary>
        /// データ線を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderDataLines(IRenderContext rc)
        {

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

        #endregion
    }
}
