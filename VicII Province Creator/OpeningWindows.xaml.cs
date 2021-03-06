using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace VicII_Province_Creator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        string mapPath;
        public Window1()
        {
            InitializeComponent();
        }

        public string getPath()
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Map Files (*.map)|*.map";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            string fail = "operation failed";

            if (choofdlog.ShowDialog() == true)
            {
                string sFileName = choofdlog.FileName;
                return sFileName;
            }
            return fail;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mapPath = getPath();
            mapPathTb.Text = mapPath;
            if (mapPath != "operation failed") gotoMainwidowsBtn.IsEnabled = true;

        }

        private void gotoMainwidowsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mapPath != "operation failed")
            {
                MainWindow window = new MainWindow(mapPath);
                window.Show();
                this.Close();
            }

        }

        private void mapPathTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mapPath == "operation failed")
            {
                gotoMainwidowsBtn.IsEnabled = false;
            }
        }
    }
}
