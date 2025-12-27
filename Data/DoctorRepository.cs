using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;

namespace DentalClinic.Data
{
    public class DoctorRepository
    {
        public List<Doctor> GetAll()
        {
            var result = new List<Doctor>();

            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT doctor_id, full_name, specialization, phone, email, percent_from_income, is_active " +
                "FROM doctor ORDER BY full_name",
                conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Doctor
                {
                    DoctorId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    Specialization = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                    PercentFromIncome = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                    IsActive = reader.GetBoolean(6)
                });
            }

            return result;
        }

        public void Add(Doctor doctor)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO doctor (full_name, specialization, phone, email, percent_from_income, is_active) " +
                "VALUES (@full_name, @spec, @phone, @email, @percent, @active)",
                conn);

            cmd.Parameters.AddWithValue("full_name", doctor.FullName);
            cmd.Parameters.AddWithValue("spec", (object?)doctor.Specialization ?? DBNull.Value);
            cmd.Parameters.AddWithValue("phone", (object?)doctor.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("email", (object?)doctor.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("percent",
                doctor.PercentFromIncome.HasValue ? (object)doctor.PercentFromIncome.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("active", doctor.IsActive);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int doctorId)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM doctor WHERE doctor_id = @id",
                conn);

            cmd.Parameters.AddWithValue("id", doctorId);
            cmd.ExecuteNonQuery();
        }
    }
}
