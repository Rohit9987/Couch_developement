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
using VMS.TPS;

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
            Couch_ComboBox.SelectedIndex = 1;
        }

        private void insertCouch_button_Click(object sender, RoutedEventArgs e)
        {
            VMS.TPS.Script.insertCouch(Couch_ComboBox.SelectedIndex);
        }

        private void shiftCouch_button_Click(object sender, RoutedEventArgs e)
        {

            VMS.TPS.Script.moveCouch(double.Parse(distanceToMove_Label.Content.ToString()));
        }

        private void acquireCSV_button_Click(object sender, RoutedEventArgs e) { }

        internal void enableInsertButton()
        {
            shiftCouch_button.IsEnabled = false;
            acquireCSV_button.IsEnabled = false;
            insertCouch_button.IsEnabled = true;
        }

        internal void enableShiftButton()
        {
            insertCouch_button.IsEnabled = false;
            acquireCSV_button.IsEnabled = false;
            shiftCouch_button.IsEnabled = true;
            
        }

        internal void enableAcquireButton()
        {
            insertCouch_button.IsEnabled = false;
            shiftCouch_button.IsEnabled = false;
            acquireCSV_button.IsEnabled = true;
        }

        internal void displayDistanceToMove(double coarseDistance)
        {
            distanceToMove_Label.Content = coarseDistance.ToString();
        }

        internal void updateCollisionMessage(string message)
        {
            collision_textBlock.Text = message;
        }
    }
}
