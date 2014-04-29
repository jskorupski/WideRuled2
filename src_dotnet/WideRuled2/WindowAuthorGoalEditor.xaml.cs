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
    /// Interaction logic for AuthorGoalEditorWindow.xaml
    /// </summary>
    public partial class WindowAuthorGoalEditor : Window
    {
        private AuthorGoal _currentEntity;
        private StoryData _currentStoryData;
        private List<Parameter> _newList;
        private bool _isStartGoal;

        public WindowAuthorGoalEditor(AuthorGoal currEntity, StoryData currStoryData)
        {

            InitializeComponent();
            _currentEntity = currEntity;
            _currentStoryData = currStoryData;
            _newList = new List<Parameter>();
            _isStartGoal = (_currentStoryData.StartGoalId == _currentEntity.Id);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Exiting window is same as canceling
            //if (!dataValid()) e.Cancel = true;
            //traitDataGrid.EndEdit();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(Parameter param in _currentEntity.Parameters)
            {
                _newList.Add((Parameter)param.Clone());
            }
            //_newList.AddRange(_currentEntity.Parameters);
            dataBind();

        }

        private void dataBind()
        {
            titleTextBlock.Text = _currentEntity.Name.ToString();
            checkBoxStartGoal.IsChecked = _isStartGoal;
            traitDataGrid.ItemsSource = _newList;
            traitDataGrid.Columns["TypeString"].CellEditor = this.FindResource("entityDataTypeEditor") as CellEditor;
            traitDataGrid.Columns["TypeString"].CellContentTemplate = this.FindResource("entityDataTypeCellDataTemplate") as DataTemplate;


        }
        private void clearDataGrids()
        {

            traitDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            traitDataGrid.Items.Clear();
        }
        private void btAddNew_Click(object sender, RoutedEventArgs e)
        {
            traitDataGrid.EndEdit();
            _newList.Add(new Parameter("NewParameter", TraitDataType.Text, false, _currentStoryData));
            clearDataGrids();
            dataBind();
        }

        private void btDeleteSelected_Click(object sender, RoutedEventArgs e)
        {


            if ((traitDataGrid.SelectedItems.Count == 0) || (traitDataGrid.SelectedItems[0].GetType() != typeof(Parameter))) { return; }
            traitDataGrid.EndEdit();

            foreach (Object item in traitDataGrid.SelectedItems)
            {

                _newList.Remove((Parameter)item);

            }

            clearDataGrids();
            dataBind();
        }

        private bool dataValid()
        {
            //Check for duplicate parameter names
            foreach (Parameter param1 in _newList)
            {


                foreach (Parameter param2 in _newList)
                {
                    if ((param1 != param2) && (param1.Name.Equals(param2.Name)))
                    {
                        Utilities.MakeErrorDialog("Duplicate parameter names are not allowed. Please choose unique names for each.",this);
                        return false;
                    }

                }
            }

            //Make sure all parameters do not conflict with existing variable names in child plot fragments -
            //This check is needed here, because modifying an author goal's parameter list can introduce new variables
            //into the scope of the child plot fragments

            
            foreach(PlotFragment currFrag in _currentEntity.PlotFragments)
            {
                //Get all variables in plot fragment (by asking for all variables bound after actions, 
                //which are the last Plot Fragment elements to contain bindings
                Action nullAction = null;
                List<string> fragScopeVars = currFrag.getAllPreviouslyBoundVariableNames(nullAction, false);
                foreach(Parameter param in _newList)
                {
                    foreach(String fragVariable in fragScopeVars)
                    {
                        if(param.Name.Equals(fragVariable))
                        {
                            Utilities.MakeErrorDialog("New parameter '" +  
                                            param.Name + 
                                            "' conflicts with saved variable of same name in Plot Fragment '" + 
                                            currFrag.Name + 
                                            "'. Please change the name of this parameter or remove it from the Plot Fragment before adding that parameter here.", 
                                            this);
                            return false;
                        }
                    }
                }

            }
            ;

            return true;
        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            traitDataGrid.EndEdit();
            if (dataValid())
            {
                if(_isStartGoal)
                {
                    _currentStoryData.StartGoalId = _currentEntity.Id;
                }
                Utilities.SaveAuthorGoalParams(_currentEntity, _newList, _currentStoryData);
                this.Close();
            }

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Closing -= new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Close();
        }

        private void checkBoxStartGoal_Checked(object sender, RoutedEventArgs e)
        {
            if((bool)checkBoxStartGoal.IsChecked)
            {
                _isStartGoal = true;
            }
            else
            {
                _isStartGoal = false;
            }
            
        }

        private void btEditGoalName_Click(object sender, RoutedEventArgs e)
        {
            // Get name
            List<string> choiceConstraints = new List<string>();
            

            foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
            {
                if (goal != _currentEntity) //Allow user to name this author goal to the old name, so don't add the current name to the list
                {

                    choiceConstraints.Add(goal.Name);
                }


            }

            string newName = Utilities.MakeConstrainedTextDialog(
                "Please enter a new name for this Author Goal:",
                "That name is already in use by another Author Goal.",
                choiceConstraints, this);

            if (newName == null)
            {
                return;
            }

            _currentEntity.Name = newName;
            clearDataGrids();
            dataBind();
        }


    }
}