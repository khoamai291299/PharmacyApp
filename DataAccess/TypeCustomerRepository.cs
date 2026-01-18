using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class TypeCustomerRepository
    {
        private readonly string _connString;

        public TypeCustomerRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<TypeCustomerModel> GetAll()
        {
            var list = new List<TypeCustomerModel>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, name, minimumLevel, maximumLevel FROM TypeCustomer";
                SqlCommand cmd = new SqlCommand(query, conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new TypeCustomerModel
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        MinimumLevel = reader.GetInt32(2),
                        MaximumLevel = reader.GetInt32(3)
                    });
                }
            }
            return list;
        }

        public TypeCustomerModel GetById(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, name, minimumLevel, maximumLevel FROM TypeCustomer WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new TypeCustomerModel
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        MinimumLevel = reader.GetInt32(2),
                        MaximumLevel = reader.GetInt32(3)
                    };
                }
            }
            return null;
        }

        public bool Insert(TypeCustomerModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query =
                    "INSERT INTO TypeCustomer(id, name, minimumLevel, maximumLevel) VALUES(@id, @name, @min, @max)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@min", model.MinimumLevel);
                cmd.Parameters.AddWithValue("@max", model.MaximumLevel);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(TypeCustomerModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query =
                    "UPDATE TypeCustomer SET name=@name, minimumLevel=@min, maximumLevel=@max WHERE id=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@min", model.MinimumLevel);
                cmd.Parameters.AddWithValue("@max", model.MaximumLevel);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "DELETE FROM TypeCustomer WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
