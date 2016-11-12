using GAF;
using GAF.Operators;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cecs545FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double crossoverProbability = 0.85;
        double mutationProbability = 0.08;
        int elitismPercentage = 5;
        int initialPopulationSize = 50;
        int maxGenerations = 1000;
        ClickOMania.Board gameBoard;

        Queue<double> lastFiveGens;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fitness Function
        /// </summary>
        /// <returns>Between 0 and 1 with 1 being being most fit</returns>
        private double CalculateFitness(Chromosome chromosome)
        {
            double fitnessValue = -1;
            if (chromosome != null)
            {
                //this is a range constant that is used to keep the x/y range between -100 and +100
                var rangeConst = 200 / (System.Math.Pow(2, chromosome.Count / 2) - 1);

                //get x and y from the solution
                var x1 = Convert.ToInt32(chromosome.ToBinaryString(0, chromosome.Count / 2), 2);
                var y1 = Convert.ToInt32(chromosome.ToBinaryString(chromosome.Count / 2, chromosome.Count / 2), 2);

                //Adjust range to -100 to +100
                var x = (x1 * rangeConst) - 100;
                var y = (y1 * rangeConst) - 100;

                //using binary F6 for fitness.
                var temp1 = System.Math.Sin(System.Math.Sqrt(x * x + y * y));
                var temp2 = 1 + 0.001 * (x * x + y * y);
                var result = 0.5 + (temp1 * temp1 - 0.5) / (temp2 * temp2);

                fitnessValue = 1 - result;
            }
            else
            {
                //chromosome is null
                throw new ArgumentNullException("chromosome", "The specified Chromosome is null.");
            }

            return fitnessValue;
        }

        /// <summary>
        /// End condition checker
        /// </summary>
        /// <returns>True to stop genetic algorithm</returns>
        private bool TerminateFunction(Population population,
            int currentGeneration,
            long currentEvaluation)
        {
            // If we passed the max number of generations, terminate
            if (currentGeneration > maxGenerations)
                return true;

            // If we found the max possible fitness, terminate
            if (population.MaximumFitness == 1)
                return true;

            // If we got the same fitness for the last five generations, terminate
            if (lastFiveGens.Count >= 5) // Only run if we have five generations worth of data to check
            {
                double value = lastFiveGens.ElementAt(0); // Get the value of the oldest run
                bool flag = false; // This flag goes true if we have different values and need to continue
                for (int i = 1; i < lastFiveGens.Count; i++) // Iterate through the queue
                {
                    if (System.Math.Abs(lastFiveGens.ElementAt(i) - value) > 0.000001) // If the values are sufficeintly different
                    {
                        flag = true; // Flag that we need to keep running
                        break; // No point continuing, break
                    }
                }
                return !flag; // If flag is true we need to keep going, if we got here and flag is false we need to terminate
            }

            // We aren't past maxGenerations, we haven't solved the problem, and we don't have enough data to detect a plateu
            // Keep going
            return false;
        }

        private void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            //get the best solution 
            var chromosome = e.Population.GetTop(1)[0];

            //decode chromosome

            //get x and y from the solution 
            var x1 = Convert.ToInt32(chromosome.ToBinaryString(0, chromosome.Count / 2), 2);
            var y1 = Convert.ToInt32(chromosome.ToBinaryString(chromosome.Count / 2, chromosome.Count / 2), 2);

            //Adjust range to -100 to +100 
            var rangeConst = 200 / (System.Math.Pow(2, chromosome.Count / 2) - 1);
            var x = (x1 * rangeConst) - 100;
            var y = (y1 * rangeConst) - 100;

            //display the X, Y and fitness of the best chromosome in this generation 
            Console.WriteLine("x:{0} y:{1} Fitness{2}", x, y, e.Population.MaximumFitness);

            // Maintain lastFiveGens queue
            // Put this generations fitness onto the queue
            lastFiveGens.Enqueue(e.Population.MaximumFitness);
            // Pull 6th last generation from queue, if it exists
            if (lastFiveGens.Count > 5)
            {
                lastFiveGens.Dequeue();
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            gameBoard = ClickOMania.Board.GenerateRandomBoard();

            crossoverProbability = crossoverProbabilitySlider.Value/100;
            mutationProbability = mutationProbabilitySlider.Value/100;
            elitismPercentage = Convert.ToInt32(elitismPercentageSlider.Value);
            initialPopulationSize = Convert.ToInt32(populationSizeSlider.Value);
            maxGenerations = Convert.ToInt32(maxGenerationsSlider.Value);

            lastFiveGens = new Queue<double>();

            var population = new Population(initialPopulationSize, 44);

            //create the genetic operators 
            var elite = new Elite(elitismPercentage);

            var crossover = new Crossover(crossoverProbability, true)
            {
                CrossoverType = CrossoverType.SinglePoint,
                ReplacementMethod = ReplacementMethod.DeleteLast
            };

            var mutation = new BinaryMutate(mutationProbability, true);

            var ga = new GeneticAlgorithm(population, new FitnessFunction(CalculateFitness));

            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            ga.OnGenerationComplete += ga_OnGenerationComplete;

            ga.Run(new TerminateFunction(TerminateFunction));
        }
    }
}
