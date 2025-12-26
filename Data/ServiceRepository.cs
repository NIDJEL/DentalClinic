using System.Collections.Generic;
using DentalClinic.Database;
using DentalClinic.Models;
using Npgsql;

namespace DentalClinic.Data
{
    public class ServiceRepository
    {
        public List<ServiceWithCategory> GetAll()
        {
            var result = new List<ServiceWithCategory>();

            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT s.service_id, sc.name AS category_name, s.name, " +
                "       s.default_price, s.default_duration_min, s.is_active " +
                "FROM service s " +
                "JOIN service_category sc ON sc.category_id = s.category_id " +
                "ORDER BY sc.name, s.name",
                conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ServiceWithCategory
                {
                    ServiceId = reader.GetInt32(0),
                    CategoryName = reader.GetString(1),
                    Name = reader.GetString(2),
                    DefaultPrice = reader.GetDecimal(3),
                    DefaultDurationMin = reader.GetInt32(4),
                    IsActive = reader.GetBoolean(5)
                });
            }

            return result;
        }

        public List<ServiceCategory> GetCategories()
        {
            var result = new List<ServiceCategory>();

            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT category_id, name, description FROM service_category ORDER BY name",
                conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ServiceCategory
                {
                    CategoryId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }

            return result;
        }

        public void Add(Service service)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO service (category_id, name, description, default_price, default_duration_min, is_active) " +
                "VALUES (@cat_id, @name, @descr, @price, @dur, @active)",
                conn);

            cmd.Parameters.AddWithValue("cat_id", service.CategoryId);
            cmd.Parameters.AddWithValue("name", service.Name);
            cmd.Parameters.AddWithValue("descr",
                (object?)service.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("price", service.DefaultPrice);
            cmd.Parameters.AddWithValue("dur", service.DefaultDurationMin);
            cmd.Parameters.AddWithValue("active", service.IsActive);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int serviceId)
        {
            using var conn = DbConnection.CreateConnection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM service WHERE service_id = @id",
                conn);

            cmd.Parameters.AddWithValue("id", serviceId);
            cmd.ExecuteNonQuery();
        }
    }
}
