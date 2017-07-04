using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace BIAI_Projekt
{
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

    class FileReader
    {
        const String ConfigFileName = "ConfigFile.txt";
        const String TrainDataFolderName = "traindata";
        const String TestDataFolderName = "testdata";

        public StreamReader streamReader;
        public List<double[]> mainList;
        public List<Language> languageList;

        String ConfigFilePath;
        public String TrainDataFolderPath;
        public String TestDataFolderPath;

        public FileReader()
        {
            mainList = new List<double[]>();
            languageList = new List<Language>();
            var dir = Directory.GetCurrentDirectory();
            ConfigFilePath += dir + "\\data\\" + ConfigFileName;
            TrainDataFolderPath += dir + "\\data\\" + TrainDataFolderName;
            TestDataFolderPath += dir + "\\data\\" + TestDataFolderName;
        }

        public void ReadLanguageFile()
        {
            using (streamReader = new StreamReader(ConfigFilePath))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.ToUpper();
                    String languageName = line.Substring(0, 3);
                    int[] languageBits = new int[3];
                    languageBits[0] = (int)Char.GetNumericValue(line.ElementAt(4));
                    languageBits[1] = (int)Char.GetNumericValue(line.ElementAt(5));
                    languageBits[2] = (int)Char.GetNumericValue(line.ElementAt(6));
                    languageList.Add(new Language(languageName, languageBits));
                }
            }
        }

        public void CreateListOfArrays(String path)
        {
            try
            {
                var languageFiles = Directory.EnumerateDirectories(path);
                foreach (string currentDir in languageFiles)
                {
                    var txtFiles = Directory.EnumerateFiles(currentDir, "*.txt");
                    foreach (string currentFile in txtFiles)
                    {
                        double[] percentageArray = new double[30];
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
                            for (int i = 0; i < percentageArray.Length - 3; i++)
                            {
                                percentageArray[i] = ((percentageArray[i]) / charAmountInFile) * 100;
                            }
                        }
                        mainList.Add(percentageArray);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("blad");
            }
        }

        private String PrintPercentageArray(double[] array)
        {
            String arrayString = "";

            for (int i = 0; i < array.Length; i++)
            {
                if (i == array.Length - 4)
                {
                    arrayString += ("inne - " + array[i] + "\n");
                }
                else if (i == array.Length - 3)
                {
                    arrayString += ("jezyk [0]- " + array[i] + "\n");
                }
                else if (i == array.Length - 2)
                {
                    arrayString += ("jezyk [1]- " + array[i] + "\n");
                }
                else if (i == array.Length - 1)
                {
                    arrayString += ("jezyk [2]- " + array[i] + "\n");
                }
                else
                {
                    arrayString += ((char)(i + 97) + " - " + array[i] + "\n");
                }
            }
            return arrayString;
        }

        public String PrintListOfArrays(List<double[]> list)
        {
            String listToPrint = "";

            for (int i = 0; i < list.Count; i++)
            {
                listToPrint += (PrintPercentageArray(list.ElementAt(i)) + "/////////////////\n");
            }
            return listToPrint;
        }

        private void ConvertLanguageToTable(string languageName, double[] percentageArray)
        {
            languageName = languageName.ToUpper();
            foreach(Language language in languageList)
            {
                if(language.LanguageName.ToUpper().Equals(languageName))
                {
                    percentageArray[27] = language.BitCode[0];
                    percentageArray[28] = language.BitCode[1];
                    percentageArray[29] = language.BitCode[2];
                }
            }
        }
    }
}
