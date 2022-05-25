using CrackaSmile.Tools;
using CrackaSmile.Views;
using ModelsApi;
using Spire.Xls;
using Spire.Xls.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class ProductListViewModel: BaseViewModel
    {

        #region properties

        public ObservableCollection<string> AutoTB { get; set; }
        public List<DeliveryNoteApi> deliveryNotes { get; set; }
        public List<DeliveryNoteApi> DeliveryNotes
        {
            get => deliveryNotes;
            set
            {
                deliveryNotes = value;
                SignalChanged();
            }
        }

        public List<ProductApi> countForSearch { get; set; }
        public List<ProductApi> CountForSearch
        {
            get => countForSearch;
            set
            {
                countForSearch = value;
                SignalChanged();
            }
        }

        public ProductApi selectedProduct;
        public ProductApi SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                SignalChanged();
            }
        }

        public List<UnitApi> units { get; set; }
        public List<UnitApi> Units
        {
            get => units;
            set
            {
                units = value;
                SignalChanged();
            }
        }

        public List<ProductTypeApi> productTypes { get; set; }
        public List<ProductTypeApi> ProductTypes
        {
            get => productTypes;
            set
            {
                productTypes = value;
                SignalChanged();
            }
        }


        public List<ProductApi> products { get; set; }
        public List<ProductApi> Products
        {
            get => products;
            set
            {
                products = value;
                SignalChanged();
            }
        }
        #endregion

        #region search&sort
        public List<ProductApi> mysearch { get; set; }

        public string SearchCountRows
        {
            get => searchCountRows;
            set
            {
                searchCountRows = value;
                SignalChanged();
            }
        }
        private string pages;
        public string Pages
        {
            get => pages;
            set
            {
                pages = value;
                SignalChanged();
            }
        }
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }
        public List<string> SortTypes { get; set; }
        private string selectedSortType;
        public string SelectedSortType
        {
            get => selectedSortType;
            set
            {
                selectedSortType = value;
                Sort();
            }
        }
        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Search();
            }
        }
        public List<string> ViewCountRows { get; set; }
        public string SelectedViewCountRows
        {
            get => selectedViewCountRows;
            set
            {
                selectedViewCountRows = value;
                paginationPageIndex = 0;
                Pagination();
            }
        }
        public int rows = 0;
        public int CountPages = 0;
        public List<ProductApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        #endregion

        #region commands
        public CustomCommand AddProduct { get; set; }
        public CustomCommand EditProduct { get; set; }
        public CustomCommand InfoProduct { get; set; }
        public CustomCommand DeleteProduct { get; set; }
        public CustomCommand ExportExcel { get; set; }
        public CustomCommand Delete { get; set; }
        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        #endregion

        #region ctor
        public ProductListViewModel()
        {
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                InitPagination();
                Pagination();
            });

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "15", "все" });
            selectedViewCountRows = ViewCountRows.First();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование" });
            selectedSearchType = SearchType.First();

            SortTypes = new List<string>();
            SortTypes.AddRange(new string[] { "По умолчанию", "По алфавиту: А-Я", "По алфавиту: Я-А" });
            selectedSortType = SortTypes.First();

            //Task.Run(LoadEntities);//

            #region команды по работе с записями


            AddProduct = new CustomCommand(() =>
            {
                EditProductWin editProduct = new EditProductWin();
                editProduct.ShowDialog();
                Thread.Sleep(200);
                Task.Run(TakeListProducts);
            });

            Delete = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;///////////////////////////

            });

            ExportExcel = new CustomCommand(() =>
            {
                Print();
            });

            InfoProduct = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;
                ProductInfoWin infoProduct = new ProductInfoWin(SelectedProduct);
                infoProduct.ShowDialog();
            });

            EditProduct = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;
                EditProductWin editProduct = new EditProductWin(SelectedProduct);
                editProduct.ShowDialog();
                Thread.Sleep(200);
                Task.Run(TakeListProducts);
            });

            DeleteProduct = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить поставщика?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Task.Run(DeleteProductMethod);
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message);
                    }
                }
                else return;
            });
            #endregion

            #region странички

            BackPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                if (paginationPageIndex > 0)
                    paginationPageIndex--;
                Pagination();
            });

            ForwardPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                int.TryParse(SelectedViewCountRows, out int rowsOnPage);
                if (rowsOnPage == 0)
                    return;
                int countPage = searchResult.Count() / rowsOnPage;
                CountPages = countPage;
                if (searchResult.Count() % rowsOnPage != 0)
                    countPage++;
                if (countPage > paginationPageIndex + 1)
                    paginationPageIndex++;
                Pagination();

            });
            #endregion
        }
        #endregion

        internal void Sort()
        {

            if (SelectedSortType == "По умолчанию")
                return;
            else if (SelectedSortType == "По алфавиту: А-Я")
                searchResult.Sort((x, y) => x.Name.CompareTo(y.Name));
            else if (SelectedSortType == "По алфавиту: Я-А")
                searchResult.Sort((x, y) => y.Name.CompareTo(x.Name));

            paginationPageIndex = 0;
            Pagination();
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            //Task.Run(LoadEntities);
            searchResult = mysearch.Where(c => c.Name.ToLower().Contains(search) ||
            c.Code.Contains(search)).ToList();//c.DeliveryNote.Number.ToString().Contains(search) ||

            Sort();
            InitPagination();
            Pagination();
        }

        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            Products = new List<ProductApi>(result);
            
            var result1 = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            DeliveryNotes = new List<DeliveryNoteApi>(result1);
            
            foreach (var product in Products)
            {
                if (product.DeliveryNoteId != null)
                {
                    product.DeliveryNote = DeliveryNotes.First(s => s.Id == product.DeliveryNoteId);
                }
                
            }

            #region AutoTB
            AutoTB = new ObservableCollection<string>();
            foreach (var item in Products)
            {
                AutoTB.Add(item.Name);
                AutoTB.Add(item.Name.ToLower());
                AutoTB.Add(item.Code);
                AutoTB.Add(item.Code.ToLower());
            }
            SignalChanged("AutoTB");
            #endregion

            var res1 = await Api.GetListAsync<ProductTypeApi[]>("ProductType");
            var res2 = await Api.GetListAsync<UnitApi[]>("Unit");

            productTypes = new List<ProductTypeApi>(res1);

            units = new List<UnitApi>(res2);


            searchResult = new List<ProductApi>(Products);
            mysearch = new List<ProductApi>(Products);

            CountForSearch = new List<ProductApi>(result);//для вывода кол-ва записей снизу
        }

        //public async Task LoadEntities()
        //{
        //    var result = await Api.GetListAsync<ProductApi[]>("Product");
        //    Products = new List<ProductApi>(result);
        //}

        public async Task DeleteProductMethod()
        {

            await Api.PutAsync<ProductApi>(selectedProduct, "Product");
        }

        private void InitPagination()
        {
            if (CountForSearch != null)
            {
                SearchCountRows = $"Найдено записей: {searchResult.Count} из {CountForSearch.Count()}";
            }
            else
                SearchCountRows = $"Ни одной записи не найдено";

            paginationPageIndex = 0;
        }
        

        private void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Products = searchResult;
            }
            else
            {
                Products = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }


        #region Вывод в эксель
        public void Print()
        {




            var workbook = new Workbook();
            var sheet = workbook.Worksheets[0];
            sheet.Range["F1"].Value = DateTime.Now.ToShortDateString();
            sheet.Range["F1"].Style.Borders.Color = Color.FromArgb(0, 0, 128);
            //sheet.Range["F1"].Style.Borders.LineStyle = LineStyleType.Thin;
            sheet.Range["F1"].Style.Color = Color.ForestGreen;

            sheet.Range["A3"].Value = $"№  ";

            sheet.Range["B3"].Value = $"Код   ";

            sheet.Range["C3"].Value = $"Наименование   ";

            sheet.Range["D3"].Value = $"Количество   ";

            sheet.Range["E3"].Value = $"Цена   ";

            sheet.Range["F3"].Value = $"Ед. изм.   ";

            sheet.Range["G3"].Value = $"Тип продукта   ";

            int index = 4;
            int count = 1;

            foreach (var product in products)
            {
                var unitid = product.Unit = units.First(s => s.Id == product.UnitId);
                //var delid = product.DeliveryNote = ChangeQuantityProducts.First(s => s.Id == product.Idsqp);
                //var depid = product.DeliveryNote = ChangeQuantityProducts.First(s => s.Id == product.Idsqp);
                var prodtype = product.ProductType = productTypes.First(s => s.Id == product.ProductTypeId);


                sheet.Range[$"A{index}"].NumberValue = count++;

                sheet.Range[$"B{index}"].Value = product.Code;

                sheet.Range[$"C{index}"].Value = product.Name;

                sheet.Range[$"D{index}"].NumberValue = product.Count;

                sheet.Range[$"E{index}"].Value = product.Price.ToString();

                sheet.Range[$"F{index}"].Value = product.Unit.Title;
                
                sheet.Range[$"G{index}"].Value = product.ProductType.Title;

                index++;
            }

            int bolding = products.Count + 3;
            sheet.Range["A3:G3"].Style.Font.IsBold = true;

            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeTop].Color = Color.FromArgb(0, 0, 128);
            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;

            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeBottom].Color = Color.FromArgb(0, 0, 128);
            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;

            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeLeft].Color = Color.FromArgb(0, 0, 128);
            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;

            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeRight].Color = Color.FromArgb(0, 0, 128);
            sheet.Range[$"A3:G{bolding}"].Style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;

            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeTop].Color = Color.FromArgb(0, 0, 128);
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeBottom].Color = Color.FromArgb(0, 0, 128);
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeLeft].Color = Color.FromArgb(0, 0, 128);
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeRight].Color = Color.FromArgb(0, 0, 128);
            //sheet.Range["A3:I100"].Style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;

            Chart chart = sheet.Charts.Add(ExcelChartType.Pie3D);
            chart.DataRange = sheet.Range["K2:L5"];
            chart.SeriesDataFromRange = false;
            chart.LeftColumn = 10; //колонка спавна слева
            chart.TopRow = 7; // отступ строк сверху
            chart.RightColumn = 15; //колонка спавна справа
            chart.BottomRow = 20; // высота диаграмы
            chart.ChartTitle = "Соотношение количества";
            chart.ChartTitleArea.IsBold = true;
            chart.ChartTitleArea.Size = 12;
            ChartSerie cs = chart.Series[0];
            cs.Values = sheet.Range[$"D4:D{bolding}"];
            cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;



            Chart chartCercle = sheet.Charts.Add();
            chartCercle.ChartType = ExcelChartType.Doughnut;
            chartCercle.DataRange = sheet.Range[$"D4:D{bolding}"];
            chartCercle.SeriesDataFromRange = false;

            chartCercle.LeftColumn = 10;//10
            chartCercle.TopRow = 22;//22
            chartCercle.RightColumn = 15;//15
            chartCercle.BottomRow = 32;//34

            chartCercle.ChartTitle = "Кол-во товаров";
            chartCercle.ChartTitleArea.IsBold = true;
            chartCercle.ChartTitleArea.Size = 12;
            chartCercle.Legend.Delete();
            foreach (ChartSerie csCercle in chartCercle.Series)
            {
                csCercle.DataPoints.DefaultDataPoint.DataLabels.HasPercentage = true;
            }




            sheet.AllocatedRange.AutoFitColumns();

            workbook.SaveToFile("text1.xls");
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "/text1.xls")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        #endregion


    }
}
