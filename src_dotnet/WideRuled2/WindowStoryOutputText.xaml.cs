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
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading;
using System.Speech;
using System.Speech.Synthesis;

namespace WideRuled2
{
    /// <summary>
    /// Interaction logic for StoryOutputTextWindow.xaml
    /// </summary>
    public partial class WindowStoryOutputText : Window
    {

        private string _textContent;
        private BackgroundWorker _uiUpdater;
        private BackgroundWorker _uiSpeaker;
        private WindowInteraction _interactionWindow;
        private SpeechSynthesizer _speechSynth;
        private bool _speakText;

   
        public WindowStoryOutputText(WindowInteraction interactWindow)
        {
            InitializeComponent();
            _uiUpdater = new BackgroundWorker();
            _uiSpeaker = new BackgroundWorker();

            _interactionWindow = interactWindow;
            _speechSynth = new SpeechSynthesizer();
            _speakText = false;
            _textContent = "";


            InitializeBackgoundWorkers();
 
        }

        private void InitializeBackgoundWorkers()
        {
            _uiUpdater.DoWork += new DoWorkEventHandler(uiUpdater_DoWork);
            _uiUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(uiUpdater_RunWorkerCompleted);
            _uiUpdater.ProgressChanged += new ProgressChangedEventHandler(uiUpdater_ProgressChanged);
            _uiUpdater.WorkerReportsProgress = true;
            _uiUpdater.WorkerSupportsCancellation = true;

            _uiSpeaker.DoWork += new DoWorkEventHandler(uiSpeaker_DoWork);
            _uiSpeaker.WorkerReportsProgress = false;
            _uiSpeaker.WorkerSupportsCancellation = true;


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _interactionWindow.Show();
            // Start the asynchronous operations.
            _uiUpdater.RunWorkerAsync(null);
            _uiSpeaker.RunWorkerAsync(null);

    

        }

        private void uiUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                Thread.Sleep(200);
                worker.ReportProgress(0);
                if (AblCommObject.Instance.StoryFinished)
                {
                    return;
                }
            }

        }


        // This event handler updates the story text output
        private void uiUpdater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _textContent = AblCommObject.Instance.StoryOutput.ToString();
            textBoxOutput.Text = _textContent;
            textBoxOutput.ScrollToEnd();
        }

        private void uiUpdater_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e)
        {
            _textContent = AblCommObject.Instance.StoryOutput.ToString();
            textBoxOutput.Text = _textContent;
        }


        private void uiSpeaker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                if(!_speakText)
                {
                    Thread.Sleep(200);
                }
                else 
                {
                    string textToSpeak = AblCommObject.Instance.LatestText;
                    if (textToSpeak != null)
                    {
                        _speechSynth.Speak(textToSpeak);
                    }
                }
            }
            

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _uiUpdater.CancelAsync();
            _uiSpeaker.CancelAsync();
            AblCommObject.Instance.AbortStory = true;
  
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
          
            this.Close();
        }

        private void checkBoxSpeak_Changed(object sender, RoutedEventArgs e)
        {

            _speakText = (bool)checkBoxSpeak.IsChecked;
          
        }

 
    }
}
