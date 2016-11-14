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

namespace cecs545FinalProject
{
    /// <summary>
    /// Interaction logic for ResultNavigator.xaml
    /// </summary>
    public partial class ResultNavigator : Window
    {
        private Log log;

        public ResultNavigator(Log logIn)
        {
            InitializeComponent();

            log = logIn;

            List<string> items = new List<string>();
            for(int i = 0; i < log.Length; i++)
            {
                items.Add(log.ReadShort(i));
            }
            genListBox.ItemsSource = items;

            /*genListBox.BeginUpdate();
            genListBox.Items.Clear();
            foreach (string item in log.readResultDataItems())
            {
                genListBox.Items.Add(item);
            }
            selectedStepIndex = genListBox.Items.Count - 1;
            genListBox.SetSelected(selectedStepIndex, true);
            genListBox.EndUpdate();*/
        }

        private void genListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (genListBox.SelectedItem != null)
            {
                int genIndex = genListBox.SelectedIndex;
                Log.GenerationData selectedGen = log.ReadFull(genIndex);
                generationValueLabel.Content = selectedGen.GenNum;
                wocFitnessValueLabel.Content = String.Format("{0:0.000}", selectedGen.WocFitness);
                maxFitnessValueLabel.Content = String.Format("{0:0.000}", selectedGen.MaxFitness);
                minFitnessValueLabel.Content = String.Format("{0:0.000}", selectedGen.MinFitness);
            }
        }
    }
}
