using DentalClinic.Data;
using Npgsql;
using System.Windows;

namespace DentalClinic.UI.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginBox.Text;
            var password = PasswordBox.Password;
            var dbPassword = DbPasswordBox.Password;

            if (!string.IsNullOrEmpty(dbPassword))
            {
                Database.DbConnection.SetDbPassword(dbPassword);
            }

            try
            {
                var repo = new AuthRepository();
                var user = repo.Login(login, password);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var main = new MainWindow(user);
                main.Show();
                Close();
            }
            catch (NpgsqlException ex)
            {
                string errorMessage = "Ошибка подключения к базе данных.\n";

                if (ex.Message.Contains("password") || ex.Message.Contains("authentication"))
                {
                    errorMessage += "Неверный пароль базы данных.";
                }
                else if (ex.Message.Contains("could not connect"))
                {
                    errorMessage += "Не удалось подключиться к серверу базы данных.\nПроверьте, что PostgreSQL запущен.";
                }
                else
                {
                    errorMessage += $"Детали: {ex.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка подключения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
