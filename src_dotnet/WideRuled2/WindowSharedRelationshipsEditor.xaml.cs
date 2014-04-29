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
    /// Interaction logic for SharedRelationshipsEditorWindow.xaml
    /// </summary>
    public partial class WindowSharedRelationshipsEditor : Window
    {   
        private StoryEntity _parentEntity;
        private StoryData _currentStoryData;
        private List<Relationship> _newList;

 
        public WindowSharedRelationshipsEditor(StoryEntity parentEntity, StoryData storyData)
        {

            InitializeComponent();
            _parentEntity = parentEntity;
            _currentStoryData = storyData;
            _newList = new List<Relationship>();

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            relDataGrid.EndEdit();
                
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(Relationship rel in _parentEntity.Relationships)
            {

                _newList.Add((Relationship)rel.Clone());
            }
           // _newList.AddRange(_parentEntity.Relationships);
            dataBind();
            
        }

        private void dataBind() 
        {
            relDataGrid.ItemsSource = _newList;
        }
        private void clearDataGrids() 
        {

            relDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            relDataGrid.Items.Clear();
        }
        private void btAddNew_Click(object sender, RoutedEventArgs e)
        {
            relDataGrid.EndEdit();
            _newList.Add(new Relationship("NewRelationship", 0, _currentStoryData.getNewId(), _currentStoryData));
            clearDataGrids();

            dataBind();
        }

        private void btDeleteSelected_Click(object sender, RoutedEventArgs e)
        {


            if ((relDataGrid.SelectedItems.Count == 0) || (relDataGrid.SelectedItems[0].GetType() != typeof(Relationship))) { return; }
            relDataGrid.EndEdit();

            foreach (Object item in relDataGrid.SelectedItems) 
            {
                _newList.Remove((Relationship)item);

            }
            clearDataGrids();
            dataBind();
        }

        private bool dataValid() 
        {

            foreach (Relationship rel1 in _newList)
            {
                foreach (Relationship rel2 in _newList)
                {
                    if ((rel1 != rel2) && (rel1.Name.Equals(rel2.Name)))
                    {
                        Utilities.MakeErrorDialog("Duplicate Relationship names are not allowed. Please choose unique names for each.", this);
                        return false;
                    }
                }
            }
            return true;
        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            relDataGrid.EndEdit();
            if (dataValid())
            {

     
                if (_parentEntity.GetType() == typeof(Character))
                {

                    Utilities.SynchronizeGlobalCharacterRelationships(_newList, _currentStoryData);
                }
                else if (_parentEntity.GetType() == typeof(Environment))
                {

                    Utilities.SynchronizeGlobalEnvironmentRelationships(_newList, _currentStoryData);
                }


                this.Close();
            }

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Closing -= new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Close();
        }


    }
}
