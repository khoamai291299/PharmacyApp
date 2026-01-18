using PharmacyApp.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyApp.DataAccess
{
    public class UserRepository
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString;

        // ================= LOGIN =================
        public UserModel Login(string username, string password)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(@"
                SELECT id, username, password, email, phone, eid, role, status
                FROM [Users]
                WHERE username=@username 
                  AND password=@password 
                  AND status='Active'", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            conn.Open();
            using var rd = cmd.ExecuteReader();

            if (!rd.Read()) return null;

            // kiểm tra trạng thái
            if (rd["status"].ToString() != "Active")
                return null;

            return new UserModel
            {
                Id = (int)rd["id"],
                Username = rd["username"].ToString(),
                Password = rd["password"].ToString(),
                Email = rd["email"].ToString(),
                Phone = rd["phone"].ToString(),
                Eid = rd["eid"].ToString(),
                Role = rd["role"].ToString(),
                Status = rd["status"].ToString()
            };
        }

        // ================= CRUD =================
        public void Add(UserModel u)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(@"
                INSERT INTO [Users](username,password,email,phone,eid,role,status)
                VALUES(@username,@password,@email,@phone,@eid,@role,@status)", conn);

            AddParams(cmd, u);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(UserModel u)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(@"
                UPDATE [Users] SET
                    password = @password,
                    email    = @email,
                    phone    = @phone,
                    eid      = @eid,
                    role     = @role,
                    status   = @status
                WHERE username = @username", conn);

            AddParams(cmd, u);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(string username)
        {
            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(
                "DELETE FROM [Users] WHERE username=@username", conn);

            cmd.Parameters.AddWithValue("@username", username);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<UserModel> GetAll()
        {
            var list = new List<UserModel>();

            using var conn = new SqlConnection(_connString);
            var cmd = new SqlCommand(@"
                SELECT id, username, password, email, phone, eid, role, status
                FROM [Users]", conn);

            conn.Open();
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new UserModel
                {
                    Id = (int)rd["id"],
                    Username = rd["username"].ToString(),
                    Password = rd["password"].ToString(),
                    Email = rd["email"].ToString(),
                    Phone = rd["phone"].ToString(),
                    Eid = rd["eid"].ToString(),
                    Role = rd["role"].ToString(),
                    Status = rd["status"].ToString()
                });
            }

            return list;
        }

        public bool ExistsByEmployeeId(string employeeId)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM USERS WHERE eid = @eid", conn);
            cmd.Parameters.AddWithValue("@eid", employeeId);

            return (int)cmd.ExecuteScalar() > 0;
        }


        private void AddParams(SqlCommand cmd, UserModel u)
        {
            cmd.Parameters.AddWithValue("@username", u.Username);
            cmd.Parameters.AddWithValue("@password", u.Password);
            cmd.Parameters.AddWithValue("@email", u.Email ?? "");
            cmd.Parameters.AddWithValue("@phone", u.Phone ?? "");
            cmd.Parameters.AddWithValue("@eid", u.Eid ?? "");
            cmd.Parameters.AddWithValue("@role", u.Role);
            cmd.Parameters.AddWithValue("@status", u.Status);
        }
    }
}
