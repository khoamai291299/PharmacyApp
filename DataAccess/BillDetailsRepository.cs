using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class BillDetailsRepository
    {
        private readonly string _connString;

        public BillDetailsRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<BillDetailsModel> GetAll()
        {
            var list = new List<BillDetailsModel>();
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand("SELECT bid, mid, quantity, unitPrice, totalAmount FROM BillDetails", conn);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new BillDetailsModel
                        {
                            Bid = rd["bid"] as string,
                            Mid = rd["mid"] as string,
                            Quantity = rd["quantity"] == DBNull.Value ? 0 : (int)rd["quantity"],
                            UnitPrice = rd["unitPrice"] == DBNull.Value ? 0 : (int)rd["unitPrice"],
                            TotalAmount = rd["totalAmount"] == DBNull.Value ? 0 : (int)rd["totalAmount"]
                        });
                    }
                }
            }
            return list;
        }

        public void Add(BillDetailsModel m)
        {
            // Tính tổng tiền
            m.TotalAmount = m.Quantity * m.UnitPrice;

            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(
                @"INSERT INTO BillDetails(bid, mid, quantity, unitPrice, totalAmount)
                  VALUES (@bid, @mid, @quantity, @unitPrice, @totalAmount)", conn);

            cmd.Parameters.AddWithValue("@bid", m.Bid);
            cmd.Parameters.AddWithValue("@mid", m.Mid);
            cmd.Parameters.AddWithValue("@quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@unitPrice", m.UnitPrice);
            cmd.Parameters.AddWithValue("@totalAmount", m.TotalAmount);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(BillDetailsModel m)
        {
            m.TotalAmount = m.Quantity * m.UnitPrice;

            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(
                @"UPDATE BillDetails SET
                    quantity = @quantity,
                    unitPrice = @unitPrice,
                    totalAmount = @totalAmount
                  WHERE bid = @bid AND mid = @mid", conn);

            cmd.Parameters.AddWithValue("@bid", m.Bid);
            cmd.Parameters.AddWithValue("@mid", m.Mid);
            cmd.Parameters.AddWithValue("@quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@unitPrice", m.UnitPrice);
            cmd.Parameters.AddWithValue("@totalAmount", m.TotalAmount);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(string bid, string mid)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand("DELETE FROM BillDetails WHERE bid = @bid AND mid = @mid", conn);

            cmd.Parameters.AddWithValue("@bid", bid);
            cmd.Parameters.AddWithValue("@mid", mid);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
