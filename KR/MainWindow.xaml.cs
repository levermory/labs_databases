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
using System.Data;


//Сайт знакомств
//Вариант 9.
//connected layer
//CRUD – интерфейс без кнопок. С автоматическим доставлением
//изменений в БД, при изменении DATAgrid.

//Реализуйте возможность ввода новой строки в List<> через консоль,
//по нажатию комбинации клавиш Ctrl + 6. Предложите пользователю в консоли
//доставить изменения в БД. Если ответ положительный - доставьте. 



namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DAL da = new DAL();
        string connString = "server=localhost;uid=root;pwd=12345;database=tinder";
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("window loaded");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            da.OpenConnection(connString);
            infoGrid.ItemsSource = da.GetUsers().DefaultView;
            da.CloseConnection();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            da.OpenConnection(connString);           
            infoGrid.ItemsSource = da.GetMatches().DefaultView;
            da.CloseConnection();

        }

        private void infoGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (infoGrid.SelectedItem != null)
            {
                try
                {
                    if ((infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString() != "")
                    {
                        string newData = ((TextBox)e.EditingElement).Text;
                        string col = infoGrid.CurrentCell.Column.Header.ToString();
                        string id = (infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString();
                        da.OpenConnection(connString);
                        da.UpdateTable(id, col, newData);
                        da.CloseConnection();
                    }
                    else
                    {
                        string newID = ((TextBox)e.EditingElement).Text;
                        da.OpenConnection(connString);
                        da.CreateUser(newID);
                        da.CloseConnection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void infoGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(infoGrid.SelectedItem != null && e.Key.ToString() == "Delete")
            {
                da.OpenConnection(connString);
                da.DeleteRow((infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString());
                infoGrid.ItemsSource = da.GetUsers().DefaultView;
                da.CloseConnection();
            }
            else if(e.Key == Key.LeftCtrl)
            {
                Console.WriteLine("hello world");
                //List<User> list = new List<User>();
                //Console.Write("ID: ");
                //string newID = Console.ReadLine();
                //Console.Write("Name: ");
                //string newName = Console.ReadLine();
                //Console.Write("Age: ");
                //string newAge = Console.ReadLine();
                //Console.Write("Sex: ");
                //string newSex = Console.ReadLine();
                //list.Add(new User(Convert.ToInt16(newID), (string)newName, Convert.ToInt16(newAge), (string)newSex));
            }
        }
    }
}