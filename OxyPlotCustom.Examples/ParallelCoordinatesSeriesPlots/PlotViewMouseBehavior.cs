using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using OxyPlot;
using OxyPlot.Wpf;

namespace OxyPlotCustom.Examples.ParallelCoordinatesSeriesPlots
{
    /// <summary>
    /// PlotViewのマウスイベントをViewModelにバインドするビヘイビア
    /// </summary>
    public class PlotViewMouseBehavior : Behavior<PlotView>
    {
        /// <summary>
        /// マウス移動時のコマンドを取得または設定します。
        /// パラメータとしてScreenPointが渡されます。
        /// </summary>
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(PlotViewMouseBehavior));
        public ICommand MouseMoveCommand
        {
            get { return (ICommand)GetValue(MouseMoveCommandProperty); }
            set { SetValue(MouseMoveCommandProperty, value); }
        }

        /// <summary>
        /// マウス左ボタン押下時のコマンドを取得または設定します。
        /// パラメータとしてScreenPointが渡されます。
        /// </summary>
        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(PlotViewMouseBehavior));
        public ICommand MouseDownCommand
        {
            get { return (ICommand)GetValue(MouseDownCommandProperty); }
            set { SetValue(MouseDownCommandProperty, value); }
        }

        /// <summary>
        /// マウス左ボタン解放時のコマンドを取得または設定します。
        /// パラメータとしてScreenPointが渡されます。
        /// </summary>
        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.Register(nameof(MouseUpCommand), typeof(ICommand), typeof(PlotViewMouseBehavior));
        public ICommand MouseUpCommand
        {
            get { return (ICommand)GetValue(MouseUpCommandProperty); }
            set { SetValue(MouseUpCommandProperty, value); }
        }

        /// <summary>
        /// マウスがコントロールから離れたときのコマンドを取得または設定します。
        /// パラメータはnullが渡されます。
        /// </summary>
        public static readonly DependencyProperty MouseLeaveCommandProperty =
            DependencyProperty.Register(nameof(MouseLeaveCommand), typeof(ICommand), typeof(PlotViewMouseBehavior));

        /// <summary>
        /// マウスがコントロールから離れたときのコマンドを取得または設定します。
        /// パラメータはnullが渡されます。
        /// </summary>
        public ICommand MouseLeaveCommand
        {
            get { return (ICommand)GetValue(MouseLeaveCommandProperty); }
            set { SetValue(MouseLeaveCommandProperty, value); }
        }


        /// <summary>
        /// ビヘイビアがPlotViewにアタッチされたときに呼び出されます。
        /// マウスイベントのハンドラーを登録します。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeave += OnMouseLeave;
            // Previewイベントを使用して、OxyPlotがイベントを消費する前に処理する
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        }

        /// <summary>
        /// ビヘイビアがPlotViewからデタッチされるときに呼び出されます。
        /// マウスイベントのハンドラーを解除します。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
        }

        /// <summary>
        /// マウス移動イベントのハンドラー
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="e">マウスイベントの引数</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMoveCommand != null && MouseMoveCommand.CanExecute(null))
            {
                var position = e.GetPosition(AssociatedObject);
                var screenPoint = new ScreenPoint(position.X, position.Y);
                MouseMoveCommand.Execute(screenPoint);
            }
        }

        /// <summary>
        /// マウス左ボタン押下イベントのハンドラー（Previewイベント）
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="e">マウスボタンイベントの引数</param>
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseDownCommand != null && MouseDownCommand.CanExecute(null))
            {
                var position = e.GetPosition(AssociatedObject);
                var screenPoint = new ScreenPoint(position.X, position.Y);
                MouseDownCommand.Execute(screenPoint);
                
                // OxyPlotのデフォルト動作（パン、ズームなど）を抑制して、シングルクリックでドラッグできるようにする
                e.Handled = true;
            }
        }

        /// <summary>
        /// マウス左ボタン解放イベントのハンドラー（Previewイベント）
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="e">マウスボタンイベントの引数</param>
        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseUpCommand != null && MouseUpCommand.CanExecute(null))
            {
                var position = e.GetPosition(AssociatedObject);
                var screenPoint = new ScreenPoint(position.X, position.Y);
                MouseUpCommand.Execute(screenPoint);
                
                // OxyPlotのデフォルト動作を抑制
                e.Handled = true;
            }
        }

        /// <summary>
        /// マウスがコントロールから離れたときのイベントハンドラー
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="e">マウスイベントの引数</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseLeaveCommand != null && MouseLeaveCommand.CanExecute(null))
            {
                MouseLeaveCommand.Execute(null);
            }
        }
    }
}