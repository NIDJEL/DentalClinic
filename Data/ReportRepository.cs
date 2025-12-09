using System;
using System.Collections.Generic;
using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;

namespace DentalClinic.Data
{
    public class ReportRepository
    {
        // Доход по врачам за всё время
        public List<DoctorIncomeReport> GetDoctorIncomeReport()
        {
            var result = new List<DoctorIncomeReport>();

            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT d.full_name, " +
                "       COUNT(DISTINCT a.appointment_id) AS appointments_count, " +
                "       COALESCE(SUM(p.amount), 0)       AS total_income " +
                "FROM doctor d " +
                "LEFT JOIN appointment a ON a.doctor_id = d.doctor_id " +
                "LEFT JOIN payment p     ON p.appointment_id = a.appointment_id " +
                "GROUP BY d.full_name " +
                "ORDER BY total_income DESC;",
                conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new DoctorIncomeReport
                {
                    DoctorName = reader.GetString(0),
                    AppointmentsCount = reader.GetInt32(1),
                    TotalIncome = reader.GetDecimal(2)
                });
            }

            return result;
        }

        // Доход по врачам за указанный месяц
        public List<DoctorIncomeReport> GetDoctorIncomeReportByMonth(int year, int month)
        {
            var result = new List<DoctorIncomeReport>();

            using var conn = DbConnection.CreateConnection();

            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);

            using var cmd = new NpgsqlCommand(
                "SELECT d.full_name, " +
                "       COUNT(DISTINCT a.appointment_id) AS appointments_count, " +
                "       COALESCE(SUM(p.amount), 0)       AS total_income " +
                "FROM doctor d " +
                "LEFT JOIN appointment a ON a.doctor_id = d.doctor_id " +
                "LEFT JOIN payment p     ON p.appointment_id = a.appointment_id " +
                "     AND p.payment_date >= @from AND p.payment_date < @to " +
                "GROUP BY d.full_name " +
                "ORDER BY total_income DESC;",
                conn);

            cmd.Parameters.AddWithValue("from", from);
            cmd.Parameters.AddWithValue("to", to);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new DoctorIncomeReport
                {
                    DoctorName = reader.GetString(0),
                    AppointmentsCount = reader.GetInt32(1),
                    TotalIncome = reader.GetDecimal(2)
                });
            }

            return result;
        }
    }
}
    