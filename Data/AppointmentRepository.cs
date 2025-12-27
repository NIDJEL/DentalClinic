using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;

namespace DentalClinic.Data
{
    public class AppointmentRepository
    {
        public List<AppointmentView> GetAll(int? doctorId = null)
        {
            var result = new List<AppointmentView>();

            using var conn = DbConnection.CreateConnection();

            var sql =
                "SELECT a.appointment_id, a.start_time, a.end_time, a.status, " +
                "       d.full_name AS doctor_name, " +
                "       p.full_name AS patient_name, " +
                "       dg.name     AS diagnosis_name " +
                "FROM appointment a " +
                "JOIN doctor d   ON d.doctor_id = a.doctor_id " +
                "JOIN patient p  ON p.patient_id = a.patient_id " +
                "LEFT JOIN diagnosis dg ON dg.diagnosis_id = a.diagnosis_id ";

            if (doctorId.HasValue)
                sql += "WHERE a.doctor_id = @docId ";

            sql += "ORDER BY a.start_time";

            using var cmd = new NpgsqlCommand(sql, conn);

            if (doctorId.HasValue)
                cmd.Parameters.AddWithValue("docId", doctorId.Value);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new AppointmentView
                {
                    AppointmentId = reader.GetInt32(0),
                    StartTime = reader.GetDateTime(1),
                    EndTime = reader.GetDateTime(2),
                    Status = reader.GetString(3),
                    DoctorName = reader.GetString(4),
                    PatientName = reader.GetString(5),
                    DiagnosisName = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }

            return result;
        }

        // <<< ВОТ ЭТО НОВЫЙ МЕТОД >>>
        public void Add(Appointment appointment)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO appointment " +
                " (doctor_id, patient_id, diagnosis_id, start_time, end_time, status, notes, created_at) " +
                "VALUES (@doctor_id, @patient_id, @diagnosis_id, @start_time, @end_time, @status, @notes, @created_at)",
                conn);

            cmd.Parameters.AddWithValue("doctor_id", appointment.DoctorId);
            cmd.Parameters.AddWithValue("patient_id", appointment.PatientId);
            cmd.Parameters.AddWithValue("diagnosis_id",
                (object?)appointment.DiagnosisId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("start_time", appointment.StartTime);
            cmd.Parameters.AddWithValue("end_time", appointment.EndTime);
            cmd.Parameters.AddWithValue("status", appointment.Status);
            cmd.Parameters.AddWithValue("notes",
                (object?)appointment.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("created_at", appointment.CreatedAt);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int appointmentId)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM appointment WHERE appointment_id = @id",
                conn);

            cmd.Parameters.AddWithValue("id", appointmentId);
            cmd.ExecuteNonQuery();
        }
    }
}