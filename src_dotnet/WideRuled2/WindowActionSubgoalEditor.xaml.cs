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
using Xceed.Wpf.Controls;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.ThemePack;

namespace WideRuled2
{
    /// <summary>
    /// Interaction logic for WindowActionSubgoalEditor.xaml
    /// </summary>
    public partial class WindowActionSubgoalEditor : Window
    {

        private bool _interactionMode;
        private ActionSubgoal _currentEntity;
        private StoryData _currentStoryData;
        private PlotFragment _parentPlotFragment;
        private Parameter _currentlySelectedParameter;
       // private Parameter _previouslySlectedParameter;
        private bool _validData;


        public WindowActionSubgoalEditor(bool interactionMode, PlotFragment frag, ActionSubgoal currEntity, StoryData currStoryData)
        {
            InitializeComponent();
            _currentEntity = currEntity;
            _currentStoryData = currStoryData;
            _parentPlotFragment = frag;
            _currentlySelectedParameter = null;
            //_previouslySlectedParameter = null;
            _validData = true;
            _interactionMode = interactionMode;

            txtBoxNumberInput.NullValue = 0.0;
            _currentEntity.syncParametersWithSubgoal();
            dataBind();
        }

        private void dataBind()
        {
            clearDataElements();
            dataBindList();
            dataBindGrid();
        }
        private void clearDataElements()
        {
            paramDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            paramDataGrid.Items.Clear();
        }
        private void dataBindList()
        {
            titleTextBlock.Text = _currentStoryData.findAuthorGoalById(_currentEntity.SubGoalId).Name;
            paramDataGrid.ItemsSource = _currentEntity.ParametersToPass;
            paramDataGrid.SelectedItem = null;
            _currentlySelectedParameter = null;
        }

        private void dataBindGrid()
        {
            if(_interactionMode)
            {
                //This window is being used to generate a new interactive
                //story action. Therefore, there are no saved variables to pass 
                //along, so the "use saved variable" checkbox will be hidden
                checkBoxUseVariable.Visibility = Visibility.Hidden;
            }
            Object itemToEdit = paramDataGrid.SelectedItem;
           // if ((itemToEdit == null) || !(itemToEdit is Parameter))
            if(_currentlySelectedParameter == null)
            {
                gridDetails.Visibility = Visibility.Hidden;
            }
            else
            {
                gridDetails.Visibility = Visibility.Visible;
                textBlockParamName.Text = "Parameter: " + _currentlySelectedParameter.Name;
                checkBoxUseVariable.IsChecked = _currentlySelectedParameter.ValueIsBoundToVariable;
                setupDataInputBoxes(_currentlySelectedParameter.Type, _currentlySelectedParameter.ValueIsBoundToVariable);
            }
        }

        private void setupDataInputBoxes(TraitDataType type, bool variable)
        {
            if (variable)
            {
                textBoxTextInput.Visibility = Visibility.Hidden;
                txtBoxNumberInput.Visibility = Visibility.Hidden;
                comboBoxTrueFalse.Visibility = Visibility.Hidden;
                comboChoiceVariables.Visibility = Visibility.Visible;
                List<Trait> prevVars = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentlySelectedParameter.Type, false, _currentEntity);
                comboChoiceVariables.ClearValue(ComboBox.ItemsSourceProperty);
                comboChoiceVariables.Items.Clear();
                comboChoiceVariables.ItemsSource = prevVars;

                //Find variable in combobox list by name
                int choiceIndex = 0;
                bool foundVar = false;
                foreach(Trait traitItem in prevVars)
                {
                    if(traitItem.Name == (string)_currentlySelectedParameter.LiteralValueOrBoundVarName)
                    {
                        foundVar = true;
                        break;
                    }
                    choiceIndex++;
                }
                if (foundVar)
                {
                    comboChoiceVariables.SelectedIndex = choiceIndex;
                }
                else
                {
                    comboChoiceVariables.SelectedIndex = 0;
                }
                
            }
            else 
            {
                if(type == TraitDataType.Text)
                {
                    textBoxTextInput.Visibility = Visibility.Visible;
                    textBoxTextInput.Text = (string)_currentlySelectedParameter.Value;

                    txtBoxNumberInput.Visibility = Visibility.Hidden;
                    comboBoxTrueFalse.Visibility = Visibility.Hidden;
                    comboChoiceVariables.Visibility = Visibility.Hidden;

                }
                else if (type == TraitDataType.Number)
                {
                    textBoxTextInput.Visibility = Visibility.Hidden;

                    txtBoxNumberInput.Visibility = Visibility.Visible;
                    txtBoxNumberInput.Value = _currentlySelectedParameter.Value;
                    comboBoxTrueFalse.Visibility = Visibility.Hidden;
                    comboChoiceVariables.Visibility = Visibility.Hidden;
                }
                else if (type == TraitDataType.TrueFalse)
                {
                    textBoxTextInput.Visibility = Visibility.Hidden;

                    txtBoxNumberInput.Visibility = Visibility.Hidden;
                    
                    comboBoxTrueFalse.Visibility = Visibility.Visible;
                    if((bool)_currentlySelectedParameter.Value == true)
                    {
                        comboBoxTrueFalse.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBoxTrueFalse.SelectedIndex = 1;
                    }
                    comboChoiceVariables.Visibility = Visibility.Hidden;
                }

            }

        }

