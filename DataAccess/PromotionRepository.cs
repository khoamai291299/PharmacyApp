using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class PromotionRepository
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;

        public List<PromotionModel> GetAll()
        {
            var list = new List<PromotionModel>();
            using var conn = new SqlConnection(_connString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM Promotion", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PromotionModel
                {
                    Id = reader["Id"].ToString(),
                    StartDate = reader["StartDate"] != DBNull.Value ? (DateTime)reader["StartDate"] : DateTime.Today,
                    EndDate = reader["EndDate"] != DBNull.Value ? (DateTime)reader["EndDate"] : DateTime.Today,
                    RequiredPoints = reader["RequiredPoints"] != DBNull.Value ? (int)reader["RequiredPoints"] : 0,
                    Quantity = reader["Quantity"] != DBNull.Value ? (int)reader["Quantity"] : 0,
                    DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? (int)reader["DiscountPercent"] : 0
                });
            }
            return list;
        }

        public void Add(PromotionModel promo)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();

            using var cmd = new SqlCommand(

                "INSERT INTO Promotion(Id, StartDate, EndDate, RequiredPoints, Quantity, DiscountPercent) " +
                "VALUES(@Id, @StartDate, @EndDate, @RequiredPoints, @Quantity, @DiscountPercent)", conn);

            cmd.Parameters.AddWithValue("@Id", promo.Id);
            cmd.Parameters.AddWithValue("@StartDate", promo.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", promo.EndDate);
            cmd.Parameters.AddWithValue("@RequiredPoints", promo.RequiredPoints);
            cmd.Parameters.AddWithValue("@Quantity", promo.Quantity);
            cmd.Parameters.AddWithValue("@DiscountPercent", promo.DiscountPercent);

            cmd.ExecuteNonQuery();
        }

        public void Update(PromotionModel promo)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();

            using var cmd = new SqlCommand(

                "UPDATE Promotion SET StartDate=@StartDate, EndDate=@EndDate, RequiredPoints=@RequiredPoints, Quantity=@Quantity, " +
                "DiscountPercent=@DiscountPercent " + "WHERE Id=@Id", conn);

            cmd.Parameters.AddWithValue("@Id", promo.Id);
            cmd.Parameters.AddWithValue("@StartDate", promo.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", promo.EndDate);
            cmd.Parameters.AddWithValue("@RequiredPoints", promo.RequiredPoints);
            cmd.Parameters.AddWithValue("@Quantity", promo.Quantity);
            cmd.Parameters.AddWithValue("@DiscountPercent", promo.DiscountPercent);

            cmd.ExecuteNonQuery();
        }

        public void Delete(string id)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Promotion WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public string GenerateNewId()
        {
            // Lấy tất cả promotion từ database
            var allPromotions = GetAll();

            // Lấy số lớn nhất hiện có
            var lastIdNumber = allPromotions
                .Select(p => p.Id)
                .Where(id => id.StartsWith("KM"))
                .Select(id => int.TryParse(id.Substring(2), out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            // Tăng 1 và format thành 3 chữ số
            var newIdNumber = lastIdNumber + 1;

            return $"KM{newIdNumber:D3}";
        }

        public PromotionModel GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            using var conn = new SqlConnection(_connString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM Promotion WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new PromotionModel
                {
                    Id = reader["Id"].ToString(),
                    StartDate = reader["StartDate"] != DBNull.Value ? (DateTime)reader["StartDate"] : DateTime.Today,
                    EndDate = reader["EndDate"] != DBNull.Value ? (DateTime)reader["EndDate"] : DateTime.Today,
                    RequiredPoints = reader["RequiredPoints"] != DBNull.Value ? (int)reader["RequiredPoints"] : 0,
                    Quantity = reader["Quantity"] != DBNull.Value ? (int)reader["Quantity"] : 0,
                    DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? (int)reader["DiscountPercent"] : 0
                };
            }

            return null; // không tìm thấy
        }

    }
}
