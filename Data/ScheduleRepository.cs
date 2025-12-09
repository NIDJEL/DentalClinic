using System.Collections.Generic;
using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;

namespace DentalClinic.Data
{
    public class ScheduleRepository
    {
        public List<DoctorScheduleView> GetDoctorSchedule(int? doctorId = null)
        {
            var result = new List<DoctorScheduleView>();

            using var conn = DbConnection.CreateConnection();

            var sql =
                "SELECT d.full_name, ds.work_date, ds.time_from, ds.time_to, ds.is_day_off " +
                "FROM doctor_schedule ds " +
                "JOIN doctor d ON d.doctor_id = ds.doctor_id ";

            if (doctorId.HasValue)
                sql += "WHERE d.doctor_id = @docId ";

            sql += "ORDER BY d.full_name, ds.work_date";

            using var cmd = new NpgsqlCommand(sql, conn);

            if (doctorId.HasValue)
                cmd.Parameters.AddWithValue("docId", doctorId.Value);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new DoctorScheduleView
                {
                    DoctorName = reader.GetString(0),
                    WorkDate = reader.GetDateTime(1),
                    TimeFrom = reader.IsDBNull(2) ? (TimeSpan?)null : reader.GetTimeSpan(2),
                    TimeTo = reader.IsDBNull(3) ? (TimeSpan?)null : reader.GetTimeSpan(3),
                    IsDayOff = reader.GetBoolean(4)
                });
            }

            return result;
        }

        public void Add(DoctorSchedule schedule)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO doctor_schedule (doctor_id, work_date, time_from, time_to, is_day_off) " +
                "VALUES (@doc_id, @date, @from, @to, @dayoff)",
                conn);

            cmd.Parameters.AddWithValue("doc_id", schedule.DoctorId);
            cmd.Parameters.AddWithValue("date", schedule.WorkDate);

            cmd.Parameters.AddWithValue("from",
                schedule.TimeFrom.HasValue ? (object)schedule.TimeFrom.Value : DBNull.Value);

            cmd.Parameters.AddWithValue("to",
                schedule.TimeTo.HasValue ? (object)schedule.TimeTo.Value : DBNull.Value);

            cmd.Parameters.AddWithValue("dayoff", schedule.IsDayOff);

            cmd.ExecuteNonQuery();
        }
    }
}