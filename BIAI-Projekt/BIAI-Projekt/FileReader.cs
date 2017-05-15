using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace BIAI_Projekt
{
    class FileReader
    {
        public StreamReader streamReader;
        public String path;
        String fileName;

        public List<double[]> mainList;

        public FileReader()
        {
            mainList = new List<double[]>();
            var dir = Directory.GetCurrentDirectory();
            path += dir;
            path += "\\data";
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
                        double[] percentageArray = new double[29];
                        for (int i = 0; i < percentageArray.Length; i++)
                        {
                            percentageArray[i] = 0;
                        }
                        String language = currentDir.Remove(0, currentDir.Length - 1);
                        //percentageArray[percentageArray.Length - 1] = Convert.ToDouble(language);
                        ConvertLanguageToTable(language, percentageArray, 2);

                        using (streamReader = new StreamReader(currentFile))
                        {
                            String line;
                            int charAmountInFile = 0;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                line = line.ToLower();
                                foreach (char c in line)
                                {

                                    if ((c > 97) && (c < 122))
                                    {
                                        int i = c - 97;
                                        percentageArray[i]++;
                                    }
                                    else if (c > 127)
                                    {
                                        percentageArray[26]++;
                                    }

                                    charAmountInFile++;
                                }
                            }
                            for (int i = 0; i < percentageArray.Length - 2; i++)
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
                if (i == array.Length - 3)
                {
                    arrayString += ("inne - " + array[i] + "\n");
                }
                else if (i == array.Length - 2)
                {
                    arrayString += ("jezyk [0]- " + array[i] + "\n");
                }
                else if(i == array.Length - 1)
                {
                    arrayString += ("jezyk [1]- " + array[i] + "\n");
                }
                else
                {
                    arrayString += ((char) (i + 97) + " - " + array[i] + "\n");
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

        public void ConvertLanguageToTable(string language, double[] percentageArray, int numberOfOutputs)
        {
            switch(language.ToUpper())
            {
                case "POLISH":
                    percentageArray[27] = 0;
                    percentageArray[28] = 1;
                    break;
                case "ENGLISH":
                    percentageArray[27] = 1;
                    percentageArray[28] = 0;
                    break;
                default:
                    percentageArray[27] = 0;
                    percentageArray[28] = 0;
                    break;
            }
        }
    }
}
