using System.Windows;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace LB9_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataSet shopDS = new DataSet("Shop");
        private string connStr = string.Empty;
        private MySqlDataAdapter manufApadter;
        private MySqlDataAdapter productAdapter;
        private MySqlDataAdapter custAdapter;
        private MySqlDataAdapter purchAdapter;

        private MySqlCommandBuilder manufCommands;
        private MySqlCommandBuilder productCommands;
        private MySqlCommandBuilder custCommands;
        private MySqlCommandBuilder purchCommands;
        public MainWindow()
        {
            InitializeComponent();
            connStr = ConfigurationManager.ConnectionStrings["shop"].ConnectionString;
            manufApadter = new MySqlDataAdapter("select * from manufact", connStr);
            productAdapter = new MySqlDataAdapter("select * from product", connStr);
            custAdapter = new MySqlDataAdapter("select * from customer", connStr);
            purchAdapter = new MySqlDataAdapter("select * from purchase", connStr);

            manufCommands = new MySqlCommandBuilder(manufApadter);
            productCommands = new MySqlCommandBuilder(productAdapter);
            custCommands = new MySqlCommandBuilder(custAdapter);
            purchCommands = new MySqlCommandBuilder(purchAdapter);


            manufApadter.Fill(shopDS, "manufact");
            productAdapter.Fill(shopDS, "product");
            custAdapter.Fill(shopDS, "customer");
            purchAdapter.Fill(shopDS, "purchase");

            DataRelation manufToProduct = new DataRelation("manuf_id_ref", shopDS.Tables["manufact"].Columns["id"],
                shopDS.Tables["product"].Columns["manuf_id"]);
            DataRelation custToPurch = new DataRelation("cust_id_ref", 
                shopDS.Tables["customer"].Columns["id"], 
                shopDS.Tables["purchase"].Columns["cust_id"]);
            DataRelation prodToPurch = new DataRelation("prod_id_ref",
                shopDS.Tables["product"].Columns["id"],
                shopDS.Tables["purchase"].Columns["prod_id"]);

            shopDS.Relations.Add(manufToProduct);
            shopDS.Relations.Add(custToPurch);
            shopDS.Relations.Add(prodToPurch);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = shopDS.Tables["manufact"].DefaultView;
            Type.ItemsSource = shopDS.Tables["product"].AsEnumerable().Select(u => u["type"]).Distinct();
            
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            if (infoGrid.SelectedItem != null)
            {
                var id = (int)(infoGrid.SelectedItem as DataRowView).Row.ItemArray[0];
                var result = shopDS.Tables["manufact"].Select($"id = {id}")[0].GetChildRows("manuf_id_ref");
                resultGrid.Visibility = Visibility.Visible;
                resultGrid.ItemsSource = result.CopyToDataTable().DefaultView;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                manufApadter.Update(shopDS, "manufact");
                productAdapter.Update(shopDS, "product");
                custAdapter.Update(shopDS, "customer");
                purchAdapter.Update(shopDS, "customer");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {

                //var result = shopDS.Tables["product"]
                //    .Select($"price >= {a.Text} and price <={b.Text} and type = '{Type.SelectedItem}'");
                var result = from m in shopDS.Tables["product"].AsEnumerable()
                             where (double)m["price"] >= Double.Parse(a.Text) && (double)m["price"] <= Double.Parse(b.Text) && (string)m["type"] == Type.SelectedItem.ToString()
                             select m;
                resultGrid.ItemsSource = result.CopyToDataTable().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = from t in shopDS.Tables["purchase"].AsEnumerable() 
                            join m in shopDS.Tables["customer"].AsEnumerable()
                            on   t["cust_id"] equals m["id"]
                            select new { Name = m["name"], Id = t["id"], Qua = t["amount"], Date = t["date"] };
                

                DataTable result = new DataTable();
                result.Columns.Add(new DataColumn("Name", System.Type.GetType("System.String")));
                result.Columns.Add(new DataColumn("ID", System.Type.GetType("System.Int16")));
                result.Columns.Add(new DataColumn("Date", System.Type.GetType("System.DateTime")));
                result.Columns.Add(new DataColumn("Amount", System.Type.GetType("System.Int16")));

                foreach (var x in query)
                {
                    var newRow = result.Rows.Add();
                    newRow.SetField("Name", x.Name);
                    newRow.SetField("ID", x.Id);
                    newRow.SetField("Date", x.Date);
                    newRow.SetField("Amount", x.Qua);
                }
                resultGrid.ItemsSource = result.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);    
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var query = from purch in shopDS.Tables["purchase"].AsEnumerable()
                        join cust in shopDS.Tables["customer"].AsEnumerable() on purch["cust_id"] equals cust["id"]
                        join prod in shopDS.Tables["product"].AsEnumerable() on purch["prod_id"] equals prod["id"]
                        group new { purch, cust, prod } by cust.Field<string>("name") into g
                        select new
                        {
                            custName = g.Key,
                            totalPrice = g.Sum(x => x.purch.Field<int>("amount") * x.prod.Field<double>("price"))
                        };
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn("Name", System.Type.GetType("System.String")));
            result.Columns.Add(new DataColumn("Price", System.Type.GetType("System.Double")));


            foreach (var x in query)
            {
                var newRow = result.Rows.Add();
                newRow.SetField("Name", x.custName);
                newRow.SetField("Price", x.totalPrice);
            }
            resultGrid.ItemsSource = result.DefaultView;


        }

        private void infoGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            using (StreamWriter file = File.AppendText(@"log.txt"))
            {
                if((infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString() != null)
                {
                    file.WriteLine($"({DateTime.Now}) row({(infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString()}) edited");
                }
                else
                {
                    file.WriteLine($"({DateTime.Now}) row({(infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString()}) created");
                }   
            }
            
        }

        private void infoGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(infoGrid.SelectedItems != null && e.Key == System.Windows.Input.Key.Delete)
            {
                using (StreamWriter file = File.AppendText(@"log.txt"))
                {
                    file.WriteLine($"({DateTime.Now}) row({(infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString()}) delete");
                }
            }
        }
    }
}