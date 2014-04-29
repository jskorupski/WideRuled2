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
    /// Interaction logic for PreconditionEditorWindow.xaml
    /// </summary>
    public partial class WindowPreconditionEditor : Window
    {
        private int _editingMode;
        //0 = Character
        //1 = Environment
        //2 = plot point

        private PreconditionStatement _currentEntity;
        private StoryData _currentStoryData;
        private PlotFragment _parentPlotFragment;
        private Constraint _currentlySelectedConstraint;
        private PlotPointType _ppType; //for plot point precondition statement editing mode
        private string _typeString;
        private bool _currentlyDataBinding;

        public WindowPreconditionEditor(PlotFragment frag, PreconditionStatement currEntity, StoryData currStoryData)
        {
            InitializeComponent();

            _currentStoryData = currStoryData;
            _currentEntity = currEntity;
            _parentPlotFragment = frag;
            _currentlySelectedConstraint = null;
            _currentlyDataBinding = false;

            //Find editing type
            if (currEntity is PreconditionStatementCharacter)
            {
                _editingMode = 0;
            }
            else if (currEntity is PreconditionStatementEnvironment)
            {
                _editingMode = 1;
            }
            else
            {
                PlotPointType currType = currStoryData.findPlotPointTypeById(((PreconditionStatementPlotPoint)currEntity).MatchTypeId);

                _editingMode = 2;
                _ppType = currType;

            }
            txtBoxNumberInput.NullValue = 0.0;

            dataBind();
        }

        private void saveData()
        {
            //Save value/variable name
            if (_currentlySelectedConstraint == null) { return; }

            if (comboBoxTraitRelationship.SelectedIndex == 0)
            {

                TraitDataType type = _currentlySelectedConstraint.ComparisonValue.Type;

                if (_currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable)
                {
                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = ((Trait)comboChoiceVariables.SelectedItem).Name;
                }
                else if (type == TraitDataType.Number)
                {

                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = txtBoxNumberInput.Value;

                }
                else if (type == TraitDataType.Text)
                {
                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = textBoxTextInput.Text;
                }
                else if (type == TraitDataType.TrueFalse)
                {

                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = (comboBoxTrueFalse.SelectedIndex == 0);
                }
            }
            else if (comboBoxTraitRelationship.SelectedIndex == 1) //relationship target name
            {
               
                if (_currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable)
                {
                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = ((Trait)comboChoiceVariables.SelectedItem).Name;
                }
                
                else //get literal text value
                {
                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = textBoxTextInput.Text;
                }
            }
            else if (comboBoxTraitRelationship.SelectedIndex == 2) //relationship strength
            {
                if (_currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable)
                {
                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = ((Trait)comboChoiceVariables.SelectedItem).Name;
                }

                else //get literal number value
                {
                    _currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName = txtBoxNumberInput.Value;
                }
            }


        }

        private void clearDataElements()
        {
            clearHeaderElements();
            clearDetailElements();

        }
        private void clearHeaderElements()
        {
            _currentlyDataBinding = true;
            constraintDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            constraintDataGrid.Items.Clear();


            _currentlyDataBinding = false;
        }

        private void clearDetailElements()
        {
            _currentlyDataBinding = true;
            comboBoxTraitRelationship.ClearValue(DataGridControl.ItemsSourceProperty);
            comboBoxTraitRelationship.Items.Clear();

            comboBoxSelectTraitRel.ClearValue(DataGridControl.ItemsSourceProperty);
            comboBoxSelectTraitRel.Items.Clear();

            comboChoiceVariables.ClearValue(DataGridControl.ItemsSourceProperty);
            comboChoiceVariables.Items.Clear();

  
            comboBoxComparisonOp.ClearValue(DataGridControl.ItemsSourceProperty);
            comboBoxComparisonOp.Items.Clear();

            _currentlyDataBinding = false;

        }

        private void dataBind()
        {
            dataBindHeaderItems();
            dataBindDetails();
        }

        private void dataBindDetails()
        {

            _currentlyDataBinding = true;


            if (_currentlySelectedConstraint == null)
            {
                //gridPrecondDetails.Visibility = Visibility.Hidden;
                gridPrecondDetails.IsEnabled = false;
                return;
            }
            else
            {
                //gridPrecondDetails.Visibility = Visibility.Visible;
                gridPrecondDetails.IsEnabled = true;
            }

            



            //Top combobox
            List<string> attributeOptions = new List<string>();
            attributeOptions.Add("Trait");

            //Only add relationship editors if there relationships that can be edited
            if ((_editingMode == 0) && (Utilities.getGlobalCharacterList(_currentStoryData)[0].Relationships.Count > 0))
            {

                attributeOptions.Add("Relationship Target Name");
                attributeOptions.Add("Relationship Strength");
            }
            else if ((_editingMode == 1) && (Utilities.getGlobalEnvironmentList(_currentStoryData)[0].Relationships.Count > 0))
            {

                attributeOptions.Add("Relationship Target Name");
                attributeOptions.Add("Relationship Strength");
            }

            comboBoxTraitRelationship.ClearValue(ComboBox.ItemsSourceProperty);
            comboBoxTraitRelationship.Items.Clear();
            comboBoxTraitRelationship.ItemsSource = attributeOptions;

            if (_currentlySelectedConstraint is TraitConstraint)
            {
                comboBoxTraitRelationship.SelectedIndex = 0;
            }
            else if ((_currentlySelectedConstraint is RelationshipConstraint) &&
                ((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode)
            {
                comboBoxTraitRelationship.SelectedIndex = 1;
            }
            else
            {
                comboBoxTraitRelationship.SelectedIndex = 2;
            }



            //Left combobox

            if (comboBoxTraitRelationship.SelectedIndex == 0) //Trait 
            {
                if (_editingMode == 0)
                {
                    comboBoxSelectTraitRel.ItemsSource = Utilities.getGlobalCharacterList(_currentStoryData)[0].Traits;
                }
                else if (_editingMode == 1)
                {
                    comboBoxSelectTraitRel.ItemsSource = Utilities.getGlobalEnvironmentList(_currentStoryData)[0].Traits;
                }
                else if (_editingMode == 2)
                {
                    comboBoxSelectTraitRel.ItemsSource = _ppType.Traits;
                }

            }
            else if (comboBoxTraitRelationship.SelectedIndex > 0) //relationship target name or strength
            {
                if (_editingMode == 0)
                {   // Using get GLOBAL list (includes not just static characters, but ones
                    //created within plot fragments. This way the user doesn't need to
                    //have created any static character lists to make plot fragment actions
                    //that edit dynamically generated objects
                    comboBoxSelectTraitRel.ItemsSource = Utilities.getGlobalCharacterList(_currentStoryData)[0].Relationships;
                }
                else if (_editingMode == 1)
                {
                    comboBoxSelectTraitRel.ItemsSource = Utilities.getGlobalEnvironmentList(_currentStoryData)[0].Relationships;
                }
            }

            //comparison operation dropdown list
            List<string> allowedOperations = new List<string>();
            allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.Equals));
            allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.NotEquals));

            if (_currentlySelectedConstraint.ComparisonValue.Type == TraitDataType.Number)
            {
                allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.GreaterThan));
                allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.LessThan));
                allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.GreaterThanEqualTo));
                allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.LessThanEqualTo));

            }

            if (_currentlySelectedConstraint.AllowedToSave)
            {
                allowedOperations.Add(Constraint.ConstraintComparisonTypeToString(ConstraintComparisonType.None));
            }
            comboBoxComparisonOp.ItemsSource = allowedOperations;

            checkBoxAlwaysTrue.IsChecked = (bool)_currentlySelectedConstraint.MustAlwaysBeTrue;

            switch (_currentlySelectedConstraint.ConstraintType)
            {
                case ConstraintComparisonType.Equals:
                    comboBoxComparisonOp.SelectedIndex = 0;
                    break;
                case ConstraintComparisonType.NotEquals:
                    comboBoxComparisonOp.SelectedIndex = 1;
                    break;
                case ConstraintComparisonType.GreaterThan:
                    comboBoxComparisonOp.SelectedIndex = 2;
                    break;
                case ConstraintComparisonType.LessThan:
                    comboBoxComparisonOp.SelectedIndex = 3;
                    break;
                case ConstraintComparisonType.GreaterThanEqualTo:
                    comboBoxComparisonOp.SelectedIndex = 4;
                    break;
                case ConstraintComparisonType.LessThanEqualTo:
                    comboBoxComparisonOp.SelectedIndex = 5;
                    break;
                case ConstraintComparisonType.None:
                    if (_currentlySelectedConstraint.ComparisonValue.Type == TraitDataType.Number)
                    {
                        comboBoxComparisonOp.SelectedIndex = 6;
                    }
                    else
                    {
                        comboBoxComparisonOp.SelectedIndex = 2;
                    }
                    
                    break;
            }

            if (_currentlySelectedConstraint.ConstraintType == ConstraintComparisonType.None)
            {
                //check box and values are irrelevant for this comparison
                checkBoxUseVariable.IsEnabled = false;
                textBoxTextInput.IsEnabled = false;
                txtBoxNumberInput.IsEnabled = false;
                comboBoxTrueFalse.IsEnabled = false;
                comboChoiceVariables.IsEnabled = false;
             
                checkBoxUseVariable.IsChecked = false;
                checkBoxAlwaysTrue.IsEnabled = false;
                checkBoxAlwaysTrue.IsChecked = false;
            }
            else
            {
                checkBoxUseVariable.IsEnabled = true;
                textBoxTextInput.IsEnabled = true;
                txtBoxNumberInput.IsEnabled = true;
                comboBoxTrueFalse.IsEnabled = true;
                comboChoiceVariables.IsEnabled = true;
    
                checkBoxAlwaysTrue.IsEnabled = true;


                //checkbox
                checkBoxUseVariable.IsChecked = _currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable;

                //value boxes
                if (_currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable)
                {
                    textBoxTextInput.Visibility = Visibility.Hidden;
                    txtBoxNumberInput.Visibility = Visibility.Hidden;
                    comboBoxTrueFalse.Visibility = Visibility.Hidden;
                    comboChoiceVariables.Visibility = Visibility.Visible;
                    checkBoxUseVariable.Visibility = Visibility.Visible;

                    //Data bind variable list
                    comboChoiceVariables.ItemsSource = _currentEntity.getPreviouslyBoundPrimitiveVariables(_currentlySelectedConstraint.ComparisonValue.Type, false, _currentlySelectedConstraint);
                    comboChoiceVariables.ItemTemplate = this.FindResource("comboBoxDataTemplate") as DataTemplate;


                }
                else
                {
                    TraitDataType type = _currentlySelectedConstraint.ComparisonValue.Type;
                    if (type == TraitDataType.Number)
                    {
                        textBoxTextInput.Visibility = Visibility.Hidden;
                        txtBoxNumberInput.Visibility = Visibility.Visible;
                        comboBoxTrueFalse.Visibility = Visibility.Hidden;
                        comboChoiceVariables.Visibility = Visibility.Hidden;

                        //Only show checkbox if there are actual variables to select
                        List<Trait> varList = _currentEntity.getPreviouslyBoundPrimitiveVariables(type, false, _currentlySelectedConstraint);
                        if (varList.Count > 0)
                        {
                            checkBoxUseVariable.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            checkBoxUseVariable.Visibility = Visibility.Hidden;
                        }

                        //Data bind Number 
                        txtBoxNumberInput.Value = (double)_currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName;

                    }
                    else if (type == TraitDataType.Text)
                    {
                        textBoxTextInput.Visibility = Visibility.Visible;
                        txtBoxNumberInput.Visibility = Visibility.Hidden;
                        comboBoxTrueFalse.Visibility = Visibility.Hidden;
                        comboChoiceVariables.Visibility = Visibility.Hidden;

                        //Only show checkbox if there are actual variables to select
                        List<Trait> varList = _currentEntity.getPreviouslyBoundPrimitiveVariables(type, false, _currentlySelectedConstraint);
                        if (varList.Count > 0)
                        {
                            checkBoxUseVariable.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            checkBoxUseVariable.Visibility = Visibility.Hidden;
                        }

                        //Data bind text 
                        textBoxTextInput.Text = (string)_currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName;
                    }
                    else if (type == TraitDataType.TrueFalse)
                    {
                        textBoxTextInput.Visibility = Visibility.Hidden;
                        txtBoxNumberInput.Visibility = Visibility.Hidden;
                        comboBoxTrueFalse.Visibility = Visibility.Visible;
                        comboChoiceVariables.Visibility = Visibility.Hidden;

                        //Only show checkbox if there are actual variables to select
                        List<Trait> varList = _currentEntity.getPreviouslyBoundPrimitiveVariables(type, false, _currentlySelectedConstraint);
                        if (varList.Count > 0)
                        {
                            checkBoxUseVariable.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            checkBoxUseVariable.Visibility = Visibility.Hidden;
                        }

                        //Data bind true/false
                        comboBoxTrueFalse.SelectedIndex = ((bool)_currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName) ? 0 : 1;
                    }

                }

            }

            if (_currentlySelectedConstraint.AllowedToSave)
            {
                //bottom text and button
                if (_currentlySelectedConstraint.ContainsSavedVariable)
                {
                    textBlockSavedStatus.Visibility = Visibility.Visible;
                    textBlockSavedStatus.Text = "Value saved as: " + _currentlySelectedConstraint.SavedVariable.Name;
                    btSaveValButton.Content = "Edit this Variable ...";
                    btSaveValButton.Visibility = Visibility.Visible;
                }
                else
                {
                    textBlockSavedStatus.Visibility = Visibility.Hidden;
                    btSaveValButton.Content = "Save this Value ...";
                    btSaveValButton.Visibility = Visibility.Visible;
                }


            }
            else
            {
                textBlockSavedStatus.Visibility = Visibility.Hidden;
                btSaveValButton.Visibility = Visibility.Hidden;
            }

          

            syncDataBoundComboBoxes();
            _currentlyDataBinding = false;
        }
            
        private void syncDataBoundComboBoxes()
        {
            //Point to the right trait/relationship item in the list
             if(comboBoxTraitRelationship.SelectedIndex == 0)
             {
                 int count = 0;
                 int foundIndex = -1;
                 if (comboBoxSelectTraitRel.ItemsSource == null) { return; }
                 foreach(Object item in comboBoxSelectTraitRel.ItemsSource)
                 {
                     if (
                         (((Trait)item).Name == _currentlySelectedConstraint.ComparisonValue.Name) &&
                         (((Trait)item).Type == _currentlySelectedConstraint.ComparisonValue.Type)
                        )
                     { foundIndex = count; break; }

                     count++;
                 }
                 if(foundIndex > -1)
                 {
                     comboBoxSelectTraitRel.SelectedIndex = foundIndex;

                 }
                 else 
                 {
                     comboBoxSelectTraitRel.SelectedIndex = 0;
                 }
                 
             }
             else 
             {
                 int count = 0;
                 int foundIndex = -1;
                 if (comboBoxSelectTraitRel.ItemsSource == null) { return; }
                 foreach(Object item in comboBoxSelectTraitRel.ItemsSource)
                 {
                     if (
                         ((Relationship)item).Name == _currentlySelectedConstraint.ComparisonValue.Name
                        )
                     { foundIndex = count; break; }

                     count++;
                 }
                 if(foundIndex > -1)
                 {
                    comboBoxSelectTraitRel.SelectedIndex = foundIndex;
                                      
                 }
                 else 
                 {
                     comboBoxSelectTraitRel.SelectedIndex = 0;
                 }
             }

           if (_currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable) //Traits in combobox
            {
                //Point to the right trait/relationship item in the list

                int count = 0;
                int foundIndex = -1;
                if (comboChoiceVariables.ItemsSource == null) { return; }
                foreach (Object item in comboChoiceVariables.ItemsSource)
                {
                    if (
                        (((Trait)item).Name == (string)_currentlySelectedConstraint.ComparisonValue.LiteralValueOrBoundVarName) &&
                        (((Trait)item).Type == _currentlySelectedConstraint.ComparisonValue.Type)
                       )
                    { foundIndex = count; break; }

                    count++;
                }
                if(foundIndex > -1)
                {
                    comboChoiceVariables.SelectedIndex = foundIndex;
                                 
                }
                else 
                {
                    comboChoiceVariables.SelectedIndex = 0;
                }
            }

        }

        private void dataBindHeaderItems()
        {
            _currentlyDataBinding = true;


            _typeString = "";
            switch(_editingMode)
            {
                case 0:
                    
                    _typeString = "Character";
                    imageIconChar.Visibility = Visibility.Visible;
                    imageIconEnv.Visibility = Visibility.Hidden;
                    imageIconPlotPoints.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    _typeString = "Environment";
                    imageIconChar.Visibility = Visibility.Hidden;
                    imageIconEnv.Visibility = Visibility.Visible;
                    imageIconPlotPoints.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    _typeString = _ppType.Description;
                    imageIconChar.Visibility = Visibility.Hidden;
                    imageIconEnv.Visibility = Visibility.Hidden;
                    imageIconPlotPoints.Visibility = Visibility.Visible;
                    break;
            }



            titleTextBlock.Text = "Edit " + _typeString + " Precondition Statement";

            
            if(_currentEntity.ObjectExists)
            {
                textBlockHeader.Text = "This " + _typeString + " must exist";
                btChangeVarName.Visibility = Visibility.Visible;

               
            }
            else
            {
                textBlockHeader.Text = "This " + _typeString + " must not exist";
                btChangeVarName.Visibility = Visibility.Hidden;
            }

            if(_currentEntity.SaveMatchedObject)
            {
                btChangeVarName.Content = "Edit Variable Name ...";
                textBlockHeader.Text += ", and is saved as: " + _currentEntity.SaveObjectVariableName;
            }
            else
            {
                btChangeVarName.Content = "Save " + _typeString + " ...";
            }

            constraintDataGrid.ItemsSource = _currentEntity.Constraints;

          







            _currentlyDataBinding = false;
            
        }

        private void btNew_Click(object sender, RoutedEventArgs e)
        {

            TraitConstraint newCons = null;

            saveData();

            if(_editingMode == 0) //characters
            {
                newCons = new TraitConstraint(_currentEntity.ParentPlotFragmentId,
                    _currentEntity.Id,
                    _currentEntity.ObjectExists,
                    _currentStoryData.CharTypeId,
                    _currentStoryData);

                List<Character> globalChars = Utilities.getGlobalCharacterList(_currentStoryData);
                newCons.ComparisonValue.Name = globalChars[0].Traits[0].Name;
                newCons.ComparisonValue.Type = globalChars[0].Traits[0].Type;
                newCons.SavedVariable.Type = globalChars[0].Traits[0].Type;
            }
            else if(_editingMode == 1) //environments
            {
                newCons = new TraitConstraint(_currentEntity.ParentPlotFragmentId,
                    _currentEntity.Id,
                    _currentEntity.ObjectExists,
                    _currentStoryData.EnvTypeId,
                    _currentStoryData);
                List<Environment> globalEnv = Utilities.getGlobalEnvironmentList(_currentStoryData);
                newCons.ComparisonValue.Name = globalEnv[0].Traits[0].Name;
                newCons.ComparisonValue.Type = globalEnv[0].Traits[0].Type;
                newCons.SavedVariable.Type = globalEnv[0].Traits[0].Type;
            }
            else
            {

                newCons = new TraitConstraint(_currentEntity.ParentPlotFragmentId,
                    _currentEntity.Id,
                    _currentEntity.ObjectExists,
                    _ppType.Id,
                    _currentStoryData);

                newCons.ComparisonValue.Name = _ppType.Traits[0].Name;
                newCons.ComparisonValue.Type = _ppType.Traits[0].Type;
                newCons.SavedVariable.Type = _ppType.Traits[0].Type;
            }
            
           
            _currentEntity.Constraints.Add(newCons);



            clearHeaderElements();
            dataBindHeaderItems();

            constraintDataGrid.SelectedItem = _currentEntity.Constraints[_currentEntity.Constraints.Count - 1];
            constraintDataGrid_MouseUp(null, null);

        }

        private void btDelConstraint_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = constraintDataGrid.SelectedItem;
            if (itemToEdit == null) { return; }
            saveData();
            MessageBoxResult result = Utilities.MakeYesNoWarningDialog("Are you sure you want to delete this constraint?", "Confirm Deletion", this);

            if(result == MessageBoxResult.Yes)
            {
                _currentEntity.Constraints.Remove((Constraint)itemToEdit);
                _currentlySelectedConstraint = null;

                clearDataElements();
                dataBind();
                constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                constraintDataGrid_MouseUp(null, null);
            }
  
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
 
            this.Close();
        }

        private void constraintDataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
          

            Object selectedItem = constraintDataGrid.SelectedItem;
            if(selectedItem ==  null)
            {
                saveData();
                _currentlySelectedConstraint = null;
                clearDataElements();
                dataBind();
            }
            else
            {
                //Save before switching;
                saveData();
                _currentlySelectedConstraint = (Constraint)selectedItem;
                clearDataElements();
                dataBind();
                constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
         
            }

        }

        private void btChangeVarName_Click(object sender, RoutedEventArgs e)
        {
            string tempSavedName = _currentEntity.SaveObjectVariableName;
            _currentEntity.SaveObjectVariableName = "";

            int result = 0;

            if(_currentEntity.SaveMatchedObject == true)
            {
                //Already has a variable name stored, so ask them if they want to remove it or edit it
                List<string> editOrRemove = new List<string>();
                editOrRemove.Add("Edit variable name");
                
                editOrRemove.Add("Remove saved variable");
                result = Utilities.MakeListChoiceDialog("Would you like to edit the variable name or remove the variable completely?", editOrRemove, this);
            }

           
            if (result == 1)//remove variable
            {
                _currentEntity.SaveMatchedObject = false;

                clearDataElements();
                dataBind();
                constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                constraintDataGrid_MouseUp(null, null);
            } 
            else
            {
                //Either edit or create a new variable
                Action nullAction = null;
                List<string> prevVars = _parentPlotFragment.getAllPreviouslyBoundVariableNames(nullAction, true);

                string newVarName = Utilities.MakeConstrainedTextDialog("Please enter the name of the variable for this saved " + _typeString + ":", "That variable name is already in use", prevVars, this);
                if (newVarName != null)
                {
                    _currentEntity.SaveObjectVariableName = newVarName;
                    _currentEntity.SaveMatchedObject = true;

                    clearDataElements();
                    dataBind();
                    constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                    constraintDataGrid_MouseUp(null, null);
                }
                else
                {
                    _currentEntity.SaveObjectVariableName = tempSavedName;
                }
            }
          
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           // if (_currentEntity.Constraints.Count == 0)
            //{
            //    Utilities.MakeErrorDialog("This precondition statement must have at least one constraint in order for it to be valid.", this);
            //    e.Cancel = true;
                
           // }
           // else
           // {
                saveData();
           // }

            
        }

        private void btSaveValButton_Click(object sender, RoutedEventArgs e)
        {
            string tempSavedName = _currentlySelectedConstraint.SavedVariable.Name;
            _currentlySelectedConstraint.SavedVariable.Name = "";


            int result = 0;

            if((_currentlySelectedConstraint.ContainsSavedVariable) && (_currentlySelectedConstraint.ConstraintType != ConstraintComparisonType.None))
            {
                //Check to see if variable already exists to edit
                List<string> editOrRemove = new List<string>();
                editOrRemove.Add("Edit variable name");
                editOrRemove.Add("Remove saved variable");
                result = Utilities.MakeListChoiceDialog("Would you like to edit the variable name or remove the variable completely?", editOrRemove, this);

            }
           
            if (result == 1)//remove
            {
                _currentlySelectedConstraint.ContainsSavedVariable = false;
                clearDataElements();
                dataBind();
                constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                constraintDataGrid_MouseUp(null, null);
            }
            else
            {

                Action nullAction = null;
                List<string> prevVars = _parentPlotFragment.getAllPreviouslyBoundVariableNames(nullAction, true);

                string newVarName = Utilities.MakeConstrainedTextDialog("Please enter the name of the variable for this saved value:", "That variable name is already in use", prevVars, this);
                if (newVarName != null)
                {
                    _currentlySelectedConstraint.ContainsSavedVariable = true;
                    _currentlySelectedConstraint.SavedVariable.Name = newVarName;

                    clearDataElements();
                    dataBind();
                    constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                    constraintDataGrid_MouseUp(null, null);
                }
                else
                {
                    _currentlySelectedConstraint.SavedVariable.Name = tempSavedName;
                }
            }
        }

        private void comboBoxSelectTraitRel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentlyDataBinding) return;

            Object itemSelected = comboBoxSelectTraitRel.SelectedItem;
            if (itemSelected == null) { return; }


       
            if (comboBoxTraitRelationship.SelectedIndex == 0)
            {
               _currentlySelectedConstraint.ComparisonValue.Name = ((Trait)itemSelected).Name;
                _currentlySelectedConstraint.ComparisonValue.Type = ((Trait)itemSelected).Type;
                _currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable = false;
                _currentlySelectedConstraint.SavedVariable.Type = ((Trait)itemSelected).Type;
                //_currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.Equals;
            }
            else if (comboBoxTraitRelationship.SelectedIndex == 1)
            {
                _currentlySelectedConstraint.ComparisonValue.Name = ((Relationship)itemSelected).Name;
                _currentlySelectedConstraint.ComparisonValue.Type = TraitDataType.Text;
                _currentlySelectedConstraint.SavedVariable.Type = TraitDataType.Text;
                _currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable = false;
                // _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.Equals;
                ((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode = true;
            }

            else if (comboBoxTraitRelationship.SelectedIndex == 2)
            {
                _currentlySelectedConstraint.ComparisonValue.Name = ((Relationship)itemSelected).Name;
                _currentlySelectedConstraint.ComparisonValue.Type = TraitDataType.Number;
                _currentlySelectedConstraint.SavedVariable.Type = TraitDataType.Number;
                _currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable = false;
                //_currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.Equals;
                ((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode = false;
            }




            clearDataElements();
            dataBind();
            constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
            constraintDataGrid_MouseUp(null, null);
        }

        private void comboBoxTraitRelationship_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentlyDataBinding) return;

            if (comboBoxTraitRelationship.SelectedIndex < 0) { return; }


            //First check for no changes

            if (
                (_currentlySelectedConstraint is TraitConstraint) &&
                (comboBoxTraitRelationship.SelectedIndex == 0)
                )
            {
                return;
            }
            else if (
                (_currentlySelectedConstraint is RelationshipConstraint) &&
                (((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode == true) &&
                (comboBoxTraitRelationship.SelectedIndex == 1)
                )
            {
                return;
            }
            else if (
                (_currentlySelectedConstraint is RelationshipConstraint) &&
                (((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode == false) &&
                (comboBoxTraitRelationship.SelectedIndex == 2)
                )
            {
                return;
            }
          


           if(comboBoxTraitRelationship.SelectedIndex == 0) //Trait type constraint
           {
                if (_currentlySelectedConstraint is RelationshipConstraint)
                {
                    //convert relationship constraint to trait constraint
                    TraitConstraint newCons = new TraitConstraint((RelationshipConstraint)_currentlySelectedConstraint, _currentStoryData);
                    newCons.ComparisonValue.ValueIsBoundToVariable = false;

                    if(_editingMode == 0) //characters
                    {
                        List<Character> globalChars = Utilities.getGlobalCharacterList(_currentStoryData);
                        newCons.ComparisonValue.Name = globalChars[0].Traits[0].Name;
                        newCons.ComparisonValue.Type = globalChars[0].Traits[0].Type;
                        newCons.SavedVariable.Type = globalChars[0].Traits[0].Type;
                    }
                    else if(_editingMode == 1) //environments
                    {
                        List<Environment> globalEnv = Utilities.getGlobalEnvironmentList(_currentStoryData);
                        newCons.ComparisonValue.Name = globalEnv[0].Traits[0].Name;
                        newCons.ComparisonValue.Type = globalEnv[0].Traits[0].Type;
                        newCons.SavedVariable.Type = globalEnv[0].Traits[0].Type;
                    }
                    else
                    {
                        newCons.ComparisonValue.Name = _ppType.Traits[0].Name;
                        newCons.ComparisonValue.Type = _ppType.Traits[0].Type;
                        newCons.SavedVariable.Type = _ppType.Traits[0].Type;
                    }

                    // Replace constraint in the constraint list
                    _currentEntity.Constraints[
                        _currentEntity.Constraints.IndexOf(_currentlySelectedConstraint)]
                         = newCons;
                    clearDataElements();
                    dataBind();
                    constraintDataGrid.SelectedItem = newCons;
                    constraintDataGrid_MouseUp(null, null);
                    
                }
           }
           else if ((comboBoxTraitRelationship.SelectedIndex == 1) || (comboBoxTraitRelationship.SelectedIndex == 2)) //Relationship type constraint
           {
               if (_currentlySelectedConstraint is TraitConstraint)
               {
                   //convert trait constraint to rel constraint
                    RelationshipConstraint newCons = new RelationshipConstraint((TraitConstraint)_currentlySelectedConstraint, _currentStoryData);
                    if(_editingMode == 0) //characters
                    {
                        List<Character> globalChars = Utilities.getGlobalCharacterList(_currentStoryData);
                        newCons.ComparisonValue.Name = globalChars[0].Relationships[0].Name;
                        newCons.ComparisonValue.ValueIsBoundToVariable = false;
                        if(comboBoxTraitRelationship.SelectedIndex == 1)
                        {
                            newCons.ComparisonValue.Type = TraitDataType.Text;
                            newCons.SavedVariable.Type = TraitDataType.Text;
                            newCons.TargetNameMode = true; // relationship target name
                    
                        }
                        else
                        {
                            newCons.ComparisonValue.Type = TraitDataType.Number;
                            newCons.SavedVariable.Type = TraitDataType.Number;
                            newCons.TargetNameMode = false; // relationship strength
                        }
                    }
                    else if(_editingMode == 1) //environments
                    {
                        List<Environment> globalEnv = Utilities.getGlobalEnvironmentList(_currentStoryData);
                        newCons.ComparisonValue.Name = globalEnv[0].Relationships[0].Name;
                        newCons.ComparisonValue.ValueIsBoundToVariable = false;
                        if(comboBoxTraitRelationship.SelectedIndex == 1)
                        {
                            newCons.ComparisonValue.Type = TraitDataType.Text;
                            newCons.SavedVariable.Type = TraitDataType.Text;
                            newCons.TargetNameMode = true; // relationship target name
                    
                        }
                        else
                        {
   
                            newCons.ComparisonValue.Type = TraitDataType.Number;
                            newCons.SavedVariable.Type = TraitDataType.Number;
                            newCons.TargetNameMode = false; // relationship strength
                        }
                    }


                    // Replace constraint in the constraint list
                    _currentEntity.Constraints[
                        _currentEntity.Constraints.IndexOf(_currentlySelectedConstraint)]
                         = newCons;
                    clearDataElements();
                    dataBind();
                    constraintDataGrid.SelectedItem = newCons;
                    constraintDataGrid_MouseUp(null, null);

               }
               else if (
                        ((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode &&
                        (comboBoxTraitRelationship.SelectedIndex == 2)
                        )
               {
                   //Convert relationship target name constraint to strength constraint
                   ((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode = false;
                   ((RelationshipConstraint)_currentlySelectedConstraint).ComparisonValue.Type = TraitDataType.Number;
                   ((RelationshipConstraint)_currentlySelectedConstraint).SavedVariable.Type = TraitDataType.Number;
                   ((RelationshipConstraint)_currentlySelectedConstraint).ComparisonValue.ValueIsBoundToVariable = false;
                   //((RelationshipConstraint)_currentlySelectedConstraint).ContainsSavedVariable = false;
                   //((RelationshipConstraint)_currentlySelectedConstraint).ConstraintType = ConstraintComparisonType.Equals;
                   //((RelationshipConstraint)_currentlySelectedConstraint).SavedVariable.Name = "";
                   clearDataElements();
                   dataBind();
                   constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                   constraintDataGrid_MouseUp(null, null);
               }
               else if (
                        ((!(((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode) )) &&
                        (comboBoxTraitRelationship.SelectedIndex == 1)
                        )
               {
                   //Convert relationship strength constraint to target name constraint
                   ((RelationshipConstraint)_currentlySelectedConstraint).TargetNameMode = true;
                   ((RelationshipConstraint)_currentlySelectedConstraint).ComparisonValue.Type = TraitDataType.Text;
                   ((RelationshipConstraint)_currentlySelectedConstraint).SavedVariable.Type = TraitDataType.Text;
                   //((RelationshipConstraint)_currentlySelectedConstraint).ContainsSavedVariable = false;
                   ((RelationshipConstraint)_currentlySelectedConstraint).ComparisonValue.ValueIsBoundToVariable = false;
                   //((RelationshipConstraint)_currentlySelectedConstraint).ConstraintType = ConstraintComparisonType.Equals;
                   //((RelationshipConstraint)_currentlySelectedConstraint).SavedVariable.Name = "";
                   clearDataElements();
                   dataBind();
                   constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
                   constraintDataGrid_MouseUp(null, null);
               }

           }
          


    
        }

        private void comboBoxComparisonOp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentlyDataBinding) {  return;}

            
            switch (comboBoxComparisonOp.SelectedIndex)
            {
                case 0:
                    _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.Equals;
                    break;
                case 1:
                    _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.NotEquals;
                    break;
                case 2:
                    if (_currentlySelectedConstraint.ComparisonValue.Type == TraitDataType.Number)
                    {
                        _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.GreaterThan;
                    }
                    else
                    {
                        if(_currentlySelectedConstraint.SavedVariable.Name.Trim() == "")
                        {
                            
                            btSaveValButton_Click(null, null);

                            if (_currentlySelectedConstraint.SavedVariable.Name.Trim() != "")
                            {
                                _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.None;
                                if (_currentlySelectedConstraint.MustAlwaysBeTrue)
                                {
                                    _currentlySelectedConstraint.MustAlwaysBeTrue = false;
                                }
                            }
                        }
                        else
                        {
                            _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.None;
                            if (_currentlySelectedConstraint.MustAlwaysBeTrue)
                            {
                                _currentlySelectedConstraint.MustAlwaysBeTrue = false;
                            }
                        }

                       

                    }
                    
                    break;
                case 3:
                    _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.LessThan;
                    break;
                case 4:
                    _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.GreaterThanEqualTo;
                    break;
                case 5:
                    _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.LessThanEqualTo;
                    break;
                case 6:
                    
                    if (_currentlySelectedConstraint.SavedVariable.Name.Trim() == "")
                    {
                        btSaveValButton_Click(null, null);

                        if (_currentlySelectedConstraint.SavedVariable.Name.Trim() != "")
                        {
                            _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.None;
                            if (_currentlySelectedConstraint.MustAlwaysBeTrue)
                            {
                                _currentlySelectedConstraint.MustAlwaysBeTrue = false;
                            }
                        }
                    }
                    else
                    {
                        _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.None;
                       
                        if (_currentlySelectedConstraint.MustAlwaysBeTrue)
                        {
                            _currentlySelectedConstraint.MustAlwaysBeTrue = false;
                        }
                    }

                    break;
            }
            saveData();
            clearDataElements();
            dataBind();
           // constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
            //constraintDataGrid_MouseUp(null, null);

        }

        private void checkBoxUseVariable_Checked(object sender, RoutedEventArgs e)
        {
            if (_currentlyDataBinding) return;


            _currentlySelectedConstraint.ComparisonValue.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;

            
            clearDataElements();
            dataBind();
            constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
            constraintDataGrid_MouseUp(null, null);

        }

        private void checkBoxAlwaysTrue_Checked(object sender, RoutedEventArgs e)
        {
            if (_currentlyDataBinding) return;

            _currentlySelectedConstraint.MustAlwaysBeTrue = (bool)checkBoxAlwaysTrue.IsChecked;

            if(_currentlySelectedConstraint.MustAlwaysBeTrue)
            {
                //_currentlySelectedConstraint.AllowedToSave = false;
               // _currentlySelectedConstraint.ContainsSavedVariable = false;
                if(_currentlySelectedConstraint.ConstraintType == ConstraintComparisonType.None)
                {
                    _currentlySelectedConstraint.ConstraintType = ConstraintComparisonType.Equals;
                }
            }
            else
            {
                //_currentlySelectedConstraint.AllowedToSave = true;
                //_currentlySelectedConstraint.ContainsSavedVariable = true;
            }

            saveData();
            clearDataElements();
            dataBind();
            constraintDataGrid.SelectedItem = _currentlySelectedConstraint;
            constraintDataGrid_MouseUp(null, null);

        }

        private void txtBoxNumberInput_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                saveData();
                clearDataElements();
                dataBind();
                constraintDataGrid_MouseUp(null, null);
                txtBoxNumberInput.Focus();
            }
           
        }

        private void textBoxTextInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                saveData();
                clearDataElements();
                dataBind();
                constraintDataGrid_MouseUp(null, null);
                textBoxTextInput.Focus();
            }
           
        }

     

    }
}
