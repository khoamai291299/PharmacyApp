using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class TypeMedicineRepository
    {
        private readonly string _connString;

        public TypeMedicineRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<TypeMedicineModel> GetAll()
        {
            var list = new List<TypeMedicineModel>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, name, description FROM TypeMedicine";
                SqlCommand cmd = new SqlCommand(query, conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new TypeMedicineModel
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    });
                }
            }
            return list;
        }

        public TypeMedicineModel GetById(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT id, name, description FROM TypeMedicine WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new TypeMedicineModel
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    };
                }
            }
            return null;
        }

        public bool Insert(TypeMedicineModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query =
                    "INSERT INTO TypeMedicine(id, name, description) VALUES(@id, @name, @desc)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@desc", model.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(TypeMedicineModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query =
                    "UPDATE TypeMedicine SET name=@name, description=@desc WHERE id=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@desc", model.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "DELETE FROM TypeMedicine WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
