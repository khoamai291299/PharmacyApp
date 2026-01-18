using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class WarehouseDetailsRepository
    {
        private readonly string _connString;

        public WarehouseDetailsRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<WarehouseDetailsModel> GetAll()
        {
            var list = new List<WarehouseDetailsModel>();
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT wid, mid, quantity, unitPrice, totalAmount FROM WarehouseDetails";
                var cmd = new SqlCommand(sql, conn);
                var rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    list.Add(new WarehouseDetailsModel
                    {
                        Wid = rd["wid"].ToString(),
                        Mid = rd["mid"].ToString(),
                        Quantity = (int)rd["quantity"],
                        UnitPrice = (int)rd["unitPrice"],
                        TotalAmount = (int)rd["totalAmount"]
                    });
                }
            }
            return list;
        }

        public WarehouseDetailsModel GetById(string wid, string mid)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT wid, mid, quantity, unitPrice, totalAmount FROM WarehouseDetails WHERE wid=@wid AND mid=@mid";
                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@wid", wid);
                cmd.Parameters.AddWithValue("@mid", mid);

                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    return new WarehouseDetailsModel
                    {
                        Wid = rd["wid"].ToString(),
                        Mid = rd["mid"].ToString(),
                        Quantity = (int)rd["quantity"],
                        UnitPrice = (int)rd["unitPrice"],
                        TotalAmount = (int)rd["totalAmount"]
                    };
                }
            }
            return null;
        }

        public bool Insert(WarehouseDetailsModel model)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = @"INSERT INTO WarehouseDetails(wid, mid, quantity, unitPrice, totalAmount)
                               VALUES(@wid, @mid, @quantity, @unitPrice, @totalAmount)";
                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@wid", model.Wid);
                cmd.Parameters.AddWithValue("@mid", model.Mid);
                cmd.Parameters.AddWithValue("@quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@unitPrice", model.UnitPrice);
                cmd.Parameters.AddWithValue("@totalAmount", model.TotalAmount);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(WarehouseDetailsModel model)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = @"UPDATE WarehouseDetails SET
                                quantity=@quantity,
                                unitPrice=@unitPrice,
                                totalAmount=@totalAmount
                               WHERE wid=@wid AND mid=@mid";

                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@wid", model.Wid);
                cmd.Parameters.AddWithValue("@mid", model.Mid);
                cmd.Parameters.AddWithValue("@quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@unitPrice", model.UnitPrice);
                cmd.Parameters.AddWithValue("@totalAmount", model.TotalAmount);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string wid, string mid)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "DELETE FROM WarehouseDetails WHERE wid=@wid AND mid=@mid";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@wid", wid);
                cmd.Parameters.AddWithValue("@mid", mid);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
