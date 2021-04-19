using System;
using System.Windows;
using System.Data;

namespace LR8_3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DAL dal = new DAL();
        public string curTable;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void newGame(object sender, RoutedEventArgs e)
        {
            Window1 newGameWindow = new Window1();
            newGameWindow.Owner = this;
            newGameWindow.Show();
        }

        private void showTeams(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = dal.showTeams();
            curTable = "team";
        }

        private void showPlayers(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = dal.showPlayers();
            curTable = "player";
        }

        private void deleteRow(object sender, RoutedEventArgs e)
        {
            try
            {
                if(infoGrid.SelectedItem != null)
                {
                    switch (curTable)
                    {
                        case "team":
                            var team = infoGrid.SelectedItem as Team;
                            dal.deleteTeam(team);
                            infoGrid.ItemsSource = dal.showTeams();
                            break;
                        case "player":
                            var player = infoGrid.SelectedItem as Player;
                            dal.deletePlayer(player);
                            infoGrid.ItemsSource = dal.showPlayers();
                            break;
                        default:
                            MessageBox.Show("Wrong table selected");
                            break;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void crossQuery(object sender, RoutedEventArgs e)
        {
            curTable = "default";
            DataTable dtTeams = dal.viewTeams();
            infoGrid.ItemsSource = dtTeams.DefaultView;
        }

        private void infoGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (infoGrid.SelectedItems != null && e.Key == System.Windows.Input.Key.Delete)
                {
                    switch (curTable)
                    {
                        case "team":
                            var team = infoGrid.SelectedItem as Team;
                            dal.deleteTeam(team);
                            infoGrid.ItemsSource = dal.showTeams();
                            break;
                        case "player":
                            var player = infoGrid.SelectedItem as Player;
                            dal.deletePlayer(player);
                            infoGrid.ItemsSource = dal.showPlayers();
                            break;
                        default:
                            MessageBox.Show("Wrong table selected");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void infoGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            try
            {
                switch (curTable)
                {
                    case "team":
                        var team = infoGrid.SelectedItem as Team;
                        if (team.id != 0)
                        {
                            var t = e.EditingElement as System.Windows.Controls.TextBox;
                            string col = infoGrid.CurrentCell.Column.Header.ToString();
                            string new_data = t.Text;
                            dal.updateTeam(team, col, new_data);
                        }
                        infoGrid.ItemsSource = dal.showTeams();
                        break;
                    case "player":
                        var player = infoGrid.SelectedItem as Player;
                        if (player.id != 0)
                        {
                            var t = e.EditingElement as System.Windows.Controls.TextBox;
                            string col = infoGrid.CurrentCell.Column.Header.ToString();
                            string new_data = t.Text;
                            dal.updatePlayer(player, col, new_data);
                        }

                        infoGrid.ItemsSource = dal.showPlayers();

                        break;
                    default:
                        MessageBox.Show("Wrong table selected");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = dal.showChemp();
        }
    }
}
