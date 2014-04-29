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
    /// Interaction logic for ChooseActionDialog.xaml
    /// </summary>
    public partial class WindowChooseActionDialog : Window
    {

        private string _displayText;
        private string _resultText;
        private int _resultIndex;
        private List<string> _choices;



        public WindowChooseActionDialog(string displayText, List<string> choices)
        {
            InitializeComponent();
            _displayText = displayText;
            _resultIndex = -1;
            txtBlockOutputText.Text = _displayText;
            _resultText = "";
            _choices = choices;

            comboChoice.ItemsSource = _choices;
            comboChoice.SelectedIndex = 0;


        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            _resultIndex = -1;
            this.DialogResult = false;
           
        }

        public string ResultText
        {
            get { return _resultText; }
        }

        public int ResultIndex
        {
            get { return _resultIndex; }
        }

      
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            Object result = comboChoice.SelectedItem;

            if((result != null) && (result.GetType() == typeof(string)))
            {
                _resultText = (string)result;
                _resultIndex = comboChoice.SelectedIndex;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboChoice.Focus();
        }

        private void comboChoice_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btOK_Click(sender, e);
            }
        }


    }
}
