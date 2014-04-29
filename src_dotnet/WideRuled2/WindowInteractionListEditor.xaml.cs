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
    /// Interaction logic for WindowInteractionEditor.xaml
    /// </summary>
    public partial class WindowInteractionListEditor : Window
    {

        private StoryData _currentStoryData;
        public WindowInteractionListEditor(StoryData world)
        {
            InitializeComponent();

            _currentStoryData = world;
            dataBind();
        }   

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void dataBind()
        {

            interactionsDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            interactionsDataGrid.Items.Clear();

            interactionsDataGrid.ItemsSource = _currentStoryData.Interactions;

        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btDeleteSelected_Click(object sender, RoutedEventArgs e)
        {

            if ((interactionsDataGrid.SelectedItems.Count == 0) || (interactionsDataGrid.SelectedItems[0].GetType() != typeof(Interaction))) { return; }
            List<Interaction> deletionList = new List<Interaction>();
            foreach (Object item in interactionsDataGrid.SelectedItems)
            {
                deletionList.Add((Interaction)item);

            }
            foreach (Interaction delItem in deletionList)
            {
                _currentStoryData.Interactions.Remove(delItem);
            }

            dataBind();
        }

        private void btAddNew_Click(object sender, RoutedEventArgs e)
        {

            if(_currentStoryData.AuthorGoals.Count == 0)
            {
                Utilities.MakeErrorDialog("There must be at least one Author Goal in this story for you to make a new Interactive Action.", this);
                return;
            }
            interactionsDataGrid.EndEdit();
            string newName = Utilities.MakeTextDialog("Please enter a title for this new Interactive Action:", this);
            if (newName != null)
            {
                List<string> goalChoices = new List<string>();
                //Get author goal to activate
                foreach (AuthorGoal goal in _currentStoryData.AuthorGoals)
                {

                    goalChoices.Add(goal.Description);
                }

                int result = Utilities.MakeListChoiceDialog("Choose the Author Goal you would like to activate for this Interactive Action.", goalChoices, this);
                if (result < 0)
                {
                    return;
                }

                AuthorGoal authorGoalSelection = _currentStoryData.AuthorGoals[result];

               
                ActionSubgoal newSubGoal = new ActionSubgoal(0, authorGoalSelection.Id, _currentStoryData);
                Interaction newInteraction = new Interaction(newName, newSubGoal, _currentStoryData);
                _currentStoryData.Interactions.Add(newInteraction);
                dataBind();

                WindowActionSubgoalEditor newWin = new WindowActionSubgoalEditor(true, null, newSubGoal, _currentStoryData);
                newWin.Owner = this;
                newWin.ShowDialog();

                

            }
        }


        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            Object itemToEdit = interactionsDataGrid.SelectedItem;
            if ((itemToEdit == null) || (itemToEdit.GetType() != typeof(Interaction))) { return; }

            interactionsDataGrid.EndEdit();

            WindowActionSubgoalEditor newWin = new WindowActionSubgoalEditor(true, null, ((Interaction)itemToEdit).SubgoalAction, _currentStoryData);
            newWin.Owner = this;
            newWin.ShowDialog();

            dataBind();
        }

        private void interactionsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEdit_Click(null, null);
        }
    }
}
