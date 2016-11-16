using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoardBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button[,] boardGrid;

        public MainWindow()
        {
            InitializeComponent();

            PlaceButtons();
        }

        private void PlaceButtons()
        {
            boardGrid = new Button[5, 10];

            for(int i = 0; i < EditGrid.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < EditGrid.RowDefinitions.Count; j++)
                {
                    Button btn = new Button();
                    btn.Name = String.Format("EditImage_{0}_{1}", i, j);
                    btn.Click += EditButton_Click;
                    EditGrid.Children.Add(btn);
                    Grid.SetColumn(btn, i);
                    Grid.SetRow(btn, j);
                    boardGrid[i, j] = btn;
                }
            }
        }

        private void LoadFromFile(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    for (int i = 9, i2 = 0; i >= 0; i--, i2++)
                    {
                        if (sr.EndOfStream) { throw new ApplicationException("Invalid File! Too few lines"); }
                        string[] line = sr.ReadLine().Split(',');
                        for (int j = 0; j < 5; j++)
                        {
                            try
                            {
                                int color = Convert.ToInt32(line[j]);
                                switch(color)
                                {
                                    case 1:
                                        boardGrid[j, i2].Background = Brushes.Red;
                                        break;
                                    case 2:
                                        boardGrid[j, i2].Background = Brushes.Green;
                                        break;
                                    case 3:
                                        boardGrid[j, i2].Background = Brushes.Purple;
                                        break;
                                    default:
                                        throw new ApplicationException(String.Format("Invalid value at line {0} position {1}", i + 1, j + 1));
                                }

                            }
                            catch (IndexOutOfRangeException)
                            {
                                throw new ApplicationException(String.Format("Too few columns on line {0}", i + 1));
                            }
                        }
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new ApplicationException("No path specified");
            }
            catch (IOException e)
            {
                throw new ApplicationException("Input Path Not Valid");
            }
        }

        private void SaveToFile(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    for (int i = 9, i2 = 0; i >= 0; i--, i2++)
                    {
                        string line = "";
                        for (int j = 0; j < 5; j++)
                        {
                            Brush br = boardGrid[j, i2].Background;
                            if (br == Brushes.Red)
                            {
                                line += "1,";
                            }
                            else if (br == Brushes.Green)
                            {
                                line += "2,";
                            }
                            else if (br == Brushes.Purple)
                            {
                                line += "3,";
                            }
                            else
                            {
                                throw new ApplicationException("All squares must be colored!");
                            }
                        }
                        line = line.Substring(0, line.Length - 1);
                        sw.WriteLine(line);
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new ApplicationException("No path specified");
            }
            catch (IOException e)
            {
                throw new ApplicationException("Input Path Not Valid");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (redRadioButton.IsChecked == true)
            {
                btn.Background = Brushes.Red;
            }
            else if(greenRadioButton.IsChecked == true)
            {
                btn.Background = Brushes.Green;
            }
            else if (purpleRadioButton.IsChecked == true)
            {
                btn.Background = Brushes.Purple;
            }
        }

        private void loadPathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                loadPathTextBox.Text = openFileDialog.FileName;
        }

        private void savePathBrowseButton_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                savePathTextBox.Text = saveFileDialog.FileName;
        }

        private void loadPathSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var message = "Loading a file will delete unsaved work." + Environment.NewLine + "Do you want to continue?";
            var title = "Warning";
            var dialog = new DialogOkCancel(message, title);
            dialog.ShowDialog();

            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                try
                {
                    LoadFromFile(loadPathTextBox.Text);
                }
                catch (ApplicationException ex)
                {
                    MessageBox.Show(ex.Message, "Error Loading File");
                }
            }
        }

        private void savePathSaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveToFile(savePathTextBox.Text);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error Saving File");
            }
        }
    }
}
