using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Linq;

namespace LR8_3
{
    public class DAL
    {
        private MySqlConnection _connection = null;
        private readonly string _connectionString;

        public DAL(string connString)
        {
            _connectionString = connString;
        }
        public DAL() : this(ConfigurationManager.ConnectionStrings["curling"].ConnectionString)
        {}

        public void OpenConnection()
        {
            _connection = new MySqlConnection(this._connectionString); 
            _connection.Open();
            
        }
        public void CloseConnection()
        {
            if(_connection?.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
        }

        public List<Team> showTeams()
        {
            List<Team> teams = new List<Team>();
            string command = "select * from team order by rating";
            OpenConnection();
            var cmd = new MySqlCommand(command, _connection);
            using (MySqlDataReader curRead = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (curRead.Read())
                {
                    teams.Add(new Team
                    {
                        id = (int)curRead["id"],
                        country = (string)curRead["country"],
                        points = (int)curRead["points"],
                        rating = (int)curRead["rating"]
                    });
                }
            }
            return teams;
        }
        public List<Player> showPlayers()
        {
            List<Player> players= new List<Player>();
            string command = "select * from player";
            OpenConnection();
            var cmd = new MySqlCommand(command, _connection);
            using (MySqlDataReader curRead = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (curRead.Read())
                {
                    players.Add(new Player
                    {
                        id = (int)curRead["id"],
                        name = (string)curRead["name"],
                        height = curRead.IsDBNull(2) ? 0 : (float)curRead["height"],
                        weight = curRead.IsDBNull(3) ? 0 : (float)curRead["weight"],
                        age = curRead.IsDBNull(4) ? 0 : (int)curRead["age"],
                        exp = curRead.IsDBNull(5) ? 0 : (int)curRead["exp"]
                    });
                }
            }
            return players;
        }
        public void deleteTeam(Team team)
        {
            string command = $"delete from team where id={team.id}";
            OpenConnection();
            var cmd = new MySqlCommand(command, _connection);
            cmd.ExecuteNonQuery(); ;
        }
        public void deletePlayer(Player player)
        {
            string command = $"delete from player where id={player.id}";
            OpenConnection();
            var cmd = new MySqlCommand(command, _connection);
            cmd.ExecuteNonQuery(); ;
        }
        public void NewGame(String winner, String looser, String place, String date)
        {
            OpenConnection();
            var cmd = new MySqlCommand("newGame", _connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_winner", winner);
            cmd.Parameters.AddWithValue("_looser", looser);
            cmd.Parameters.AddWithValue("_place", place);
            cmd.Parameters.AddWithValue("_date", date);
            var res = cmd.ExecuteNonQuery();
            CloseConnection();
        }
        public DataTable viewTeams()
        {
            OpenConnection();
            var cmd = new MySqlCommand(
                "select " +
                "player.name, " +
                "team.country " +
                "from team, player, team_player " +
                "where player.id = team_player.player_id " +
                "and team.id = team_player.team_id", _connection);
            var result = new DataTable();
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                result.Load(dr);
            }
            return result;
        }
        public void updateTeam(Team team, string col, string new_data)
        {
            try
            {
                string cmd = $"update team set {col}='{new_data}' where id={team.id}";
                if (col == "points")
                {
                    cmd += "; call updateRating();";
                }
                OpenConnection();
                var command = new MySqlCommand(cmd, _connection);
                command.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void updatePlayer(Player player, string col, string new_data)
        {
            try
            {
                string cmd = $"update player set {col}='{new_data}' where id={player.id}";
                OpenConnection();
                var command = new MySqlCommand(cmd, _connection);
                command.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void insertPlayer(Player newPlayer)
        {
            string cmd = $"insert into player " +
                $"(id) " +
                $"values ({newPlayer.id})";
            OpenConnection();
            var command = new MySqlCommand(cmd, _connection);
            command.ExecuteNonQuery();
            CloseConnection();
        }
        public void insertTeam(Team newTeam)
        {
            string cmd = $"insert into team " +
                $"(id) " +
                $"values ({newTeam.id})";
            OpenConnection();
            var command = new MySqlCommand(cmd, _connection);
            command.ExecuteNonQuery();
            CloseConnection();
        }
        public IEnumerable<Team> showChemp()
        {
            MySqlTransaction tx = null;
            IEnumerable<Team> chemp = null;
            try
            {
                this.OpenConnection();
                tx = _connection.BeginTransaction();
                List<Team> teams = new List<Team>();
                string command = "select * from team";
                var cmd = new MySqlCommand(command, _connection);
                cmd.Transaction = tx;
                using (MySqlDataReader curRead = cmd.ExecuteReader())
                {
                    while (curRead.Read())
                    {
                        teams.Add(new Team
                        {
                            id = (int)curRead["id"],
                            country = (string)curRead["country"],
                            points = (int)curRead["points"],
                            rating = (int)curRead["rating"]
                        });
                    }
                }
                tx.Commit();


                chemp = from t in teams where t.rating == 1 select t;
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                tx?.Rollback();
            }
            finally
            {
                CloseConnection();
                
            }
            return chemp;
        }
    }
    public class Team
    {
        public int id {get; set;}
        public string country { get; set; }
        public int points { get; set; }
        public int rating { get; set;}
    }
    public class Player
    {
        public int id { get; set; }
        public string name { get; set; }
        public float height { get; set; }
        public float weight { get; set; }
        public int age { get; set; }
        public int exp { get; set; }
    }
}