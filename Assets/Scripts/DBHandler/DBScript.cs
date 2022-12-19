using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class DBScript : MonoBehaviour
{
    //Stores name of DataBase
    private string dbName = "URI=file:myDB.db";
    //string Tscore;
   // string Tname;
    private int x;
    public List<string> names = new List<string>();
    public List<string> scores = new List<string>();

    void Awake()
    {
        CreateDB();
        //AddScore("joakim",100);
        ReadLB();
    }
    private void Start()
    {
        //shows curser again
        Cursor.lockState = CursorLockMode.None;
    }

    public void CreateDB()
    {
        //creates connection to DataBase
        using(var connection = new SqliteConnection(dbName))
        {
            //opens connection (DUH)
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                //creates the table scoreboard if not already existing
                command.CommandText = "CREATE TABLE IF NOT EXISTS scoreboard(id INTEGER PRIMARY KEY,name VARCHAR(3), score INT)";
                //Execute Order 66
                command.ExecuteNonQuery();
            }
            //i wonder what this does  :             ^)
            connection.Close();
        }
    }

    public void AddScore(string name, int score){
          using(var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO scoreboard(name,score) VALUES('{name}',{score})";
                //Execute Order 66
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void ReadLB()
    {
        using(var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                //takes all the info from table
                command.CommandText = "SELECT * FROM scoreboard ORDER BY score DESC";
                //for every iterations in table
                using(IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        if (x < 5)
                        {
                            //Tname = reader["name"].ToString() + ' ' + reader["score"].ToString();
                            //Debug.Log(Tname);
                            names.Add(reader["name"].ToString());
                            scores.Add(reader["score"].ToString());
                            x++;
                        }
                    }
                    reader.Close();
                }
            }
            //i wonder what this does  :             ^)
            connection.Close();
        }
    }
}
