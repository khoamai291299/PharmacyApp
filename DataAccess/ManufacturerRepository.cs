using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class ManufacturerRepository
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;

        public List<ManufacturerModel> GetAll()
        {
            var list = new List<ManufacturerModel>();

            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "SELECT id, name, country FROM Manufacturer", conn);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ManufacturerModel
                {
                    Id = rd["id"] as string,
                    Name = rd["name"] as string,
                    Country = rd["country"] as string
                });
            }

            return list;
        }

        public bool Exists(string id)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Manufacturer WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public void Add(ManufacturerModel m)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                @"INSERT INTO Manufacturer(id, name, country)
                  VALUES(@id,@name,@country)", conn);

            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@name", m.Name);
            cmd.Parameters.AddWithValue("@country", m.Country);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(ManufacturerModel m)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                @"UPDATE Manufacturer
                  SET name=@name, country=@country
                  WHERE id=@id", conn);

            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@name", m.Name);
            cmd.Parameters.AddWithValue("@country", m.Country);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(string id)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "DELETE FROM Manufacturer WHERE id=@id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public string GenerateNewId()
        {
            var list = GetAll();
            if (list.Count == 0)
                return "M01";
            int max = list
                .Select(m => int.Parse(m.Id.Substring(1)))
                .Max();
            return $"M{(max + 1):D2}";
        }
    }
}
