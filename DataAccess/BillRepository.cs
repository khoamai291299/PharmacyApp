using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class BillRepository
    {
        private readonly string _connString;

        public BillRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<BillModel> GetAll()
        {
            var list = new List<BillModel>();
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand("SELECT id, dateOfcreate, cid, eid, totalAmount FROM Bill", conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new BillModel
                {
                    Id = rd["id"] as string,
                    DateOfcreate = rd["dateOfcreate"] as DateTime?,
                    Cid = rd["cid"] as string,
                    Eid = rd["eid"] as string,
                    TotalAmount = rd["totalAmount"] == DBNull.Value ? 0 : (int)rd["totalAmount"]
                });
            }
            return list;
        }

        public void Add(BillModel bill)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(
                @"INSERT INTO Bill (id, dateOfcreate, cid, eid, totalAmount, PromotionId)
          VALUES (@id, @dateOfcreate, @cid, @eid, @totalAmount, @promotionId)", conn);

            cmd.Parameters.AddWithValue("@id", bill.Id);
            cmd.Parameters.AddWithValue("@dateOfcreate", bill.DateOfcreate);
            cmd.Parameters.AddWithValue("@cid", bill.Cid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@eid", bill.Eid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@totalAmount", bill.TotalAmount);
            cmd.Parameters.AddWithValue("@promotionId", bill.PromotionId ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }


        public void Update(BillModel bill)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(
                @"UPDATE Bill SET dateOfcreate=@dateOfcreate, cid=@cid, eid=@eid, totalAmount=@totalAmount 
                  WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", bill.Id);
            cmd.Parameters.AddWithValue("@dateOfcreate", bill.DateOfcreate);
            cmd.Parameters.AddWithValue("@cid", bill.Cid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@eid", bill.Eid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@totalAmount", bill.TotalAmount);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(string id)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand("DELETE FROM Bill WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public string GenerateNewId()
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand("SELECT MAX(id) FROM Bill", conn);
            conn.Open();
            var result = cmd.ExecuteScalar()?.ToString();
            if (string.IsNullOrEmpty(result)) return "B001";
            int number = int.Parse(result.Substring(1)) + 1;
            return $"B{number:D3}";
        }
    }
}
