using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;
using DentalClinic.Utils;


namespace DentalClinic.Data
{
    public class AuthRepository
    {
        public AppUser? Login(string username, string password)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT user_id, username, role, doctor_id, is_active " +
                "FROM app_user " +
                "WHERE username = @u AND password_hash = @pHash",
                conn);

            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("pHash", PasswordHelper.Hash(password));

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new AppUser
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Role = reader.GetString(2),
                DoctorId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                IsActive = reader.GetBoolean(4)
            };
        }
    }
}
