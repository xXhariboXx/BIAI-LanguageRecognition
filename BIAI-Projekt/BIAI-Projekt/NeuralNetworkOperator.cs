using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIAI_Projekt
{
    class NeuralNetworkOperator
    {
        public void run(List<double[]> inputVector)
        {
            Console.WriteLine("\nBegin neural network back-propagation demo");

            int numInput = 27; // number features
            int numHidden = 7;
            int numOutput = 2; // number of classes for Y
            int numRows = inputVector.Count;
            int seed = 1; // gives nice demo

            Console.WriteLine("\nGenerating " + numRows +
              " artificial data items with " + numInput + " features");
            double[][] allData = MakeAllData(numInput, numHidden, numOutput,
              numRows, seed, inputVector);
            Console.WriteLine("Done");

            //ShowMatrix(allData, allData.Length, 2, true);

            Console.WriteLine("\nCreating train (80%) and test (20%) matrices");
            double[][] trainData;
            double[][] testData;
            SplitTrainTest(allData, 0.70, seed, out trainData, out testData);
            Console.WriteLine("Done\n");

            Console.WriteLine("Training data:");
            ShowMatrix(trainData, 4, 2, true);
            Console.WriteLine("Test data:");
            ShowMatrix(testData, 4, 2, true);

            Console.WriteLine("Creating a " + numInput + "-" + numHidden +
              "-" + numOutput + " neural network");
            NeuralNetwork nn = new NeuralNetwork(numInput, numHidden, numOutput);

            int maxEpochs = 2000;
            double learnRate = 0.05;
            double momentum = 0.01;
            Console.WriteLine("\nSetting maxEpochs = " + maxEpochs);
            Console.WriteLine("Setting learnRate = " + learnRate.ToString("F2"));
            Console.WriteLine("Setting momentum  = " + momentum.ToString("F2"));

            Console.WriteLine("\nStarting training");
            double[] weights = nn.Train(trainData, maxEpochs, learnRate, momentum);
            Console.WriteLine("Done");
            Console.WriteLine("\nFinal neural network model weights and biases:\n");
            ShowVector(weights, 2, 10, true);

            //double[] y = nn.ComputeOutputs(new double[] { 1.0, 2.0, 3.0, 4.0 });
            //ShowVector(y, 3, 3, true);

            double trainAcc = nn.Accuracy(trainData);
            Console.WriteLine("\nFinal accuracy on training data = " +
              trainAcc.ToString("F4"));

            double testAcc = nn.Accuracy(testData);
            Console.WriteLine("Final accuracy on test data     = " +
              testAcc.ToString("F4"));

            Console.WriteLine("\nEnd back-propagation demo\n");
            Console.ReadLine();
        }

        public static void ShowMatrix(double[][] matrix, int numRows,
     int decimals, bool indices)
        {
            int len = matrix.Length.ToString().Length;
            for (int i = 0; i < numRows; ++i)
            {
                if (indices == true)
                    Console.Write("[" + i.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    double v = matrix[i][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
                Console.WriteLine("");
            }

            if (numRows < matrix.Length)
            {
                Console.WriteLine(". . .");
                int lastRow = matrix.Length - 1;
                if (indices == true)
                    Console.Write("[" + lastRow.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[lastRow].Length; ++j)
                {
                    double v = matrix[lastRow][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
            }
            Console.WriteLine("\n");
        }

        public static void ShowVector(double[] vector, int decimals,
          int lineLen, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % lineLen == 0) Console.WriteLine("");
                if (vector[i] >= 0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " ");
            }
            if (newLine == true)
                Console.WriteLine("");
        }

        static double[][] MakeAllData(int numInput, int numHidden,
          int numOutput, int numRows, int seed, List<double[]> inputVector)
        {
            Random rnd = new Random(seed);
            //counting weights
            int numWeights = (numInput * numHidden) + numHidden +
              (numHidden * numOutput) + numOutput;
            double[] weights = new double[numWeights]; // actually weights & biases
            for (int i = 0; i < numWeights; ++i)
                weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

            Console.WriteLine("Generating weights and biases:");
            ShowVector(weights, 2, 10, true);
            //end weights

            //creating neural network
            NeuralNetwork gnn = new NeuralNetwork(numInput, numHidden, numOutput); // generating NN
            gnn.SetWeights(weights);
            //end creating neural network


            //alocating memory
            double[][] result = new double[numRows][]; // allocate return-result
            for (int i = 0; i < numRows; ++i)
                //alocating space for input+output
                result[i] = new double[numInput + numOutput]; // 1-of-N in last column
            //end of allocating

            for(int i = 0; i < inputVector.Count; i++)
            {
                for(int j = 0; j < (numInput + numOutput); j++)
                {
                    result[i][j] = inputVector.ElementAt(i)[j];
                }
            }

            //for (int r = 0; r < numRows; ++r) // for each row
            //{
            //    // generate random inputs
            //    double[] inputs = new double[numInput];
            //    for (int i = 0; i < numInput; ++i)
            //        inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]

            //    // compute outputs
            //    double[] outputs = gnn.ComputeOutputs(inputs);

            //    // translate outputs to 1-of-N
            //    double[] oneOfN = new double[numOutput]; // all 0.0

            //    int maxIndex = 0;
            //    double maxValue = outputs[0];
            //    for (int i = 0; i < numOutput; ++i)
            //    {
            //        if (outputs[i] > maxValue)
            //        {
            //            maxIndex = i;
            //            maxValue = outputs[i];
            //        }
            //    }
            //    oneOfN[maxIndex] = 1.0;

            //    // place inputs and 1-of-N output values into curr row
            //    int c = 0; // column into result[][]
            //    for (int i = 0; i < numInput; ++i) // inputs
            //        result[r][c++] = inputs[i];
            //    for (int i = 0; i < numOutput; ++i) // outputs
            //        result[r][c++] = oneOfN[i];
            //} // each row
            return result;
        } // MakeAllData

        static void SplitTrainTest(double[][] allData, double trainPct,
          int seed, out double[][] trainData, out double[][] testData)
        {
            Random rnd = new Random(seed);
            int totRows = allData.Length;
            int numTrainRows = (int)(totRows * trainPct); // usually 0.80
            int numTestRows = totRows - numTrainRows;
            trainData = new double[numTrainRows][];
            testData = new double[numTestRows][];

            double[][] copy = new double[allData.Length][]; // ref copy of data
            for (int i = 0; i < copy.Length; ++i)
                copy[i] = allData[i];

            for (int i = 0; i < copy.Length; ++i) // scramble order
            {
                int r = rnd.Next(i, copy.Length); // use Fisher-Yates
                double[] tmp = copy[r];
                copy[r] = copy[i];
                copy[i] = tmp;
            }
            for (int i = 0; i < numTrainRows; ++i)
                trainData[i] = copy[i];

            for (int i = 0; i < numTestRows; ++i)
                testData[i] = copy[i + numTrainRows];
        } // 

    }
}
