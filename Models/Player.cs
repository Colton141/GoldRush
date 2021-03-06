using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace GoldRush.Models
{
  public class Player
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int PlayerGold { get; set; }


    public Player( string name, int playerGold = 0, int id = 0)
    {
      Id = id;
      Name = name;
      PlayerGold = playerGold;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO `players` (`name`, `player_gold`) VALUES (@playersName, @playersPlayerGold);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@playersName";
      name.Value = this.Name;

      MySqlParameter PlayerGold = new MySqlParameter();
      PlayerGold.ParameterName = "@playersPlayerGold";
      PlayerGold.Value = this.PlayerGold;

      cmd.Parameters.Add(name);
      cmd.Parameters.Add(PlayerGold);
      cmd.ExecuteNonQuery();
      Id = (int) cmd.LastInsertedId;
      // more logic will go here in a moment
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public static List<Player> GetAll()
    {
      List<Player> allPlayers = new List<Player> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM players ORDER BY player_gold DESC;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
        string name =  rdr.GetString(1);
        int playerGold = rdr.GetInt32(2);
        int id = rdr.GetInt32(0);

        Player newPlayer = new Player(name, playerGold, id);
        allPlayers.Add(newPlayer);
      }
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
      return allPlayers;
    }

    public static Player Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM players WHERE id = (@searchId);", conn);
      cmd.Parameters.AddWithValue("@searchId", id);
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      int PlayerId = 0;
      string PlayerName = "";
      int PlayerGolds = 0;
      while(rdr.Read())
      {
        PlayerId = rdr.GetInt32(0);
        PlayerName = rdr.GetString(1);
        PlayerGolds = rdr.GetInt32(2);
      }
      Player newPlayer = new Player(PlayerName, PlayerGolds, PlayerId);
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return newPlayer;
    }

    public void Edit(int newGold)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"UPDATE players SET player_gold = @newGold WHERE id = @searchId;", conn);
      cmd.Parameters.AddWithValue("@searchId", Id);
      cmd.Parameters.AddWithValue("@newGold", newGold);
      cmd.ExecuteNonQuery();
      PlayerGold = newGold;
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

  }
}
