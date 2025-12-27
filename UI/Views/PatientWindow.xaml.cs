using DentalClinic.Data;
using DentalClinic.Models;
using System.Windows;

namespace DentalClinic.UI.Views
{
    public partial class PatientWindow : Window
    {
        private readonly PatientRepository _repo = new PatientRepository();

        public PatientWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО пациента.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var patient = new Patient
            {
                FullName = FullNameBox.Text.Trim(),
                BirthDate = BirthDatePicker.SelectedDate,
                Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(AddressBox.Text) ? null : AddressBox.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim()
            };

            _repo.Add(patient);

            MessageBox.Show("Пациент добавлен.", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
