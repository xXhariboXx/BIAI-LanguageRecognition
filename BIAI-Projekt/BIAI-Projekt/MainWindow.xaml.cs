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
using System.IO;

namespace BIAI_Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileReader fileReader;
        

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            fileReader = new FileReader();
            
        }

        private async void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            
            fileReader.mainList.Clear();
            fileReader.CreateListOfArrays(fileReader.path);
            textBox.Text = fileReader.PrintListOfArrays(fileReader.mainList);
        }
    }
}
