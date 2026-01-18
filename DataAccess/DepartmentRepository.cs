using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class DepartmentRepository
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;

        public List<DepartmentModel> GetAll()
        {
            var list = new List<DepartmentModel>();

            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand("SELECT id, name FROM Department", conn);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new DepartmentModel
                {
                    Id = rd["id"] as string,
                    Name = rd["name"] as string
                });
            }

            return list;
        }

        public bool Exists(string id)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Department WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public void Add(DepartmentModel m)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "INSERT INTO Department(id, name) VALUES(@id,@name)", conn);

            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@name", m.Name);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(DepartmentModel m)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "UPDATE Department SET name=@name WHERE id=@id", conn);

            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@name", m.Name);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(string id)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "DELETE FROM Department WHERE id=@id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public string GenerateNewId()
        {
            var list = GetAll();
            if (list.Count == 0)
                return "D01";
            int max = list
                .Select(d => int.Parse(d.Id.Substring(1)))
                .Max();
            return $"D{(max + 1):D2}";
        }
    }
}
