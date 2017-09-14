using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BIAI_Projekt
{
    public partial class MainWindow : Window
    {
        FileReader fileReader;
        NeuralNetworkOperator neuralNetworkOperator;
        List<Language> languageList;
        double roundingFactor = 0.7;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            fileReader = new FileReader();
            fileReader.ReadLanguagesConfigFile();
            languageList = fileReader.LanguageList;
            DisplayLanguages();
            neuralNetworkOperator = new NeuralNetworkOperator(roundingFactor);
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            fileReader.MainList.Clear();
            fileReader.CreateListOfArrays(fileReader.TrainDataFolderPath);
            ResultTextBox.Text = fileReader.PrintListOfArrays(fileReader.MainList);
            neuralNetworkOperator.Train(fileReader.MainList);
            SaveNetworkButton.IsEnabled = true;
            TestButton.IsEnabled = true;
            OpenFileButton.IsEnabled = true;
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            fileReader.SaveWeights(neuralNetworkOperator.Weights);
        }

        private void DisplayLanguages()
        {
            string languages = "";
            foreach (Language language in languageList)
            {
                languages += (language.LanguageName + " " + ArrayToString(language.BitCode) + "\n");
            }
            LanguagesTextBox.Text = languages;
        }

        private string ArrayToString(int[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }
            return result;
        }

        private string ArrayToString(double[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }
            return result;
        }

        private void LoadNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            neuralNetworkOperator.SetWeights(fileReader.ReadWeights());
            TestButton.IsEnabled = true;
            OpenFileButton.IsEnabled = true;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            fileReader.MainList.Clear();
            fileReader.CreateListOfArrays(fileReader.TestDataFolderPath);
            ResultTextBox.Clear();
            ResultTextBox.Text = neuralNetworkOperator.Test(fileReader.MainList);
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            double[] results = null;
            if (openFileDialog.ShowDialog() == true)
            {
                double[] percentageArray = fileReader.ReadFile(openFileDialog.FileName);
                results = neuralNetworkOperator.RecognizeLanguage(percentageArray);

                if (results.Length != 0)
                {
                    MessageBox.Show(GetLanguageName(results));
                }
            }
        }

        private string GetLanguageName(double[] outputVector)
        {
            int[] intResult = new int[outputVector.Length];
            string languageResult = "";
            for (int i = 0; i < outputVector.Length; i++)
            {
                if (outputVector[i] >= roundingFactor)
                {
                    intResult[i] = 1;
                }
            }
            foreach (Language language in languageList)
            {
                if (language.BitCode.SequenceEqual(intResult))
                {
                    languageResult = language.LanguageName;
                    break;
                }
            }

            if (languageResult.Equals(""))
            {
                languageResult = "Language not reconized";
            }

            languageResult += "\nResult: ";
            foreach (double resultBit in outputVector)
            {
                languageResult += "\n" + resultBit.ToString("N3");
            }

            return languageResult;
        }
    }
}
