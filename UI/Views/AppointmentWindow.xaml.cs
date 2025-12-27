using DentalClinic.Data;
using DentalClinic.Models;
using System.Windows;

namespace DentalClinic.UI.Views
{
    public partial class AppointmentWindow : Window
    {
        private readonly AppointmentRepository _repo;
        private readonly DoctorRepository _doctors;
        private readonly PatientRepository _patients;
        private readonly int? _doctorId;


        public AppointmentWindow(int? doctorId = null)
        {
            InitializeComponent();

            _doctorId = doctorId;

            _repo = new AppointmentRepository();
            _doctors = new DoctorRepository();
            _patients = new PatientRepository();

            DoctorBox.ItemsSource = _doctors.GetAll();
            DoctorBox.DisplayMemberPath = "FullName";
            DoctorBox.SelectedValuePath = "DoctorId";

            PatientBox.ItemsSource = _patients.GetAll();
            PatientBox.DisplayMemberPath = "FullName";
            PatientBox.SelectedValuePath = "PatientId";

            if (_doctorId.HasValue)
            {
                DoctorBox.SelectedValue = _doctorId.Value;
                DoctorBox.IsEnabled = false;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorBox.SelectedValue == null ||
                PatientBox.SelectedValue == null ||
                DatePickerStart.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var startDate = DatePickerStart.SelectedDate.Value;

            if (!TimeSpan.TryParse(TimeStartBox.Text, out var time))
            {
                MessageBox.Show("Неверный формат времени.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var start = startDate + time;
            var duration = int.Parse(DurationBox.Text);
            var end = start.AddMinutes(duration);

            var appt = new Appointment
            {
                DoctorId = (int)DoctorBox.SelectedValue,
                PatientId = (int)PatientBox.SelectedValue,
                StartTime = start,
                EndTime = end,
                Status = "scheduled",
                CreatedAt = DateTime.Now
            };

            _repo.Add(appt);

            MessageBox.Show("Запись создана!", "Успех", MessageBoxButton.OK);
            Close();
        }
    }
}