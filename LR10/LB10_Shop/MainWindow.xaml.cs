using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;


namespace LB10_Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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

            //new options
            shopDS.Tables["manufact"].PrimaryKey = new DataColumn[]{shopDS.Tables["manufact"].Columns["id"]};
            shopDS.Tables["product"].PrimaryKey = new DataColumn[]{shopDS.Tables["product"].Columns["id"]};

            shopDS.Tables["product"].Columns["id"].AutoIncrement = true;
            var lastId = (from m in shopDS.Tables["product"].AsEnumerable()
                select m["id"]).Max(); 
            shopDS.Tables["product"].Columns["id"].AutoIncrementSeed = (int)lastId + 1;
            shopDS.Tables["product"].Columns["id"].AutoIncrementStep = 1;

            shopDS.Tables["manufact"].Columns["id"].AutoIncrement = true;
            lastId = (from m in shopDS.Tables["manufact"].AsEnumerable()
                select m["id"]).Max(); 
            shopDS.Tables["manufact"].Columns["id"].AutoIncrementSeed = (int)lastId + 1;
            shopDS.Tables["manufact"].Columns["id"].AutoIncrementStep = 1;
            //end new options

            DataRelation manufToProduct = new DataRelation("manuf_id_ref", 
                shopDS.Tables["manufact"].Columns["id"],
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

           manufToProduct.ChildKeyConstraint.DeleteRule = Rule.Cascade;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = shopDS.Tables["manufact"].DefaultView;
            resGrid.ItemsSource = shopDS.Tables["product"].DefaultView;
        }

        private void showProperty(string colname)
        {
            string colproperty = "";
            colproperty += "Name - > " + shopDS.Tables["product"].Columns[colname].ColumnName + "\n";
            colproperty += "Type - > " + shopDS.Tables["product"].Columns[colname].DataType + "\n";
            colproperty += "Allow NULL - > " + shopDS.Tables["product"].Columns[colname].AllowDBNull + "\n";
            colproperty += "Autoincrement - > " + shopDS.Tables["product"].Columns[colname].AutoIncrement + "\n";
            colproperty += "Unique - > " + shopDS.Tables["product"].Columns[colname].Unique + "\n";
            colproperty += "Number of primary keys - >" + shopDS.Tables["product"].PrimaryKey.Length + "\n";
            MessageBox.Show(colproperty);
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e) 
        {
            RadioButton curButton = (RadioButton)sender;
            showProperty(curButton.Content.ToString()); 
        }

        private void deletebutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (infoGrid.SelectedItems[0] as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                infoGrid.SelectedItems.Clear();
            }
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            shopDS.RejectChanges();
        }

        //private void update_withLINQ_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var prodId = (int)(infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray[0];
        //        (from p in shopDS.Tables["product"].AsEnumerable()
        //            where p.Field<int>("prod_id") == prodId
        //            select p)
        //            .FirstOrDefault().SetField("prod_quantyty", updateBOX.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        manufGrid.SelectedItems.Clear();
        //    }
        //}

        private void update_withLINQ_Click(object sender, RoutedEventArgs e)
        {
            (from p in shopDS.Tables["product"].AsEnumerable()
             where p.Field<int>("quantity") > 19
             select p)
                .ToList<DataRow>().ForEach(row => { row["quantity"] = 15; });
            resGrid.ItemsSource = shopDS.Tables["product"].DefaultView;
        }

        private void infoGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            using (StreamWriter file = File.AppendText(@"log.txt"))
            {

                if ((infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString() != "")
                {
                    file.WriteLine($"({DateTime.Now}) row({(infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString()}) edited");
                }
                else
                {
                    file.WriteLine($"({DateTime.Now}) new row created");
                }
            }

        }

        private void infoGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (infoGrid.SelectedItems != null && e.Key == System.Windows.Input.Key.Delete)
            {
                using (StreamWriter file = File.AppendText(@"log.txt"))
                {
                    file.WriteLine($"({DateTime.Now}) row({(infoGrid.SelectedItems[0] as DataRowView).Row.ItemArray.GetValue(0).ToString()}) delete");
                }
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
    }
}
