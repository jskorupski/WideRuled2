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
    /// Interaction logic for ActionCalculationEdit.xaml
    /// </summary>
    public partial class WindowActionCalculationEditor : Window
    {
        private ActionCalculation _currentEntity;
        private StoryData _currentStoryData;
        private PlotFragment _parentPlotFragment;

        public WindowActionCalculationEditor(PlotFragment parentPlotFragment, ActionCalculation calc, StoryData world)
        {

            InitializeComponent();
            _currentEntity = calc;
            _parentPlotFragment = parentPlotFragment;
            _currentStoryData = world;

            textBoxNumericLeft.NullValue = 0.0;
            textBoxNumericRight.NullValue = 0.0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataBind();
        }


        private bool dataValid()
        {
            if((!_currentEntity.ParamRight.ValueIsBoundToVariable) && (comboBoxOp.SelectedIndex == 3))
            {
                if(((double)textBoxNumericRight.Value < 0.00000001) &&((double)textBoxNumericRight.Value > -0.00000001))
                {
                    Utilities.MakeErrorDialog("You cannot divide by zero.", this);
                    return false;
                }
            }
            return true;
        }

        private void saveData()
        {

            //Op
            switch (comboBoxOp.SelectedIndex)
            {
                case 0:
                    _currentEntity.CalculationOp = CalculationOperation.Add;
            	    break;
                case 1:
                    _currentEntity.CalculationOp = CalculationOperation.Subtract;
                    break;
                case 2:
                    _currentEntity.CalculationOp = CalculationOperation.Multiply;
                    break;
                case 3:
                    _currentEntity.CalculationOp = CalculationOperation.Divide;
                    break;
            }

            //Left val
            if(_currentEntity.ParamLeft.ValueIsBoundToVariable)
            {
                Trait varName = (Trait)comboBoxLeft.SelectedItem;
                if(varName != null)
                {
                    _currentEntity.ParamLeft.LiteralValueOrBoundVarName = varName.Name;
                }
                
            }
            else
            {
                _currentEntity.ParamLeft.LiteralValueOrBoundVarName = textBoxNumericLeft.Value;
            }


            //Right val
            if (_currentEntity.ParamRight.ValueIsBoundToVariable)
            {
                Trait varName = (Trait)comboBoxRight.SelectedItem;
                if (varName != null)
                {
                    _currentEntity.ParamRight.LiteralValueOrBoundVarName = varName.Name;
                }

            }
            else
            {
                _currentEntity.ParamRight.LiteralValueOrBoundVarName = textBoxNumericRight.Value;
            }



        }
        private void dataBind()
        {
            titleTextBlock.Text = "Variable \"" +_currentEntity.ResultVarName + "\" = ";
            switch (_currentEntity.CalculationOp)
            {
                case CalculationOperation.Add:
                    comboBoxOp.SelectedIndex = 0;
            	    break;
                case CalculationOperation.Subtract:
                    comboBoxOp.SelectedIndex = 1;
                    break;
                case CalculationOperation.Multiply:
                    comboBoxOp.SelectedIndex = 2;
                    break;
                case CalculationOperation.Divide:
                    comboBoxOp.SelectedIndex = 3;
                    break;
            }

            if(_currentEntity.ParamLeft.ValueIsBoundToVariable)
            {
                checkBoxLeftUseVar.IsChecked = true;
                checkBoxLeftUseVar.Visibility = Visibility.Visible;
                textBoxNumericLeft.Visibility = Visibility.Hidden;
                comboBoxLeft.Visibility = Visibility.Visible;

                List<Trait> prevVars = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.ParamLeft.Type, false, _currentEntity);

                
                //Left number
                comboBoxLeft.ClearValue(ComboBox.ItemsSourceProperty);
                comboBoxLeft.Items.Clear();
                comboBoxLeft.ItemsSource = prevVars;

                
                int choiceIndex = 0;
                bool foundVar = false;
                foreach (Trait traitItem in prevVars)
                {
                    if (traitItem.Name == (string)_currentEntity.ParamLeft.LiteralValueOrBoundVarName)
                    {
                        foundVar = true;
                        break;
                    }
                    choiceIndex++;
                }
                if (foundVar)
                {
                    comboBoxLeft.SelectedIndex = choiceIndex;
                }
                else
                {
                    comboBoxLeft.SelectedIndex = 0;
                }

               
            }
            else 
            {
                checkBoxLeftUseVar.IsChecked = false;

                List<Trait> prevVars = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.ParamLeft.Type, false, _currentEntity);
                if(prevVars.Count == 0)
                {
                    checkBoxLeftUseVar.Visibility = Visibility.Hidden;
                }
                else
                {
                    checkBoxLeftUseVar.Visibility = Visibility.Visible;
                }
                
                comboBoxLeft.Visibility = Visibility.Hidden;
                textBoxNumericLeft.Visibility = Visibility.Visible;
                textBoxNumericLeft.Value = (double)_currentEntity.ParamLeft.LiteralValueOrBoundVarName;
                

            }
            
            
            if(_currentEntity.ParamRight.ValueIsBoundToVariable)
            {
                checkBoxRightUseVar.IsChecked = true;
                checkBoxRightUseVar.Visibility = Visibility.Visible;
                textBoxNumericRight.Visibility = Visibility.Hidden;
                comboBoxRight.Visibility = Visibility.Visible;

                //Right number
                List<Trait> prevVars = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.ParamLeft.Type, false, _currentEntity);
                comboBoxRight.ClearValue(ComboBox.ItemsSourceProperty);
                comboBoxRight.Items.Clear();
                comboBoxRight.ItemsSource = prevVars;


                int choiceIndex = 0;
                bool foundVar = false;
                foreach (Trait traitItem in prevVars)
                {
                    if (traitItem.Name == (string)_currentEntity.ParamRight.LiteralValueOrBoundVarName)
                    {
                        foundVar = true;
                        break;
                    }
                    choiceIndex++;
                }
                if (foundVar)
                {
                    comboBoxRight.SelectedIndex = choiceIndex;
                }
                else
                {
                    comboBoxRight.SelectedIndex = 0;
                }

            }
            else
            {

                List<Trait> prevVars = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.ParamLeft.Type, false, _currentEntity);
                if (prevVars.Count == 0)
                {
                    checkBoxRightUseVar.Visibility = Visibility.Hidden;
                }
                else
                {
                    checkBoxRightUseVar.Visibility = Visibility.Visible;
                }
                checkBoxRightUseVar.IsChecked = false;
                comboBoxRight.Visibility = Visibility.Hidden;
                textBoxNumericRight.Visibility = Visibility.Visible;
                textBoxNumericRight.Value = (double)_currentEntity.ParamRight.LiteralValueOrBoundVarName;


            }

        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void checkBoxLeftUseVar_Checked(object sender, RoutedEventArgs e)
        {
            _currentEntity.ParamLeft.ValueIsBoundToVariable = (bool)checkBoxLeftUseVar.IsChecked;
            dataBind();
        }


        private void checkBoxRightUseVar_Checked(object sender, RoutedEventArgs e)
        {
            _currentEntity.ParamRight.ValueIsBoundToVariable = (bool)checkBoxRightUseVar.IsChecked;
            dataBind();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveData();

            if (!dataValid())
                e.Cancel = true;
        }


    }
}
