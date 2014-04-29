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
    /// Interaction logic for PlotPointTypeEditorWindow.xaml
    /// </summary>
    public partial class WindowPlotPointTypeEditor : Window
    {
       
        private PlotPointType _currentEntity;
        private StoryData _currentStoryData;
        private List<Trait> _newList;


        public WindowPlotPointTypeEditor(PlotPointType currentEntity, StoryData storyData)
        {

            InitializeComponent();
            _currentEntity = currentEntity;
            _currentStoryData = storyData;
            _newList = new List<Trait>(); 
            
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

            foreach(Trait tr in _currentEntity.Traits)
            {
                _newList.Add((Trait)tr.Clone());
            }
            //_newList.AddRange(_currentEntity.Traits);
            dataBind();

        }

        private void dataBind()
        {
            titleTextBlock.Text = _currentEntity.Name.ToString();
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
            _newList.Add(new Trait("NewTrait", 0, _currentStoryData.getNewId(), _currentStoryData));
            clearDataGrids();
            dataBind();
        }

        private void btDeleteSelected_Click(object sender, RoutedEventArgs e)
        {


            if ((traitDataGrid.SelectedItems.Count == 0) || (traitDataGrid.SelectedItems[0].GetType() != typeof(Trait))) { return; }

            traitDataGrid.EndEdit();
            foreach (Object item in traitDataGrid.SelectedItems)
            {
                
                _newList.Remove((Trait)item);

            }

            clearDataGrids();
            dataBind();
        }

        private bool dataValid()
        {
          
            foreach (Trait trait1 in _newList)
            {
               

                foreach (Trait trait2 in _newList)
                {
                    if ((trait1 != trait2) && (trait1.Name.Equals(trait2.Name)))
                    {
                        Utilities.MakeErrorDialog("Duplicate Trait names are not allowed. Please choose unique names for each.", this);
                        return false;
                    }

                }
            }

            return true;
        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            traitDataGrid.EndEdit();
            if (dataValid())
            {
                Utilities.SavePPTypeTraits(_currentEntity, _newList, _currentStoryData);
                Utilities.SynchronizeGlobalPlotPointsWithType(_currentEntity, _currentStoryData);
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
