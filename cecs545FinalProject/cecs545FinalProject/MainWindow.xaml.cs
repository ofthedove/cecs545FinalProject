using GAF;
using GAF.Operators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        // board size is 5*10
        const int numIntsInSolution = 25;
        const int solutionValueMin = 0; // Probably shouldn't change this. It would screw with array stuff in WoC
        const int solutionValueMax = 25;

        Random rand = new Random();
        BackgroundWorker b;

        double crossoverProbability = 0.85;
        double mutationProbability = 0.08;
        int elitismPercentage = 5;
        int initialPopulationSize = 50;
        int maxGenerations = 1000;
        ClickOMania.Board gameBoard;

        Queue<double> lastFiveGens;
        Log log;

        public MainWindow()
        {
            InitializeComponent();
        }

        private Population GenerateInitialPopulation(int popSize)
        {
            var population = new Population();

            for (int i = 0; i < popSize; i++)
            {
                var chromosome = new Chromosome();
                for(int j = 0; j < numIntsInSolution; j++)
                {
                    chromosome.Add(new Gene(rand.Next(solutionValueMin, solutionValueMax)));
                }

                population.Solutions.Add(chromosome);
            }

            return population;
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
                int[] arr = new int[25];
                int i = 0;

                foreach(Gene gene in chromosome)
                {
                    arr[i] = (int)gene.ObjectValue;
                    i++;
                }

                int fitness = ClickOMania.Game(arr, gameBoard.GetBoardAsArray());

                double adjustedFitness = (50 - fitness); // fitness is how many squares are empty, not how many are left
                adjustedFitness = adjustedFitness * (1F / 50F); // fitness needs to be between 0 and one, so multiply by min/max

                fitnessValue = adjustedFitness;
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
            if (currentGeneration >= maxGenerations)
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
                    if (System.Math.Abs((double)lastFiveGens.ElementAt(i) - (double)value) > 0.000001) // If the values are sufficeintly different
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
            // Basic log debugging stuff. Not really needed
            Console.WriteLine("Generation {0} | Max Fitness {1}", e.Generation, e.Population.MaximumFitness);
            
            // Do WoC stuff
            Chromosome wocChrom = CalculateWoC(e.Population);

            // Add this generation's data to our log
            log.Write(Log.GenerationData.GenDataFromPopulation(e.Generation, e.Population, wocChrom));

            // Report the current state of execution back to the main UI thread
            GenerationState gs = new GenerationState() { genNum = e.Generation, maxFit = e.Population.MaximumFitness };
            b.ReportProgress(-1, gs);

            // Maintain lastFiveGens queue
            // Put this generations fitness onto the queue
            lastFiveGens.Enqueue(e.Population.MaximumFitness);
            // Pull 6th last generation from queue, if it exists
            if (lastFiveGens.Count > 5)
            {
                lastFiveGens.Dequeue();
            }
        }

        /// <summary>
        /// Calculate a WoC solution from a population
        /// Each gene value is the most popular value for a gene in that position across the population
        /// </summary>
        /// <param name="pop">The input population. Must contain individuals</param>
        /// <returns>A chromosome representing the WoC solution</returns>
        private Chromosome CalculateWoC(Population pop)
        {
            // Create a hash table. Columns are gene positions and rows are possible gene values
            // The values are the number of times that gene value occured in that position in the genome
            byte[,] hash = new byte[numIntsInSolution, solutionValueMax + 1];

            int i = 0, j = 0;
            // Fill the hash table
            foreach(Chromosome chrom in pop.Solutions)
            { // Iterate through all individuals
                i = 0; // i tracks the current gene position in this individual
                foreach(Gene gene in chrom)
                { // Iterate through this individuals genes
                    hash[i, (int)gene.ObjectValue]++; // Increment the correct location in the hash table
                    i++; // Increment our iterator
                }
            }

            // Create an empty Chromosome to fill and return
            Chromosome retChrom = new Chromosome();

            // Iterate over hash table again calculating values for return chromosome
            for (i = 0; i < numIntsInSolution; i++)
            { // Iterate through columns, which are gene positions that will be colapsed into single gene values based on which value is most populat in that position
                int mostPop = -1;
                int timesRepeated = -1;
                for (j = 0; j < solutionValueMax; j++)
                { // Iterate through the rows in this column, finding the most popular value
                    // Make current value the most popular value if it's been here more times. If they're tied choose randomly
                    if(hash[i, j] > timesRepeated || (hash[i, j] == timesRepeated && rand.Next(0, 1) == 1))
                    {
                        timesRepeated = hash[i, j];
                        mostPop = j;
                    }
                }

                // If we don't have any most popular value, the column was empty, something went very wrong, throw an exception.
                // (This shouldn't be able to happen, but it's still good to double check, as it might take some time to find this otherwise)
                if (mostPop == -1) { throw new ApplicationException("Something went very wrong in WoC"); }

                // Add this most popular gene value to the return chromosome
                var newGene = new Gene(mostPop);
                retChrom.Add(newGene);
            }

            // Let the WoC chromosome we built calculate it's fitness
            retChrom.Evaluate(CalculateFitness);

            // Return the WoC chromosome we built
            return retChrom;
        }

        private void ga_OnRunComplete(object sender, GaEventArgs e)
        {


        }

        private void bw_ProgressChanged(object o, ProgressChangedEventArgs args)
        {
            var gs = args.UserState as GenerationState;
            generationValueLabel.Content = gs.genNum;
            fitnessValueLabel.Content = String.Format("{0,5:0.000}", gs.maxFit);
        }

        private void bw_RunWorkerComplete(object o, RunWorkerCompletedEventArgs args)
        {
            statusLabel.Content = "Done...";
            startButton.IsEnabled = true;

            log.SaveBrief(@"C:\Users\Andrew\Desktop\output.txt");

            ShowResultNavigator();
        }

        private void ShowResultNavigator()
        {
            /*Thread t = new Thread(
                delegate ()
                {
                    ResultNavigator rn = new ResultNavigator(log);
                    rn.ShowDialog();
                });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();*/

            ResultNavigator rn = new ResultNavigator(log);
            rn.Show();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            statusLabel.Content = "Running...";

            gameBoard = ClickOMania.Board.GenerateRandomBoard(rand);
            log = new Log(gameBoard);

            crossoverProbability = crossoverProbabilitySlider.Value/100;
            mutationProbability = mutationProbabilitySlider.Value/100;
            elitismPercentage = Convert.ToInt32(elitismPercentageSlider.Value);
            initialPopulationSize = Convert.ToInt32(populationSizeSlider.Value);
            maxGenerations = Convert.ToInt32(maxGenerationsSlider.Value);

            lastFiveGens = new Queue<double>();

            var population = GenerateInitialPopulation(initialPopulationSize);

            //create the genetic operators 
            var elite = new Elite(elitismPercentage);

            var crossover = new Crossover(crossoverProbability, true)
            {
                AllowDuplicates = true,
                CrossoverType = CrossoverType.SinglePoint,
                ReplacementMethod = ReplacementMethod.GenerationalReplacement
            };

            var mutation = new SwapMutate(mutationProbability);

            var ga = new GeneticAlgorithm(population, CalculateFitness);

            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;



            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(
                delegate (object o, DoWorkEventArgs args)
                {
                    b = o as BackgroundWorker;
                    ga.Run(TerminateFunction);
                });
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerComplete);
            bw.RunWorkerAsync();
        }

        private class GenerationState
        {
            public int genNum;
            public double maxFit;
        }
    }
}
