using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class RollCallRepository
    {
        private readonly string _connString;

        public RollCallRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<RollCallModel> GetAll()
        {
            var list = new List<RollCallModel>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, months, years FROM RollCall";
                SqlCommand cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new RollCallModel
                    {
                        Id = reader.GetInt32(0),
                        Months = reader.GetInt32(1),
                        Years = reader.GetInt32(2),
                    });
                }
            }
            return list;
        }

        public RollCallModel GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, months, years FROM RollCall WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new RollCallModel
                    {
                        Id = reader.GetInt32(0),
                        Months = reader.GetInt32(1),
                        Years = reader.GetInt32(2),
                    };
                }
            }
            return null;
        }

        public bool Insert(RollCallModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "INSERT INTO RollCall(months, years) VALUES(@months, @years)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@months", model.Months);
                cmd.Parameters.AddWithValue("@years", model.Years);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(RollCallModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "UPDATE RollCall SET months=@months, years=@years WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@months", model.Months);
                cmd.Parameters.AddWithValue("@years", model.Years);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "DELETE FROM RollCall WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
