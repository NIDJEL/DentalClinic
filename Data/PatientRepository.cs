using System.Collections.Generic;
using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;

namespace DentalClinic.Data
{
    public class PatientRepository
    {
        public List<Patient> GetAll()
        {
            var result = new List<Patient>();

            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT patient_id, full_name, birth_date, phone, address, email " +
                "FROM patient ORDER BY full_name",
                conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Patient
                {
                    PatientId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    BirthDate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                    Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Email = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }

            return result;
        }

        public void Add(Patient patient)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO patient (full_name, birth_date, phone, address, email) " +
                "VALUES (@full_name, @birth_date, @phone, @address, @mail)",
                conn);

            cmd.Parameters.AddWithValue("full_name", patient.FullName);
            cmd.Parameters.AddWithValue("birth_date",
                patient.BirthDate.HasValue ? (object)patient.BirthDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("phone",
                (object?)patient.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("address",
                (object?)patient.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("mail",
                (object?)patient.Email ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int patientId)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM patient WHERE patient_id = @id",
                conn);

            cmd.Parameters.AddWithValue("id", patientId);
            cmd.ExecuteNonQuery();
        }
    }
}