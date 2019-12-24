using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Sales.Files
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmMultilstEditor : Window
    {
        public ObservableCollection<MultiList> myList { get; set; }
        public frmMultilstEditor()
        {
            InitializeComponent();
        }

        public class MultiList
        {
            public string TheText { get; set; }
            public int TheValue { get; set; }
            public bool IsSelected { get; set; }
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
