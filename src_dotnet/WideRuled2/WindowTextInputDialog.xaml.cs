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
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    public partial class WindowTextInputDialog : Window
    {
        private string _displayText;
        private string _resultText;
        private List<string> _constraints;
        private string _constraintErrorText;

        public WindowTextInputDialog(string displayText)
        {
            InitializeComponent();
            _displayText = displayText;
            txtBlockOutputText.Text = _displayText;
            _resultText = "";
        }
        public WindowTextInputDialog(string displayText, string constraintErrorText, List<string> constraints)
        {
            InitializeComponent();
            _displayText = displayText;
            txtBlockOutputText.Text = _displayText;
            _resultText = "";
            _constraintErrorText = constraintErrorText;
            _constraints = constraints;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
           
        }

        public string ResultText
        {
            get { return _resultText; }
        }
        private Boolean dataValid()
        {
            string result = textBoxInput.Text.Trim();
            if (result.Equals(""))
            {
                
                Utilities.MakeErrorDialog("Please enter non-blank text", this);
                return false;
            }

            if(_constraints != null)
            {
                foreach (string constraintItem in _constraints)
                {
                    if (constraintItem.Equals(result))
                    {
                        Utilities.MakeErrorDialog(_constraintErrorText, this);
                        return false;

                    }
                }
            }


            return true;

        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            string result = textBoxInput.Text.Trim();

            
            if(dataValid())
            {
                _resultText = result;
                this.DialogResult = true;
                this.Close();
            }
        }



        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btOK_Click(sender, e);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxInput.Focus();
        }
    }
}
