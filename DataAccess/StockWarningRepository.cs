using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class StockWarningRepository
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;

        public List<StockWarningModel> GetStockWarnings()
        {
            var list = new List<StockWarningModel>();

            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand("SELECT * FROM vw_StockWarning", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var model = new StockWarningModel
                {
                    MedicineId = reader["MedicineId"] != DBNull.Value ? reader["MedicineId"].ToString() : string.Empty,
                    MedicineName = reader["MedicineName"] != DBNull.Value ? reader["MedicineName"].ToString() : string.Empty,
                    Quantity = reader["quantity"] != DBNull.Value ? (int)reader["quantity"] : 0,
                    ExpirationDate = reader["expirationDate"] != DBNull.Value ? (DateTime)reader["expirationDate"] : DateTime.MinValue,
                    WarningType = reader["WarningType"] != DBNull.Value ? reader["WarningType"].ToString() : string.Empty
                };

                list.Add(model);
            }

            return list;
        }

    }
}
