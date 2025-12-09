using DentalClinic.Data;
using DentalClinic.Models;
using Microsoft.Win32;
using DentalClinic.Services;

using System.Windows;

namespace DentalClinic.UI.Views
{
    public partial class MainWindow : Window
    {
        private readonly AppUser _currentUser;
        private readonly ReportRepository _reportRepo = new ReportRepository();


        public MainWindow(AppUser user)
        {
            InitializeComponent();
            _currentUser = user;

            Title = $"Стоматологический кабинет – {_currentUser.Username} ({_currentUser.Role})";

            // Настройка растягивания колонок DataGrid
            MainGrid.AutoGeneratingColumn += MainGrid_AutoGeneratingColumn;

            ApplyRolePermissions();
        }

        private void MainGrid_AutoGeneratingColumn(object? sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            // Для колонок с ID устанавливаем меньшую ширину
            if (e.Column.Header != null && e.Column.Header.ToString()!.ToLower().Contains("id"))
            {
                e.Column.Width = new System.Windows.Controls.DataGridLength(80, System.Windows.Controls.DataGridLengthUnitType.Pixel);
            }
            else
            {
                // Для остальных колонок - равномерное распределение
                e.Column.Width = new System.Windows.Controls.DataGridLength(1, System.Windows.Controls.DataGridLengthUnitType.Star);
            }
        }

        private void ApplyRolePermissions()
        {
            if (_currentUser.Role == "doctor")
            {
                // скрытие разделов для врача
                BtnDoctors.Visibility = Visibility.Collapsed;
                BtnAddDoctor.Visibility = Visibility.Collapsed;
                BtnServices.Visibility = Visibility.Collapsed;
                BtnReports.Visibility = Visibility.Collapsed;
                BtnAddPatient.Visibility = Visibility.Collapsed;
                BtnAddService.Visibility = Visibility.Collapsed;
                BtnAddSchedule.Visibility = Visibility.Collapsed;

                // Скрытие заголовков разделов, где все кнопки скрыты
                HeaderDoctors.Visibility = Visibility.Collapsed;
                HeaderServices.Visibility = Visibility.Collapsed;
                HeaderReports.Visibility = Visibility.Collapsed;

                BtnAppointments_Click(null, null);
            }
            else if (_currentUser.Role == "registrar")
            {
                BtnReports.Visibility = Visibility.Collapsed;
                HeaderReports.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadAppointments()
        {
            var repo = new AppointmentRepository();

            int? doctorIdFilter = null;

            if (_currentUser.Role == "doctor" && _currentUser.DoctorId.HasValue)
                doctorIdFilter = _currentUser.DoctorId.Value;

            MainGrid.ItemsSource = repo.GetAll(doctorIdFilter);
        }

        private void LoadDoctors()
        {
            var repo = new DoctorRepository();
            MainGrid.ItemsSource = repo.GetAll();
        }

        private void LoadReportForMonth(DateTime monthDate)
        {
            // Все время
            var allList = _reportRepo.GetDoctorIncomeReport();
            var totalAll = allList.Sum(r => r.TotalIncome);

            // За выбранный месяц
            var monthList = _reportRepo.GetDoctorIncomeReportByMonth(monthDate.Year, monthDate.Month);
            var totalMonth = monthList.Sum(r => r.TotalIncome);

            // Показываем в таблице данные за месяц
            MainGrid.ItemsSource = monthList;

            // Обновляем подписи
            LblTotalOverall.Text = totalAll.ToString("N2");
            LblTotalMonth.Text = totalMonth.ToString("N2");
        }


        private void BtnBuildReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportMonthPicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите месяц для отчёта.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var date = ReportMonthPicker.SelectedDate.Value;
            LoadReportForMonth(date);
        }



        // Кнопки меню

        private void BtnDoctors_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            LoadDoctors();
        }

        private void BtnPatients_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var repo = new PatientRepository();
            MainGrid.ItemsSource = repo.GetAll();
        }

        private void BtnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            int? doctorId = null;

            if (_currentUser.Role == "doctor" && _currentUser.DoctorId.HasValue)
                doctorId = _currentUser.DoctorId.Value;

            var win = new AppointmentWindow(doctorId);
            win.Owner = this;
            win.ShowDialog();

            LoadAppointments();
        }

        private void BtnAddDoctor_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var win = new DoctorWindow();
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                BtnDoctors_Click(null, null);
            }
        }

        private void BtnServices_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var repo = new ServiceRepository();
            MainGrid.ItemsSource = repo.GetAll();
        }

        private void BtnAppointments_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            LoadAppointments();
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var repo = new ScheduleRepository();

            int? doctorIdFilter = null;
            if (_currentUser.Role == "doctor" && _currentUser.DoctorId.HasValue)
                doctorIdFilter = _currentUser.DoctorId.Value;

            MainGrid.ItemsSource = repo.GetDoctorSchedule(doctorIdFilter);
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            if (ReportMonthPicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите месяц отчёта перед выгрузкой в PDF.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var monthDate = ReportMonthPicker.SelectedDate.Value;

            // Получаем данные так же, как в LoadReportForMonth
            var allList = _reportRepo.GetDoctorIncomeReport();
            var totalAll = allList.Sum(r => r.TotalIncome);

            var monthList = _reportRepo.GetDoctorIncomeReportByMonth(monthDate.Year, monthDate.Month);
            var totalMonth = monthList.Sum(r => r.TotalIncome);

            if (monthList.Count == 0)
            {
                if (MessageBox.Show("За выбранный месяц нет данных. Всё равно сохранить PDF?",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Сохранить отчёт в PDF",
                Filter = "PDF файлы (*.pdf)|*.pdf",
                FileName = $"Отчёт_РЖДМедицина_Стоматология_{monthDate:yyyy_MM}.pdf"
            };

            if (dlg.ShowDialog(this) == true)
            {
                try
                {
                    ReportPdfGenerator.GenerateMonthlyIncomeReport(
                        dlg.FileName, monthDate, monthList, totalAll, totalMonth);

                    MessageBox.Show("Отчёт успешно сохранён.", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при формировании PDF: " + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Visible;
            var today = DateTime.Today;
            if (ReportMonthPicker != null)
                ReportMonthPicker.SelectedDate = today;

            LoadReportForMonth(today);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // ====== НОВЫЕ ОБРАБОТЧИКИ КНОПОК ДОБАВЛЕНИЯ ======

        private void BtnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var win = new PatientWindow();
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                BtnPatients_Click(null, null);
            }
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var win = new ServiceWindow();
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                BtnServices_Click(null, null);
            }
        }

        private void BtnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            var win = new ScheduleWindow();
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                BtnSchedule_Click(null, null);
            }
        }
    }
}
