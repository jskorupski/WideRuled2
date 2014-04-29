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
    /// Interaction logic for EnvironmentEditorWindow.xaml
    /// </summary>
    public partial class WindowEnvironmentEditor : Window
    {
        private Environment _currentEntity;
        private StoryData _currentStoryData;

        public WindowEnvironmentEditor(Environment currEntity, StoryData currStoryData)
        {

            InitializeComponent();
            _currentEntity = currEntity;
            _currentStoryData = currStoryData;
        }



        public Environment CurrentEntity
        {
            get { return _currentEntity; }
            set { _currentEntity = value; }

        }

        public StoryData CurrentStoryData
        {
            get { return _currentStoryData; }
            set { _currentStoryData = value; }

        }

        private void clearDataGrids() {

            traitDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            traitDataGrid.Items.Clear();
            relDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            relDataGrid.Items.Clear();

        }
        private void bindData() 
        {
            if (_currentEntity == null) return;


            textBlockEnv.Text = _currentEntity.Name.ToString();
            traitDataGrid.ItemsSource = _currentEntity.Traits;
            relDataGrid.ItemsSource = _currentEntity.Relationships;

            relDataGrid.Columns["ToEnvironment"].CellEditor = this.FindResource("entityDataTypeEditor") as CellEditor;
            relDataGrid.Columns["ToEnvironment"].CellContentTemplate = this.FindResource("entityDataTypeCellDataTemplate") as DataTemplate;




        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bindData();
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            traitDataGrid.EndEdit();
            relDataGrid.EndEdit();
            this.Close();
        }

        private void btEditSharedTraits_Click(object sender, RoutedEventArgs e)
        {


            Window newWin = new WindowSharedTraitsEditor(_currentEntity, _currentStoryData);
            newWin.Owner = this;
            newWin.ShowDialog();
            clearDataGrids();
            bindData();
        }

        private void btEditSharedRels_Click(object sender, RoutedEventArgs e)
        {


            Window newWin = new WindowSharedRelationshipsEditor(_currentEntity, _currentStoryData);
            newWin.Owner = this;
            newWin.ShowDialog();
            clearDataGrids();
            bindData();
        }

        private void traitDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEditSharedTraits_Click(null, null);
        }

        private void relDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEditSharedRels_Click(null, null);
        }
    }
}