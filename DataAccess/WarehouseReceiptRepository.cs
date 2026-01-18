using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class WarehouseReceiptRepository
    {
        private readonly string _conn;

        public WarehouseReceiptRepository()
        {
            _conn = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<WarehouseReceiptModel> GetAll()
        {
            var list = new List<WarehouseReceiptModel>();
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT * FROM WarehouseReceipt", conn);
            conn.Open();

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new WarehouseReceiptModel
                {
                    Id = rd["id"].ToString(),
                    InputDay = rd["inputDay"] as DateTime?,
                    Eid = rd["eid"].ToString(),
                    TotalImport = (int)rd["totalImport"]
                });
            }
            return list;
        }

        public void Insert(WarehouseReceiptModel wr)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                @"INSERT INTO WarehouseReceipt(id, inputDay, eid, totalImport)
          VALUES (@id, @day, @eid, @total)", conn);

            cmd.Parameters.AddWithValue("@id", wr.Id);
            cmd.Parameters.AddWithValue("@day", wr.InputDay);
            cmd.Parameters.AddWithValue("@eid", wr.Eid);
            cmd.Parameters.AddWithValue("@total", wr.TotalImport);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public string GenerateNewId()
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT MAX(id) FROM WarehouseReceipt", conn);
            conn.Open();

            var last = cmd.ExecuteScalar()?.ToString();
            if (string.IsNullOrEmpty(last)) return "W01";

            int num = int.Parse(last.Substring(1)) + 1;
            return $"W{num:D2}";
        }

    }

}
