using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using System.Windows;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;


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
        private MySqlDataAdapter Game;

        private MySqlCommandBuilder TeamCmd;
        private MySqlCommandBuilder PlayerCmd;
        private MySqlCommandBuilder TeamPlayerCmd;
        private MySqlCommandBuilder GameCmd;

        List<string> cols = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            connStr = ConfigurationManager.ConnectionStrings["curling"].ConnectionString;

            Team = new MySqlDataAdapter("select * from team", connStr);
            Player = new MySqlDataAdapter("select * from player", connStr);
            TeamPlayer = new MySqlDataAdapter("select * from team_player", connStr);
            Game = new MySqlDataAdapter("select * from game", connStr);

            TeamCmd = new MySqlCommandBuilder(Team);
            PlayerCmd = new MySqlCommandBuilder(Player);
            TeamPlayerCmd = new MySqlCommandBuilder(TeamPlayer);
            GameCmd = new MySqlCommandBuilder(Game);

            Team.Fill(Curling, "team");
            Player.Fill(Curling, "player");
            TeamPlayer.Fill(Curling, "team_player");
            Game.Fill(Curling, "game");

            Curling.Tables["player"].PrimaryKey = new DataColumn[] { Curling.Tables["player"].Columns["id"] };
            Curling.Tables["team"].PrimaryKey = new DataColumn[] { Curling.Tables["team"].Columns["id"] };
            Curling.Tables["game"].PrimaryKey = new DataColumn[] { Curling.Tables["game"].Columns["id"] };

            Curling.Tables["game"].Columns["id"].AutoIncrement = true;
            var lastID = (from game in Curling.Tables["game"].AsEnumerable()
                         select game["id"]).Max();
            Curling.Tables["game"].Columns["id"].AutoIncrementSeed = (int)lastID + 1;
            Curling.Tables["game"].Columns["id"].AutoIncrementStep = 1;

            DataRelation pToT = new DataRelation("player_id_ref", Curling.Tables["player"].Columns["id"], Curling.Tables["team_player"].Columns["player_id"]);
            DataRelation tToP = new DataRelation("team_id_ref", Curling.Tables["team"].Columns["id"], Curling.Tables["team_player"].Columns["team_id"]);
            Curling.Relations.Add(pToT);
            Curling.Relations.Add(tToP);

            pToT.ChildKeyConstraint.DeleteRule = Rule.Cascade;
            tToP.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            pToT.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            tToP.ChildKeyConstraint.UpdateRule = Rule.Cascade;


        }
        

        private void showProperty(string colname)
        {
            string colproperty = "";
            colproperty += "Name -> " + Curling.Tables["player"].Columns[colname].ColumnName + "\n";
            colproperty += "Type -> " + Curling.Tables["player"].Columns[colname].DataType + "\n";
            colproperty += "Allow NULL -> " + Curling.Tables["player"].Columns[colname].AllowDBNull + "\n";
            colproperty += "Autoincrement -> " + Curling.Tables["player"].Columns[colname].AutoIncrement + "\n";
            colproperty += "Unique -> " + Curling.Tables["player"].Columns[colname].Unique + "\n";
            colproperty += "Number of primary keys -> " + Curling.Tables["player"].PrimaryKey.Length + "\n";
            Console.WriteLine(colproperty);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Team.Fill(Curling, "team");
            Player.Fill(Curling, "player");
            TeamPlayer.Fill(Curling, "team_player");
            Game.Fill(Curling, "game");

            infoGrid.ItemsSource = Curling.Tables["team"].Select().CopyToDataTable().DefaultView;
            gameGrid.ItemsSource = Curling.Tables["game"].Select().CopyToDataTable().DefaultView;
            gameGrid.Columns[0].Visibility = Visibility.Hidden;


            foreach (var column in Curling.Tables["player"].Columns)
            {
                cols.Add(column.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach(string col in cols)
            {
                showProperty(col);
            }   
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Curling.RejectChanges();
            Window_Loaded(null, null);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Team.Update(Curling, "team");
            Player.Update(Curling, "player");
            TeamPlayer.Update(Curling, "team_player");
            Game.Update(Curling, "game");
            Window_Loaded(null, null);
        }

        private void infoGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {

            string newData = ((TextBox)e.EditingElement).Text;
            var col = infoGrid.CurrentCell.Column.DisplayIndex;
            var rowChange = infoGrid.SelectedItems[0] as DataRowView;
            rowChange.Row.BeginEdit();
            rowChange[col] = newData;
            rowChange.Row.EndEdit();
        }

        private void infoGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(infoGrid.SelectedItems != null && e.Key == System.Windows.Input.Key.Delete)
            {
                Console.WriteLine((infoGrid.SelectedItems[0] as DataRowView).Row.RowState.ToString());
                (infoGrid.SelectedItems[0] as DataRowView).Row.Delete();
            }

        }

        private void gameGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void gameGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {

        }

        private void infoGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(infoGrid.SelectedItems.Count != 0)
            {
                try
                {
                    Console.WriteLine((infoGrid.SelectedItems[0] as DataRowView).Row.RowState.ToString());
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
        }

        private void gameGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
            try
            {
                Console.WriteLine((gameGrid.SelectedItems[0] as DataRowView).Row.RowState.ToString());
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

        private void infoGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {

        }
    }
}
