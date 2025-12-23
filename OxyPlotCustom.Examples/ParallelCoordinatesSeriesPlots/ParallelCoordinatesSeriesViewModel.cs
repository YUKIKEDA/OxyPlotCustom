using OxyPlot;
using Reactive.Bindings;
using OxyPlotCustom.ParallelCoordinatesSeriesPlots;

namespace OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots
{
    public class ParallelCoordinatesSeriesViewModel
    {
        public PlotModel? PlotModel { get; private set; }

        public ReactiveCommand<ScreenPoint> MouseMoveCommand { get; }
        public ReactiveCommand MouseLeaveCommand { get; }
        public ReactiveCommand<ScreenPoint> MouseDownCommand { get; }
        public ReactiveCommand<ScreenPoint> MouseUpCommand { get; }

        /// <summary>
        /// 編集モードが有効かどうか
        /// </summary>
        public ReactiveProperty<bool> IsEditMode { get; }

        private ParallelCoordinatesSeries? Series { get; set; }
        private PointAdditionHandler? PointAdditionHandler { get; set; }

        public ParallelCoordinatesSeriesViewModel()
        {
            PlotModel = new PlotModel 
            { 
                Title = "Parallel Coordinates Plot Demo" 
            };

            // データを生成
            var dimensions = GenerateStudentScoreData();

            // ParallelCoordinatesSeriesを作成してPlotModelに追加
            Series = new ParallelCoordinatesSeries(dimensions);
            
            // カラーマップを「数学」軸に設定
            Series.ColorMapDimensionName = "Physical";
            Series.ColorMap = OxyPalettes.Jet(256);
            
            // 範囲調整ハンドラーを追加
            var rangeHandler = new DimensionRangeHandler();
            Series.InteractionHandlers.Add(rangeHandler);
            
            // 点追加ハンドラーを追加
            PointAdditionHandler = new PointAdditionHandler();
            Series.InteractionHandlers.Add(PointAdditionHandler);
            
            PlotModel.Series.Add(Series);

            // 編集モードのプロパティを初期化
            IsEditMode = new ReactiveProperty<bool>(false);
            IsEditMode.Subscribe(isEditMode =>
            {
                if (PointAdditionHandler != null && Series != null)
                {
                    PointAdditionHandler.IsEditMode = isEditMode;
                    Series.IsEditMode = isEditMode;
                    if (isEditMode)
                    {
                        // 編集モードを有効にしたときにハイライトをクリア
                        if (Series.HighlightedLineId != null)
                        {
                            Series.HighlightedLineId = null;
                        }
                    }
                    else
                    {
                        // 編集モードを無効にしたときにリセット
                        PointAdditionHandler.ResetEditMode();
                    }
                    PlotModel?.InvalidatePlot(false);
                }
            });

            // コマンドを初期化
            MouseMoveCommand = new ReactiveCommand<ScreenPoint>().WithSubscribe(OnMouseMove);
            MouseLeaveCommand = new ReactiveCommand().WithSubscribe(OnMouseLeave);
            MouseDownCommand = new ReactiveCommand<ScreenPoint>().WithSubscribe(OnMouseDown);
            MouseUpCommand = new ReactiveCommand<ScreenPoint>().WithSubscribe(OnMouseUp);
        }

        /// <summary>
        /// 学生の成績データを生成します
        /// </summary>
        /// <returns>次元データのディクショナリ</returns>
        private static Dictionary<string, ParallelCoordinatesDimension> GenerateStudentScoreData()
        {
            // 100名の学生の成績データを生成
            const int studentCount = 100;
            var random = new Random(42); // 再現性のためシードを固定

            var mathScores = new double[studentCount];
            var englishScores = new double[studentCount];
            var scienceScores = new double[studentCount];
            var socialScores = new double[studentCount];
            var physicalScores = new double[studentCount];

            for (int i = 0; i < studentCount; i++)
            {
                // 各学生の基礎学力レベルを生成（30-90の範囲）
                double baseLevel = 30 + random.NextDouble() * 60;

                // 各科目の成績を生成（基礎レベルに基づき、ランダムな変動を加える）
                // 数学：基礎レベルに強く相関、やや高めの傾向
                mathScores[i] = Math.Clamp(baseLevel + (random.NextDouble() - 0.5) * 20 + 5, 0, 100);

                // 英語：基礎レベルに相関、やや低めの傾向
                englishScores[i] = Math.Clamp(baseLevel + (random.NextDouble() - 0.5) * 25 - 3, 0, 100);

                // 科学：基礎レベルに強く相関
                scienceScores[i] = Math.Clamp(baseLevel + (random.NextDouble() - 0.5) * 20, 0, 100);

                // 社会：基礎レベルに中程度相関、ばらつきが大きい
                socialScores[i] = Math.Clamp(baseLevel + (random.NextDouble() - 0.5) * 30, 0, 100);

                // 体育：基礎レベルとの相関が弱く、独立した分布
                physicalScores[i] = Math.Clamp(40 + random.NextDouble() * 40 + (random.NextDouble() - 0.5) * 20, 0, 100);
            }

            // 次元データを作成
            return new Dictionary<string, ParallelCoordinatesDimension>
            {
                { "Math", new ParallelCoordinatesDimension("数学", mathScores) },
                { "English", new ParallelCoordinatesDimension("英語", englishScores) },
                { "Science", new ParallelCoordinatesDimension("科学", scienceScores) },
                { "Social", new ParallelCoordinatesDimension("社会", socialScores) },
                { "Physical", new ParallelCoordinatesDimension("体育", physicalScores) }
            };
        }

        /// <summary>
        /// マウス移動時のハイライト処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseMove(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理（ドラッグなど）
            bool handled = false;
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseMove(Series, screenPoint))
                {
                    handled = true;
                    break;
                }
            }

            // 編集モード時はハイライト処理をスキップ
            if (Series.IsEditMode)
            {
                // ハイライトをクリア
                if (Series.HighlightedLineId != null)
                {
                    Series.HighlightedLineId = null;
                    PlotModel.InvalidatePlot(false);
                }
                return;
            }

            // ハンドラーで処理されなかった場合のみハイライト処理
            if (!handled)
            {
                // 最も近いラインを取得
                var nearestLineId = Series.GetNearestLineId(screenPoint);

                // ハイライトを更新
                if (Series.HighlightedLineId != nearestLineId)
                {
                    Series.HighlightedLineId = nearestLineId;
                    PlotModel.InvalidatePlot(false);
                }
            }
        }

        /// <summary>
        /// マウスが離れたときのハイライト解除処理
        /// </summary>
        private void OnMouseLeave()
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // 編集モード時はハイライト処理をスキップ
            if (Series.IsEditMode)
            {
                return;
            }

            // ハイライトを解除
            if (Series.HighlightedLineId != null)
            {
                Series.HighlightedLineId = null;
                PlotModel.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// マウスダウン時の処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseDown(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseDown(Series, screenPoint))
                {
                    PlotModel.InvalidatePlot(false);
                    return;
                }
            }
        }

        /// <summary>
        /// マウスアップ時の処理
        /// </summary>
        /// <param name="screenPoint">マウス位置のスクリーン座標</param>
        private void OnMouseUp(ScreenPoint screenPoint)
        {
            if (Series == null || PlotModel == null)
            {
                return;
            }

            // インタラクションハンドラーで処理
            foreach (var handler in Series.InteractionHandlers)
            {
                if (handler.IsEnabled && handler.HandleMouseUp(Series, screenPoint))
                {
                    PlotModel.InvalidatePlot(false);
                    return;
                }
            }
        }
    }
}
