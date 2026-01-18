using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class CustomerRepository
    {
        private readonly string _connString;

        public CustomerRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<CustomerModel> GetAll()
        {
            var list = new List<CustomerModel>();
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand("SELECT id, name, phone, address, tid, totalExpenditure, cumulativePoints FROM Customer", conn);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CustomerModel
                {
                    Id = rd["id"].ToString(),
                    Name = rd["name"].ToString(),
                    Phone = rd["phone"].ToString(),
                    Address = rd["address"].ToString(),
                    Tid = rd["tid"].ToString(),
                    TotalExpenditure = rd["totalExpenditure"] == DBNull.Value ? 0 : (int)rd["totalExpenditure"],
                    CumulativePoints = rd["cumulativePoints"] == DBNull.Value ? 0 : (int)rd["cumulativePoints"]
                });
            }
            return list;
        }

        public CustomerModel GetByPhone(string phone)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Customer WHERE phone=@phone", conn);
            cmd.Parameters.AddWithValue("@phone", phone);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new CustomerModel
            {
                Id = rd["id"].ToString(),
                Name = rd["name"].ToString(),
                Phone = rd["phone"].ToString(),
                Address = rd["address"].ToString(),
                Tid = rd["tid"].ToString(),
                TotalExpenditure = (int)rd["totalExpenditure"],
                CumulativePoints = (int)rd["cumulativePoints"]
            };
        }

        public void Add(CustomerModel m)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                @"INSERT INTO Customer(id, name, phone, address, tid, totalExpenditure, cumulativePoints)
                  VALUES(@id,@name,@phone,@address,@tid,@totalExpenditure,@cumulativePoints)", conn);
            AddParameters(cmd, m);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(CustomerModel m)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(

                @"UPDATE Customer SET
                        name=@name,
                        phone=@phone,
                        address=@address,
                        tid=@tid,
                        totalExpenditure=@totalExpenditure,
                        cumulativePoints=@cumulativePoints
                  WHERE id=@id", conn);
            AddParameters(cmd, m);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateNameAddress(string id, string name, string address)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "UPDATE Customer SET name=@name, address=@address WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", address ?? (object)DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(string id)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand("DELETE FROM Customer WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        //public void UpdateCumulativePoints(string id, int points)
        //{
        //    using var conn = new SqlConnection(_connString);
        //    var cmd = new SqlCommand("UPDATE Customer SET cumulativePoints=@points WHERE id=@id", conn);
        //    cmd.Parameters.AddWithValue("@id", id);
        //    cmd.Parameters.AddWithValue("@points", points);
        //    conn.Open();
        //    cmd.ExecuteNonQuery();
        //}

        private void AddParameters(SqlCommand cmd, CustomerModel m)
        {
            cmd.Parameters.AddWithValue("@id", m.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", m.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", m.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", m.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@tid", m.Tid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@totalExpenditure", m.TotalExpenditure);
            cmd.Parameters.AddWithValue("@cumulativePoints", m.CumulativePoints);
        }

        public string GenerateNewId()
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand("SELECT MAX(id) FROM Customer", conn);
            conn.Open();
            var result = cmd.ExecuteScalar()?.ToString();
            if (string.IsNullOrEmpty(result)) return "C01";
            int number = int.Parse(result.Substring(1)) + 1;
            return $"C{number:D2}";
        }

        public string GetDefaultTypeId()
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand("SELECT TOP 1 Id FROM TypeCustomer ORDER BY Id", conn);
            conn.Open();
            return cmd.ExecuteScalar()?.ToString();
        }
    }
}
