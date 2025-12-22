using OxyPlot;
using OxyPlotCustom.ParallelCoordinatesSeriesPlots;

namespace OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots
{
    public class ParallelCoordinatesSeriesViewModel
    {
        public PlotModel? PlotModel { get; private set; }

        public ParallelCoordinatesSeriesViewModel()
        {
            PlotModel = new PlotModel 
            { 
                Title = "Parallel Coordinates Plot Demo" 
            };

            // データを生成
            var dimensions = GenerateStudentScoreData();

            // ParallelCoordinatesSeriesを作成してPlotModelに追加
            var series = new ParallelCoordinatesSeries(dimensions);
            PlotModel.Series.Add(series);
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
    }
}
