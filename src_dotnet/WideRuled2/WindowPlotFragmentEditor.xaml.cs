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
    /// Interaction logic for PlotFragmentEditorWindow.xaml
    /// </summary>
    public partial class WindowPlotFragmentEditor : Window
    {
        private PlotFragment _currentEntity;
        private StoryData _currentStoryData;
        private AuthorGoal _parentGoal;

        public WindowPlotFragmentEditor(PlotFragment currEntity, StoryData currStoryData)
        {

            InitializeComponent();
            _currentEntity = currEntity;
            _currentStoryData = currStoryData;
            _parentGoal = currStoryData.findAuthorGoalById(currEntity.ParentAuthorGoalId);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataBind();
        }
        private void dataBind()
        {

            if (_currentEntity == null)
            {
                return;
            }

            textBlockAuthGoalName.Text = _parentGoal.Name;
            textBlockFrag.Text = _currentEntity.Name;
            paramDataGrid.ItemsSource = _parentGoal.Parameters;
            precondDataGrid.ItemsSource = _currentEntity.PrecStatements;
            actionsDataGrid.ItemsSource = _currentEntity.Actions;




        }

        private void clearDataBindings()
        {
            paramDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            paramDataGrid.Items.Clear();
            precondDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            precondDataGrid.Items.Clear();
            actionsDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            actionsDataGrid.Items.Clear();

        }

        private void btChangeAuthGoal_Click(object sender, RoutedEventArgs e)
        {
            int oldIndex = 0;
            int count = 0;
            List<string> choiceList = new List<string>();
            foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
            {
                choiceList.Add(goal.Description);
                if(goal.Id == _currentEntity.ParentAuthorGoalId)
                {
                    oldIndex = count;
                }
                count++;
            }
            
            int result = Utilities.MakeListChoiceDialog("Select a parent Author Goal", choiceList, this);
            if(result > -1)
            {
                if(result != oldIndex)
                {
                    _currentStoryData.AuthorGoals[oldIndex].PlotFragments.Remove(_currentEntity);

                    _parentGoal = _currentStoryData.AuthorGoals[result];
                    _parentGoal.PlotFragments.Add(_currentEntity);

                    _currentEntity.ParentAuthorGoalId = _parentGoal.Id;

                    dataBind();
                }


            }
        }

        private void btNewPrecond_Click(object sender, RoutedEventArgs e)
        {
            List<string> newChoices = new List<string>();
            newChoices.Add("Character Constraint");
            newChoices.Add("Environment Constraint");

            foreach (PlotPointType pp in _currentStoryData.PlotPointTypes)
            {
                newChoices.Add(pp.Description + " Constraint");
            }

            int result = -1;

            result = Utilities.MakeListChoiceDialog("What type of constraint would you like to create?", newChoices, this);

  
            if(result < 0)
            {
                return;
            }


            //ASK FOR EXISTENCE OR NONEXISTENCE - THIS CANNOT BE CHANGED IN THE EDITOR
            //changes would result in the editor having to wipe out variable saves within constraints and generally it would be extra work
            //to maintain

            List<string> existNonExistChoices = new List<string>();
            existNonExistChoices.Add("The object must exist");
            existNonExistChoices.Add("The object must NOT exist");

            int resultExistence = Utilities.MakeListChoiceDialog("Would like the object you are matching to exist or not exist in the story world?", existNonExistChoices, this);


            if (resultExistence < 0)
            {
                return;
            }

            bool objectExists = (resultExistence == 0);

            if (result == 0) //Character
            {
                if(Utilities.getGlobalCharacterList(_currentStoryData).Count == 0)
                {
                    Utilities.MakeErrorDialog("Please make a new Character, either in the Character editor, or within a Create Object action, before creating Character-matching precondition statements.", this);
                    return;
                }
                PreconditionStatementCharacter newPrecond = new PreconditionStatementCharacter(_currentEntity.Id, _currentStoryData);
                newPrecond.ObjectExists = objectExists;
                _currentEntity.PrecStatements.Add(newPrecond);
                clearDataBindings();
                dataBind();

                Window newWin = new WindowPreconditionEditor(_currentEntity, newPrecond, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
          
            }
            else if (result == 1) //Environment
            {
                if (Utilities.getGlobalEnvironmentList(_currentStoryData).Count == 0)
                {
                    Utilities.MakeErrorDialog("Please make a new Environment, either in the Environment editor, or within a Create Environment action, before creating Environment-matching precondition statements.", this);
                    return;
                }
                PreconditionStatementEnvironment newPrecond = new PreconditionStatementEnvironment(_currentEntity.Id, _currentStoryData);
                newPrecond.ObjectExists = objectExists;
                _currentEntity.PrecStatements.Add(newPrecond);
                clearDataBindings();
                dataBind();

                Window newWin = new WindowPreconditionEditor(_currentEntity, newPrecond, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
     
            }
            else if (result > 1) //PlotPointType
            {
                int plotPointIndex = result - 2;
                PlotPointType currType = _currentStoryData.PlotPointTypes[plotPointIndex];

                if(currType.Traits.Count == 0)
                {
                    Utilities.MakeErrorDialog("The " + currType.Description + " type has no traits to save or compare, and therefore cannot be used in a Precondition statement", this);
                    return;
                }
                PreconditionStatementPlotPoint newPrecond = new PreconditionStatementPlotPoint(_currentEntity.Id, currType, _currentStoryData);
                newPrecond.ObjectExists = objectExists;
                _currentEntity.PrecStatements.Add(newPrecond);
                clearDataBindings();
                dataBind();
                Window newWin = new WindowPreconditionEditor(_currentEntity, newPrecond, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
           

            }
            clearDataBindings();
            dataBind();
        }

        private void btEditPrecond_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = precondDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is PreconditionStatement)) { return; }

           
            if(itemToEdit.GetType() == typeof(PreconditionStatementCharacter))
            {
                Window newWin = new WindowPreconditionEditor(_currentEntity, (PreconditionStatementCharacter)itemToEdit, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();

            }

            else if(itemToEdit.GetType() == typeof(PreconditionStatementEnvironment))
            {
                Window newWin = new WindowPreconditionEditor(_currentEntity, (PreconditionStatementEnvironment)itemToEdit, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();

            }

            else if (itemToEdit.GetType() == typeof(PreconditionStatementPlotPoint))
            {
                PreconditionStatementPlotPoint ppPrecond = (PreconditionStatementPlotPoint)itemToEdit;
                if(null == _currentStoryData.findPlotPointTypeById(ppPrecond.MatchTypeId))
                {
                    //Very bad - precondition statement has no associated type. The user
                    //is not allowed to edit this, and must delete it 
                    Utilities.MakeErrorDialog("This Precondition Statement refers to a Plot Point Type that no longer exists. You cannot edit its contents. Please delete it in order to generate a story.", this);
                    return;
                }

                Window newWin = new WindowPreconditionEditor(_currentEntity, ppPrecond, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
           

            }
            clearDataBindings();
            dataBind();
        }

        private void btDeletePrecond_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = precondDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is PreconditionStatement)) { return; }

            PreconditionStatement itemToDelete = (PreconditionStatement)itemToEdit;

            if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete this Precondition Statement?", "Confirm Deletion", this) == MessageBoxResult.No)
            {
                return;
            }

            _currentEntity.PrecStatements.Remove(itemToDelete);
            clearDataBindings();
            dataBind();
        }

        private void btPrecondMoveUp_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = precondDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is PreconditionStatement)) { return; }

            Utilities.MoveItemUp(
                 _currentEntity.PrecStatements,
                  itemToEdit);

            clearDataBindings();
            dataBind();
            precondDataGrid.SelectedItem = itemToEdit;
        }

        private void btPrecondMoveDown_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = precondDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is PreconditionStatement)) { return; }

            Utilities.MoveItemDown(
                 _currentEntity.PrecStatements,
                  itemToEdit);

            clearDataBindings();
            dataBind();
            precondDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveDownAction_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = actionsDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is Action)) { return; }

            Utilities.MoveItemDown(
                 _currentEntity.Actions,
                  itemToEdit);

            clearDataBindings();
            dataBind();
            actionsDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveUpAction_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = actionsDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is Action)) { return; }

            Utilities.MoveItemUp(
                 _currentEntity.Actions,
                  itemToEdit);

            clearDataBindings();
            dataBind();
            actionsDataGrid.SelectedItem = itemToEdit;
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      

        private void btNewAction_Click(object sender, RoutedEventArgs e)
        {
            List<string> editChoices = new List<string>();
            editChoices.Add("Output Text");
            editChoices.Add("Pursue a Subgoal");
            editChoices.Add("Make a Calculation");
            editChoices.Add("Create a new Character/Environment/Plot Point");
            editChoices.Add("Edit a saved Character/Environment/Plot Point");
            editChoices.Add("Delete a saved Character/Environment/Plot Point");

            int result = -1;

            result = Utilities.MakeListChoiceDialog("What type of Action would you like to create?", editChoices, this);

            if (result < 0)
            {
                return;
            }
            else if (result == 0) //Output Text
            {
                ActionTextOutput newTextOutputAction = new ActionTextOutput(_currentEntity.Id, "", _currentStoryData);
                _currentEntity.Actions.Add(newTextOutputAction);
                Window newWin = new WindowActionTextOutput(_currentEntity, newTextOutputAction);
                newWin.Owner = this;
                newWin.ShowDialog();

            }
            else if (result == 1) //Pursue Subgoal
            {
                List<string> choiceList = new List<string>();
                foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
                {
                    choiceList.Add(goal.Description);
                }
                result = Utilities.MakeListChoiceDialog("Select an Author Goal to pursue", choiceList, this);
                if (result > -1)
                {
                    AuthorGoal goalToPursue = _currentStoryData.AuthorGoals[result];

                    ActionSubgoal newAction = new ActionSubgoal(_currentEntity.Id, goalToPursue.Id, _currentStoryData);
                    _currentEntity.Actions.Add(newAction);

                    if(goalToPursue.Parameters.Count > 0)
                    {
                        Window newWin = new WindowActionSubgoalEditor(false, _currentEntity, newAction, _currentStoryData);
                        newWin.Owner = this;
                        newWin.ShowDialog();
                    }
                }

            }
            else if (result == 2) //Make a calculation
            {
                Action nullAction = null;
                List<string> existingVarNames = _currentEntity.getAllPreviouslyBoundVariableNames(nullAction, true);
               

                string varName = Utilities.MakeConstrainedTextDialog(
                    "Please enter a name for the variable that will store the data in this calculation:", 
                    "That variable name has already been used.", 
                    existingVarNames, 
                    this);

                if(varName != null)
                {
                    ActionCalculation newCalc = new ActionCalculation(_currentEntity.Id, varName, _currentStoryData);
                    _currentEntity.Actions.Add(newCalc);

                    Window newWin = new WindowActionCalculationEditor(_currentEntity, newCalc, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();
  

                }
            }
            else if (result == 3) //Create a new object
            {
                List<string> newObjectChoices = new List<string>();
                newObjectChoices.Add("Character");
                newObjectChoices.Add("Environment");

                foreach (PlotPointType pp in _currentStoryData.PlotPointTypes)
                {
                    newObjectChoices.Add(pp.Description);
                }


                int resultCreateObject = Utilities.MakeListChoiceDialog("What type of object would you like to create?", newObjectChoices, this);

                if (resultCreateObject < 0) { return; }
                

                Action nullAction = null;
                List<string> existingVarNames = _currentEntity.getAllPreviouslyBoundVariableNames(nullAction, true);


                string varName = Utilities.MakeConstrainedTextDialog(
                    "Please enter a name for the variable that will store your new Object",
                    "That variable name has already been used.",
                    existingVarNames,
                    this);

                if(varName == null) return;

                if(resultCreateObject == 0)
                {


                    string entityName = Utilities.MakeTextDialog("Please enter a name for the new Character:", this);

                    if (entityName == null) return;

                    ActionCreateCharacter newCreateEntityAction = new ActionCreateCharacter(_currentEntity.Id, entityName, varName, _currentStoryData);


                    _currentEntity.Actions.Add(newCreateEntityAction);

                    //Synchronize all traits and relationships
                    if (_currentStoryData.Characters.Count > 1)
                    {
                        Utilities.SynchronizeTwoCharacters(_currentStoryData.Characters[0], newCreateEntityAction.NewCharacter, _currentStoryData);
                    }
                    clearDataBindings();
                    dataBind();

                    Window newWin = new WindowCharacterEditor(newCreateEntityAction.NewCharacter, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();


                }
                else if(resultCreateObject == 1)
                {

                    string entityName = Utilities.MakeTextDialog("Please enter a name for the new Environment:", this);

                    if (entityName == null) return;

                    ActionCreateEnvironment newCreateEntityAction = new ActionCreateEnvironment(_currentEntity.Id, entityName, varName, _currentStoryData);


                    _currentEntity.Actions.Add(newCreateEntityAction);

                    //Synchronize all traits and relationships
                    if (_currentStoryData.Environments.Count > 1)
                    {
                        Utilities.SynchronizeTwoEnvironments(_currentStoryData.Environments[0], newCreateEntityAction.NewEnvironment, _currentStoryData);
                    }
                    clearDataBindings();
                    dataBind();

                    Window newWin = new WindowEnvironmentEditor(newCreateEntityAction.NewEnvironment, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();
       

                }
                else
                {
                    int plotPointIndex = resultCreateObject - 2;
                    PlotPointType currType = _currentStoryData.PlotPointTypes[plotPointIndex];

                    ActionCreatePlotPoint newCreateEntityAction = new ActionCreatePlotPoint(_currentEntity.Id, currType, varName, _currentStoryData);


                    _currentEntity.Actions.Add(newCreateEntityAction);


                    clearDataBindings();
                    dataBind();

                    Window newWin = new WindowPlotPointEditor(newCreateEntityAction.NewPlotPoint, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();
               

                }
             
            }
            else if (result == 4) //Edit a saved object
            {
                List<string> editObjectChoices = new List<string>();
                editObjectChoices.Add("Character");
                editObjectChoices.Add("Environment");

                foreach (PlotPointType pp in _currentStoryData.PlotPointTypes)
                {
                    editObjectChoices.Add(pp.Description);
                }


                int resultEditObject = Utilities.MakeListChoiceDialog("What type of object would you like to edit?", editObjectChoices, this);




                if (resultEditObject < 0) { return; }
                else if (resultEditObject == 0) //Edit character
                {
                    List<string> editObjVarChoices = _currentEntity.getPreviouslyBoundCharacterVarNames(null);
                    if (editObjVarChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved Character variables to edit", this);
                        return;
                    }
                    int resultChooseEditTarget = Utilities.MakeListChoiceDialog(
                        "What saved Character would you like to edit?",
                        editObjVarChoices, this);

                    if (resultChooseEditTarget < 0) { return; }
                    ActionEditObject newEditObj = new ActionEditObject(
                        _currentEntity.Id,
                        editObjVarChoices[resultChooseEditTarget],
                        _currentStoryData.CharTypeId, ObjectEditingMode.Trait, _currentStoryData);


                    _currentEntity.Actions.Add(newEditObj);
                    clearDataBindings();
                    dataBind();

                    Window newWin = new WindowActionEditEntity(_currentEntity, newEditObj, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();


                }
                else if (resultEditObject == 1) //Edit Environment
                {
                    List<string> editObjVarChoices = _currentEntity.getPreviouslyBoundEnvironmentVarNames(null);
                    if (editObjVarChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved Environment variables to edit", this);
                        return;
                    }
                    int resultChooseEditTarget = Utilities.MakeListChoiceDialog(
                        "What saved Environment would you like to edit?",
                        editObjVarChoices, this);


                    if (resultChooseEditTarget < 0) { return; }
                    ActionEditObject newEditObj = new ActionEditObject(
                        _currentEntity.Id,
                        editObjVarChoices[resultChooseEditTarget],
                        _currentStoryData.EnvTypeId, ObjectEditingMode.Trait, _currentStoryData);


                    _currentEntity.Actions.Add(newEditObj);
                    clearDataBindings();
                    dataBind();

                    Window newWin = new WindowActionEditEntity(_currentEntity, newEditObj, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();
                }
                else if (resultEditObject > 1) //Edit a Plot Point
                {
                    int plotPointIndex = resultEditObject - 2;
                    PlotPointType currType = _currentStoryData.PlotPointTypes[plotPointIndex];

                    if(currType.Traits.Count == 0)
                    {
                        Utilities.MakeErrorDialog("The " + currType.Description + " type has no traits to edit.", this);
                        return;
                    }
                    List<string> editObjVarChoices = _currentEntity.getPreviouslyBoundPlotPointTypeVarNames(currType, null);
                    if (editObjVarChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved " + currType.Description + " variables to edit", this);
                        return;
                    }
                    int resultChooseEditTarget = Utilities.MakeListChoiceDialog(
                        "What saved " + currType.Description + " would you like to edit?",
                        editObjVarChoices, this);

                    if (resultChooseEditTarget < 0) { return; }
                    ActionEditObject newEditObj = new ActionEditObject(
                        _currentEntity.Id,
                        editObjVarChoices[resultChooseEditTarget],
                        currType.Id, ObjectEditingMode.Trait, _currentStoryData);


                    _currentEntity.Actions.Add(newEditObj);
                    clearDataBindings();
                    dataBind();

                    Window newWin = new WindowActionEditEntity(_currentEntity, newEditObj, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();

                }
            }
            else if (result == 5) //Delete a saved object
            {
                List<string> editObjectChoices = new List<string>();
                editObjectChoices.Add("Character");
                editObjectChoices.Add("Environment");

                foreach (PlotPointType pp in _currentStoryData.PlotPointTypes)
                {
                    editObjectChoices.Add("Plot Point: " + pp.Name);
                }


                int resultDelObject = Utilities.MakeListChoiceDialog("What type of object would you like to delete?", editObjectChoices, this);

                if (resultDelObject < 0) { return; }
                else if (resultDelObject == 0) //Delete character
                {
                    List<string> deleteObjectChoices = _currentEntity.getPreviouslyBoundCharacterVarNames(null);
                    if (deleteObjectChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved Character variables to delete", this);
                        return;
                    }
                    int resultChooseDeletionTarget = Utilities.MakeListChoiceDialog(
                        "What saved Character would you like to delete?",
                        deleteObjectChoices, this);

                    if (resultChooseDeletionTarget < 0) { return; }
                    ActionDeleteEntity newDelEntity = new ActionDeleteEntity(
                        _currentEntity.Id,
                        deleteObjectChoices[resultChooseDeletionTarget],
                        _currentStoryData.CharTypeId,
                        _currentStoryData);

                    _currentEntity.Actions.Add(newDelEntity);
       

                }
                else if (resultDelObject == 1) //Delete Environment
                {
                    List<string> deleteObjectChoices = _currentEntity.getPreviouslyBoundEnvironmentVarNames(null);
                    if (deleteObjectChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved Environment variables to delete", this);
                        return;
                    }
                    int resultChooseDeletionTarget = Utilities.MakeListChoiceDialog(
                        "What saved Environment would you like to delete?",
                        deleteObjectChoices, this);

                    if (resultChooseDeletionTarget < 0) { return; }
                    ActionDeleteEntity newDelEntity = new ActionDeleteEntity(
                        _currentEntity.Id,
                        deleteObjectChoices[resultChooseDeletionTarget],
                        _currentStoryData.EnvTypeId,
                        _currentStoryData);

                    _currentEntity.Actions.Add(newDelEntity);
      

                }
                else if (resultDelObject > 1) //Delete a Plot Point
                {
                    int plotPointIndex = resultDelObject - 2;
                    PlotPointType currType = _currentStoryData.PlotPointTypes[plotPointIndex];

                    List<string> deleteObjectChoices = _currentEntity.getPreviouslyBoundPlotPointTypeVarNames(currType,null);
                    if (deleteObjectChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved " + currType.Description + " variables to delete", this);
                        return;
                    }
                    int resultChooseDeletionTarget = Utilities.MakeListChoiceDialog(
                        "What saved " + currType.Description + " would you like to delete?",
                        deleteObjectChoices, this);

                    if (resultChooseDeletionTarget < 0) { return; }
                    ActionDeleteEntity newDelEntity = new ActionDeleteEntity(
                        _currentEntity.Id,
                        deleteObjectChoices[resultChooseDeletionTarget],
                        currType.Id,
                        _currentStoryData);

                    _currentEntity.Actions.Add(newDelEntity);
           

                }
            }

            clearDataBindings();
            dataBind();
        }

        private void btDuplicatePrecond_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = precondDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is PreconditionStatement)) { return; }

            _currentEntity.PrecStatements.Add((PreconditionStatement)((PreconditionStatement)itemToEdit).Clone());
            clearDataBindings();
            dataBind();
        }

        private void btEditAction_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = actionsDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is Action)) { return; }


            if (itemToEdit.GetType() == typeof(ActionSubgoal))
            {
                AuthorGoal subGoal = _currentStoryData.findAuthorGoalById(((ActionSubgoal)itemToEdit).SubGoalId);
                if(subGoal == null)
                {
                    Utilities.MakeErrorDialog("This Pursue Subgoal Action refers to an Author Goal that no longer exists. You cannot edit its contents. Please delete it in order to generate a story.", this);
                    return;
                }
                else
                {
                    Window newWin = new WindowActionSubgoalEditor(false, _currentEntity, (ActionSubgoal)itemToEdit, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();
               
                }
     

            }
            else if (itemToEdit.GetType() == typeof(ActionEditObject))
            {
                UInt64 editObjTypeId = ((ActionEditObject)itemToEdit).ObjectTypeId;


                if ((editObjTypeId == _currentStoryData.CharTypeId) || (editObjTypeId == _currentStoryData.EnvTypeId))
                {
                   
                }
                else
                {
                    PlotPointType currType = _currentStoryData.findPlotPointTypeById(editObjTypeId);
                    if (currType == null)
                    {
                        //Very bad - precondition statement has no associated type. The user
                        //is not allowed to edit this, and must delete it 
                        Utilities.MakeErrorDialog("This Edit Plot Point Action refers to a Plot Point Type that no longer exists. You cannot edit its contents. Please delete it in order to generate a story.", this);
                        return;
                    }
                }

                Window newWin = new WindowActionEditEntity(_currentEntity, (ActionEditObject)itemToEdit, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
           
            }
            else if (itemToEdit.GetType() == typeof(ActionTextOutput))
            {

                Window newWin = new WindowActionTextOutput(_currentEntity, (ActionTextOutput)itemToEdit);
                newWin.Owner = this;
                newWin.ShowDialog();
             
            }
            else if (itemToEdit.GetType() == typeof(ActionCalculation))
            {
                Window newWin = new WindowActionCalculationEditor(_currentEntity, (ActionCalculation)itemToEdit, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
        
            }
            else if (itemToEdit.GetType() == typeof(ActionCreatePlotPoint))
            {

                if (null == _currentStoryData.findPlotPointTypeById(((ActionCreatePlotPoint)itemToEdit).NewPlotPoint.TypeId))
                {
                    //Very bad - precondition statement has no associated type. The user
                    //is not allowed to edit this, and must delete it 
                    Utilities.MakeErrorDialog("This Create Plot Point Action refers to a Plot Point Type that no longer exists. You cannot edit its contents. Please delete it in order to generate a story.", this);
                    return;
                }

                Window newWin = new WindowPlotPointEditor(((ActionCreatePlotPoint)itemToEdit).NewPlotPoint, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
     
            }
            else if (itemToEdit.GetType() == typeof(ActionCreateEnvironment))
            {

                Window newWin = new WindowEnvironmentEditor(((ActionCreateEnvironment)itemToEdit).NewEnvironment, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
       

            }
            else if (itemToEdit.GetType() == typeof(ActionCreateCharacter))
            {
                Window newWin = new WindowCharacterEditor(((ActionCreateCharacter)itemToEdit).NewCharacter, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();


            }
            else if (itemToEdit.GetType() == typeof(ActionDeleteEntity)) //Edit an object deletion action
            {
                ActionDeleteEntity deletionAction = (ActionDeleteEntity)itemToEdit;

                if (deletionAction.TypeId == _currentStoryData.CharTypeId) //Edit char deletion
                {
                    List<string> deleteObjectChoices = _currentEntity.getPreviouslyBoundCharacterVarNames(null);
                    if (deleteObjectChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved Character variables to delete", this);
                        return;
                    }
                    int resultChooseDeletionTarget = Utilities.MakeListChoiceDialog(
                        "What saved Character would you like to delete?",
                        deleteObjectChoices, this);

                    if (resultChooseDeletionTarget < 0) { return; }

                    deletionAction.VariableName = deleteObjectChoices[resultChooseDeletionTarget];
                }
                else if(deletionAction.TypeId == _currentStoryData.EnvTypeId) //Edit environment deletion
                {
                    
                    List<string> deleteObjectChoices = _currentEntity.getPreviouslyBoundEnvironmentVarNames(null);
                    if (deleteObjectChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved Environment variables to delete", this);
                        return;
                    }
                    int resultChooseDeletionTarget = Utilities.MakeListChoiceDialog(
                        "What saved Environment would you like to delete?",
                        deleteObjectChoices, this);

                    if (resultChooseDeletionTarget < 0) { return; }

                    deletionAction.VariableName = deleteObjectChoices[resultChooseDeletionTarget];
                }
                else //Edit some plot point type deletion
                {
                    PlotPointType currType = _currentStoryData.findPlotPointTypeById(deletionAction.TypeId);

                    if(currType == null)
                    {
                        Utilities.MakeErrorDialog("This Delete Plot Point Action refers to a Plot Point Type that no longer exists. You cannot edit its contents. Please delete it in order to generate a story.", this);
                        return;

                    }
                    List<string> deleteObjectChoices = _currentEntity.getPreviouslyBoundPlotPointTypeVarNames(currType, null);
                    if (deleteObjectChoices.Count == 0)
                    {
                        Utilities.MakeErrorDialog("There are no saved " + currType.Description + " variables to delete", this);
                        return;
                    }
                    int resultChooseDeletionTarget = Utilities.MakeListChoiceDialog(
                        "What saved " + currType.Description + " would you like to delete?",
                        deleteObjectChoices, this);

                    if (resultChooseDeletionTarget < 0) { return; }

                    deletionAction.VariableName = deleteObjectChoices[resultChooseDeletionTarget];

                }
                
            }
       


            clearDataBindings();
            dataBind();
        }

        private void btDuplicateAction_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = actionsDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is Action)) { return; }

            if(itemToEdit is ActionDeleteEntity)
            {
                Utilities.MakeErrorDialog("You cannot Delete an Object more than once.", this);
                return;
            }
            _currentEntity.Actions.Add((Action)((Action)itemToEdit).Clone());
            clearDataBindings();
            dataBind();
        }

        private void btDeleteAction_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = actionsDataGrid.SelectedItem;
            if ((itemToEdit == null) || !(itemToEdit is Action)) { return; }

            Action itemToDelete = (Action)itemToEdit;

            if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete this Action?", "Confirm Deletion", this) == MessageBoxResult.No)
            {
                return;
            }

            _currentEntity.Actions.Remove(itemToDelete);
            clearDataBindings();
            dataBind();
        }

        private void precondDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEditPrecond_Click(null, null);
        }

        private void actionsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEditAction_Click(null, null);
        }

        private void btEditFragName_Click(object sender, RoutedEventArgs e)
        {
            // Get name
            List<string> choiceConstraints = new List<string>();
            AuthorGoal parentGoal = _currentStoryData.findAuthorGoalById(_currentEntity.ParentAuthorGoalId);

            if(parentGoal == null) 
            {
                return;
            }

            foreach (PlotFragment frag in parentGoal.PlotFragments)
            {
                if (frag != _currentEntity) //Allow user to name this fragment to the old name, so don't add the current name to the list
                {
                    
                    choiceConstraints.Add(frag.Name);
                }
                

            }

            string newName = Utilities.MakeConstrainedTextDialog(
                "Please enter a new name for this Plot Fragment:",
                "That name is already in use by another Plot Fragment.",
                choiceConstraints, this);

            if (newName == null)
            {
                return;
            }

            _currentEntity.Name = newName;
            clearDataBindings();
            dataBind();

        }



    }
}
