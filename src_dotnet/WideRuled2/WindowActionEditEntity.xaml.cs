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
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.ThemePack;

namespace WideRuled2
{
    /// <summary>
    /// Interaction logic for WindowActionEditEntity.xaml
    /// </summary>
    public partial class WindowActionEditEntity : Window
    {

        private int _editingMode;
        //0 = character
        //1 = environment
        //2 = plot point
        private PlotPointType _ppType; //only valid if in pp editing mode
        private ActionEditObject _currentEntity;
        private StoryData _currentStoryData;
        private PlotFragment _parentPlotFragment;
        private bool _currentlyDataBinding;

        public WindowActionEditEntity(PlotFragment frag, ActionEditObject currEntity, StoryData currStoryData)
        {
            InitializeComponent();
            _currentEntity = currEntity;
            _currentStoryData = currStoryData;
            _parentPlotFragment = frag;

            _ppType = null;
            _currentlyDataBinding = false;

            //Find editing type
            if (_currentEntity.ObjectTypeId == currStoryData.CharTypeId)
            {
                _editingMode = 0;
            }
            else if (_currentEntity.ObjectTypeId == currStoryData.EnvTypeId)
            {
                _editingMode = 1;
            }
            else
            {
                PlotPointType currType = currStoryData.findPlotPointTypeById(_currentEntity.ObjectTypeId);

                _editingMode = 2;
                _ppType = currType;

            }

            txtBoxNumberInput.NullValue = 0.0;
            dataBind();

        }

