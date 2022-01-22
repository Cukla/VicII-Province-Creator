using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;

namespace VicII_Province_Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProvinceListWindow w = new ProvinceListWindow();

        string TextColor = "Black";

        string id;
        string provName;
        string pathToMap;
        int cantPops;
        string RGB;
        string red;
        string green;
        string blue;
        string countryName;
        string continent;
        string climate;
        string region;
        string tradegood;
        string controller;
        string liferating;
        string owner;
        bool uncolonized = false;
        string popDir;

        List<string> AutoCompletationCultureList = new List<string>();
        List<string> CulturesRGBsColorsList = new List<string>();
        List<string> provinceNames = new List<string>();
        List<string> cores = new List<string>();
        List<string> popType = new List<string>();
        List<string> popCul = new List<string>();
        List<string> popRel = new List<string>();
        List<string> popSize = new List<string>();
        List<int> listId = new List<int>();
        List<string> listNamesProv = new List<string>();
        List<string> RGBs = new List<string>();
        List<string> RegionSuggestionList = new List<string>();

        public MainWindow(string Path)
        {
            InitializeComponent();
            pathToMap = Path;
            readDefFile(getFolder("default") + "definition.csv");
            RegionSuggestionList = GetRegions(getFolder("default") + "region.txt");
            coreTb.Text = "Core";
            var lastId = listId.Max() + 1;
            idTb.Text = lastId.ToString();

            popDir = getFolder("map") + @"history\pops\1836.1.1\";
            popDirTb.Text = popDir;

            SeriesCollection = new SeriesCollection { };
            SeriesCollection2 = new SeriesCollection { };
            AutoCompletationCultureList = GetAllCulturesNames();

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }

        public SeriesCollection SeriesCollection2 { get; set; }

        public List<String> GetRegions(string path)
        {
            List<string> Regions = new List<string>();
            string[] RegionsFile = System.IO.File.ReadAllLines(@path);
            foreach (string line in RegionsFile)
            {
                if (line.Contains(" = {") == true && line.IndexOf("#") != 0)
                {
                    if (line.Contains('#'))
                        Regions.Add(line.Remove(0, line.IndexOf("#") + 1) + " - " + line.Remove(line.IndexOf("= {")));
                    else
                        Regions.Add(line.Remove(line.IndexOf("= {")));
                }
            }
            return Regions;
        }

        private void createBtn_Click(object sender, RoutedEventArgs e)
        {

            id = idTb.Text;
            var idlast = Int32.Parse(id) + 1;
            idTb.Text = idlast.ToString();

            provName = provNameTb.Text;
            red = redTb.Text;
            green = greenTb.Text;
            blue = blueTb.Text;
            countryName = countryNameTb.Text;
            continent = GetTextCb(continentCb);
            climate = GetTextCb(climateCb);
            region = regionTb.Text;
            tradegood = goodTb.Text;
            liferating = liferatingTb.Text;

            if (controllerTb.IsEnabled != true)
            {
                controller = ownerTb.Text;
            }
            else
            {
                controller = controllerTb.Text;
            }

            if (uncolonized != true)
            {
                controller = ownerTb.Text;
            }
            else
            {
                controller = controllerTb.Text;
            }

            owner = ownerTb.Text;
            cantPops = popCul.Count;


            CreatePops();
            writeDefFile();
            WriteCliArch(getFolder("default") + "climate.txt");
            WriteContArch(getFolder("default") + "continent.txt");
            WriteRegArch(getFolder("default") + "region.txt");
            WritePosFile(getFolder("default") + "positions.txt");
            CreateProvHis(uncolonized);
            WriteLocFile();
            writeDefaultArc();
            MessageBox.Show("Province created succefully");
            CreateLogArchive();
            provinceNames.Add(provName);
            w.ShowProvinceNames(provinceNames);
        }

        public void CreateLogArchive()
        {
            string dir = @"log\" + countryName;
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filename = dir + @"\" + this.id + " - " + this.provName + ".txt";

            // Check if file already exists. If yes, delete it.     
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.WriteLine("created at = " + DateTime.Now.ToString());
                sw.WriteLine("id = " + this.id);
                sw.WriteLine("province name = " + provName);
                sw.WriteLine("rgb = " + red + ";" + green + ";" + blue);
                sw.WriteLine("country name = " + countryName);
                sw.WriteLine("continent = " + this.continent);
                sw.WriteLine("climate = " + this.climate);
                sw.WriteLine("region = " + this.region);
                sw.WriteLine("owner = " + this.owner);
                sw.WriteLine("controller = " + this.controller);
                for (int l = 0; l < cores.Count; l++)
                {
                    sw.WriteLine("core = " + cores[l]);
                }
                sw.WriteLine("trade good = " + this.tradegood);
                sw.WriteLine("life rating = " + this.liferating);
            }

            MessageBox.Show("Log file created succefully at " + filename);

        }

        public string[] ReadTxt(string path)
        {

            string[] archContent = System.IO.File.ReadAllLines(@path);

            return archContent;
        }

        public void writeDefaultArc()
        {
            var path = pathToMap;
            string[] defArchCont = ReadTxt(path);

            int max_provinces = Int32.Parse(id) + 1;

            defArchCont[0] = "max_provinces = " + max_provinces.ToString();

            File.WriteAllLines(path, defArchCont);
        }

        public void WriteLocFile()
        {
            var path = getFolder("map");
            path += "localisation/00_newmap.csv";
            var csv = new StringBuilder();

            var id = this.id.ToString();
            var name = this.provName.ToString();

            var newLine = string.Format("PROV{0};{1};;;;;;;;;;;;;x,,,,,,,,,,,,,,,,,,,,,,", id, name);
            csv.AppendLine(newLine);

            File.AppendAllText(path, csv.ToString());
        }

        public void CreateProvHis(bool col)
        {
            string dir = @getFolder("map") + "history/provinces/new provinces " + countryName;
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filename = getFolder("map") + @"history\provinces\new provinces " + countryName + @"\" + this.id + " - " + this.provName + ".txt";

            // Check if file already exists. If yes, delete it.     
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            if (col == false)
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine("owner = " + this.owner);
                    sw.WriteLine("controller = " + this.controller);
                    for (int l = 0; l < cores.Count; l++)
                    {
                        sw.WriteLine("add_core = " + cores[l]);
                    }
                    sw.WriteLine("trade_goods = " + this.tradegood);
                    sw.WriteLine("life_rating = " + this.liferating);
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine("trade_goods = " + this.tradegood);
                    sw.WriteLine("life_rating = " + this.liferating);
                }
            }
            // Create a new file     


        }

        public void WritePosFile(string path)
        {

            try
            {

                // Create a new file     
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(this.id + " = {");
                    sw.WriteLine("    text_position = {");
                    sw.WriteLine("        x = 100");
                    sw.WriteLine("        y = 100");
                    sw.WriteLine("    }\n");
                    sw.WriteLine("    text_scale = 3");
                    sw.WriteLine("}");
                }



            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error", Ex.ToString());
            }
        }

        public void WriteRegArch(string path)
        {
            int index = 0;
            int ind = 0;
            string regg = String.Empty;
            if (region.Contains('-'))
                regg = region.Split('-')[1].Remove(0, 1);
            else
                regg = region;
            string[] contArchCont = ReadTxt(path);
            foreach (string reg in contArchCont)
            {
                if (reg.Contains(regg))
                {
                    index = ind;
                }
                ind++;
            }

            string s = contArchCont[index];
            string[] subs = s.Split('}');
            var ss = subs[1];

            contArchCont[index] = contArchCont[index].Remove(contArchCont[index].IndexOf("}"));
            contArchCont[index] += " " + this.id + " }" + ss;

            File.WriteAllLines(path, contArchCont);
        }

        public void WriteContArch(string path)
        {
            string[] contArchCont = ReadTxt(path);
            int index = Array.LastIndexOf(contArchCont, reformatString(continent) + " = {");

            contArchCont[index + 3] += " " + this.id;

            File.WriteAllLines(path, contArchCont);

        }

        public void WriteCliArch(string path)
        {
            string[] cliArchCont = ReadTxt(path);
            int index = Array.LastIndexOf(cliArchCont, reformatString(climate) + " = {");

            cliArchCont[index + 1] += " " + this.id;

            File.WriteAllLines(path, cliArchCont);

        }

        private void writeDefFile()
        {

            var path = getFolder("default") + "definition.csv";
            var csv = new StringBuilder();
            var id = this.id.ToString();
            var red = this.red.ToString();
            var green = this.green.ToString();
            var blue = this.blue.ToString();
            var name = this.provName.ToString();
            var newLine = string.Format("{0};{1};{2};{3};{4};x", id, red, green, blue, name);
            csv.AppendLine(newLine);
            File.AppendAllText(path, csv.ToString());
        }

        public void CreatePops()
        {

            string filename = popDir + countryName + ".txt";
            try
            {

                // Create a new file     
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(id + " = {");

                    for (int z = 0; z < this.cantPops; z++)
                    {
                        sw.WriteLine("    " + reformatString(popType[z]) + " = {");
                        sw.WriteLine("        culture = " + this.popCul[z]);
                        sw.WriteLine("        religion = " + reformatString(popRel[z]));
                        sw.WriteLine("        size = " + this.popSize[z]);
                        sw.WriteLine("    }");

                    }
                    sw.WriteLine("}");

                }


            }
            catch (Exception Ex)
            {
                MessageBox.Show("Pop creation failed due to " + Ex.ToString());
                MessageBox.Show(getFolder("map") + @"history\pops\1836.1.1\" + countryName + ".txt");
            }
        }

        public void readDefFile(string path)
        {
            using (var reader = new StreamReader(@path))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    try
                    {
                        listId.Add(Int32.Parse(values[0]));
                    }
                    catch { }

                    RGBs.Add(values[1] + values[2] + values[3]);
                    listNamesProv.Add(values[4]);
                }
            }

        }

        public List<string> GenerateRGB()
        {
            System.Random random = new System.Random();

            int red = random.Next(0, 255);
            int green = random.Next(0, 255);
            int blue = random.Next(0, 255);
            List<string> rgb = new List<string>();

            rgb.Add(red.ToString() + green.ToString() + blue.ToString());
            rgb.Add(red.ToString());
            rgb.Add(green.ToString());
            rgb.Add(blue.ToString());

            return rgb;
        }

        public List<string> GenerateUniqueRGB()
        {
            bool condition = true;
            List<string> rgb = new List<string>();
            List<string> r = new List<string>();
            while (condition == true)
            {
                r.Clear();
                rgb.Clear();
                r = GenerateRGB();
                rgb.Add(r[0]);
                rgb.Add(r[1]);
                rgb.Add(r[2]);
                rgb.Add(r[3]);
                condition = RGBs.Contains(rgb[0]);
            }
            return rgb;
        }

        public bool CheckRGB()
        {
            bool ans = true;
            var condition = RGBs.Contains(this.RGB);

            if (condition == false)
            {
                ans = false;
                return ans;
            }

            return ans;
        }

        private void addCoreBtn_Click(object sender, RoutedEventArgs e)
        {
            coresLb.Items.Add(new core() { name = coreTb.Text });
            cores.Add(coreTb.Text);
        }

        public class core
        {

            public string name { get; set; }

        }

        public class popInfo
        {
            public string Type { get; set; }
            public string Culture { get; set; }
            public string Size { get; set; }
            public string Religion { get; set; }
        }

        private string GetTextCb(ComboBox cb)
        {
            string selectedText = "";

            ComboBoxItem popT = (ComboBoxItem)cb.SelectedItem;
            TextBlock typeSelected = (TextBlock)popT.Content;

            selectedText = typeSelected.Text;

            return selectedText;
        }

        public List<int> getPopIndexes(List<int> saveList, List<string> List, string val)
        {
            saveList.Clear();
            var i = 0;
            foreach (var value in List)
            {
                if (value == val)
                    saveList.Add(i);
                i++;
            }
            return saveList;
        }

        public int grupTypes(List<int> saveList, List<string> numList, List<string> cul, string val)
        {
            int total = 0;
            saveList.Clear();
            List<int> indexes;
            indexes = getPopIndexes(saveList, cul, val);

            for (int i = 0; i < indexes.Count; i++)
            {
                total += Int32.Parse(numList[indexes[i]]);
            }
            return total;
        }

        public void drawCharts(List<int> saveList, List<string> numList, List<string> cul, List<string> tempNames, List<int> tempSize, SeriesCollection series)
        {
            foreach (var a in cul)
            {
                if (tempNames.Contains(a) == false)
                {
                    tempSize.Add(grupTypes(saveList, numList, cul, a));
                    tempNames.Add(a);
                }
            }

            series.Clear();

            if (tempNames.Count > 0)
            {
                for (int ai = 0; ai < tempNames.Count; ai++)
                {
                    if (AutoCompletationCultureList.IndexOf(tempNames[ai]) != -1)
                        series.Add(new PieSeries
                        {
                            Title = tempNames[ai],
                            Values = new ChartValues<ObservableValue> { new ObservableValue(tempSize[ai]) },
                            DataLabels = true,
                            Fill = new SolidColorBrush(Color.FromArgb(255,
                            (byte)Convert.ToInt32(CulturesRGBsColorsList[AutoCompletationCultureList.IndexOf(tempNames[ai])].Split(' ')[0].Replace(" ", string.Empty)),
                            (byte)Convert.ToInt32(CulturesRGBsColorsList[AutoCompletationCultureList.IndexOf(tempNames[ai])].Split(' ')[1].Replace(" ", string.Empty)),
                            (byte)Convert.ToInt32(CulturesRGBsColorsList[AutoCompletationCultureList.IndexOf(tempNames[ai])].Split(' ')[2].Replace(" ", string.Empty))))
                        });
                    else
                        series.Add(new PieSeries
                        {
                            Title = tempNames[ai],
                            Values = new ChartValues<ObservableValue> { new ObservableValue(tempSize[ai]) },
                            DataLabels = true
                        });
                }

            }
        }

        private void addPopBtn_Click(object sender, RoutedEventArgs e)
        {
            List<int> culIndexes = new List<int>();
            List<int> typeIndexes = new List<int>();
            List<string> tempCul = new List<string>();
            List<int> tempCulSize = new List<int>();
            List<int> tempSize = new List<int>();
            List<string> tempType = new List<string>();
            popsLb.Items.Add(new popInfo() { Size = sizeTb.Text, Culture = cultureTb.Text.ToLower(), Religion = GetTextCb(religionCb).ToLower(), Type = GetTextCb(typeCb).ToLower() });
            popType.Add(GetTextCb(typeCb));
            popCul.Add(cultureTb.Text);
            popRel.Add(GetTextCb(religionCb));
            popSize.Add(sizeTb.Text);

            drawCharts(culIndexes, popSize, popCul, tempCul, tempCulSize, SeriesCollection);
            drawCharts(typeIndexes, popSize, popType, tempType, tempSize, SeriesCollection2);

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (controllerTb.IsEnabled != true)
            {
                controllerTb.IsEnabled = true;
            }
            else
            {
                controllerTb.IsEnabled = false;
            }
        }

        private void checkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.RGB = redTb.Text + greenTb.Text + blueTb.Text;
            if (CheckRGB() == true)
            {
                MessageBox.Show("The RGB code is not disponible");
            }
            else
            {
                MessageBox.Show("The RGB code is disponible");
            }

        }

        private void delCoreBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = (coresLb.SelectedIndex);
                coresLb.Items.RemoveAt(coresLb.Items.IndexOf(coresLb.SelectedItem));
                cores.RemoveAt(x);
            }
            catch
            {
                MessageBox.Show("Select an item", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void delPopBtn_Click(object sender, RoutedEventArgs e)
        {
            List<int> culIndexes = new List<int>();
            List<int> typeIndexes = new List<int>();
            List<string> tempCul = new List<string>();
            List<int> tempCulSize = new List<int>();
            List<int> tempSize = new List<int>();
            List<string> tempType = new List<string>();

            try
            {
                int x = (popsLb.SelectedIndex);
                popsLb.Items.RemoveAt(popsLb.Items.IndexOf(popsLb.SelectedItem));
                popCul.RemoveAt(x);
                popRel.RemoveAt(x);
                popSize.RemoveAt(x);
                popType.RemoveAt(x);

                drawCharts(culIndexes, popSize, popCul, tempCul, tempCulSize, SeriesCollection);
                drawCharts(typeIndexes, popSize, popType, tempType, tempSize, SeriesCollection2);
            }
            catch
            {
                MessageBox.Show("Select an item", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ownerTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            coreTb.Text = ownerTb.Text;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public string getFolder(string index)
        {
            string path = this.pathToMap;
            if (index == "map")
            {
                path = path.Remove(path.LastIndexOf("."));
            }
            string folder = String.Empty;
            int i = 0;
            var count = Regex.Matches(path, index).Count;
            if (count > 1)
            {
                i = path.LastIndexOf(index);
                folder = path.Remove(i);
            }
            else
            {
                i = path.IndexOf(index);
                folder = path.Remove(i);
            }

            return folder;
        }

        private void mixRgbBtn_Click(object sender, RoutedEventArgs e)
        {
            var rgb = GenerateUniqueRGB();
            redTb.Text = rgb[1];
            greenTb.Text = rgb[2];
            blueTb.Text = rgb[3];
        }

        public string reformatString(string s)
        {
            var rs = s;

            rs = rs.ToLower();
            rs = rs.Replace(" ", "_");

            return rs;
        }

        private void uncolonizedCx_Click(object sender, RoutedEventArgs e)
        {
            if (uncolonized == false)
            {
                uncolonized = true;
                coreTb.IsEnabled = false;
                ownerTb.IsEnabled = false;
                addCoreBtn.IsEnabled = false;
            }
            else
            {
                uncolonized = false;
                coreTb.IsEnabled = true;
                ownerTb.IsEnabled = true;
                addCoreBtn.IsEnabled = true;
            }

        }
        public void getFolder()
        {

            try
            {
                var dialog = new FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();
                popDir = Directory.GetFiles(dialog.SelectedPath)[0];
                popDir = popDir.Remove(popDir.LastIndexOf(@"\"));
                popDir += @"\";
                popDirTb.Text = popDir;
            }
            catch
            {
                MessageBox.Show("Error", "Error");
            }

        }

        private void popDirBtn_Click(object sender, RoutedEventArgs e)
        {
            getFolder();
        }

        private List<string> GetAllCulturesNames()
        {
            var dir = getFolder("map") + @"common\cultures.txt";
            List<string> rl = new List<string>();
            string[] culCont = ReadTxt(dir);
            int i = 0;
            foreach (string cul in culCont)
            {
                if (i != culCont.Length)
                    if (cul.Contains(" = {") && culCont[i + 1].Contains("color") && !cul.Contains("#"))
                    {
                        CulturesRGBsColorsList.Add(culCont[i + 1].Split('{')[1]);
                        if (CulturesRGBsColorsList.Last().Length > CulturesRGBsColorsList.Last().IndexOf("}"))
                            CulturesRGBsColorsList[CulturesRGBsColorsList.Count() - 1] = CulturesRGBsColorsList[CulturesRGBsColorsList.Count() - 1].Remove(CulturesRGBsColorsList.Last().IndexOf("}"));
                        else
                            CulturesRGBsColorsList[CulturesRGBsColorsList.Count() - 1] = CulturesRGBsColorsList[CulturesRGBsColorsList.Count() - 1].Replace("}", String.Empty);
                        if (CulturesRGBsColorsList.Last()[0] == ' ')
                            CulturesRGBsColorsList[CulturesRGBsColorsList.Count() - 1] = CulturesRGBsColorsList[CulturesRGBsColorsList.Count() - 1].Remove(0, 1);
                        rl.Add(cul.Split('=')[0].Replace(" ", string.Empty));
                    }
                i++;
            }
            return rl;
        }

        private List<string> GetAllCountriesNames()
        {
            var dir = getFolder("map") + @"common\countries\";
            List<string> rl = new List<string>();
            DirectoryInfo dinfo = new DirectoryInfo(dir);
            FileInfo[] Files = dinfo.GetFiles("*.txt");
            foreach (FileInfo file in Files)
                rl.Add(file.Name.Split('.')[0]);

            return rl;
        }

        #region Popups
        #region Countries
        //for the region autocompletation
        private void OpenAutoCountrySuggestionBox()
        {
            try
            {
                // Enable.  

                autoCountryPopup.Visibility = Visibility.Visible;
                autoCountryPopup.IsOpen = true;
                CountryList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void CloseAutoCountrySuggestionBox()
        {
            try
            {
                // Enable.  
                autoCountryPopup.Visibility = Visibility.Collapsed;
                autoCountryPopup.IsOpen = false;
                CountryList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void countryTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Verification.  
                if (string.IsNullOrEmpty(this.countryNameTb.Text))
                {
                    // Disable.  
                    CloseAutoCountrySuggestionBox();

                    // Info.  
                    return;
                }

                // Enable.  
                OpenAutoCountrySuggestionBox();

                // Settings.  
                CountryList.ItemsSource = GetAllCountriesNames().Where(p => p.ToLower().Contains(countryNameTb.Text.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void countryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Verification.  
                if (CountryList.SelectedIndex <= -1)
                {
                    // Disable.  
                    CloseAutoCountrySuggestionBox();

                    // Info.  
                    return;
                }

                // Disable.  
                CloseAutoCountrySuggestionBox();

                // Settings.  
                countryNameTb.Text = CountryList.SelectedItem.ToString();
                CountryList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }
        #endregion
        #region Region
        //for the region autocompletation
        private void OpenAutoSuggestionBox()
        {
            try
            {
                // Enable.  

                autoRegionPopup.Visibility = Visibility.Visible;
                autoRegionPopup.IsOpen = true;
                regionList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void CloseAutoSuggestionBox()
        {
            try
            {
                // Enable.  
                autoRegionPopup.Visibility = Visibility.Collapsed;
                autoRegionPopup.IsOpen = false;
                regionList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void regionTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Verification.  
                if (string.IsNullOrEmpty(this.regionTb.Text))
                {
                    // Disable.  
                    CloseAutoSuggestionBox();

                    // Info.  
                    return;
                }

                // Enable.  
                OpenAutoSuggestionBox();

                // Settings.  
                regionList.ItemsSource = RegionSuggestionList.Where(p => p.ToLower().Contains(regionTb.Text.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void regionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Verification.  
                if (regionList.SelectedIndex <= -1)
                {
                    // Disable.  
                    CloseAutoSuggestionBox();

                    // Info.  
                    return;
                }

                // Disable.  
                CloseAutoSuggestionBox();

                // Settings.  
                regionTb.Text = regionList.SelectedItem.ToString();
                regionList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }
        #endregion
        #region cultures 
        //for the region autocompletation
        private void OpenCultureAutoSuggestionBox()
        {
            try
            {
                // Enable.  

                autoCulturePopup.Visibility = Visibility.Visible;
                autoCulturePopup.IsOpen = true;
                CultureList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void CloseCultureAutoSuggestionBox()
        {
            try
            {
                // Enable.  
                autoCulturePopup.Visibility = Visibility.Collapsed;
                autoCulturePopup.IsOpen = false;
                CultureList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void CultureTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Verification.  
                if (string.IsNullOrEmpty(this.cultureTb.Text))
                {
                    // Disable.  
                    CloseCultureAutoSuggestionBox();

                    // Info.  
                    return;
                }

                // Enable.  
                OpenCultureAutoSuggestionBox();

                // Settings.  
                CultureList.ItemsSource = AutoCompletationCultureList.Where(p => p.ToLower().Contains(cultureTb.Text.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void CultureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Verification.  
                if (CultureList.SelectedIndex <= -1)
                {
                    // Disable.  
                    CloseCultureAutoSuggestionBox();

                    // Info.  
                    return;
                }

                // Disable.  
                CloseCultureAutoSuggestionBox();

                // Settings.  
                cultureTb.Text = CultureList.SelectedItem.ToString();
                CultureList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }

        }
        #endregion

        #endregion

        private void seeCreatedProvs_Click(object sender, RoutedEventArgs e)
        {
            w.Show();
            w.ShowProvinceNames(provinceNames);
        }

        public bool IsClosed { get; private set; }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            w.Close();
            IsClosed = true;
        }

        private void NightModeBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
