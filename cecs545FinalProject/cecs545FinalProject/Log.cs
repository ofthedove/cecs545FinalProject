using GAF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cecs545FinalProject
{
    public class Log
    {
        private ClickOMania.Board board;
        private List<GenerationData> logData;

        public ClickOMania.Board OriginalBoard { get { return board;  } }
        
        public int Length { get { return logData.Count;  } }

        public class GenerationData
        {
            private double genNum;
            private double avgFitness;
            private double stdDevFit;

            private Chromosome mostFit;
            private Chromosome leastFit;
            private Chromosome WocSln;

            public double GenNum { get { return genNum; } }
            public double MaxFitness { get { return mostFit.Fitness; } }
            public double MinFitness { get { return leastFit.Fitness; } }
            public double AvgFitness { get { return avgFitness; } }
            public double StdDevFit { get { return stdDevFit; } }
            public double WocFitness { get { return WocSln.Fitness; } }

            public int[] MostFitSolution { get { return ChromToSolution(mostFit); } }
            public int[] LeastFitSolution { get { return ChromToSolution(leastFit); } }
            public int[] WocSlnSolution { get { return ChromToSolution(WocSln); } }

            public string Blurb
            {
                get
                {
                    return string.Format("Gen {0,3:###} : WoC {1,5:0.000} | Max {2,5:0.000} | Avg {3,5:0.000} | SDv {4,5:0.000}", genNum, WocFitness, MaxFitness, avgFitness, stdDevFit);
                }
            }

            private GenerationData(int genNumIn, double avgFitIn, double stdDevFitIn, Chromosome mostFitIn, Chromosome leastFitIn, Chromosome WocSlnIn)
            {
                genNum = genNumIn;
                avgFitness = avgFitIn;
                stdDevFit = stdDevFitIn;

                mostFit = mostFitIn;
                leastFit = leastFitIn;
                WocSln = WocSlnIn;
            }

            public static GenerationData GenDataFromPopulation(int genNumIn, Population pop, Chromosome WocSlnIn)
            {
                double avgFitness = pop.AverageFitness;

                double stdDevFit = 0;
                double runningSum = 0;
                foreach(Chromosome chrom in pop.Solutions)
                {
                    runningSum += System.Math.Pow(chrom.Fitness - avgFitness, 2);
                }
                stdDevFit = runningSum / (double)pop.PopulationSize;
                stdDevFit = System.Math.Sqrt(stdDevFit);

                Chromosome mostFit = pop.GetTop(1)[0];
                Chromosome leastFit = pop.GetBottom(1)[0];

                return new GenerationData(genNumIn, avgFitness, stdDevFit, mostFit, leastFit, WocSlnIn);
            }

            public static int[] ChromToSolution(Chromosome chrom)
            {
                int[] arr = new int[25];
                int i = 0;

                foreach (Gene gene in chrom)
                {
                    arr[i] = (int)gene.ObjectValue;
                    i++;
                }

                return arr;
            }
        }

        public Log(ClickOMania.Board brd)
        {
            board = brd;
            logData = new List<GenerationData>();
        }

        public void Write(GenerationData item)
        {
            logData.Add(item);
        }

        public string ReadShort(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentException("Specified index does not exist in log");
            }

            return logData[index].Blurb;
        }

        public GenerationData ReadFull(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentException("Specified index does not exist in log");
            }

            return logData[index];
        }

        public void SaveBrief(string filePath)
        {
            StreamWriter writer = new StreamWriter(filePath);
            for(int i = 0; i < this.Length; i++)
            {
                writer.WriteLine(this.ReadShort(i));
            }
            writer.Close();
        }
    }
}
