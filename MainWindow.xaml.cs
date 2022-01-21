using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private enum CHECK_TYPE
        {
            REPLACE_WITH,
            INSERT_INTO,
            NUMBERING,
            DATE,
            REGEX,
        }

        /// <summary>
        /// 名前で順番
        /// </summary>
        private readonly string SORTBY_NAME = "Name";
        /// <summary>
        /// 編集日で順番
        /// </summary>
        private readonly string SORTBY_MODIFIED = "Modified Date";
        /// <summary>
        /// 作成日で順番
        /// </summary>
        private readonly string SORTBY_CREATED = "Created Date";
        /// <summary>
        /// サイズで順番
        /// </summary>
        private readonly string SORTBY_SIZE = "Size";

        /// <summary>
        /// Current Software Version
        /// </summary>
        private readonly Version currentVersion;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Get current software version
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            currentVersion = new Version(fileVersionInfo.ProductVersion);

            // Set title
            Title = "Automatic Engine v" + currentVersion;

            // Set focus to Path textbox
            TxtPath.Focus();

            // Check for update
            CheckVersion();
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
        private bool CheckInput(CHECK_TYPE type)
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
                else if (!(bool)ChkInsertInto_Last.IsChecked)
                {
                    if (string.IsNullOrEmpty(TxtInsertInto_Index.Text))
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
            }
            // Numberingの場合
            else if (type.Equals(CHECK_TYPE.NUMBERING))
            {
                if (string.IsNullOrEmpty(TxtNumbering_Index.Text) && !(bool)ChkNumbering_Last.IsChecked)
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtNumbering_Index.Focus();

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
                else if (!int.TryParse(TxtNumbering_Index.Text, out _) && !(bool)ChkNumbering_Last.IsChecked)
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Invalid Index", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtNumbering_Index.Focus();

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
            // Dateの場合
            else if (type.Equals(CHECK_TYPE.DATE))
            {
                if (!string.IsNullOrEmpty(DpDate.Text)
                    && !DateTime.TryParse(DpDate.Text, out _))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Invalid date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    DpDate.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (string.IsNullOrEmpty(TxtDate_Index.Text) && !(bool)ChkDate_Last.IsChecked)
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtDate_Index.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }
                else if (!int.TryParse(TxtDate_Index.Text, out _) && !(bool)ChkDate_Last.IsChecked)
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Invalid Index", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtDate_Index.Focus();

                    // チェック結果がFALSEを戻す
                    return false;
                }

                if (!string.IsNullOrEmpty(TxtDate_Time.Text))
                {
                    if (!TimeSpan.TryParse(TxtDate_Time.Text, out _))
                    {
                        // エラーメッセージを表示する
                        System.Windows.Forms.MessageBox.Show("Invalid time format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (!DateTime.TryParse(DpDate.Text + " " + TxtDate_Time.Text, out _))
                    {
                        // エラーメッセージを表示する
                        System.Windows.Forms.MessageBox.Show("Invalid datetime format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            // Regexの場合
            else if (type.Equals(CHECK_TYPE.REGEX))
            {
                // 変更前条件が入力されていない場合、エラーになる
                if (string.IsNullOrEmpty(TxtRegex_Find.Text))
                {
                    // エラーメッセージを表示する
                    System.Windows.Forms.MessageBox.Show("Please input Condition", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // エラーコントロールをフォーカスする
                    TxtRegex_Find.Focus();

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
        private void ResetInput(object sender, RoutedEventArgs e)
        {
            TxtReplace.Text = string.Empty;
            TxtWith.Text = string.Empty;

            TxtInsertInto_Word.Text = string.Empty;
            TxtInsertInto_Index.Text = string.Empty;

            TxtNumbering_Index.Text = string.Empty;
            TxtNumbering_From.Text = string.Empty;
            RdoNumbering_Increase.IsChecked = true;
            ChkNumbering_Format.IsChecked = false;
            ChkNumbering_SortBy.IsChecked = false;
            ChkNumbering_Reverse.IsChecked = false;

            DpDate.Text = string.Empty;
            TxtDate_Index.Text = string.Empty;

            TxtRegex_Find.Text = string.Empty;
            TxtRegex_Replace.Text = string.Empty;
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
            // Dateの場合
            else if (sender.Equals(LblDate))
            {
                // Dateラジオを選択する
                RdoDate.IsChecked = true;
            }
            // Regexの場合
            else if (sender.Equals(LblRegex))
            {
                // Regexラジオを選択する
                RdoRegex.IsChecked = true;
            }
        }

        /// <summary>
        /// 処理実行する
        /// </summary>
        /// <param name="isPreview">プレビューフラグ</param>
        /// <param name="isExecuteAfterPreviewed">プレビュー後実行フラグ</param>
        private void Execute(bool isPreview, bool isExecuteAfterPreviewed)
        {
            try
            {
                // 単項目チェック
                if (((bool)RdoReplace.IsChecked
                    && !CheckInput(CHECK_TYPE.REPLACE_WITH))
                    || ((bool)RdoInsert.IsChecked
                        && !CheckInput(CHECK_TYPE.INSERT_INTO))
                    || ((bool)RdoNumbering.IsChecked
                       && !CheckInput(CHECK_TYPE.NUMBERING))
                    || ((bool)RdoDate.IsChecked
                       && !CheckInput(CHECK_TYPE.DATE))
                    || ((bool)RdoRegex.IsChecked
                       && !CheckInput(CHECK_TYPE.REGEX)))
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
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2)))
                    {
                        return;
                    }
                }

                // 指定パスを取得する
                var strPath = TxtPath.Text;

                // 変更前ファイル名一覧
                var listOriginalName = new List<string>();

                // 変更後ファイル名一覧
                var listChangeName = new List<string>();

                // 変更前ファイルフル名一覧
                var listOriginalNameWithPath = new List<string>();

                // 変更後ファイルフル名一覧
                var listChangeNameWithPath = new List<string>();

                // 実行ファイル数
                var executedFileCount = 0;

                // フォルダー情報を取得
                var directoryInfo = new DirectoryInfo(strPath);

                var directoryInfoList = directoryInfo.GetDirectories("*", SearchOption.AllDirectories).ToList();
                directoryInfoList.Add(directoryInfo);

                // 実施前バックアップ
                if ((bool)ChkBackup.IsChecked && !isPreview)
                {
                    var strBackupPath = Directory.CreateDirectory(TxtPath.Text).FullName + "\\" + "backup_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    Directory.CreateDirectory(strBackupPath);

                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(TxtPath.Text, "*", SearchOption.AllDirectories))
                    {
                        if (dirPath.Equals(strBackupPath))
                        {
                            continue;
                        }
                        Directory.CreateDirectory(dirPath.Replace(TxtPath.Text, strBackupPath));
                    }

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(TxtPath.Text, "*.*", SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(TxtPath.Text, strBackupPath), true);
                    }
                }

                // 指定されたフォルダの中にすべてのファイルかつフォルダをループする
                foreach (DirectoryInfo dictInfo in directoryInfoList)
                {
                    // フォルダーにすべてファイルの情報を取得する
                    var fileInfos = dictInfo.GetFiles();

                    // Numberingモードの場合
                    int numbering_at = 0;
                    int numbering_from = 0;
                    if ((bool)RdoNumbering.IsChecked)
                    {
                        if (!(bool)ChkNumbering_Last.IsChecked)
                        {
                            numbering_at = int.Parse(TxtNumbering_Index.Text);
                        }
                        numbering_from = int.Parse(TxtNumbering_From.Text);
                    }

                    // 整合性チェック
                    int shortestNameLength = int.MaxValue;
                    foreach (FileInfo info in fileInfos)
                    {
                        if (shortestNameLength > info.Name.Length)
                        {
                            shortestNameLength = info.Name.Length;
                        }
                    }
                    if ((bool)RdoInsert.IsChecked)
                    {
                        if (!(bool)ChkInsertInto_Last.IsChecked && int.Parse(TxtInsertInto_Index.Text) > shortestNameLength)
                        {
                            // エラーメッセージを表示する
                            System.Windows.Forms.MessageBox.Show(
                                string.Format("Invalid index\r\nIndex larger than shortest Filename: {0}", shortestNameLength),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // エラーコントロールをフォーカスする
                            TxtInsertInto_Index.Focus();

                            // 処理停止
                            return;
                        }
                    }
                    else if ((bool)RdoNumbering.IsChecked)
                    {
                        if ((bool)RdoNumbering_Decrease.IsChecked
                            && numbering_from < fileInfos.Length)
                        {
                            // エラーメッセージを表示する
                            System.Windows.Forms.MessageBox.Show(
                                string.Format("Invalid decrease number\r\nDecrease number smaller than total num of files: {0}", fileInfos.Length),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // エラーコントロールをフォーカスする
                            TxtNumbering_From.Focus();

                            // 処理停止
                            return;
                        }
                        else if (!(bool)ChkNumbering_Last.IsChecked && int.Parse(TxtNumbering_Index.Text) > shortestNameLength)
                        {
                            // エラーメッセージを表示する
                            System.Windows.Forms.MessageBox.Show(
                                string.Format("Invalid index\r\nIndex larger than shortest Filename: {0}", shortestNameLength),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // エラーコントロールをフォーカスする
                            TxtNumbering_Index.Focus();

                            // 処理停止
                            return;
                        }
                    }
                    else if ((bool)RdoDate.IsChecked)
                    {
                        if (!(bool)ChkDate_Last.IsChecked && int.Parse(TxtDate_Index.Text) > shortestNameLength)
                        {
                            // エラーメッセージを表示する
                            System.Windows.Forms.MessageBox.Show(
                                string.Format("Invalid index (Index larger than shortest Filename: {0})", shortestNameLength),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // エラーコントロールをフォーカスする
                            TxtDate_Index.Focus();

                            // 処理停止
                            return;
                        }
                    }

                    // 順番処理
                    if ((bool)ChkNumbering_SortBy.IsChecked)
                    {
                        string selectedValue = ((ComboBoxItem)CbbSortBy.SelectedValue).Content.ToString();

                        // 名前で順番する
                        if (SORTBY_NAME.Equals(selectedValue))
                        {
                            fileInfos = fileInfos.OrderBy(v => v.Name).ToArray();
                        }
                        // 編集日で順番する
                        else if (SORTBY_MODIFIED.Equals(selectedValue))
                        {
                            fileInfos = fileInfos.OrderBy(v => v.LastWriteTime).ToArray();
                        }
                        // 作成日で順番する
                        else if (SORTBY_CREATED.Equals(selectedValue))
                        {
                            fileInfos = fileInfos.OrderBy(v => v.CreationTime).ToArray();
                        }
                        // サイズで順番する
                        else if (SORTBY_SIZE.Equals(selectedValue))
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

                        // 変更後ファイル名
                        var strChangeName = strOriginName;

                        // Replace-With処理
                        if ((bool)RdoReplace.IsChecked)
                        {
                            // ファイル名にReplace文字が含まない場合、処理対象以外になる
                            if (!strOriginName.Contains(TxtReplace.Text))
                            {
                                // 次のレコードを移動する
                                continue;
                            }

                            strChangeName = strOriginName.Replace(TxtReplace.Text, TxtWith.Text);
                        }
                        // Insert-Into処理
                        else if ((bool)RdoInsert.IsChecked)
                        {
                            // 最後位置チェックボックスをチェックする場合
                            if ((bool)ChkInsertInto_Last.IsChecked)
                            {
                                strChangeName = strOriginName.Insert(strOriginName.IndexOf(info.Extension), TxtInsertInto_Word.Text);
                            }
                            else
                            {
                                strChangeName = strOriginName.Insert(int.Parse(TxtInsertInto_Index.Text), TxtInsertInto_Word.Text);
                            }
                        }
                        // Numbering処理
                        else if ((bool)RdoNumbering.IsChecked)
                        {
                            // 変換位置を設定する
                            int changeIndex = 0;
                            if ((bool)ChkNumbering_Last.IsChecked)
                            {
                                changeIndex = strOriginName.IndexOf(info.Extension);
                            }
                            else
                            {
                                changeIndex = numbering_at;
                            }

                            // フォーマットチェックする時
                            if ((bool)ChkNumbering_Format.IsChecked)
                            {
                                // 変数をフォーマットする
                                int padNum = fileInfos.Length.ToString().Length;

                                strChangeName = strOriginName.Insert(changeIndex, numbering_from.ToString().PadLeft(padNum, '0'));
                            }
                            else
                            {
                                strChangeName = strOriginName.Insert(changeIndex, numbering_from.ToString());
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
                        // Date処理
                        else if ((bool)RdoDate.IsChecked)
                        {
                            // 変換位置を設定する
                            int changeIndex = 0;
                            if ((bool)ChkDate_Last.IsChecked)
                            {
                                changeIndex = strOriginName.IndexOf(info.Extension);
                            }
                            else
                            {
                                changeIndex = int.Parse(TxtDate_Index.Text);
                            }

                            // 日付を選択しない場合
                            if (string.IsNullOrEmpty(DpDate.Text))
                            {
                                if (string.IsNullOrEmpty(TxtDate_Time.Text))
                                {
                                    // システム日付で印刷する
                                    strChangeName = strOriginName.Insert(changeIndex, DateTime.Now.ToString(TxtDateFormat.Text));
                                }
                                else
                                {
                                    DateTime dateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + TxtDate_Time.Text);
                                    strChangeName = strOriginName.Insert(changeIndex, dateTime.ToString(TxtDateFormat.Text));
                                }

                            }
                            // 日付を選択する場合
                            else
                            {
                                if (string.IsNullOrEmpty(TxtDate_Time.Text))
                                {
                                    // 指定された日付で印刷する
                                    strChangeName = strOriginName.Insert(changeIndex, DateTime.Parse(DpDate.Text).ToString(TxtDateFormat.Text));
                                }
                                else
                                {
                                    // 指定された日付で印刷する
                                    DateTime date = DateTime.Parse(DpDate.Text + " " + TxtDate_Time.Text);
                                    strChangeName = strOriginName.Insert(changeIndex, date.ToString(TxtDateFormat.Text));
                                }
                            }
                        }
                        // Regex処理
                        else if ((bool)RdoRegex.IsChecked)
                        {
                            strChangeName = Regex.Replace(strOriginName, TxtRegex_Find.Text, TxtRegex_Replace.Text);
                        }

                        // 変更前ファイル名一覧に変更前ファイル名を追加する
                        listOriginalName.Add(strOriginName);
                        listOriginalNameWithPath.Add(info.FullName);

                        // 変更後ファイル名一覧に変更後ファイル名を追加する
                        listChangeName.Add(strChangeName);
                        listChangeNameWithPath.Add(info.Directory.FullName + "\\" + strChangeName);

                        // 実行モードの場合
                        if (!isPreview)
                        {
                            // 名の変更を実行する
                            File.Move(info.FullName, string.Concat(info.DirectoryName, System.IO.Path.DirectorySeparatorChar, strChangeName));

                            // 処理対象数をカウントアップする
                            executedFileCount++;
                        }
                    }
                }

                // プレビューモードの場合
                if (isPreview)
                {
                    // プレビュー画面を作成する
                    var previewWindow = new PreviewWindow(
                        listOriginal: listOriginalName,
                        listChanged: listChangeName,
                        listOriginalFull: listOriginalNameWithPath,
                        listChangedFull: listChangeNameWithPath)
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
            catch (Exception e)
            {
                // エラーメッセージを表示する
                System.Windows.Forms.MessageBox.Show(
                    string.Format("Internal error\r\nError message:{0}", e.Message),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // エラーコントロールをフォーカスする
                TxtPath.Focus();

                // 処理停止
                return;
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

        /// <summary>
        /// InsertInto_Lastチェックボックスをチェックする時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkInsertInto_Last_Click(object sender, RoutedEventArgs e)
        {
            // 最後位置チェックボックスをチェックする場合
            if ((bool)ChkInsertInto_Last.IsChecked)
            {
                // 位置設定テキストボックスを非活性になる
                TxtInsertInto_Index.IsEnabled = false;
            }
            // 上記以外の場合
            else
            {
                // 位置設定テキストボックスを活性になる
                TxtInsertInto_Index.IsEnabled = true;
            }
        }

        /// <summary>
        /// Numbering_Lastチェックボックスをチェックする時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkNumbering_Last_Click(object sender, RoutedEventArgs e)
        {
            // 最後位置チェックボックスをチェックする場合
            if ((bool)ChkNumbering_Last.IsChecked)
            {
                // 位置設定テキストボックスを非活性になる
                TxtNumbering_Index.IsEnabled = false;
            }
            // 上記以外の場合
            else
            {
                // 位置設定テキストボックスを活性になる
                TxtNumbering_Index.IsEnabled = true;
            }
        }

        /// <summary>
        /// Date_Lastチェックボックスをチェックする時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkDate_Last_Click(object sender, RoutedEventArgs e)
        {
            // 最後位置チェックボックスをチェックする場合
            if ((bool)ChkDate_Last.IsChecked)
            {
                // 位置設定テキストボックスを非活性になる
                TxtDate_Index.IsEnabled = false;
            }
            // 上記以外の場合
            else
            {
                // 位置設定テキストボックスを活性になる
                TxtDate_Index.IsEnabled = true;
            }
        }

        /// <summary>
        /// Aboutページを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            // Aboutページを表示する
            new AboutPage().ShowDialog();
        }

        /// <summary>
        /// 新しいバージョンが存在するかチェックする
        /// </summary>
        /// <returns></returns>
        private void CheckVersion()
        {
            try
            {
                const string GITHUB_API = "https://api.github.com/repos/{0}/{1}/releases";
                WebClient webClient = new GitHubWebClient();
                webClient.Headers.Add("User-Agent", "Unity web player");
                Uri uri = new Uri(string.Format(GITHUB_API, "phamngocvinh", "automatic-engine"));
                string releases = webClient.DownloadString(uri);

                // Get newest version number
                string pattern = @"v(\d+.\d+.\d+)";
                Regex rg = new Regex(pattern);
                MatchCollection matchedAuthors = rg.Matches(releases);
                Version version = new Version(matchedAuthors[0].Groups[1].Value);

                // Get newest version changes log
                pattern = "body\\\":\\\"(.*?)\\\"";
                rg = new Regex(pattern);
                matchedAuthors = rg.Matches(releases);
                var changesLog = matchedAuthors[0].Groups[1].Value;
                changesLog = changesLog.Replace("\\r\\n", "{0}");

                // If newest version newer than current version
                if (version.CompareTo(currentVersion) > 0)
                {
                    // Show update dialog
                    string msg = "A new version of Automatic Engine is available\r\n・Current: {0}\r\n・New: {1}\r\n\r\nChanges log:\r\n{2}\r\n\r\nWould you like to upgrade it now ?";
                    msg = string.Format(msg, currentVersion, version, string.Format(changesLog, Environment.NewLine));
                    DialogResult result = System.Windows.Forms.MessageBox.Show(msg, "Update App?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // If user click yes then open project link and shutdown program
                    if (result.Equals(System.Windows.Forms.DialogResult.Yes))
                    {
                        Process.Start("https://github.com/phamngocvinh/automatic-engine/releases/latest");
                        System.Windows.Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// カスタマイズWebClient
        /// </summary>
        private class GitHubWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 3000;
                return w;
            }
        }

        /// <summary>
        /// ヘルプページを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/phamngocvinh/automatic-engine/wiki/Features");
        }
    }
}
