using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;

namespace CandyKeeper.Presentation.Views.Windows
{
    public partial class DiagramWindow : UserControl, INotifyPropertyChanged
    {
        private string[] _storeNames;
        public string[] StoreNames
        {
            get => _storeNames;
            set
            {
                _storeNames = value;
                OnPropertyChanged();
            }
        }

        private ChartValues<double> _averagePriceValues;
        public ChartValues<double> AveragePriceValues
        {
            get => _averagePriceValues;
            set
            {
                _averagePriceValues = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> Formatter { get; set; }

        public DiagramWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var storeDataList = await GetStoresWithHigherAveragePrice();

                if (storeDataList == null || !storeDataList.Any())
                {
                    Console.WriteLine("No data retrieved from the database.");
                    return;
                }

                var topStoreDataList = storeDataList.ToList();

                StoreNames = topStoreDataList.Select(sd => sd.StoreName).ToArray();
                AveragePriceValues = new ChartValues<double>(topStoreDataList.Select(sd => (double)sd.AveragePriceInStore));

                Formatter = value => value.ToString("N");

                Console.WriteLine("StoreNames: " + string.Join(", ", StoreNames));
                Console.WriteLine("Average Prices: " + string.Join(", ", AveragePriceValues));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading data: {ex.Message}");
            }
        }

        private async Task<List<StoreData>> GetStoresWithHigherAveragePrice()
        {
            var storeDataList = new List<StoreData>();

            await using (SqlConnection connection = new SqlConnection("data source=(localdb)\\MSSQLLocalDB;Initial Catalog=candyKeeper;Integrated Security=True;"))
            {
                await connection.OpenAsync();

                string query = $"""
                            select top 10
                                s.Name as StoreName,
                                (
                                    select AVG(pfs.Price)
                                    from ProductForSales pfs
                                    where pfs.StoreId = s.Id
                                ) as AveragePriceInStore
                            from
                                Stores s
                            where
                                (
                                    select AVG(pfs.Price)
                                    from ProductForSales pfs
                                    where pfs.StoreId = s.Id
                                ) > (
                                    select AVG(pfs.Price)
                                    from ProductForSales pfs
                                )
                            order by
                                AveragePriceInStore desc
                            """;

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var storeData = new StoreData
                            {
                                StoreName = reader.GetString(0),
                                AveragePriceInStore = reader.GetDecimal(1)
                            };
                            storeDataList.Add(storeData);
                        }
                    }
                }
            }

            return storeDataList;
        }

        public class StoreData
        {
            public string StoreName { get; set; }
            public decimal AveragePriceInStore { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExportBtn(object sender, RoutedEventArgs e)
        {
            var chartData = chart.Series[0].Values;
            
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "Store";
                worksheet.Cell(1, 2).Value = "Average Price";


                for (int i = 0; i < chartData.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = StoreNames[i];
                    worksheet.Cell(i + 2, 2).Value = (double)chartData[i]; 
                }
                
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChartData.xlsx");
                workbook.SaveAs(filePath);
            }

            MessageBox.Show("Данные успешно экспортированы ");
        }
    }
}