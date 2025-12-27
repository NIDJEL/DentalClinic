using DentalClinic.Data;
using DentalClinic.Models;
using System.Windows;

namespace DentalClinic.UI.Views
{
    public partial class ScheduleWindow : Window
    {
        private readonly ScheduleRepository _scheduleRepo = new ScheduleRepository();
        private readonly DoctorRepository _doctorRepo = new DoctorRepository();

        public ScheduleWindow()
        {
            InitializeComponent();

            DoctorBox.ItemsSource = _doctorRepo.GetAll();
            DoctorBox.DisplayMemberPath = "FullName";
            DoctorBox.SelectedValuePath = "DoctorId";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorBox.SelectedValue == null || DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите врача и дату.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var workDate = DatePicker.SelectedDate.Value;
            TimeSpan? timeFrom = null;
            TimeSpan? timeTo = null;

            if (!(IsDayOffCheck.IsChecked ?? false))
            {
                if (!TimeSpan.TryParse(TimeFromBox.Text, out var tf) ||
                    !TimeSpan.TryParse(TimeToBox.Text, out var tt))
                {
                    MessageBox.Show("Неверный формат времени.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                timeFrom = tf;
                timeTo = tt;
            }

            var schedule = new DoctorSchedule
            {
                DoctorId = (int)DoctorBox.SelectedValue,
                WorkDate = workDate,
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                IsDayOff = IsDayOffCheck.IsChecked ?? false
            };

            _scheduleRepo.Add(schedule);

            MessageBox.Show("Запись в график добавлена.", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}