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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            DialogResult dialogResult = folderDialog.ShowDialog();
            if (System.Windows.Forms.DialogResult.OK.Equals(dialogResult))
            {
                TxtPath.Text = folderDialog.SelectedPath;
            }
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            var strPath = TxtPath.Text;
            if (!string.IsNullOrEmpty(strPath))
            {
                var arrFiles = Directory.GetFiles(strPath);
                foreach (string filePath in arrFiles)
                {

                }
            }
        }
    }
}
