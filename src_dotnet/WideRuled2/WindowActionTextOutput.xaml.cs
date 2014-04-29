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
    /// Interaction logic for TextActionWindow.xaml
    /// </summary>
    public partial class WindowActionTextOutput : Window
    {

        private PlotFragment _parentPlotFrag;
        private ActionTextOutput _textOutputAction;

        public WindowActionTextOutput(PlotFragment frag, ActionTextOutput textOutputAction)
        {
            InitializeComponent();

            _parentPlotFrag = frag;
            _textOutputAction = textOutputAction;
            textBoxInput.Focus();
            dataBind();
        }

    
        private void dataBind() 
        {
            textBoxInput.Text = _textOutputAction.TextOutput;



            comboVariables.ItemsSource = _parentPlotFrag.getPreviouslyBoundPrimitiveVariables(TraitDataType.Number, true, _textOutputAction);
            comboVariables.SelectedIndex = 0;

            if(comboVariables.Items.Count == 0)
            {
                comboVariables.IsEnabled = false;
                btInsertVariable.IsEnabled = false;
            }
        }
 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _textOutputAction.TextOutput = textBoxInput.Text;
        }

        private void btInsertVariable_Click(object sender, RoutedEventArgs e)
        {
            
            if(comboVariables.SelectedItem != null)
            {
                int oldCaretIndex = textBoxInput.CaretIndex;
                textBoxInput.Text = textBoxInput.Text.Insert(textBoxInput.CaretIndex, "<" + ((Trait)comboVariables.SelectedItem).Name + ">");
                textBoxInput.CaretIndex = oldCaretIndex + ((Trait)comboVariables.SelectedItem).Name.Length + 2;
                textBoxInput.Focus();
            }
            
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
