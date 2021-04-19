using System;
using System.Configuration;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;


namespace LR9_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
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
            var query = from player in Curling.Tables["player"].AsEnumerable()
                        join joint in Curling.Tables["team_player"].AsEnumerable()
                        on (int)player["id"] equals (int)joint["player_id"]
                        join team in Curling.Tables["team"].AsEnumerable()
                        on joint["team_id"] equals team["id"]
                        select new
                        {
                            Team = (string)team["country"],
                            Height = (float)player["height"],
                            Weight = (float)player["weight"]
                        } into temp
                        group temp by temp.Team into t
                        select new
                        {
                            Country = t.Key,
                            AvgH = t.Average(x => x.Height),
                            AvgW = t.Average(x => x.Weight)
                        };
            infoGrid.ItemsSource = query;
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                Team.Update(Curling, "team");
                Player.Update(Curling, "player");
                TeamPlayer.Update(Curling, "team_player");

               

                Team.Fill(Curling, "team");
                Player.Fill(Curling, "player");
                TeamPlayer.Fill(Curling, "team_player");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
