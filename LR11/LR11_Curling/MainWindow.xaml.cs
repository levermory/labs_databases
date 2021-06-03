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
    
namespace LR11_Curling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    class Team
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int Points { get; set; }
        public int Rating { get; set; }
        public ICollection<Game> Games { get; set; }
        public ICollection<Player> Players { get; set; }
        public Team()
        {
            Games = new List<Game>();
            Players = new List<Player>();
        }
    }
    class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Exp { get; set; }
        public int? TeamId { get; set; }
        public Team Team;
        public Player()
        {

        }
    }
    class Game
    {
        public int Id { get; set; }
        public string Place { get; set; }
        public string Date { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public Team Winner { get; set; }

    }
    class CurlingContext : DbContext
    {
        public CurlingContext() : base("Curling")
        {

        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
    }
    public partial class MainWindow : Window
    {
        private CurlingContext Curling;
        public MainWindow()
        {
            using(CurlingContext db = new CurlingContext())
            {
                var sweden = new Team() { Country = "Sweden", Points = 89020, Rating = 1 };
                var canada = new Team() { Country = "Canada", Points = 75980, Rating = 2 };
                var usa = new Team() { Country = "Usa", Points = 71029, Rating = 3 };
                var switzerland = new Team() { Country = "Switzerland", Points = 65196, Rating = 4 };
                var schotland = new Team() { Country = "Schotland", Points = 52059, Rating = 5 };
                db.Teams.AddRange(new List<Team>() { sweden, canada, usa, switzerland, schotland });
                db.SaveChanges();
                var nicklas = new Player() { Name = "Nicklas Edin", Age = 35, Team = sweden };
                var oscar = new Player() { Name = "Oscar Ericsson", Age = 35, Team = sweden };
                var john = new Player() { Name = "John Shooster", Age = 35, Team = sweden };
                var rasmus = new Player() { Name = "Rasmus Vrano", Age = 35, Team = canada };
                var matt = new Player() { Name = "Matt Hamilton", Age = 35, Team = canada };
                var kevin = new Player() { Name = "Kevin Koe", Age = 35, Team = usa };
                var colton = new Player() { Name = "Colton Flash", Age = 35, Team = usa };
                var sven = new Player() { Name = "Sven Mishelle", Age = 35, Team = switzerland };
                var peter = new Player() { Name = "Peter de Cruz", Age = 35, Team = switzerland };
                var grant = new Player() { Name = "Grant Hardi", Age = 35, Team = schotland };
                var bobby = new Player() { Name = "Bobby Lemmi", Age = 35, Team = schotland };
                db.Players.AddRange(new List<Player>() { nicklas, oscar, john, rasmus, matt, kevin, colton, sven, peter, grant, bobby});
                db.Games.Add(new Game() { Place = "Torronto", Date = "2020/01/01", Team1 = sweden, Team2 = canada, Winner = sweden });
                db.SaveChanges();

            }
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Curling = new CurlingContext();
            Curling.Teams.Load();
            Curling.Players.Load();
            teamGrid.ItemsSource = Curling.Teams.Local.ToBindingList();
            playerGrid.ItemsSource = Curling.Players.Local.ToBindingList();

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Curling.Dispose();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            Curling.SaveChanges();
        }

        private void deleteTeam_Click(object sender, RoutedEventArgs e)
        {
            if(teamGrid.SelectedItems != null)
            {
                var toDelete = teamGrid.SelectedItems[0] as Team;
                Curling.Teams.Remove(toDelete);
            }
        }
    }
}
