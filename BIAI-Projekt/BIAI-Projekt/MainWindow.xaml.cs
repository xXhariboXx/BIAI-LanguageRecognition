using System.Windows;

namespace BIAI_Projekt
{
    public partial class MainWindow : Window
    {
        FileReader fileReader;
        NeuralNetworkOperator neuralNetworkOperator;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            fileReader = new FileReader();
            fileReader.ReadLanguageFile();
            DisplayLanguages();
            neuralNetworkOperator = new NeuralNetworkOperator();
        }

        private async void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            fileReader.MainList.Clear();
            fileReader.CreateListOfArrays(fileReader.TrainDataFolderPath);
            ResultTextBox.Text = fileReader.PrintListOfArrays(fileReader.MainList);
            neuralNetworkOperator.Train(fileReader.MainList);
            SaveNetworkButton.IsEnabled = true;
            TestButton.IsEnabled = true;
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            fileReader.SaveWeights(neuralNetworkOperator.Weights);
        }

        private void DisplayLanguages()
        {
            string languages = "";
            foreach(Language language in fileReader.LanguageList)
            {
                languages += (language.LanguageName + " " + ArrayToString(language.BitCode) + "\n");
            }
            LanguagesTextBox.Text = languages;
        }

        private string ArrayToString(int[] array)
        {
            string result = "";
            for(int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }
            return result;
        }

        private void LoadNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            neuralNetworkOperator.SetWeights(fileReader.ReadWeights());
            TestButton.IsEnabled = true;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            fileReader.MainList.Clear();
            fileReader.CreateListOfArrays(fileReader.TestDataFolderPath);
            ResultTextBox.Clear();
            ResultTextBox.Text = neuralNetworkOperator.Test(fileReader.MainList);
        }
    }
}
