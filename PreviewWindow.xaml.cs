using System;
using System.Collections.Generic;
using System.Windows;

namespace automatic_engine
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        /// <summary>
        /// 実行かフラグ
        /// </summary>
        public static bool isExecute = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="listOriginal">変更前一覧</param>
        /// <param name="listChanged">変更後一覧</param>
        public PreviewWindow(List<string> listOriginal, List<string> listChanged)
        {
            InitializeComponent();

            // 変更前を表示する
            foreach (string name in listOriginal)
            {
                TxtBefore.Text += string.Concat(name, Environment.NewLine);
            }

            // 変更後を表示する
            foreach (string name in listChanged)
            {
                TxtAfter.Text += string.Concat(name, Environment.NewLine);
            }
        }

        /// <summary>
        /// 実行処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            // 実行フラグをTRUEになる
            isExecute = true;
            Close();
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            isExecute = false;
            Close();
        }

        /// <summary>
        /// スクロールバーを変更する時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBeforeAfter_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            // 変更前と変更後の表示を移動します
            TxtBefore.ScrollToVerticalOffset(e.VerticalOffset);
            TxtAfter.ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}
