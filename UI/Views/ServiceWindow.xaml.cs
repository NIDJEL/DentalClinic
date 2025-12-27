using DentalClinic.Data;
using DentalClinic.Models;
using System.Globalization;
using System.Windows;

namespace DentalClinic.UI.Views
{
    public partial class ServiceWindow : Window
    {
        private readonly ServiceRepository _repo = new ServiceRepository();

        public ServiceWindow()
        {
            InitializeComponent();

            CategoryBox.ItemsSource = _repo.GetCategories();
            CategoryBox.DisplayMemberPath = "Name";
            CategoryBox.SelectedValuePath = "CategoryId";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название услуги.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                MessageBox.Show("Неверный формат цены.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(DurationBox.Text, out var dur))
            {
                MessageBox.Show("Неверная длительность.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var service = new Service
            {
                CategoryId = (int)CategoryBox.SelectedValue,
                Name = NameBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescrBox.Text) ? null : DescrBox.Text.Trim(),
                DefaultPrice = price,
                DefaultDurationMin = dur,
                IsActive = IsActiveCheck.IsChecked ?? true
            };

            _repo.Add(service);

            MessageBox.Show("Услуга добавлена.", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
