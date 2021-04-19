using System;
using System.Collections.Generic;
using System.Windows;
using System.Data;

namespace LR8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DAL dal = new DAL();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Student> itemList = dal.GetAllStudents();
                studentsGrid.ItemsSource = itemList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            if(studentsGrid.SelectedItem != null)
            {
                Student dv = (Student)studentsGrid.SelectedItems[0];
                dal.updateStudent(dv);
                onLoad(null, null);
            }
        }

        private void showExams(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dtExams = dal.showExams();
                examsGrid.ItemsSource = dtExams.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
