using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class RevenueRepository
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;

        public List<EmployeeRevenueModel> GetRevenueByEmployee(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var list = new List<EmployeeRevenueModel>();

            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(@"
            SELECT
                e.id AS EmployeeId,
                e.name AS EmployeeName,
                COUNT(b.id) AS TotalBills,
                SUM(b.totalAmount) AS TotalRevenue
            FROM Bill b
            JOIN Employee e ON b.eid = e.id
            WHERE
                (@FromDate IS NULL OR b.dateOfcreate >= @FromDate)
            AND (@ToDate   IS NULL OR b.dateOfcreate <= @ToDate)
            GROUP BY e.id, e.name
            ORDER BY TotalRevenue DESC
        ", conn);

            cmd.Parameters.AddWithValue("@FromDate",
                fromDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate",
                toDate ?? (object)DBNull.Value);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new EmployeeRevenueModel
                {
                    EmployeeId = rd["EmployeeId"].ToString(),
                    EmployeeName = rd["EmployeeName"].ToString(),
                    TotalBills = (int)rd["TotalBills"],
                    TotalRevenue = rd["TotalRevenue"] == DBNull.Value ? 0 : (int)rd["TotalRevenue"]
                });
            }

            return list;
        }
    }

}
