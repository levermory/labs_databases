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
using System.Windows.Shapes;

namespace LR8_3
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        DAL dal = new DAL();
        public Window1()
        {
            InitializeComponent();
        }

        private void submit(object sender, RoutedEventArgs e)
        {
            var _winner = winner.Text;
            var _looser = looser.Text;
            var _place = place.Text;
            var _date = date.Text;
            try
            {
                dal.NewGame(_winner, _looser, _place, _date);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Close();
        }
    }
}
