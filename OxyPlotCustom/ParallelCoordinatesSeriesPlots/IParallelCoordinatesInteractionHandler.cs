using OxyPlot;

namespace OxyPlotCustom.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// 並行座標プロットのインタラクション処理を行うハンドラーのインターフェース
    /// </summary>
    public interface IParallelCoordinatesInteractionHandler
    {
        /// <summary>
        /// マウスクリック時の処理を行います
        /// </summary>
        /// <param name="series">対象のシリーズ</param>
        /// <param name="point">クリックされたスクリーン座標</param>
        /// <returns>処理を行った場合はtrue、そうでなければfalse</returns>
        bool HandleMouseDown(ParallelCoordinatesSeries series, ScreenPoint point);

        /// <summary>
        /// マウス移動時の処理を行います
        /// </summary>
        /// <param name="series">対象のシリーズ</param>
        /// <param name="point">マウスの現在位置</param>
        /// <returns>処理を行った場合はtrue、そうでなければfalse</returns>
        bool HandleMouseMove(ParallelCoordinatesSeries series, ScreenPoint point);

        /// <summary>
        /// マウスアップ時の処理を行います
        /// </summary>
        /// <param name="series">対象のシリーズ</param>
        /// <param name="point">マウスの現在位置</param>
        /// <returns>処理を行った場合はtrue、そうでなければfalse</returns>
        bool HandleMouseUp(ParallelCoordinatesSeries series, ScreenPoint point);

        /// <summary>
        /// 追加の描画処理を行います
        /// </summary>
        /// <param name="series">対象のシリーズ</param>
        /// <param name="rc">レンダリングコンテキスト</param>
        void Render(ParallelCoordinatesSeries series, IRenderContext rc);

        /// <summary>
        /// ハンドラーが有効かどうか
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
