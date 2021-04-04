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
        /// 参照ボタン
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
            // 単項目チェック
            if (!CheckRequire(CHECK_TYPE.REPLACE_WITH))
            {
                return;
            }

            // 指定パスを取得する
            var strPath = TxtPath.Text;

            // パスが指定された場合
            if (!string.IsNullOrEmpty(strPath))
            {
                // フォルダー情報を取得
                var directoryInfo = new DirectoryInfo(strPath);

                // フォルダーにすべてファイルの情報を取得する
                var fileInfos = directoryInfo.GetFiles();

                // すべてファイルを繰返す
                foreach (FileInfo info in fileInfos)
                {
                    // 変更前ファイル名
                    var strOriginName = info.Name;

                    // 変更後ファイル名
                    var strChangeName = strOriginName.Replace(TxtReplace.Text, TxtWith.Text);

                    // 名の変更を実行する
                    File.Move(info.FullName, string.Concat(info.DirectoryName, System.IO.Path.DirectorySeparatorChar, strChangeName));
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
            // Raplce-Withの場合
            else if (type.Contains(CHECK_TYPE.REPLACE_WITH))
            {
                // 変更前または変更後条件が入力されていない場合、エラーになる。
                if (string.IsNullOrEmpty(TxtReplace.Text) 
                    || string.IsNullOrEmpty(TxtWith.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // チェック結果がFALSEを戻す
                    return false;
                }
            }

            // チェック結果がTRUEを戻す
            return true;
        }
    }
}