        private void dataBind()
        {
            _currentlyDataBinding = true;

            switch (_editingMode)
            {
                case 0:
                    titleTextBlock.Text = "Edit Character: " + _currentEntity.VariableObjectName;
                    break;
                case 1:
                    titleTextBlock.Text = "Edit Environment: " + _currentEntity.VariableObjectName;
                    break;
                case 2:
                    titleTextBlock.Text = "Edit " + _ppType.Description + ": " + _currentEntity.VariableObjectName;
                    break;
            }

            //Top combobox
            List<string> attributeOptions = new List<string>();
            attributeOptions.Add("Edit Trait");

            //Only add relationship editors if there relationships that can be edited
            if((_editingMode == 0) && (Utilities.getGlobalCharacterList(_currentStoryData)[0].Relationships.Count > 0))
            {
                
                attributeOptions.Add("Edit Relationship Target");
                attributeOptions.Add("Edit Relationship Strength");
            }
            else if ((_editingMode == 1) && (Utilities.getGlobalEnvironmentList(_currentStoryData)[0].Relationships.Count > 0))
            {

                attributeOptions.Add("Edit Relationship Target");
                attributeOptions.Add("Edit Relationship Strength");
            }
            
            comboBoxTraitRelationship.ClearValue(ComboBox.ItemsSourceProperty);
            comboBoxTraitRelationship.Items.Clear();
            comboBoxTraitRelationship.ItemsSource = attributeOptions;

            switch(_currentEntity.Mode)
            {
                case ObjectEditingMode.Trait:
                    comboBoxTraitRelationship.SelectedIndex = 0;
                    break;
                case ObjectEditingMode.RelationshipTarget:
                    comboBoxTraitRelationship.SelectedIndex = 1;
                    break;
                case ObjectEditingMode.RelationshipStrength:
                    comboBoxTraitRelationship.SelectedIndex = 2;
                    break;
            }

            //Left combobox

            if(_currentEntity.Mode == ObjectEditingMode.Trait)
            {
                if(_editingMode == 0)
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
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget)
            {
                if (_editingMode == 0)
                {                                       // Using get GLOBAL list (includes not just static characters, but ones
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
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipStrength)
            {
                if (_editingMode == 0)
                {
                    comboBoxSelectTraitRel.ItemsSource = Utilities.getGlobalCharacterList(_currentStoryData)[0].Relationships;
                }
                else if (_editingMode == 1)
                {
                    comboBoxSelectTraitRel.ItemsSource = Utilities.getGlobalEnvironmentList(_currentStoryData)[0].Relationships;
                }
            }

            //checkbox
            if (_currentEntity.Mode == ObjectEditingMode.Trait)
            {
                checkBoxUseVariable.IsChecked = _currentEntity.NewValue.ValueIsBoundToVariable;
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget)
            {
                checkBoxUseVariable.IsChecked = _currentEntity.NewTarget.ValueIsBoundToVariable;
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipStrength)
            {
                checkBoxUseVariable.IsChecked = _currentEntity.NewValue.ValueIsBoundToVariable;
            }

            //Value boxes
            if (_currentEntity.Mode == ObjectEditingMode.Trait)
            {
                if(_currentEntity.NewValue.ValueIsBoundToVariable)
                {
                    textBoxTextInput.Visibility = Visibility.Hidden;
                    txtBoxNumberInput.Visibility = Visibility.Hidden;
                    comboBoxTrueFalse.Visibility = Visibility.Hidden;
                    comboChoiceVariables.Visibility = Visibility.Visible;
                    checkBoxUseVariable.Visibility = Visibility.Visible;

                    //Data bind variable list
                    comboChoiceVariables.ItemsSource = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.NewValue.Type, false, _currentEntity);
                    comboChoiceVariables.ItemTemplate = this.FindResource("comboBoxDataTemplate") as DataTemplate;


                }
                else
                {
                    TraitDataType type = _currentEntity.NewValue.Type;
                    if(type == TraitDataType.Number)
                    {
                        textBoxTextInput.Visibility = Visibility.Hidden;
                        txtBoxNumberInput.Visibility = Visibility.Visible;
                        comboBoxTrueFalse.Visibility = Visibility.Hidden;
                        comboChoiceVariables.Visibility = Visibility.Hidden;
                       
                        //Only show checkbox if there are actual variables to select
                        List<Trait> varList = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.NewValue.Type, false, _currentEntity);
                        if (varList.Count > 0)
                        {
                            checkBoxUseVariable.Visibility = Visibility.Visible;
                        }
                        else 
                        {
                            checkBoxUseVariable.Visibility = Visibility.Hidden;
                        }
                        
                        //Data bind Number 
                        txtBoxNumberInput.Value = (double)_currentEntity.NewValue.LiteralValueOrBoundVarName;

                    }
                    else if (type == TraitDataType.Text)
                    {
                        textBoxTextInput.Visibility = Visibility.Visible;
                        txtBoxNumberInput.Visibility = Visibility.Hidden;
                        comboBoxTrueFalse.Visibility = Visibility.Hidden;
                        comboChoiceVariables.Visibility = Visibility.Hidden;

                        //Only show checkbox if there are actual variables to select
                        List<Trait> varList = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.NewValue.Type, false, _currentEntity);
                        if (varList.Count > 0)
                        {
                            checkBoxUseVariable.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            checkBoxUseVariable.Visibility = Visibility.Hidden;
                        }

                        //Data bind text 
                        textBoxTextInput.Text = (string)_currentEntity.NewValue.LiteralValueOrBoundVarName;
                    }
                    else if (type == TraitDataType.TrueFalse)
                    {
                        textBoxTextInput.Visibility = Visibility.Hidden;
                        txtBoxNumberInput.Visibility = Visibility.Hidden;
                        comboBoxTrueFalse.Visibility = Visibility.Visible;
                        comboChoiceVariables.Visibility = Visibility.Hidden;

                        //Only show checkbox if there are actual variables to select
                        List<Trait> varList = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.NewValue.Type, false, _currentEntity);
                        if (varList.Count > 0)
                        {
                            checkBoxUseVariable.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            checkBoxUseVariable.Visibility = Visibility.Hidden;
                        }

                        //Data bind true/false
                        comboBoxTrueFalse.SelectedIndex = ((bool)_currentEntity.NewValue.LiteralValueOrBoundVarName) ? 0 : 1;
                    }
                }
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget)
            {
                textBoxTextInput.Visibility = Visibility.Hidden;
                txtBoxNumberInput.Visibility = Visibility.Hidden;
                comboBoxTrueFalse.Visibility = Visibility.Hidden;
                comboChoiceVariables.Visibility = Visibility.Visible;
                checkBoxUseVariable.Visibility = Visibility.Hidden;

                //Data bind variable list
                if (_editingMode == 0)
                {
                    comboChoiceVariables.ItemsSource = _parentPlotFragment.getPreviouslyBoundCharacterVarNames(_currentEntity);
                    comboChoiceVariables.ItemTemplate = this.FindResource("comboBoxDataTemplateString") as DataTemplate;

                }
                else if (_editingMode == 1)
                {
                    comboChoiceVariables.ItemsSource = _parentPlotFragment.getPreviouslyBoundEnvironmentVarNames(_currentEntity);
                    comboChoiceVariables.ItemTemplate = this.FindResource("comboBoxDataTemplateString") as DataTemplate;

                }

            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipStrength)
            {
                if (_currentEntity.NewValue.ValueIsBoundToVariable)
                {
                    textBoxTextInput.Visibility = Visibility.Hidden;
                    txtBoxNumberInput.Visibility = Visibility.Hidden;
                    comboBoxTrueFalse.Visibility = Visibility.Hidden;
                    comboChoiceVariables.Visibility = Visibility.Visible;
                    checkBoxUseVariable.Visibility = Visibility.Visible;

                    //Data bind variable list
                    comboChoiceVariables.ItemsSource = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.NewValue.Type, false, _currentEntity);
                    comboChoiceVariables.ItemTemplate = this.FindResource("comboBoxDataTemplate") as DataTemplate;

          
                 

                }
                else
                {
              

                    textBoxTextInput.Visibility = Visibility.Hidden;
                    txtBoxNumberInput.Visibility = Visibility.Visible;
                    comboBoxTrueFalse.Visibility = Visibility.Hidden;
                    comboChoiceVariables.Visibility = Visibility.Hidden;

                    //Only show checkbox if there are actual variables to select
                    List<Trait> varList = _parentPlotFragment.getPreviouslyBoundPrimitiveVariables(_currentEntity.NewValue.Type, false, _currentEntity);
                    if (varList.Count > 0)
                    {
                        checkBoxUseVariable.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        checkBoxUseVariable.Visibility = Visibility.Hidden;
                    }
                    //Data bind Number 
                    txtBoxNumberInput.Value = (double)_currentEntity.NewValue.LiteralValueOrBoundVarName;

                }

            }



            syncDataBoundComboBoxes();
            _currentlyDataBinding = false;
        }

        private void syncDataBoundComboBoxes()
        {
            //Point to the right trait/relationship item in the list
             if(_currentEntity.Mode == ObjectEditingMode.Trait)
             {
                 int count = 0;
                 int foundIndex = -1;
                 foreach(Object item in comboBoxSelectTraitRel.ItemsSource)
                 {
                     if (
                         (((Trait)item).Name == _currentEntity.NewValue.Name) &&
                         (((Trait)item).Type == _currentEntity.NewValue.Type)
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
                 foreach(Object item in comboBoxSelectTraitRel.ItemsSource)
                 {
                     if (
                         ((Relationship)item).Name == _currentEntity.NewValue.Name
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

            //Right side combobox (variable bindings)
            if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget) //Strings in combobox
            {
                int count = 0;
                int foundIndex = -1;
                foreach (Object item in comboChoiceVariables.ItemsSource)
                {
                    if (
                        (string)item == (string)_currentEntity.NewTarget.LiteralValueOrBoundVarName
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
            else if (_currentEntity.NewValue.ValueIsBoundToVariable) //Traits in combobox
            {
                //Point to the right trait/relationship item in the list

                int count = 0;
                int foundIndex = -1;
                foreach (Object item in comboChoiceVariables.ItemsSource)
                {
                    if (
                        (((Trait)item).Name == (string)_currentEntity.NewValue.LiteralValueOrBoundVarName) &&
                        (((Trait)item).Type == _currentEntity.NewValue.Type)
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
        private void clearDataElements()
        {
            _currentlyDataBinding = true;
            comboBoxTraitRelationship.ClearValue(DataGridControl.ItemsSourceProperty);
            comboBoxTraitRelationship.Items.Clear();

            comboBoxSelectTraitRel.ClearValue(DataGridControl.ItemsSourceProperty);
            comboBoxSelectTraitRel.Items.Clear();

            comboChoiceVariables.ClearValue(DataGridControl.ItemsSourceProperty);
            comboChoiceVariables.Items.Clear();
            _currentlyDataBinding = false;
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        private void comboBoxTraitRelationship_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentlyDataBinding) return;

            if (comboBoxTraitRelationship.SelectedIndex < 0) { return; }

            switch (comboBoxTraitRelationship.SelectedIndex)
            {
                case 0:
                    _currentEntity.Mode = ObjectEditingMode.Trait;
                    break;
                case 1:
                    _currentEntity.Mode = ObjectEditingMode.RelationshipTarget;
                    break;
                case 2:
                    _currentEntity.Mode = ObjectEditingMode.RelationshipStrength;
                    break;
            }
            clearDataElements();
            dataBind();
        }

        private void comboBoxSelectTraitRel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentlyDataBinding) return;

            Object itemSelected = comboBoxSelectTraitRel.SelectedItem;
            if (itemSelected == null) { return; }


            if (_currentEntity.Mode == ObjectEditingMode.Trait)
            {
                _currentEntity.NewValue.Name = ((Trait)itemSelected).Name;
                _currentEntity.NewValue.Type = ((Trait)itemSelected).Type;
                _currentEntity.NewValue.ValueIsBoundToVariable = false;
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget)
            {
                _currentEntity.NewValue.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewTarget.Name = ((Relationship)itemSelected).Name;
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipStrength)
            {
                _currentEntity.NewValue.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewTarget.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewValue.ValueIsBoundToVariable = false;
            }

            clearDataElements();
            dataBind();
        }

        private void saveData()
        {
            //Save trait/rel name and type if necessary
            Object itemSelected = comboBoxSelectTraitRel.SelectedItem;
            if (_currentEntity.Mode == ObjectEditingMode.Trait)
            {
                _currentEntity.NewValue.Name = ((Trait)itemSelected).Name;
                _currentEntity.NewValue.Type = ((Trait)itemSelected).Type;
                _currentEntity.NewValue.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;
                TraitDataType type = _currentEntity.NewValue.Type;
                if (_currentEntity.NewValue.ValueIsBoundToVariable)
                {
                        _currentEntity.NewValue.LiteralValueOrBoundVarName = ((Trait)comboChoiceVariables.SelectedItem).Name;
                }
                else if (type == TraitDataType.Number)
                {

                    _currentEntity.NewValue.LiteralValueOrBoundVarName = txtBoxNumberInput.Value;
                   
                }
                else if (type == TraitDataType.Text)
                {
                    _currentEntity.NewValue.LiteralValueOrBoundVarName = textBoxTextInput.Text;
                }
                else if (type == TraitDataType.TrueFalse)
                {

                    _currentEntity.NewValue.LiteralValueOrBoundVarName = (comboBoxTrueFalse.SelectedIndex == 0);
                }
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget)
            {
                _currentEntity.NewValue.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewTarget.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewTarget.LiteralValueOrBoundVarName = (string)comboChoiceVariables.SelectedItem;

            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipStrength)
            {
                _currentEntity.NewValue.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewTarget.Name = ((Relationship)itemSelected).Name;
                _currentEntity.NewValue.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;
                if (_currentEntity.NewValue.ValueIsBoundToVariable)
                {
                    _currentEntity.NewValue.LiteralValueOrBoundVarName = ((Trait)comboChoiceVariables.SelectedItem).Name;
                }
                else
                {
                    _currentEntity.NewValue.LiteralValueOrBoundVarName = txtBoxNumberInput.Value;
                }
                
 
            }

            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            saveData();
        }

        private void checkBoxUseVariable_Checked(object sender, RoutedEventArgs e)
        {
            if (_currentlyDataBinding) return;

            if (_currentEntity.Mode == ObjectEditingMode.Trait)
            {
                _currentEntity.NewValue.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipStrength)
            {
                _currentEntity.NewValue.ValueIsBoundToVariable = (bool)checkBoxUseVariable.IsChecked;
            }
            else if (_currentEntity.Mode == ObjectEditingMode.RelationshipTarget)
            {
               
            }

            clearDataElements();
            dataBind();
        }
    }
}
