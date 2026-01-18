using PharmacyApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class EmployeeRepository
    {
        private readonly string _connString;

        public EmployeeRepository()
        {
            _connString = ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;
        }
        public string GenerateNewEmployeeId()
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT MAX(id) FROM Employee WHERE id LIKE 'E%'", conn);

            var lastId = cmd.ExecuteScalar()?.ToString();

            if (string.IsNullOrEmpty(lastId))
                return "E01";

            // lastId = "E03"
            int number = int.Parse(lastId.Substring(1));
            number++;

            return $"E{number:D2}";
        }


        public List<EmployeeModel> GetAll()
        {
            var list = new List<EmployeeModel>();
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand(@"
                    SELECT id, name, phone, address, startday, birthday, sex, salary, did, pid 
                    FROM Employee", conn);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new EmployeeModel
                        {
                            Id = rd["id"] as string,
                            Name = rd["name"] as string,
                            Phone = rd["phone"] as string,
                            Address = rd["address"] as string,
                            startday = rd["startday"] as DateTime?,
                            birthday = rd["birthday"] as DateTime?,
                            Gender = rd["sex"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(rd["sex"]),
                            Salary = rd["salary"] == DBNull.Value ? null : (int?)Convert.ToInt32(rd["salary"]),
                            Did = rd["did"] as string,
                            Pid = rd["pid"] as string
                        });
                    }
                }
            }
            return list;
        }

        public void Add(EmployeeModel e)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Employee(id, name, phone, address, startday, birthday, sex, salary, did, pid)
                    VALUES(@id, @name, @phone, @address, @startday, @birthday, @sex, @salary, @did, @pid)", conn);

                AddParameters(cmd, e);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(EmployeeModel e)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand(@"
                    UPDATE Employee SET
                        name=@name,
                        phone=@phone,
                        address=@address,
                        startday=@startday,
                        birthday=@birthday,
                        sex=@sex,
                        salary=@salary,
                        did=@did,
                        pid=@pid
                    WHERE id=@id", conn);

                AddParameters(cmd, e);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(string id)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var cmd = new SqlCommand("DELETE FROM Employee WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AddParameters(SqlCommand cmd, EmployeeModel e)
        {
            cmd.Parameters.AddWithValue("@id", e.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", e.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", e.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", e.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@startday", e.startday.HasValue ? (object)e.startday.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@birthday", e.birthday.HasValue ? (object)e.birthday.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@sex", e.Gender.HasValue ? (object)e.Gender.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@salary", e.Salary.HasValue ? (object)e.Salary.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@did", e.Did ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pid", e.Pid ?? (object)DBNull.Value);
        }

        public EmployeeModel? GetById(string id)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT id, name, phone, address, startday, birthday, sex, salary, did, pid
                FROM Employee
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", id);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new EmployeeModel
            {
                Id = rd["id"] as string,
                Name = rd["name"] as string,
                Phone = rd["phone"] as string,
                Address = rd["address"] as string,
                startday = rd["startday"] as DateTime?,
                birthday = rd["birthday"] as DateTime?,
                Gender = rd["sex"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(rd["sex"]),
                Salary = rd["salary"] == DBNull.Value ? null : (int?)Convert.ToInt32(rd["salary"]),
                Did = rd["did"] as string,
                Pid = rd["pid"] as string
            };
        }
    }
}
