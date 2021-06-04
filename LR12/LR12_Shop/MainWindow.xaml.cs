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

namespace LR12_Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Table("manufact")] public class Manufacturer
{
[Key]
public int manuf_id { get; set; } [Required]
public string manuf_name { get; set; } [Required]
             public string manuf_country { get; set; }
             public string manuf_phone { get; set; }
             public string manuf_adress { get; set; }
             public virtual ICollection<Product> Products { get; set; }
             public Manufacturer()
             {
Products = new List<Product>(); }
}
[Table("product")]
   
public class Product
         {
             [Key]
public int prod_id { get; set; } [Required]
public string prod_name { get; set; } public string prod_type { get; set; } public int prod_quantyty { get; set;} [Column("prod_manuf_ref_id")]
public int? manufId{ get; set; }
public virtual Manufacturer manuf { get; set; }
}

[DbConfigurationType(typeof(MySqlEFConfiguration))]
class ShopContext: DbContext
{
public ShopContext(): base("ShopDB") {
}
protected override void OnModelCreating(DbModelBuilder modelBuilder) {
modelBuilder.Entity<Manufacturer>()
.HasMany(p => p.Products)
.WithOptional(p => p.manuf).HasForeignKey(p=>p.manufId);
base.OnModelCreating(modelBuilder); }
             public DbSet<Product> Products { get; set; }
             public DbSet<Manufacturer> Manufacturers { get; set; }
}

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    private void Window_Loaded(object sender, RoutedEventArgs e) {
curCont = new ShopContext();
curCont.Manufacturers.Load();
mainGrid.ItemsSource = curCont.Manufacturers.Local.ToBindingList(); curCont.Products.Load();
dataGrid.ItemsSource = curCont.Products.Local.ToBindingList();
this.Closing += MainWindow_Closing; }
private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
curCont.Dispose(); }
}
