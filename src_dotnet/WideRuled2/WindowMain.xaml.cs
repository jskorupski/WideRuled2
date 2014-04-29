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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Xceed.Wpf.Controls;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.ThemePack;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;


namespace WideRuled2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        private StoryData _currentStoryData;
        private string _saveFileName;


        public WindowMain()
        {
            InitializeComponent();

            #if (DEBUG)

                DebugMenu.Visibility = Visibility.Visible;

            #endif

            CurrentStory = new StoryData();
            
            _saveFileName = "";
            this.WindowTitle = "(Untitled Story)";

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            
        } 

        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register("WindowTitle", typeof(String), typeof(Window));

        public String WindowTitle
        {
            get { return (String)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value + " - " + "Wide Ruled 2.0, Release " + App.VERSION); }
        }

        private StoryData CurrentStory {

            get { return _currentStoryData; }
            set 
            { 
                _currentStoryData = value;
                StoryWorldDataProvider.setStoryData(_currentStoryData);
            }
        }


        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Configure the message box to be displayed
            string messageBoxText = "Are you sure you want to exit?";
            string caption = "Exit Wide Ruled";

            MessageBoxResult result = Utilities.MakeYesNoWarningDialog(messageBoxText, caption, this);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Closing -= new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
                    App.Current.Shutdown(0);
                    break;
                case MessageBoxResult.No:
                    e.Cancel = true;
                    break;
            }

        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {


            if(_currentStoryData.AuthorGoals.Count == 0)
            {
                Utilities.MakeErrorDialog("Your story needs at least one Author Goal.", this);
                return;
            }

            if (_currentStoryData.findAuthorGoalById(_currentStoryData.StartGoalId) == null)
            {
                Utilities.MakeErrorDialog("One of your Author Goals must be selected as the start goal. You can modify this setting in the Author Goal editor window.", this);
                return;
            }

            if (_currentStoryData.findAuthorGoalById(_currentStoryData.StartGoalId).Parameters.Count > 0)
            {
                Utilities.MakeErrorDialog("Your selected start goal has parameters. Please select a start goal with no parameters, or remove these parameters from the start goal.", this);
                return;
            }

            if (_currentStoryData.findAuthorGoalById(_currentStoryData.StartGoalId).PlotFragments.Count == 0)
            {
                Utilities.MakeErrorDialog("Your Start Goal has no associated Plot Fragments that fulfill it. " + 
                "All goals which are used during story creation must have at least one Plot Fragment associated with them before generation can begin.", this);
                return;
            }


            //Author goal and plot fragment checks
            string goalName = "";
            try 
            {
                foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
                {
                    goalName = goal.Name;
                    goal.checkAndUpdateDependences(new List<Trait>(), _currentStoryData);
                }
            }
            catch(Exception authGoalException)
            {

               Utilities.MakeErrorDialog("Error in Author Goal \"" + goalName + "\": " + authGoalException.Message, this);
                return;
            }

            //Interaction checks
            string interactionTitle = "";
            try
            {
               foreach (Interaction interact in _currentStoryData.Interactions)
               {
                   interactionTitle = interact.Title;
                   interact.checkAndUpdateDependences(new List<Trait>(), _currentStoryData);
               }
            }
            catch (Exception interactExeption)
            {

               Utilities.MakeErrorDialog("Error in Interaction \"" + interactionTitle + "\": " + interactExeption.Message, this);
               return;
            }
            AblCommObject.Instance.reset();

            try
            {
                AblCodeGenerator gen = new AblCodeGenerator(_currentStoryData);
                gen.BuildCode();
            }
            catch (System.Exception buildError)
            {
                Utilities.MakeErrorDialog("Error during code generation: " + buildError.Message, this);
                return;
            }

            try
            {
               
                Thread newThread = new Thread(AblCommunicator.ExecuteStory);
                newThread.Start();
            }
            catch (System.Exception genError)
            {
                Utilities.MakeErrorDialog("Error while generating story: " + genError.Message, this);
                return;
            }


            //Interactivity Windows

           WindowInteraction interactionWindow = new WindowInteraction(_currentStoryData.Interactions);
           WindowStoryOutputText outputWin = new WindowStoryOutputText(interactionWindow);



           outputWin.ShowDialog();



        }

      



        #region File Menu
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(
                (_currentStoryData.AuthorGoals.Count > 0) ||
                (_currentStoryData.Characters.Count > 0) ||
                (_currentStoryData.Environments.Count > 0) ||
                (_currentStoryData.PlotPointTypes.Count > 0)
            )
            {
                MessageBoxResult result = Utilities.MakeYesNoWarningDialog("Creating a new Story will erase any data you haven't yet saved. Are you sure you want to create a new Story?", "Confirm New Story Creation", this);
                if(result == MessageBoxResult.No)
                {
                    return;
                }

            }
            _saveFileName = "";
            this.WindowTitle = "(Untitled Story)";
            CurrentStory = new StoryData();
            dataBindWindow();
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {

            
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
          
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ""; // Default file extension
            dlg.Filter = "Wide Ruled 2 Stories (.wr2)|*.wr2"; // Filter files by extension
            
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string resultFileName = "";
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                resultFileName = dlg.FileName;
            }



            if (!(resultFileName.Equals(""))) {
                // DeSerialize from disk

                try
                {
                    StoryData resultStoryData = Utilities.DeSerializeStoryDataFromDisk(resultFileName);
                    if (resultStoryData != null)
                    {
                        CurrentStory = resultStoryData;
                        _saveFileName = resultFileName;
                        this.WindowTitle = System.IO.Path.GetFileName(_saveFileName);

                        dataBindWindow();
                    }
                    else {
                        throw new Exception("Could not read file \"" + resultFileName + "\"");

                    }
 
                }
                catch (System.Exception openError)
                {

                    string messageBoxText = "Error opening file:\n" + openError.Message;
                    string caption = "Open Error";


                    Utilities.MakeErrorDialog(messageBoxText, caption, this);
                }
            }
            

        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
 
            this.Close();
            
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
             if (!(_saveFileName.Equals(""))) {

                // Serialize to disk
                try
                {
                    Utilities.SerializeStoryDataToDisk(_saveFileName, _currentStoryData);
                    
 
                }
                catch (System.Exception saveError)
                {

                    string messageBoxText = "Error saving file:\n" + saveError.Message;
                    string caption = "Save Error";
  

                    Utilities.MakeErrorDialog(messageBoxText, caption, this);
                }
            }
            else {

                SaveAsMenuItem_Click(sender, e);
            }



        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "MyStory"; // Default file name
            dlg.DefaultExt = ".wr2"; // Default file extension
            dlg.Filter = "Wide Ruled 2 Stories (.wr2)|*.wr2"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string resultFileName = "";

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                resultFileName = dlg.FileName;
            }


            if (!(resultFileName.Equals(""))) {

                // Serialize to disk


                try
                {
                    Utilities.SerializeStoryDataToDisk(resultFileName, _currentStoryData);
                    _saveFileName = resultFileName;
                    WindowTitle = System.IO.Path.GetFileName(_saveFileName);
                }
                catch (System.Exception saveError)
                {

                    string messageBoxText = "Error saving file:\n" + saveError.Message;
                    string caption = "Save Error";
    

                    Utilities.MakeErrorDialog(messageBoxText, caption, this);
                }


            }

        }

        private void AnalyzeDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WindowStoryDataAnalyzer analysisWindow = new WindowStoryDataAnalyzer();
            analysisWindow.Show();

        }

        #endregion

        private void clearDataElements()
        {

               

            charDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            charDataGrid.Items.Clear();
            envDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            envDataGrid.Items.Clear();

            ppDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            ppDataGrid.Items.Clear();

            goalFragsTreeView.ClearValue(TreeView.ItemsSourceProperty);
            goalFragsTreeView.Items.Clear();


  
        }
        private void dataBindWindow () 
        {

            if (_currentStoryData == null) 
            {
                return;
            }

            Object oldCharSelect = charDataGrid.SelectedItem;
            Object oldEnvSelect = envDataGrid.SelectedItem;
            Object oldPPTypeSelect = ppDataGrid.SelectedItem;

           
            charDataGrid.ItemsSource = _currentStoryData.Characters;
            envDataGrid.ItemsSource = _currentStoryData.Environments;
            ppDataGrid.ItemsSource = _currentStoryData.PlotPointTypes;


            checkBoxStatusMessages.IsChecked = _currentStoryData.ShowDebugMessages;
             goalFragsTreeView.ItemsSource = _currentStoryData.AuthorGoals;
             expandAllTreeViewItems();

             charDataGrid.SelectedItem = oldCharSelect;
             envDataGrid.SelectedItem = oldEnvSelect;
             ppDataGrid.SelectedItem = oldPPTypeSelect;
 
           // charDataGrid.Columns["Type"].CellEditor = this.FindResource("traitDataTypeEditor") as CellEditor;
           // charDataGrid.Columns["Type"].CellContentTemplate = this.FindResource("traitDataTypeCellDataTemplate") as DataTemplate;





        }

        private void expandAllTreeViewItems()
        {
            foreach (object item in goalFragsTreeView.Items)
            {
                TreeViewItem treeItem = goalFragsTreeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    ExpandAll(treeItem, true);
                treeItem.IsExpanded = true;
            }
        }

        void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                Object test = (Object)items.ItemContainerGenerator.ContainerFromItem(obj);

                TreeViewItem itemControl = items.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                if (itemControl != null)
                    itemControl.IsExpanded = true;

                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }

            }
        }



        private void buttonInteractActions_Click(object sender, RoutedEventArgs e)
        {

            Window newWin = new WindowInteractionListEditor(_currentStoryData);

            newWin.Owner = this;
            newWin.ShowDialog();
        }
 
 

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {

            //Handle null sender
            if (sender == null) 
            {
                return;
            }

            //Handle null selections
            if ((((ComboBox)sender).SelectedItem == null) || (charDataGrid.SelectedItem == null))
            {
                return;
            }

            Trait selectedTrait = (Trait)charDataGrid.SelectedItem;

 
            //Handle unexpected data type
            if ((((ComboBox)sender).SelectedItem).GetType() != typeof(TraitDataType)) {

                return;
            }


            TraitDataType selectedType = (TraitDataType)(((ComboBox)sender).SelectedItem);

            if (selectedTrait.Type == selectedType)
            {
                return;
            }


            charDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            charDataGrid.Items.Clear();

            selectedTrait.Type = selectedType;
            
            dataBindWindow();
        }

        private void btCharEdit_Click(object sender, RoutedEventArgs e)
        {

            Object itemToEdit = charDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Character))) { return; }


            Window newWin = new WindowCharacterEditor((Character)itemToEdit, _currentStoryData);

            newWin.Owner = this;
            newWin.ShowDialog();
            clearDataElements();
            dataBindWindow();
        }

        private void btEnvEdit_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = envDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Environment))) { return; }


            Window newWin = new WindowEnvironmentEditor((Environment)itemToEdit, _currentStoryData);
            newWin.Owner = this;
            newWin.ShowDialog();
            clearDataElements();
            dataBindWindow();
        }

        private void btPpTypeEdit_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = ppDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(PlotPointType))) { return; }


            Window newWin = new WindowPlotPointTypeEditor((PlotPointType)itemToEdit, _currentStoryData);
            newWin.Owner = this;
            newWin.ShowDialog();
            clearDataElements();
            dataBindWindow();
        }

        private void btGoalFragEdit_Click(object sender, RoutedEventArgs e)
        {

            Object itemToEdit = goalFragsTreeView.SelectedItem;
            if (itemToEdit == null) { return;}
            
            
            if(itemToEdit.GetType() == typeof(AuthorGoal)) 
            {
                Window newWin = new WindowAuthorGoalEditor((AuthorGoal)itemToEdit, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();
            }
            else if (itemToEdit.GetType() == typeof(PlotFragment))
            {

                Window newWin = new WindowPlotFragmentEditor((PlotFragment)itemToEdit, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();
            }
            else if (itemToEdit.GetType() == typeof(ActionSubgoal))
            {
                //Find the parent plot fragment using its stored id, then use that plot fragment to look up
                //the parent author goal id, which is then used to get the actual AuthorGoal object (whew!)
                AuthorGoal goalToEdit = _currentStoryData.findAuthorGoalById(((ActionSubgoal)itemToEdit).SubGoalId);
        
                if(goalToEdit != null)
                {
                    Window newWin = new WindowAuthorGoalEditor(goalToEdit, _currentStoryData);
                    newWin.Owner = this;
                    newWin.ShowDialog();
                    clearDataElements();
                    dataBindWindow();
                }
             
            }
            else {

                return;
            }



        }

        private void charDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btCharEdit_Click(null, null);
        }

        private void envDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEnvEdit_Click(null, null);
        }

        private void ppDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            btPpTypeEdit_Click(null, null);
        }

        private void goalFragsTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            btGoalFragEdit_Click(null, null);
        }

        private void btCharNew_Click(object sender, RoutedEventArgs e)
        {
            string newName = Utilities.MakeTextDialog("Please enter a name for the new Character:", this);
            if(newName != null)
            {
                Character newChar = new Character(newName, _currentStoryData.CharTypeId, _currentStoryData);
              

                List<Character> globalCharList = Utilities.getGlobalCharacterList(_currentStoryData);
                //Synchronize all traits and relationships
                if (globalCharList.Count > 0)
                {
                    Utilities.SynchronizeTwoCharacters(globalCharList[0], newChar, _currentStoryData);
                }
                _currentStoryData.Characters.Add(newChar);
                clearDataElements();
                dataBindWindow();

                Window newWin = new WindowCharacterEditor(newChar, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();
            }
        }

        private void btEnvNew_Click(object sender, RoutedEventArgs e)
        {
            string newName = Utilities.MakeTextDialog("Please enter a name for the new Environment:", this);
            if (newName != null)
            {
                Environment newEnv = new Environment(newName, _currentStoryData.EnvTypeId, _currentStoryData);
              

                List<Environment> globalEnvList = Utilities.getGlobalEnvironmentList(_currentStoryData);
                //Synchronize all traits and relationships
                if (globalEnvList.Count > 0)
                {
                    Utilities.SynchronizeTwoEnvironments(globalEnvList[0], newEnv, _currentStoryData);
                }
                _currentStoryData.Environments.Add(newEnv);
                clearDataElements();
                dataBindWindow();

                Window newWin = new WindowEnvironmentEditor(newEnv, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();
            }
        }

        private void btPpTypeNew_Click(object sender, RoutedEventArgs e)
        {
            List<string> existingPPTypeNames = new List<string>();

            foreach (PlotPointType ppT in _currentStoryData.PlotPointTypes)
            {

                existingPPTypeNames.Add(ppT.Name);
            }

            string constraintErrorMessage = "That name is already in use by another Plot Point Type.";
            string newName = Utilities.MakeConstrainedTextDialog("Please enter a name for your new Plot Point Type:", constraintErrorMessage, existingPPTypeNames, this);

            if (newName != null)
            {
                PlotPointType newPPType = new PlotPointType(newName, _currentStoryData);
                _currentStoryData.PlotPointTypes.Add(newPPType);

                clearDataElements();
                dataBindWindow();

                Window newWin = new WindowPlotPointTypeEditor(newPPType, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();
            }
        }

        private void btGoalFragNew_Click(object sender, RoutedEventArgs e)
        {
            List<string> editChoices = new List<string>();
            editChoices.Add("Author Goal");
            editChoices.Add("Plot Fragment");

            int result = -1;
            if(_currentStoryData.AuthorGoals.Count > 0)
                result = Utilities.MakeListChoiceDialog("What type of Story Object would you like to create?", editChoices, this);
            else
            {
                result = 0;
            }
            if(result < 0)
            {
                return;
            }
            else if(result == 0) //Author Goal
            {
                List<string> choiceConstraints = new List<string>();
                foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
                {
                    choiceConstraints.Add(goal.Name);

                }
                string newName = Utilities.MakeConstrainedTextDialog(
                    "Please enter a name for your new Author Goal:",
                    "That name is already in use by another Author Goal.",
                    choiceConstraints, this);

                if(newName == null)
                {
                    return;
                }

                AuthorGoal newAuthGoal = new AuthorGoal(newName, _currentStoryData);
                _currentStoryData.AuthorGoals.Add(newAuthGoal);
                //If first author goal, set it to the start goal
                if (_currentStoryData.AuthorGoals.Count == 1)
                {
                    _currentStoryData.StartGoalId = newAuthGoal.Id;
                }
                clearDataElements();
                dataBindWindow();

                Window newWin = new WindowAuthorGoalEditor(newAuthGoal, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();
            }
            else if(result == 1) //Plot Fragment
            {
                //Get parent author goal
                editChoices.Clear();
                foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
                {

                    editChoices.Add(goal.Description);
                }

                result = Utilities.MakeListChoiceDialog("Choose the Author Goal for this new Plot Fragment:", editChoices, this);
                if(result < 0)
                {
                    return;
                }

                AuthorGoal parentAuthorGoal = _currentStoryData.AuthorGoals[result];

                // Get name
                List<string> choiceConstraints = new List<string>();
 
                foreach (PlotFragment frag in parentAuthorGoal.PlotFragments)
                {
                    choiceConstraints.Add(frag.Name);

                }
                string newName = Utilities.MakeConstrainedTextDialog(
                    "Please enter a name for your new Plot Fragment:",
                    "That name is already in use by another Plot Fragment.",
                    choiceConstraints, this);

                if(newName == null)
                {
                    return;
                }

                
                PlotFragment newPlotFrag = new PlotFragment(newName, parentAuthorGoal.Id, _currentStoryData);
                parentAuthorGoal.PlotFragments.Add(newPlotFrag);

                clearDataElements();
                dataBindWindow();

                Window newWin = new WindowPlotFragmentEditor(newPlotFrag, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();
                clearDataElements();
                dataBindWindow();

       
            }

          
        }

        private void btCharDelete_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = charDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Character))) { return; }

            if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete this Character?", "Confirm Deletion", this) == MessageBoxResult.No)
            {
                return;
            }


            _currentStoryData.Characters.Remove((Character)itemToEdit);
            Utilities.removeRelationshipTargetReferencesFromStoryWorld((Character)itemToEdit, _currentStoryData);
            clearDataElements();
            dataBindWindow();
        }

        private void btEnvDelete_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = envDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Environment))) { return; }

            if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete Environment \"" + ((Environment)itemToEdit).Name + "\"?", "Confirm Deletion", this) == MessageBoxResult.No)
            {
                return;
            }
            
            _currentStoryData.Environments.Remove((Environment)itemToEdit);
            Utilities.removeRelationshipTargetReferencesFromStoryWorld((Environment)itemToEdit, _currentStoryData);
            clearDataElements();
            dataBindWindow();
        }

        private void btPPTypeDelete_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = ppDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(PlotPointType))) { return; }

            PlotPointType ppTypeSelected = (PlotPointType)itemToEdit;

            if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete Plot Point Type \"" + ppTypeSelected.Name + "\"?", "Confirm Deletion", this) == MessageBoxResult.No)
            {
                return;
            }

            _currentStoryData.PlotPointTypes.Remove(ppTypeSelected);
            clearDataElements();
            dataBindWindow();
        }

        private void btGoalFragDelete_Click(object sender, RoutedEventArgs e)
        {

            Object itemToEdit = goalFragsTreeView.SelectedItem;
            if (itemToEdit == null) { return; }


            if (itemToEdit.GetType() == typeof(AuthorGoal))
            {

                if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete Author Goal \"" + ((AuthorGoal)itemToEdit).Name + "\" and all of its Plot Fragments?", "Confirm Deletion", this) == MessageBoxResult.No)
                {
                    return;
                }

                AuthorGoal goalToDelete = (AuthorGoal)itemToEdit;


                _currentStoryData.AuthorGoals.Remove(goalToDelete);

                clearDataElements();
                dataBindWindow();
            }
            else if (itemToEdit.GetType() == typeof(PlotFragment))
            {

                if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete Plot Fragment \"" + ((PlotFragment)itemToEdit).Name + "\"?", "Confirm Deletion", this) == MessageBoxResult.No)
                {
                    return;
                }
                AuthorGoal parentGoal = _currentStoryData.findAuthorGoalById(((PlotFragment)itemToEdit).ParentAuthorGoalId);
                parentGoal.PlotFragments.Remove((PlotFragment)itemToEdit);
                clearDataElements();
                dataBindWindow();
            }
            else if (itemToEdit.GetType() == typeof(ActionSubgoal))
            {
                //Find the parent plot fragment using its stored id, then use that plot fragment to look up
                //the parent author goal id, which is then used to get the actual AuthorGoal object (whew!)
                AuthorGoal goalToDelete = _currentStoryData.findAuthorGoalById(_currentStoryData.findPlotFragmentById(((ActionSubgoal)itemToEdit).ParentPlotFragmentId).ParentAuthorGoalId);


                if (Utilities.MakeYesNoWarningDialog("Are you sure you want to delete Author Goal \"" + ((AuthorGoal)goalToDelete).Name + "\" and all of its Plot Fragments?", "Confirm Deletion", this) == MessageBoxResult.No)
                {
                    return;
                }

               


                _currentStoryData.AuthorGoals.Remove(goalToDelete);

                clearDataElements();
                dataBindWindow();
            }
            else
            {

                return;
            }

        }

        private void btMoveUpChar_Click(object sender, RoutedEventArgs e)
        {

            Object itemToEdit = charDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Character))) { return; }

            Utilities.MoveItemUp(
                 _currentStoryData.Characters,
                  itemToEdit);

            clearDataElements();
            dataBindWindow();
            charDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveDownChar_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = charDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Character))) { return; }

            Utilities.MoveItemDown(
                 _currentStoryData.Characters,
                  itemToEdit);

            clearDataElements();
            dataBindWindow();
            charDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveUpEnv_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = envDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Environment))) { return; }

            Utilities.MoveItemUp(
                 _currentStoryData.Environments,
                  itemToEdit);

            clearDataElements();
            dataBindWindow();
            envDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveDownEnv_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = envDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Environment))) { return; }

            Utilities.MoveItemDown(
                 _currentStoryData.Environments,
                  itemToEdit);

            clearDataElements();
            dataBindWindow();
            envDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveUpPPType_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = ppDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(PlotPointType))) { return; }

            Utilities.MoveItemUp(
                 _currentStoryData.PlotPointTypes,
                  itemToEdit);

            clearDataElements();
            dataBindWindow();
            ppDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveDownPPType_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = ppDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(PlotPointType))) { return; }

            Utilities.MoveItemDown(
                 _currentStoryData.PlotPointTypes,
                  itemToEdit);

            clearDataElements();
            dataBindWindow();
            ppDataGrid.SelectedItem = itemToEdit;
        }

        private void btMoveUpGoalFrag_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = goalFragsTreeView.SelectedItem;
            if (itemToEdit == null) { return; }


            if (itemToEdit.GetType() == typeof(AuthorGoal))
            {
                Utilities.MoveItemUp(_currentStoryData.AuthorGoals, itemToEdit);
                clearDataElements();
                dataBindWindow();
               
            }
            else if (itemToEdit.GetType() == typeof(PlotFragment))
            {

                Utilities.MoveItemUp(
                    _currentStoryData.findAuthorGoalById(((PlotFragment)itemToEdit).ParentAuthorGoalId).PlotFragments,
                    itemToEdit);

                clearDataElements();
                dataBindWindow();
               
            }
            else
            {

                return;
            }
        }

        private void btMoveDownGoalFrag_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = goalFragsTreeView.SelectedItem;
            if (itemToEdit == null) { return;}
            
            
            if(itemToEdit.GetType() == typeof(AuthorGoal)) 
            {
                Utilities.MoveItemDown(_currentStoryData.AuthorGoals, itemToEdit);
                clearDataElements();
                dataBindWindow();
            }
            else if (itemToEdit.GetType() == typeof(PlotFragment))
            {

                Utilities.MoveItemDown(
                    _currentStoryData.findAuthorGoalById(((PlotFragment)itemToEdit).ParentAuthorGoalId).PlotFragments, 
                    itemToEdit);

                clearDataElements();
                dataBindWindow();
            }
            else {

                return;
            }
        }

        private void goalFragsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

    
        private void checkBoxStatusMessages_Checked(object sender, RoutedEventArgs e)
        {
            _currentStoryData.ShowDebugMessages = (bool)checkBoxStatusMessages.IsChecked;
        }

        private void mainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
              
            
                //if(e.Delta > 0)
                //{
                //    mainGrid.RenderTransform = new MatrixTransform(
                //        Matrix.Multiply(
                //            mainGrid.RenderTransform.Value,
                //            new ScaleTransform(1.0 + 1.0 / 100.0, 1.0 + 1.0 / 100.0, e.GetPosition(mainGrid).X, e.GetPosition(mainGrid).Y).Value
                //        )
                //    );
                
                //}
                //else
                //{
                    
                //    mainGrid.RenderTransform = new MatrixTransform(
                //        Matrix.Multiply(
                //            mainGrid.RenderTransform.Value,
                //            new ScaleTransform(1.0 - 1.0 / 100.0, 1.0 - 1.0 / 100.0, e.GetPosition(mainGrid).X, e.GetPosition(mainGrid).Y).Value
                //        )
                //    );
                    
                //}

                //e.Handled = true;
            
        }

        private void mainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            
            //if(e.LeftButton == MouseButtonState.Pressed)
            //{

            //    mainGrid.RenderTransform = new MatrixTransform(
            //        Matrix.Multiply(
            //            mainGrid.RenderTransform.Value,
            //            new TranslateTransform(e.LeftButton == ).Value
            //        )
            //    ); 
            //}
        }

      




    }
}
