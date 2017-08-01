using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using Microsoft.Bot.Builder.Luis.Models;

namespace Bot_Application1
{
    [Serializable]
    public class Postgres
    {
        private string conn;



        public Postgres(string conn)
        {
            this.conn = conn;
        }



        public void PostgreSql(EntityRecommendation adressEmail)
        {
            NpgsqlConnection connection = new NpgsqlConnection(conn);
            connection.Open();
            string address = adressEmail.Entity;
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO saveemail (addres) VALUES (@email)", connection);
            command.Parameters.AddWithValue("@email", address);
            try
            {
                 command.ExecuteNonQuery();
                
            }
            finally
            {
                connection.Close();
            }
        }



        public  void PostgreSql(string numberPhone)
        {
            NpgsqlConnection connection = new NpgsqlConnection(conn);
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO numberphone (numberP) VALUES (@phone)", connection);
            command.Parameters.AddWithValue("@phone", numberPhone);
            try
            {
                command.ExecuteNonQuery();

            }
            finally
            {
                connection.Close();
            }
        }



        public  bool SearchEmail(EntityRecommendation adressEmail)
        {
            NpgsqlConnection connection = new NpgsqlConnection(conn);
            connection.Open();
            string address = adressEmail.Entity;
            NpgsqlCommand command = new NpgsqlCommand("Select  count(*) from saveemail where addres=@email", connection);
            command.Parameters.AddWithValue("@email", address);
            try
            {
                Int32 count = Convert.ToInt32(command.ExecuteScalar());
                if (count < 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                connection.Close();
            }
        }



        public  bool SearchPhone(string numberPhone)
        {
            NpgsqlConnection connection = new NpgsqlConnection(conn);
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("Select  count(*) from numberphone where numberP=@phone", connection);
            command.Parameters.AddWithValue("@phone", numberPhone);
            try
            {
                Int32 count = Convert.ToInt32(command.ExecuteScalar());
                if (count < 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}