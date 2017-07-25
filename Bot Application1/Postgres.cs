using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using Microsoft.Bot.Builder.Luis.Models;

namespace Bot_Application1
{
    public class Postgres
    {
        public static void PostgresSql(EntityRecommendation adressemail)
        {
            string conn = "Server=emailpostgres.postgres.database.azure.com;Port=5432;User Id=Evgen@emailpostgres;Password=1234Qwer;Database=myemaildb";
            NpgsqlConnection connection = new NpgsqlConnection(conn);
            connection.Open();
            string address = adressemail.Entity;
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

        public static bool SearchEmail(EntityRecommendation adressemail)
        {
            string conn = "Server=emailpostgres.postgres.database.azure.com;Port=5432;User Id=Evgen@emailpostgres;Password=1234Qwer;Database=myemaildb";
            NpgsqlConnection connection = new NpgsqlConnection(conn);
            connection.Open();
            string address = adressemail.Entity;
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

    }
}