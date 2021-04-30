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

namespace LB10_Curling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connStr = string.Empty;
        private DataSet Curling = new DataSet("curling");

        private MySqlDataAdapter Team;
        private MySqlDataAdapter Player;
        private MySqlDataAdapter TeamPlayer;

        private MySqlCommandBuilder TeamCmd;
        private MySqlCommandBuilder PlayerCmd;
        private MySqlCommandBuilder TeamPlayerCmd;

        public MainWindow()
        {
            InitializeComponent();

            connStr = ConfigurationManager.ConnectionStrings["curling"].ConnectionString;

            Team = new MySqlDataAdapter("select * from team", connStr);
            Player = new MySqlDataAdapter("select * from player", connStr);
            TeamPlayer = new MySqlDataAdapter("select * from team_player", connStr);

            TeamCmd = new MySqlCommandBuilder(Team);
            PlayerCmd = new MySqlCommandBuilder(Player);
            TeamPlayerCmd = new MySqlCommandBuilder(TeamPlayer);

            Team.Fill(Curling, "team");
            Player.Fill(Curling, "player");
            TeamPlayer.Fill(Curling, "team_player");

            DataRelation pToT = new DataRelation("player_id_ref", Curling.Tables["player"].Columns["id"], Curling.Tables["team_player"].Columns["player_id"]);
            DataRelation tToP = new DataRelation("team_id_ref", Curling.Tables["team"].Columns["id"], Curling.Tables["team_player"].Columns["team_id"]);
            Curling.Relations.Add(pToT);
            Curling.Relations.Add(tToP);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = Curling.Tables["team"].Select().CopyToDataTable().DefaultView;
            filterGrid.ItemsSource = Curling.Tables["player"].Select("weight >=70").CopyToDataTable().DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
