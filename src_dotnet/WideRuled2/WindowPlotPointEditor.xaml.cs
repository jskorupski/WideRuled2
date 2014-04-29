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
    /// Interaction logic for PlotPointEditorWindow.xaml
    /// </summary>
    public partial class WindowPlotPointEditor : Window
    {
     
        private PlotPoint _pP;
        private PlotPointType _ppType;
        private StoryData _currentStoryData;

        public WindowPlotPointEditor(PlotPoint existingPP, StoryData storyData)
        {

            InitializeComponent();
            _pP = existingPP;
            _ppType = storyData.findPlotPointTypeById(existingPP.TypeId);
            _currentStoryData = storyData;

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            dataBind();

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            traitDataGrid.EndEdit();

            this.DialogResult = true;

        }
 
        public PlotPoint PlotPointResult 
        {
            get { return _pP; }
            set { _pP = value; }

        }


        private void dataBind()
        {
            titleTextBlock.Text = _ppType.Description;
            traitDataGrid.ItemsSource = _pP.Traits;

        }
        private void clearDataGrids()
        {

            traitDataGrid.ClearValue(DataGridControl.ItemsSourceProperty);
            traitDataGrid.Items.Clear();
        }


      

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            traitDataGrid.EndEdit();
          
            this.DialogResult = true;

            this.Closing -= new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Close();
         

        }

    }
}
