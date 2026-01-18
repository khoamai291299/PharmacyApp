using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class PositionRepository
    {
        private readonly string _connString;

        public PositionRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<PositionModel> GetAll()
        {
            var list = new List<PositionModel>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, name FROM Position";
                SqlCommand cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PositionModel
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            return list;
        }

        public PositionModel GetById(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, name FROM Position WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new PositionModel
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1)
                    };
                }
            }
            return null;
        }

        public bool Insert(PositionModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "INSERT INTO Position(id, name) VALUES(@id, @name)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(PositionModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "UPDATE Position SET name=@name WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "DELETE FROM Position WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Exists(string id)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();
            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Position WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return (int)cmd.ExecuteScalar() > 0;
        }

        public string GenerateNewId()
        {
            var list = GetAll();
            if (list.Count == 0)
                return "P01";
            int max = list
                .Select(p => int.Parse(p.Id.Substring(1)))
                .Max();
            return $"P{(max + 1):D2}";
        }
    }
}
