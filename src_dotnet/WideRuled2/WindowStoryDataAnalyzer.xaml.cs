using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WideRuled2
{
    /// <summary>
    /// Interaction logic for WindowStoryDataAnalyzer.xaml
    /// </summary>
    public partial class WindowStoryDataAnalyzer : Window
    {
        private string _analysisData;
        private string _sampleStoryFilename;
       

        public WindowStoryDataAnalyzer()
        {
            
            InitializeComponent();
           

            _analysisData = "";
            _sampleStoryFilename = "";
        }


        private void btSelectDir_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Multiselect = true;
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ""; // Default file extension
            dlg.Filter = "Wide Ruled 2 Stories (.wr2)|*.wr2"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string[] resultFileNames = {};
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                resultFileNames = dlg.FileNames;
                
            }

            if(resultFileNames.Length > 0)
            {

                try
                {
                    _analysisData = StoryDataAnalyzer.analyze(resultFileNames, _sampleStoryFilename);  

                }
                catch (System.Exception analysisError)
                {

                    string messageBoxText = "Error analyzing files:\n" + analysisError.Message + ":\n" + analysisError.StackTrace;
                    string caption = "Data Analysis Error";


                    Utilities.MakeErrorDialog(messageBoxText, caption, this);
                }
                
                textBoxOutput.Text = _analysisData;
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "DataAnalysis"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Comma Separated Values (.csv)|*.csv"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string resultFileName = "";

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                resultFileName = dlg.FileName;
            }


            if (!(resultFileName.Equals("")))
            {

                // Save to disk


                try
                {
                    Utilities.SaveStringToDisk(resultFileName, _analysisData);
                }
                catch (System.Exception saveError)
                {

                    string messageBoxText = "Error saving file:\n" + saveError.Message;
                    string caption = "Save Error";


                    Utilities.MakeErrorDialog(messageBoxText, caption, this);
                }


            }
        }

        private void btSelectSample_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ""; // Default file extension
            dlg.Filter = "Wide Ruled 2 Sample Story (.wr2)|*.wr2"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string resultFileName = "";
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                resultFileName = dlg.FileName;
                

            }

            if (!resultFileName.Equals(""))
            {
                _sampleStoryFilename = resultFileName;
                btSelectSample.Content = "Sample Story: " + System.IO.Path.GetFileName(_sampleStoryFilename);

                btSelectDir.IsEnabled = true;
            }
        }
    }
}