        private void saveParameterData()
        {
            if (_currentlySelectedParameter == null) { return; }


            if ((bool)checkBoxUseVariable.IsChecked)
            {
                Object itemToEdit = comboChoiceVariables.SelectedItem;
                if ((itemToEdit == null) || !(itemToEdit is Trait)) 
                {
                    Utilities.MakeErrorDialog("Parameter to send \"" + _currentlySelectedParameter.Name +
                        "\" must have value or be associated with a saved variable.", this);
                    _validData = false;
                    return;
                }
                else
                {
                    Trait varName = (Trait)comboChoiceVariables.SelectedItem;
                    if(varName != null)
                    {
                        _currentlySelectedParameter.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;
                        _currentlySelectedParameter.LiteralValueOrBoundVarName = varName.Name;
                    }
                }
            }
            else
            {
                _currentlySelectedParameter.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;


                if (_currentlySelectedParameter.Type == TraitDataType.Number)
                {
                    _currentlySelectedParameter.Value = (double)txtBoxNumberInput.Value;
                }
                else if (_currentlySelectedParameter.Type == TraitDataType.TrueFalse)
                {
                    int selectedIndex = comboBoxTrueFalse.SelectedIndex;
                    if (selectedIndex < 0)
                    {
                        return;
                    }
                    else
                    {
                        _currentlySelectedParameter.Value = (selectedIndex == 0) ? true : false;
                    }
                }
                else if (_currentlySelectedParameter.Type == TraitDataType.Text)
                {

                    _currentlySelectedParameter.Value = textBoxTextInput.Text.Trim();
                }
            }
            _validData = true;
        }

        private void paramDataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {

            

            Object itemToEdit = paramDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is Parameter)) { return; }

           // _previouslySlectedParameter = _currentlySelectedParameter;


            if (_currentlySelectedParameter != null)
            {
                saveParameterData();
            }
            if(_validData)
            {
                _currentlySelectedParameter = (Parameter)itemToEdit;
                dataBindGrid();
                
            }

        }

        private void btChangeAuthGoal_Click(object sender, RoutedEventArgs e)
        {
            List<string> choiceList = new List<string>();
            foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
            {
                choiceList.Add(goal.Description);
            }
            int result = Utilities.MakeListChoiceDialog("Select a new Author Goal to pursue", choiceList, this);
            if(result > -1)
            {
                AuthorGoal newSubgoal = _currentStoryData.AuthorGoals[result];
                _currentEntity.SubGoalId = newSubgoal.Id;
                dataBind();

            }
        }

        private void checkBoxUseVariable_Checked(object sender, RoutedEventArgs e)
        {

            _currentlySelectedParameter.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;
            dataBindGrid();
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {

                this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveParameterData();

            if (!_validData)
                e.Cancel = true;
        }

        private void txtBoxNumberInput_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                saveParameterData();
            }
        }

        private void textBoxTextInput_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                saveParameterData();
            }
        }

    }
}
