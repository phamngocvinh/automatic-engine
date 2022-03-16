/* 
Copyright(C) 2022  Pham Ngoc Vinh

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>. 
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

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
        /// 変更前一覧
        /// </summary>
        readonly List<string> listOriginal = new List<string>();

        /// <summary>
        /// 変更後一覧
        /// </summary>
        readonly List<string> listChanged = new List<string>();

        /// <summary>
        /// 変更前フル一覧
        /// </summary>
        readonly List<string> listOriginalFull = new List<string>();

        /// <summary>
        /// 変更後フル一覧
        /// </summary>
        readonly List<string> listChangedFull = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="listOriginal">変更前一覧</param>
        /// <param name="listChanged">変更後一覧</param>
        /// <param name="listOriginalFull">変更前フル一覧</param>
        /// <param name="listChangedFull">変更後フル一覧</param>
        public PreviewWindow(List<string> listOriginal, List<string> listChanged, List<string> listOriginalFull, List<string> listChangedFull)
        {
            InitializeComponent();

            this.listOriginal= listOriginal;
            this.listChanged = listChanged;
            this.listOriginalFull = listOriginalFull;
            this.listChangedFull = listChangedFull;

            // 変更前を表示する
            foreach (string name in listOriginalFull)
            {
                TxtBefore.Text += string.Concat(name, Environment.NewLine);
            }

            // 変更後を表示する
            foreach (string name in listChangedFull)
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
            // 確認ダイアログを表示する
            if (System.Windows.Forms.DialogResult.No.Equals(
                System.Windows.Forms.MessageBox.Show(
                    "Do you want to execute rename?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2)))
            {
                return;
            }
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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Close();
        }

        private void ChkFullpath_Click(object sender, RoutedEventArgs e)
        {
            TxtBefore.Clear();
            TxtAfter.Clear();

            if ((bool)ChkFullpath.IsChecked)
            {
                // 変更前を表示する
                foreach (string name in listOriginalFull)
                {
                    TxtBefore.Text += string.Concat(name, Environment.NewLine);
                }

                // 変更後を表示する
                foreach (string name in listChangedFull)
                {
                    TxtAfter.Text += string.Concat(name, Environment.NewLine);
                }
            }
            else
            {
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
        }
    }
}
