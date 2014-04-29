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
using System.Threading;
using System.ComponentModel;

namespace WideRuled2
{
    /// <summary>
    /// Interaction logic for InteractionWindow.xaml
    /// </summary>
    public partial class WindowInteraction : Window
    {

    
        private List<Interaction> _interactionList;

        private BackgroundWorker _autoCloser;


        public WindowInteraction(List<Interaction> interactList)
        {
            InitializeComponent();
            _autoCloser = new BackgroundWorker();
            InitializeBackgoundWorker();
            _interactionList = interactList;

            dataBind();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Start the asynchronous operation.
            _autoCloser.RunWorkerAsync(null);
        }

        private void InitializeBackgoundWorker()
        {
            _autoCloser.DoWork += new DoWorkEventHandler(AutoClose_WaitForClose);
            _autoCloser.ProgressChanged += new ProgressChangedEventHandler(AutoClose_CloseWindow);
            _autoCloser.WorkerReportsProgress = true;
        }

        private void AutoClose_WaitForClose(object sender, DoWorkEventArgs e)
        {
           
            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                Thread.Sleep(300);
                
                if (AblCommObject.Instance.StoryFinished)
                {
                    worker.ReportProgress(0);
                    return;
                }
            }

        }

        private void AutoClose_CloseWindow(object sender, ProgressChangedEventArgs e)
        {
            this.Close();
        }

        private void dataBind()
        {

            comboChoice.ItemsSource = _interactionList;
            if (_interactionList.Count == 0)
            {
                comboChoice.IsEnabled = false;
                btDoIt.IsEnabled = false;
            }
            if (_interactionList.Count > 0)
            {
                comboChoice.SelectedIndex = 0;
            }
  
        }

    
        private void btDoIt_Click(object sender, RoutedEventArgs e)
        {
            

            if(comboChoice.SelectedIndex > -1)
            {
                AblCommObject.Instance.DoInteraction = true;
                AblCommObject.Instance.InteractionId = comboChoice.SelectedIndex;
            }

        }

        private void btUndo_Click(object sender, RoutedEventArgs e)
        {
            AblCommObject.Instance.Undo = true;

        }

        private void btStopStory_Click(object sender, RoutedEventArgs e)
        {
            AblCommObject.Instance.AbortStory = true;

            //TEST

           // AblCommObject.Instance.StoryFinished = true;
            //this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            AblCommObject.Instance.AbortStory = true;
        }

 
    }
}
