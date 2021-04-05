using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace automatic_engine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// チェック種類
        /// </summary>
        public enum CHECK_TYPE
        {
            REPLACE_WITH,
            INSERT_INTO,
            NUMBERING,
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            TxtPath.Focus();
        }

        /// <summary>
        /// フォルダーを選択するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // フォルダー選択ダイアログを開く
            var folderDialog = new FolderBrowserDialog();
            var dialogResult = folderDialog.ShowDialog();

            // OKボタンを選択する時
            if (System.Windows.Forms.DialogResult.OK.Equals(dialogResult))
            {
                // パステキストボックスに選択下パスを設定する
                TxtPath.Text = folderDialog.SelectedPath;
            }
        }

        /// <summary>
        /// 実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            // 実行する
            Execute(isPreview: false, isExecuteAfterPreviewed: false);
        }

        /// <summary>
        /// 変更前プレビューを見るボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            // 実行する
            Execute(isPreview: true, isExecuteAfterPreviewed: false);
        }

        /// <summary>
        /// 必須チェック
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckRequire(CHECK_TYPE type)
        {
            // パスチェック
            if (string.IsNullOrEmpty(TxtPath.Text))
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show("Please input Path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // エラーコントロールをフォーカスする
                TxtPath.Focus();

                // チェック結果がFALSEを戻す
                return false;
            }
            // パスがパソコンに存在しない場合、エラーになる
            else if (!Directory.Exists(TxtPath.Text))
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show("Path not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // エラーコントロールをフォーカスする
                TxtPath.Focus();

                // チェック結果がFALSEを戻す
                return false;
            }
            // Raplce-Withの場合
            else if (type.Equals(CHECK_TYPE.REPLACE_WITH))
            {
                // 変更前条件が入力されていない場合、エラーになる
                if (string.IsNullOrEmpty(TxtReplace.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtReplace.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
            }
            // Insert-Intoの場合
            else if (type.Equals(CHECK_TYPE.INSERT_INTO))
            {
                // 印刷条件を入力しない場合、エラーになる
                if (string.IsNullOrEmpty(TxtInsertInto_Word.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtInsertInto_Word.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (string.IsNullOrEmpty(TxtInsertInto_Index.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtInsertInto_Index.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (!int.TryParse(TxtInsertInto_Index.Text, out _))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Invalid Index", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtInsertInto_Index.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
            }
            // Numberingの場合
            else if (type.Equals(CHECK_TYPE.NUMBERING))
            {
                if (string.IsNullOrEmpty(TxtNumbering_At.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtNumbering_At.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (string.IsNullOrEmpty(TxtNumbering_From.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtNumbering_From.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (!int.TryParse(TxtNumbering_At.Text, out _))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Invalid Index", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtNumbering_At.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (!int.TryParse(TxtNumbering_From.Text, out _))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Invalid Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtNumbering_From.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
            }

            // チェック結果がTRUEを戻す
            return true;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// クリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtReplace.Text = string.Empty;
            TxtWith.Text = string.Empty;
            TxtInsertInto_Word.Text = string.Empty;
            TxtInsertInto_Index.Text = string.Empty;
        }

        /// <summary>
        /// Replace-Withモードを選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Replace-Withの場合
            if (sender.Equals(LblReplace)
                || sender.Equals(LblWith))
            {
                // Replace-Withラジオを選択する
                RdoReplace.IsChecked = true;
            }
            // Insert-Intoの場合
            else if (sender.Equals(LblInsertInto_Word)
                || sender.Equals(LblInsertInto_Index))
            {
                // Insert-Intoラジオを選択する
                RdoInsert.IsChecked = true;
            }
            // Numberingの場合
            else if (sender.Equals(LblNumbering))
            {
                // Numberingラジオを選択する
                RdoNumbering.IsChecked = true;
            }
        }

        /// <summary>
        /// 処理実行する
        /// </summary>
        /// <param name="isPreview">プレビューフラグ</param>
        /// <param name="isExecuteAfterPreviewed">プレビュー後実行フラグ</param>
        private void Execute(bool isPreview, bool isExecuteAfterPreviewed)
        {
            // 単項目チェック
            if (((bool)RdoReplace.IsChecked
                && !CheckRequire(CHECK_TYPE.REPLACE_WITH))
                || ((bool)RdoInsert.IsChecked
                    && !CheckRequire(CHECK_TYPE.INSERT_INTO))
                || ((bool)RdoNumbering.IsChecked
                   && !CheckRequire(CHECK_TYPE.NUMBERING)))
            {
                return;
            }

            // プレビューモードじゃない場合
            if (!isPreview && !isExecuteAfterPreviewed)
            {
                // 確認ダイアログを表示する
                if (System.Windows.Forms.DialogResult.No.Equals(
                    System.Windows.Forms.MessageBox.Show(
                        "Do you want to execute rename?",
                        "Confirm",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)))
                {
                    return;
                }

            }

            // 指定パスを取得する
            var strPath = TxtPath.Text;

            // フォルダー情報を取得
            var directoryInfo = new DirectoryInfo(strPath);

            // フォルダーにすべてファイルの情報を取得する
            var fileInfos = directoryInfo.GetFiles();

            // 変更前ファイル名一覧
            var listOriginalName = new List<string>();

            // 変更後ファイル名一覧
            var listChangeName = new List<string>();

            // 実行ファイル数
            var executedFileCount = 0;

            // Numberingモードの場合
            int numbering_at = 0;
            int numbering_from = 0;
            if ((bool)RdoNumbering.IsChecked)
            {
                numbering_at = int.Parse(TxtNumbering_At.Text);
                numbering_from = int.Parse(TxtNumbering_From.Text);
            }

            // 整合性チェック
            if ((bool)RdoNumbering_Decrease.IsChecked
                && numbering_from < fileInfos.Length)
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show("Invalid decrease number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // エラーコントロールをフォーカスする
                TxtNumbering_From.Focus();

                // 処理停止
                return;
            }

            // 順番処理
            if ((bool)ChkNumbering_SortBy.IsChecked)
            {
                string selectedValue = ((ComboBoxItem)CbbSortBy.SelectedValue).Content.ToString();
                
                // 名前で順番する
                if ("Name".Equals(selectedValue))
                {
                    fileInfos = fileInfos.OrderBy(v => v.Name).ToArray();
                }
                // 編集日で順番する
                else if ("Modified Date".Equals(selectedValue))
                {
                    fileInfos = fileInfos.OrderBy(v => v.LastWriteTime).ToArray();
                }
                // 作成日で順番する
                else if ("Created Date".Equals(selectedValue))
                {
                    fileInfos = fileInfos.OrderBy(v => v.CreationTime).ToArray();
                }
                // サイズで順番する
                else if ("Size".Equals(selectedValue))
                {
                    fileInfos = fileInfos.OrderBy(v => v.Length).ToArray();
                }

                // 順番を反対する
                if ((bool)ChkNumbering_Reverse.IsChecked)
                {
                    fileInfos = fileInfos.Reverse().ToArray();
                }
            }

            // すべてファイルを繰返す
            foreach (FileInfo info in fileInfos)
            {
                // 変更前ファイル名
                var strOriginName = info.Name;

                // ファイル名にReplace文字が含まない場合、処理対象以外になる
                if (!strOriginName.Contains(TxtReplace.Text))
                {
                    // 次のレコードを移動する
                    continue;
                }

                // 変更後ファイル名
                var strChangeName = strOriginName;

                // Replace-With処理
                if ((bool)RdoReplace.IsChecked)
                {
                    strChangeName = strOriginName.Replace(TxtReplace.Text, TxtWith.Text);
                }
                // Insert-Into処理
                else if ((bool)RdoInsert.IsChecked)
                {
                    strChangeName = strOriginName.Insert(int.Parse(TxtInsertInto_Index.Text), TxtInsertInto_Word.Text);
                }
                // Numbering処理
                else if ((bool)RdoNumbering.IsChecked)
                {
                    // フォーマットチェックする時
                    if ((bool)ChkNumbering_Format.IsChecked)
                    {
                        // 変数をフォーマットする
                        int padNum = fileInfos.Length.ToString().Length;
                        strChangeName = strOriginName.Insert(numbering_at, numbering_from.ToString().PadLeft(padNum, '0'));
                    }
                    else
                    {
                        strChangeName = strOriginName.Insert(numbering_at, numbering_from.ToString());
                    }

                    // 昇順
                    if ((bool)RdoNumbering_Increase.IsChecked)
                    {
                        numbering_from++;
                    }
                    // 降順
                    else
                    {
                        numbering_from--;
                    }
                }

                // 変更前ファイル名一覧に変更前ファイル名を追加する
                listOriginalName.Add(strOriginName);

                // 変更後ファイル名一覧に変更後ファイル名を追加する
                listChangeName.Add(strChangeName);

                // 実行モードの場合
                if (!isPreview)
                {
                    // 名の変更を実行する
                    File.Move(info.FullName, string.Concat(info.DirectoryName, System.IO.Path.DirectorySeparatorChar, strChangeName));

                    // 処理対象数をカウントアップする
                    executedFileCount++;
                }
            }

            // プレビューモードの場合
            if (isPreview)
            {
                // プレビュー画面を作成する
                var previewWindow = new PreviewWindow(listOriginal: listOriginalName, listChanged: listChangeName)
                {
                    Owner = this
                };

                // プレビュー画面を表示する
                previewWindow.ShowDialog();

                // プレビュー画面から実行ボタンを押す時
                if (PreviewWindow.isExecute)
                {
                    // 実行する
                    Execute(isPreview: false, isExecuteAfterPreviewed: true);
                }
            }
            // 実行モードの場合
            else
            {
                // 実行されたファイル数は０が超えるの場合
                if (executedFileCount > 0)
                {
                    // 結果メッセージを表示する
                    System.Windows.Forms.MessageBox.Show(
                        string.Format("Renamed {0} files!", executedFileCount),
                        "Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                // 実行されたファイルがない場合
                else
                {
                    // 結果メッセージを表示する
                    System.Windows.Forms.MessageBox.Show(
                        "No file was executed!",
                        "Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// SortByチェックボックスを選択する時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkNumbering_SortBy_Click(object sender, RoutedEventArgs e)
        {
            // チェックの場合
            if ((bool)ChkNumbering_SortBy.IsChecked)
            {
                // 活性
                CbbSortBy.IsEnabled = true;
                ChkNumbering_Reverse.IsEnabled = true;
            }
            else
            {
                // 非活性
                CbbSortBy.IsEnabled = false;
                ChkNumbering_Reverse.IsEnabled = false;
            }
        }
    }
}
