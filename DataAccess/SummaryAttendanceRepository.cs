using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class SummaryAttendanceRepository
    {
        private readonly string _connString;

        public SummaryAttendanceRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }

        public List<SummaryAttendanceModel> GetAll()
        {
            var list = new List<SummaryAttendanceModel>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = "SELECT rid, eid, numOfworkDay, numOfdayOff, netSalary FROM Summary_Attendance";
                SqlCommand cmd = new SqlCommand(query, conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SummaryAttendanceModel
                    {
                        Rid = reader.GetInt32(0),
                        Eid = reader.GetString(1),
                        NumOfworkDay = reader.GetInt32(2),
                        NumOfdayOff = reader.GetInt32(3),
                        NetSalary = reader.GetInt32(4)
                    });
                }
            }
            return list;
        }

        public SummaryAttendanceModel GetById(int rid, string eid)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = @"SELECT rid, eid, numOfworkDay, numOfdayOff, netSalary 
                                 FROM Summary_Attendance 
                                 WHERE rid=@rid AND eid=@eid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rid", rid);
                cmd.Parameters.AddWithValue("@eid", eid);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new SummaryAttendanceModel
                    {
                        Rid = reader.GetInt32(0),
                        Eid = reader.GetString(1),
                        NumOfworkDay = reader.GetInt32(2),
                        NumOfdayOff = reader.GetInt32(3),
                        NetSalary = reader.GetInt32(4)
                    };
                }
            }
            return null;
        }

        public bool Insert(SummaryAttendanceModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = @"INSERT INTO Summary_Attendance
                                 (rid, eid, numOfworkDay, numOfdayOff, netSalary)
                                 VALUES (@rid, @eid, @work, @off, @net)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rid", model.Rid);
                cmd.Parameters.AddWithValue("@eid", model.Eid);
                cmd.Parameters.AddWithValue("@work", model.NumOfworkDay);
                cmd.Parameters.AddWithValue("@off", model.NumOfdayOff);
                cmd.Parameters.AddWithValue("@net", model.NetSalary);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(SummaryAttendanceModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = @"UPDATE Summary_Attendance 
                                 SET numOfworkDay=@work, 
                                     numOfdayOff=@off, 
                                     netSalary=@net
                                 WHERE rid=@rid AND eid=@eid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rid", model.Rid);
                cmd.Parameters.AddWithValue("@eid", model.Eid);
                cmd.Parameters.AddWithValue("@work", model.NumOfworkDay);
                cmd.Parameters.AddWithValue("@off", model.NumOfdayOff);
                cmd.Parameters.AddWithValue("@net", model.NetSalary);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int rid, string eid)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string query = @"DELETE FROM Summary_Attendance 
                                 WHERE rid=@rid AND eid=@eid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rid", rid);
                cmd.Parameters.AddWithValue("@eid", eid);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
