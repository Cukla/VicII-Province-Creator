using System.Collections.Generic;
using System.Windows;

namespace VicII_Province_Creator
{
    /// <summary>
    /// Interaction logic for ProvinceListWindow.xaml
    /// </summary>
    public partial class ProvinceListWindow : Window
    {
        public ProvinceListWindow()
        {
            InitializeComponent();

        }

        public void ShowProvinceNames(List<string> TodaysList)
        {
            TodayProvinceNamesLB.Items.Clear();
            foreach (string p in TodaysList)
                TodayProvinceNamesLB.Items.Add(p);
        }
    }
}
