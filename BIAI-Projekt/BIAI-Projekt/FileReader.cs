using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace BIAI_Projekt
{
    //Struckt to contain language data - language name and language bitcode
    struct Language
    {
        public String LanguageName;
        public int[] BitCode;

        public Language(String languageName, int[] bitCode)
        {
            this.LanguageName = languageName;
            this.BitCode = bitCode;
        }
    }

    //Class to read files and process data from files
    class FileReader
    {
        //files/folders names
        const String ConfigFileName = "ConfigFile.txt";
        const String WeightsFileName = "Weights.txt";
        const String TrainDataFolderName = "traindata";
        const String TestDataFolderName = "testdata";

        //paths
        String ConfigFilePath;
        String WeightsFilePath;
        public String TrainDataFolderPath { get; }
        public String TestDataFolderPath { get; }

        //variables
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        public List<double[]> MainList { get; }
        public List<Language> LanguageList { get; }


        public FileReader()
        {
            MainList = new List<double[]>();
            LanguageList = new List<Language>();
            var dir = Directory.GetCurrentDirectory();
            ConfigFilePath += dir + "\\data\\" + ConfigFileName;
            WeightsFilePath += dir + "\\data\\" + WeightsFileName;
            TrainDataFolderPath += dir + "\\data\\" + TrainDataFolderName;
            TestDataFolderPath += dir + "\\data\\" + TestDataFolderName;
        }

        //Reads config file and creates data about supported languages
        public void ReadLanguagesConfigFile()
        {
            using (streamReader = new StreamReader(ConfigFilePath))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.ToUpper();
                    String languageName = line.Substring(0, 3);
                    int[] languageBits = new int[5];
                    languageBits[0] = (int)Char.GetNumericValue(line.ElementAt(4));
                    languageBits[1] = (int)Char.GetNumericValue(line.ElementAt(5));
                    languageBits[2] = (int)Char.GetNumericValue(line.ElementAt(6));
                    languageBits[3] = (int)Char.GetNumericValue(line.ElementAt(7));
                    languageBits[4] = (int)Char.GetNumericValue(line.ElementAt(8));
                    LanguageList.Add(new Language(languageName, languageBits));
                }
            }
        }

        //Creates language statistics of all currently supporeted languages
        public void CreateListOfArrays(String path)
        {
            try
            {
                var languageFiles = Directory.EnumerateDirectories(path);
                foreach (String currentDir in languageFiles)
                {
                    if (CheckLanguage(currentDir))
                    {
                        var txtFiles = Directory.EnumerateFiles(currentDir, "*.txt");
                        int sumOfSources = 0;
                        double[] languageAveragePercentageArray = new double[27];
                        foreach (String currentFile in txtFiles)
                        {
                            sumOfSources++;
                            double[] percentageArray = new double[32];
                            for (int i = 0; i < percentageArray.Length; i++)
                            {
                                percentageArray[i] = 0;
                            }
                            String language = currentDir.Remove(0, currentDir.Length - 3);
                            ConvertLanguageToTable(language, percentageArray);

                            using (streamReader = new StreamReader(currentFile))
                            {
                                String line;
                                int charAmountInFile = 0;
                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    line = line.ToLower();
                                    foreach (char c in line)
                                    {

                                        if ((c >= 97) && (c <= 122))
                                        {
                                            int i = c - 97;
                                            percentageArray[i]++;
                                            charAmountInFile++;
                                        }
                                        else if (c > 127)
                                        {
                                            percentageArray[26]++;
                                            charAmountInFile++;
                                        }
                                    }
                                }
                                for (int i = 0; i < percentageArray.Length - 5; i++)
                                {
                                    percentageArray[i] = ((percentageArray[i]) / charAmountInFile) * 100;
                                }
                            }
                            MainList.Add(percentageArray);
                            for (int i = 0; i < 27; i++)
                            {
                                languageAveragePercentageArray[i] += percentageArray[i];
                            }
                        }
                        for (int i = 0; i < 27; i++)
                        {
                            languageAveragePercentageArray[i] /= sumOfSources;
                        }
                        SaveAveragePercentageArray(languageAveragePercentageArray, currentDir);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("blad");
            }
        }

        //Converts list all language statistics data to farmat char - percentage 
        public String PrintListOfArrays(List<double[]> list)
        {
            String listToPrint = "";

            for (int i = 0; i < list.Count; i++)
            {
                listToPrint += (PrintPercentageArray(list.ElementAt(i)) + "/////////////////\n");
            }
            return listToPrint;
        }

        //Saves weights from current neural network to file
        public void SaveWeights(double[] weights)
        {
            using (streamWriter = new StreamWriter(WeightsFilePath))
            {
                String weightsString = "";
                for (int i = 0; i < weights.Length; i++)
                {
                    streamWriter.WriteLine(weights[i]);
                }
            }
        }

        //Reads weights from weights file
        public double[] ReadWeights()
        {
            List<double> weightsList = new List<double>();
            using (streamReader = new StreamReader(WeightsFilePath))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    weightsList.Add(Double.Parse(line));
                }
            }
            double[] result = new double[weightsList.Count];
            for (int i = 0; i < weightsList.Count; i++)
            {
                result[i] = weightsList.ElementAt(i);
            }
            return result;

        }

        //Creates language statistics from one choosen file
        public double[] ReadFile(String file)
        {
            double[] percentageArray = new double[27];
            for (int i = 0; i < percentageArray.Length; i++)
            {
                percentageArray[i] = 0;
            }

            using (streamReader = new StreamReader(file))
            {
                String line;
                int charAmountInFile = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.ToLower();
                    foreach (char c in line)
                    {

                        if ((c >= 97) && (c <= 122))
                        {
                            int i = c - 97;
                            percentageArray[i]++;
                            charAmountInFile++;
                        }
                        else if (c > 127)
                        {
                            percentageArray[26]++;
                            charAmountInFile++;
                        }
                    }
                }
                for (int i = 0; i < percentageArray.Length; i++)
                {
                    percentageArray[i] = ((percentageArray[i]) / charAmountInFile) * 100;
                }
            }

            return percentageArray;
        }

        //Saves language statistics to .csv file
        private void SaveAveragePercentageArray(double[] percentageArray, String path)
        {
            using (streamWriter = new StreamWriter(path + "LanguageAverage.csv"))
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                String result = "";
                for (int i = 0; i < percentageArray.Length; i++)
                {
                    result += percentageArray[i].ToString(nfi) + ";";
                }
                streamWriter.WriteLine(result);
            }
        }

        //Converts one vector to print to format
        private String PrintPercentageArray(double[] array)
        {
            String arrayString = "";

            for (int i = 0; i < array.Length; i++)
            {
                if (i == array.Length - 6)
                {
                    arrayString += ("inne - " + array[i] + "\n");
                }
                else if (i == array.Length - 5)
                {
                    arrayString += ("jezyk [0]- " + array[i] + "\n");
                }
                else if (i == array.Length - 4)
                {
                    arrayString += ("jezyk [1]- " + array[i] + "\n");
                }
                else if (i == array.Length - 3)
                {
                    arrayString += ("jezyk [2]- " + array[i] + "\n");
                }
                else if (i == array.Length - 2)
                {
                    arrayString += ("jezyk [3]- " + array[i] + "\n");
                }
                else if (i == array.Length - 1)
                {
                    arrayString += ("jezyk [4]- " + array[i] + "\n");
                }
                else
                {
                    arrayString += ((char)(i + 97) + " - " + array[i] + "\n");
                }
            }
            return arrayString;
        }

        //Converts language from name to bit code and inserts it to data vector
        private void ConvertLanguageToTable(String languageName, double[] percentageArray)
        {
            languageName = languageName.ToUpper();
            foreach (Language language in LanguageList)
            {
                if (language.LanguageName.ToUpper().Equals(languageName))
                {
                    percentageArray[27] = language.BitCode[0];
                    percentageArray[28] = language.BitCode[1];
                    percentageArray[29] = language.BitCode[2];
                    percentageArray[30] = language.BitCode[3];
                    percentageArray[31] = language.BitCode[4];
                }
            }
        }

        //Checks if folder contains supported language files
        private bool CheckLanguage(String dir)
        {
            bool isLanguageInConfig = false;
            String language = dir.Remove(0, dir.Length - 3).ToUpper();
            foreach (Language languageListElement in LanguageList)
            {
                if (languageListElement.LanguageName == language)
                {
                    isLanguageInConfig = true;
                    break;
                }
            }
            return isLanguageInConfig;
        }
    }
}
