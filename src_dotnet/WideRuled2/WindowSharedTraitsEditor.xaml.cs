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
    /// Interaction logic for SharedTraitsEditorWindow.xaml
    /// </summary>
    public partial class WindowSharedTraitsEditor : Window
    {
        private StoryEntity _parentEntity;
        private StoryData _currentStoryData;
        private List<Trait> newList;


        public WindowSharedTraitsEditor(StoryEntity parentEntity, StoryData storyData)
        {

            InitializeComponent();
            _parentEntity = parentEntity;
            _currentStoryData = storyData;
            newList = new List<Trait>(); 
            
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            relDataGrid.EndEdit();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            foreach(Trait tr in _parentEntity.Traits)
            {

                newList.Add((Trait)tr.Clone());
            }
            //newList.AddRange(_parentEntity.Traits);
            dataBind();

        }

        private void dataBind()
        {
            relDataGrid.ItemsSource = newList;
            relDataGrid.Columns["TypeString"].CellEditor = this.FindResource("entityDataTypeEditor") as CellEditor;
            relDataGrid.Columns["TypeString"].CellContentTemplate = this.FindResource("entityDataTypeCellDataTemplate") as DataTemplate;


        }
        private void clearDataGrids()
        {

            relDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            relDataGrid.Items.Clear();
        }
        private void btAddNew_Click(object sender, RoutedEventArgs e)
        {
            relDataGrid.EndEdit();
            newList.Add(new Trait("NewTrait", 0, _currentStoryData.getNewId(), _currentStoryData));
            clearDataGrids();
            dataBind();
        }

        private int numberOfNameTraits()
        {
            int nameCount = 0;
            foreach (Trait tr in newList)
            {
                if(tr.Name == "Name")
                {
                    nameCount++;
                }
            }
            return nameCount;
        }
        private void btDeleteSelected_Click(object sender, RoutedEventArgs e)
        {


            if ((relDataGrid.SelectedItems.Count == 0) || (relDataGrid.SelectedItems[0].GetType() != typeof(Trait))) { return; }
            relDataGrid.EndEdit();
            foreach (Object item in relDataGrid.SelectedItems)
            {
                if( (_parentEntity.GetType() == typeof(Character)) &&
                    (((Trait)item).Name == "Name") && 
                    (numberOfNameTraits() == 1)
                  )
                {
                    Utilities.MakeErrorDialog("A \"Name\" attribute is required. Could not delete from shared traits list", this);
                }
                else if ((_parentEntity.GetType() == typeof(Environment)) &&
                    (((Trait)item).Name == "Name") &&
                    (numberOfNameTraits() == 1))
                {
                    Utilities.MakeErrorDialog("A \"Name\" attribute is required. Could not delete from shared traits list", this);
                }
                else
                {
                    newList.Remove((Trait)item);

                }
            }

            clearDataGrids();
            dataBind();
        }

        private bool dataValid()
        {
            bool nameFound = false;
            foreach (Trait trait1 in newList)
            {
                if (trait1.Name.Equals("Name") && (trait1.Type == TraitDataType.Text))
                {
                    nameFound = true;
                }

                foreach (Trait trait2 in newList)
                {
                    if ((trait1 != trait2) && (trait1.Name.Equals(trait2.Name)))
                    {
                        Utilities.MakeErrorDialog("Duplicate Trait names are not allowed. Please choose unique names for each.", this);
                        return false;
                    }

                }
            }

            if(!nameFound)
            {
                Utilities.MakeErrorDialog("There must be a \"Name\" trait with Text as its type", this);
                return false;

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

                    Utilities.SynchronizeGlobalCharacterTraits(newList, _currentStoryData);
                }
                else if (_parentEntity.GetType() == typeof(Environment))
                {

                    Utilities.SynchronizeGlobalEnvironmentTraits(newList, _currentStoryData);
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
