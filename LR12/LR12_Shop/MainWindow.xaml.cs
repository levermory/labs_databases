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
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySql.Data.MySqlClient;
using MySql.Data.EntityFramework;


namespace LR12_Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Table("manufact")]
    class Manufacturer
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string country { get; set; }
        public string phone { get; set; }
        public string adress { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Manufacturer()
        {
            Products = new List<Product>();
        }
    }
    [Table("product")]

    class Product
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string type { get; set; }
        public int quantity { get; set; }
        [Column("manuf_id")]
        public int? manuf_id { get; set; }
        public virtual Manufacturer manuf { get; set; }
        public ICollection<Purchase> purchases { get; set; }
        public Product()
        {
            purchases = new List<Purchase>();
        }
    }
    [Table("customer")]
    class Customer
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public ICollection<Purchase> buys { get; set; }
        public Customer()
        {
            buys = new List<Purchase>();
        }
    }
    [Table("purchase")]
    class Purchase
    {
        [Key]
        public int id { get; set; }
        [Column("cust_id")]
        public int? cust_id { get; set; }
        public virtual Customer cust { get; set; }
        [Column("prod_id")]
        public int? prod_id { get; set; }
        public virtual Product prod { get; set; }
        public int amount { get; set; }
        public DateTime date { get; set; }

    }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    class ShopContext : DbContext
    {
        public ShopContext() : base("ShopDB")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Manufacturer>()
            .HasMany(p => p.Products)
            .WithOptional(p => p.manuf).HasForeignKey(p => p.manuf_id);
            modelBuilder.Entity<Customer>().HasMany(c => c.buys).WithRequired(p => p.cust).HasForeignKey(p => p.cust_id);
            modelBuilder.Entity<Product>().HasMany(c => c.purchases).WithRequired(p => p.prod).HasForeignKey(p => p.prod_id);

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
    }

    public partial class MainWindow : Window
    {
        private ShopContext curCont;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            curCont = new ShopContext();
            curCont.Manufacturers.Load();
            mainGrid.ItemsSource = curCont.Manufacturers.Local.ToBindingList();
            curCont.Products.Load();
            dataGrid.ItemsSource = curCont.Products.Local.ToBindingList();
            this.Closing += MainWindow_Closing;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            curCont.Dispose();
        }

        private void mainGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if(mainGrid.SelectedItems != null)
                {
                    var toDelete = mainGrid.SelectedItems[0] as Manufacturer;
                    curCont.Manufacturers.Remove(toDelete);
                }
            }
        }

        private void updButton_Click(object sender, RoutedEventArgs e)
        {
            curCont.SaveChanges();
            Window_Loaded(null, null);
        }
    }
    
}
