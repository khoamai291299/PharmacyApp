using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class DashboardRepository
    {
        private readonly string _connString;

        public DashboardRepository()
        {
            _connString = ConfigurationManager
                .ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        // ===== DOANH THU HÔM NAY =====
        public int GetTodayRevenue()
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(totalAmount),0)
                FROM Bill
                WHERE CAST(dateOfcreate AS DATE) = CAST(GETDATE() AS DATE)", conn);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ===== TỔNG HỢP THEO KHOẢNG NGÀY =====
        public (int revenue, int import) GetSummaryByDateRange(DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(@"
                SELECT
                    dbo.fn_TotalRevenue(@from,@to) AS Revenue,
                    dbo.fn_TotalImport(@from,@to)  AS Import", conn);

            cmd.Parameters.AddWithValue("@from", from.Date);
            cmd.Parameters.AddWithValue("@to", to.Date);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                return (
                    Convert.ToInt32(rd["Revenue"]),
                    Convert.ToInt32(rd["Import"])
                );
            }
            return (0, 0);
        }

        // ===== TOP THUỐC =====
        public DataTable GetTopMedicine(DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connString);
            var da = new SqlDataAdapter(@"
                SELECT TOP 10
                    m.id,
                    m.name,
                    SUM(bd.quantity) AS TotalSold
                FROM BillDetails bd
                JOIN Bill b ON bd.bid = b.id
                JOIN Medicine m ON bd.mid = m.id
                WHERE b.dateOfcreate BETWEEN @from AND @to
                GROUP BY m.id, m.name
                ORDER BY TotalSold DESC", conn);

            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
            da.SelectCommand.Parameters.AddWithValue("@to", to.Date);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // ===== TOP KHÁCH HÀNG =====
        public DataTable GetTopCustomers(DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connString);
            var da = new SqlDataAdapter(@"
                SELECT TOP 10
                    c.id, c.name, c.phone,
                    SUM(b.totalAmount) AS totalExpenditure,
                    c.cumulativePoints
                FROM Bill b
                JOIN Customer c ON b.cid = c.id
                WHERE b.dateOfcreate BETWEEN @from AND @to
                GROUP BY c.id, c.name, c.phone, c.cumulativePoints
                ORDER BY totalExpenditure DESC", conn);

            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
            da.SelectCommand.Parameters.AddWithValue("@to", to.Date);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // ===== BIỂU ĐỒ =====

        // N NGÀY (KHÔNG DÍNH GIỜ)
        public DataTable GetRevenueLastNDays(int days)
        {
            using var conn = new SqlConnection(_connString);
            var da = new SqlDataAdapter(@"
                SELECT
                    CAST(dateOfcreate AS DATE) AS Date,
                    SUM(totalAmount) AS Revenue
                FROM Bill
                WHERE CAST(dateOfcreate AS DATE) >=
                      DATEADD(DAY, -(@days-1), CAST(GETDATE() AS DATE))
                GROUP BY CAST(dateOfcreate AS DATE)
                ORDER BY Date", conn);

            da.SelectCommand.Parameters.AddWithValue("@days", days);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // N TUẦN (TÁCH NĂM – KHÔNG LỆCH)
        public DataTable GetRevenueLastNWeeks(int weeks)
        {
            using var conn = new SqlConnection(_connString);
            var da = new SqlDataAdapter(@"
                SELECT
                    DATEPART(YEAR, dateOfcreate) AS Year,
                    DATEPART(WEEK, dateOfcreate) AS Week,
                    SUM(totalAmount) AS Revenue
                FROM Bill
                WHERE dateOfcreate >= DATEADD(WEEK, -@weeks, GETDATE())
                GROUP BY DATEPART(YEAR, dateOfcreate),
                         DATEPART(WEEK, dateOfcreate)
                ORDER BY Year, Week", conn);

            da.SelectCommand.Parameters.AddWithValue("@weeks", weeks);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // N THÁNG
        public DataTable GetRevenueLastNMonths(int months)
        {
            using var conn = new SqlConnection(_connString);
            var da = new SqlDataAdapter(@"
                SELECT
                    FORMAT(dateOfcreate,'yyyy-MM') AS Month,
                    SUM(totalAmount) AS Revenue
                FROM Bill
                WHERE dateOfcreate >=
                      DATEADD(MONTH, -(@months-1), CAST(GETDATE() AS DATE))
                GROUP BY FORMAT(dateOfcreate,'yyyy-MM')
                ORDER BY Month", conn);

            da.SelectCommand.Parameters.AddWithValue("@months", months);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Doanh thu ngày
        public DataTable GetDailyRevenue(DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connString);
            using var da = new SqlDataAdapter("""
                SELECT RevenueDate, Revenue
                FROM vw_DailyRevenue
                WHERE RevenueDate BETWEEN @from AND @to
                ORDER BY RevenueDate
            """, conn);

            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
            da.SelectCommand.Parameters.AddWithValue("@to", to.Date);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public (int revenue, int import) GetDashboardSummary(
            DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(@"   
                SELECT
                    dbo.fn_TotalRevenue(@from, @to) AS Revenue,
                    dbo.fn_TotalImport(@from, @to)  AS Import
                ", conn);

            cmd.Parameters.AddWithValue("@from", from.Date);
            cmd.Parameters.AddWithValue("@to", to.Date);

            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return (0, 0);

            return (
                Convert.ToInt32(r["Revenue"]),
                Convert.ToInt32(r["Import"])
            );
        }

        public DataTable GetDailyImport(DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connString);
                    using var da = new SqlDataAdapter(@"
                SELECT
                    ImportDate,
                    ImportAmount
                FROM vw_DailyImport
                WHERE ImportDate BETWEEN @from AND @to
                ORDER BY ImportDate
                ", conn);

            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
            da.SelectCommand.Parameters.AddWithValue("@to", to.Date);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


    }
}
