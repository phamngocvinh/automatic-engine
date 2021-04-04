using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
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
        /// 処理実行する
        /// </summary>
        /// <param name="isPreview">プレビューフラグ</param>
        /// <param name="isExecuteAfterPreviewed">プレビュー後実行フラグ</param>
        private void Execute(bool isPreview, bool isExecuteAfterPreviewed)
        {
            // 単項目チェック
            if (!CheckRequire(CHECK_TYPE.REPLACE_WITH))
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
            List<string> listOriginalName = new List<string>();

            // 変更後ファイル名一覧
            List<string> listChangeName = new List<string>();

            // 実行ファイル数
            var executedFileCount = 0;

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
                var strChangeName = strOriginName.Replace(TxtReplace.Text, TxtWith.Text);

                // 変更前ファイル名一覧に変更前ファイル名を追加する
                listOriginalName.Add(strOriginName);

                // 変更後ファイル名一覧に変更後ファイル名を追加する
                listChangeName.Add(strChangeName);

                // 実行モードの場合
                if (!isPreview)
                {
                    // 名の変更を実行する
                    File.Move(info.FullName, string.Concat(info.DirectoryName, System.IO.Path.DirectorySeparatorChar, strChangeName));
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
                if (PreviewWindow.dialogResult)
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
        /// 必須チェック
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckRequire(params CHECK_TYPE[] type)
        {
            // チェック種類が設定されない場合、エラーになる
            if (type.Length == 0)
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show("Internal Error: Missing Parameter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // チェック結果がFALSEを戻す
                return false;
            }
            // パスチェック
            else if (string.IsNullOrEmpty(TxtPath.Text))
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show("Please input Path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // チェック結果がFALSEを戻す
                return false;
            }
            // パスがパソコンに存在しない場合、エラーになる
            else if (!Directory.Exists(TxtPath.Text))
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show("Path not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // チェック結果がFALSEを戻す
                return false;
            }
            // Raplce-Withの場合
            else if (type.Contains(CHECK_TYPE.REPLACE_WITH))
            {
                // 変更前または変更後条件が入力されていない場合、エラーになる。
                if (string.IsNullOrEmpty(TxtReplace.Text)
                    || string.IsNullOrEmpty(TxtWith.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // チェック結果がFALSEを戻す
                    return false;
                }
            }

            // チェック結果がTRUEを戻す
            return true;
        }
    }
}
