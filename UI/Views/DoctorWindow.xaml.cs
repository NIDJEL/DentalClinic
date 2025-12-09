using System;
using System.Globalization;
using System.Windows;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.UI.Views
{
    public partial class DoctorWindow : Window
    {
        private readonly DoctorRepository _repo = new DoctorRepository();

        public DoctorWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО врача.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal? percent = null;
            if (decimal.TryParse(PercentBox.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
            {
                percent = p;
            }

            var doctor = new Doctor
            {
                FullName = FullNameBox.Text.Trim(),
                Specialization = string.IsNullOrWhiteSpace(SpecBox.Text) ? null : SpecBox.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim(),
                PercentFromIncome = percent,
                IsActive = IsActiveCheck.IsChecked ?? true
            };

            _repo.Add(doctor);

            MessageBox.Show("Врач добавлен.", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}