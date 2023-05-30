using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace couch_mover_design
{
    /// <summary>
    /// Interaction logic for couch_mover_UI.xaml
    /// </summary>
    public partial class couch_mover_UI : UserControl
    {
        public couch_mover_UI()
        {
            InitializeComponent();
        }

        private void insertCouch_button_Click(object sender, RoutedEventArgs e) { }

        private void shiftCouch_button_Click(object sender, RoutedEventArgs e) { }

        private void acquireCSV_button_Click(object sender, RoutedEventArgs e) { }

        internal void enableInsertButton()
        {
            shiftCouch_button.IsEnabled = false;
            acquireCSV_button.IsEnabled = false;
        }

        internal void enableShiftButton()
        {
            insertCouch_button.IsEnabled = false;
            acquireCSV_button.IsEnabled = false;
        }

        internal void enableAcquireButton()
        {
            insertCouch_button.IsEnabled = false;
            shiftCouch_button.IsEnabled = false;
        }

        internal void displayDistanceToMove(double coarseDistance)
        {
            distanceToMove_Label.Content = coarseDistance.ToString();
        }
    }
}
