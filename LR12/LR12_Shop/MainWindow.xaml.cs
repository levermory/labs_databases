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
    public class Manufacturer
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

    public class Product
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
    }
    
}
