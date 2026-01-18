using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class MedicineRepository
    {
        private readonly string _connString;

        public MedicineRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        // Tạo ID mới dạng MD01, MD02, ..., MD100+
        public string GenerateNewId()
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand("SELECT MAX(id) FROM Medicine WHERE id LIKE 'MD%'", conn);
            conn.Open();
            var last = cmd.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(last))
                return "MD01";

            if (int.TryParse(last.Substring(2), out int num))
                return $"MD{(num + 1):D2}";
            else
                return "MD01";
        }

        // Lấy tất cả thuốc
        public List<MedicineModel> GetAll()
        {
            var list = new List<MedicineModel>();

            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(@"
                SELECT id, name, content, activeIngredient, inputTime, 
                       productionDate, expirationDate, unit, tid, mid, 
                       quantity, importPrice, sellingPrice, ImagePath 
                FROM Medicine", conn);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var med = new MedicineModel
                {
                    Id = reader["id"] as string,
                    Name = reader["name"] as string,
                    Content = reader["content"] != DBNull.Value ? Convert.ToInt32(reader["content"]) : null,
                    ActiveIngredient = reader["activeIngredient"] as string,
                    InputTime = reader["inputTime"] != DBNull.Value ? Convert.ToDateTime(reader["inputTime"]) : null,
                    ProductionDate = reader["productionDate"] != DBNull.Value ? Convert.ToDateTime(reader["productionDate"]) : null,
                    ExpirationDate = reader["expirationDate"] != DBNull.Value ? Convert.ToDateTime(reader["expirationDate"]) : null,
                    Unit = reader["unit"] as string,
                    Tid = reader["tid"] as string,
                    Mid = reader["mid"] as string,
                    Quantity = reader["quantity"] != DBNull.Value ? Convert.ToInt32(reader["quantity"]) : 0,
                    ImportPrice = reader["importPrice"] != DBNull.Value ? Convert.ToInt32(reader["importPrice"]) : 0,
                    SellingPrice = reader["sellingPrice"] != DBNull.Value ? Convert.ToInt32(reader["sellingPrice"]) : 0,
                    ImagePath = reader["ImagePath"] as string
                };
                list.Add(med);
            }

            return list;
        }

        // Lấy 1 thuốc theo ID
        public MedicineModel GetById(string id)
        {
            MedicineModel med = null;

            using var conn = new SqlConnection(_connString);
            string sql = @"
                SELECT id, name, content, activeIngredient, inputTime, 
                       productionDate, expirationDate, unit, tid, mid, 
                       quantity, importPrice, sellingPrice, ImagePath
                FROM Medicine
                WHERE id = @id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                med = new MedicineModel
                {
                    Id = reader["id"] as string,
                    Name = reader["name"] as string,
                    Content = reader["content"] != DBNull.Value ? Convert.ToInt32(reader["content"]) : null,
                    ActiveIngredient = reader["activeIngredient"] as string,
                    InputTime = reader["inputTime"] != DBNull.Value ? Convert.ToDateTime(reader["inputTime"]) : null,
                    ProductionDate = reader["productionDate"] != DBNull.Value ? Convert.ToDateTime(reader["productionDate"]) : null,
                    ExpirationDate = reader["expirationDate"] != DBNull.Value ? Convert.ToDateTime(reader["expirationDate"]) : null,
                    Unit = reader["unit"] as string,
                    Tid = reader["tid"] as string,
                    Mid = reader["mid"] as string,
                    Quantity = reader["quantity"] != DBNull.Value ? Convert.ToInt32(reader["quantity"]) : 0,
                    ImportPrice = reader["importPrice"] != DBNull.Value ? Convert.ToInt32(reader["importPrice"]) : 0,
                    SellingPrice = reader["sellingPrice"] != DBNull.Value ? Convert.ToInt32(reader["sellingPrice"]) : 0,
                    ImagePath = reader["ImagePath"] as string
                };
            }

            return med;
        }

        // Thêm mới thuốc
        public void Add(MedicineModel m)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Medicine
                (id, name, content, activeIngredient, inputTime, productionDate, expirationDate, unit, tid, mid, quantity, importPrice, sellingPrice, ImagePath)
                VALUES
                (@id,@name,@content,@activeIngredient,@inputTime,@productionDate,@expirationDate,@unit,@tid,@mid,@quantity,@importPrice,@sellingPrice,@ImagePath)", conn);

            AddParameters(cmd, m);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Cập nhật thuốc
        public void Update(MedicineModel m)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(@"
                UPDATE Medicine SET
                    name=@name,
                    content=@content,
                    activeIngredient=@activeIngredient,
                    inputTime=@inputTime,
                    productionDate=@productionDate,
                    expirationDate=@expirationDate,
                    unit=@unit,
                    tid=@tid,
                    mid=@mid,
                    quantity=@quantity,
                    importPrice=@importPrice,
                    sellingPrice=@sellingPrice,
                    ImagePath=@ImagePath
                WHERE id=@id", conn);

            AddParameters(cmd, m);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Xóa thuốc theo ID
        public void Delete(string id)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand("DELETE FROM Medicine WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Thêm tham số chuẩn hóa
        private void AddParameters(SqlCommand cmd, MedicineModel m)
        {
            cmd.Parameters.AddWithValue("@id", m.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", m.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@content", m.Content.HasValue ? (object)m.Content.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@activeIngredient", m.ActiveIngredient ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@inputTime", m.InputTime.HasValue ? (object)m.InputTime.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@productionDate", m.ProductionDate.HasValue ? (object)m.ProductionDate.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@expirationDate", m.ExpirationDate.HasValue ? (object)m.ExpirationDate.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@unit", m.Unit ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@tid", m.Tid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@mid", m.Mid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@importPrice", m.ImportPrice);
            cmd.Parameters.AddWithValue("@sellingPrice", m.SellingPrice);
            cmd.Parameters.AddWithValue("@ImagePath", m.ImagePath ?? (object)DBNull.Value);
        }

        public void DecreaseQuantity(string medicineId, int quantity)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                @"UPDATE Medicine
                  SET quantity = quantity - @qty
                  WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@qty", quantity);
            cmd.Parameters.AddWithValue("@id", medicineId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
