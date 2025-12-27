using DentalClinic.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace DentalClinic.Services
{
    public static class ReportPdfGenerator
    {
        public static void GenerateMonthlyIncomeReport(
            string filePath,
            DateTime monthDate,
            IList<DoctorIncomeReport> data,
            decimal totalOverall,
            decimal totalMonth)
        {
            var culture = new CultureInfo("ru-RU");
            var monthName = culture.DateTimeFormat.GetMonthName(monthDate.Month);
            var periodText = $"{monthName} {monthDate.Year} г.";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("РЖД-Медицина | Стоматология")
                            .FontSize(16).SemiBold().FontColor(Colors.Blue.Darken2);

                        col.Item().Text("Отчёт по доходам врачей")
                            .FontSize(13).SemiBold();

                        col.Item().Text(periodText)
                            .FontSize(11).FontColor(Colors.Grey.Darken2);

                        col.Item().PaddingTop(5)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Врач");
                                header.Cell().Element(HeaderCell).AlignCenter().Text("Приёмов");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Доход, руб.");

                                static IContainer HeaderCell(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold())
                                            .Padding(4)
                                            .Background(Colors.Grey.Lighten3);
                            });

                            foreach (var row in data)
                            {
                                table.Cell().Element(Cell).Text(row.DoctorName);
                                table.Cell().Element(Cell).AlignCenter().Text(row.AppointmentsCount.ToString());
                                table.Cell().Element(Cell).AlignRight().Text(row.TotalIncome.ToString("N2", culture));

                                static IContainer Cell(IContainer container) =>
                                    container.Padding(4)
                                             .BorderBottom(1)
                                             .BorderColor(Colors.Grey.Lighten3);
                            }
                        });

                        col.Item().PaddingTop(15).Column(c =>
                        {
                            c.Item().Text($"Общий доход организации: {totalOverall:N2} руб.")
                                   .SemiBold();

                            c.Item().Text($"Доход за период ({periodText}): {totalMonth:N2} руб.")
                                   .SemiBold();
                        });
                    });

                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Сформировано: ");
                        txt.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm", culture))
                           .FontColor(Colors.Grey.Darken1);
                    });
                });
            })
            .GeneratePdf(filePath);
        }
    }
}
