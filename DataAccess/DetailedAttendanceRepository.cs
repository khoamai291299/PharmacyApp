using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class DetailedAttendanceRepository
    {
        private readonly string _connString;

        public DetailedAttendanceRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<DetailedAttendanceModel> GetAll()
        {
            var list = new List<DetailedAttendanceModel>();

            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand(
                    "SELECT id, eid, days, checkIn, checkOut, status, note FROM DetailedAttendance",
                    conn);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DetailedAttendanceModel
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Eid = rd["eid"] as string,
                            Days = Convert.ToDateTime(rd["days"]),
                            CheckIn = rd["checkIn"] == DBNull.Value ? null : (TimeSpan?)rd["checkIn"],
                            CheckOut = rd["checkOut"] == DBNull.Value ? null : (TimeSpan?)rd["checkOut"],
                            Status = rd["status"] as string,
                            Note = rd["note"] as string
                        });
                    }
                }
            }

            return list;
        }

        public void Add(DetailedAttendanceModel m)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand(
                    @"INSERT INTO DetailedAttendance(eid, days, checkIn, checkOut, status, note)
                      VALUES(@eid,@days,@checkIn,@checkOut,@status,@note)",
                    conn);

                AddParameters(cmd, m);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(DetailedAttendanceModel m)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand(
                    @"UPDATE DetailedAttendance SET
                        eid=@eid,
                        days=@days,
                        checkIn=@checkIn,
                        checkOut=@checkOut,
                        status=@status,
                        note=@note
                      WHERE id=@id",
                    conn);

                AddParameters(cmd, m);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand("DELETE FROM DetailedAttendance WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AddParameters(SqlCommand cmd, DetailedAttendanceModel m)
        {
            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@eid", m.Eid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@days", m.Days);
            cmd.Parameters.AddWithValue("@checkIn", m.CheckIn.HasValue ? (object)m.CheckIn.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@checkOut", m.CheckOut.HasValue ? (object)m.CheckOut.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@status", m.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@note", m.Note ?? (object)DBNull.Value);
        }
    }
}
