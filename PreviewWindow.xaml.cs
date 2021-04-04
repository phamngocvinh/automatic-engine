using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace automatic_engine
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        /// <summary>
        /// ダイアログ結果
        /// </summary>
        public static bool dialogResult = false;

        public PreviewWindow(List<string> listOriginal, List<string> listChanged)
        {
            InitializeComponent();
            foreach (string name in listOriginal)
            {
                TxtBefore.Text += string.Concat(name, Environment.NewLine);
            }
            foreach (string name in listChanged)
            {
                TxtAfter.Text += string.Concat(name, Environment.NewLine);
            }
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = false;
            Close();
        }
    }
}
