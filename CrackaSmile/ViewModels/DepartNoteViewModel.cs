using CrackaSmile.Tools;
using CrackaSmile.Views;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class DepartNoteViewModel: BaseViewModel
    {

        #region properties

        public List<ClientApi> clients { get; set; }

        public DepartNoteApi selectedDepartNote;
        public DepartNoteApi SelectedDepartNote
        {
            get => selectedDepartNote;
            set
            {
                selectedDepartNote = value;
                SignalChanged();
            }
        }

        public List<DepartNoteApi> departNotes { get; set; }
        public List<DepartNoteApi> DepartNotes
        {
            get => departNotes;
            set
            {
                departNotes = value;
                SignalChanged();
            }
        }

        #endregion

        #region search&sort
        public List<DepartNoteApi> mysearch { get; set; }

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
        public List<DepartNoteApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        #endregion

        #region Commands
        public CustomCommand AddDepartNote { get; set; }
        public CustomCommand EditDepartNote { get; set; }
        public CustomCommand DeleteDepartNote { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        #endregion

        public DepartNoteViewModel()
        {
            Task.Run(TakeListDepartNotes).ContinueWith(s =>
            {
                Task.Run(TakeListClients);
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

            Task.Run(LoadEntities);

            #region команды по работе с записями


            AddDepartNote = new CustomCommand(() =>
            {
                EditDepartNoteWin editDepartNote = new EditDepartNoteWin();
                editDepartNote.ShowDialog();
            });

            EditDepartNote = new CustomCommand(() =>
            {
                if (SelectedDepartNote == null) return;
                EditDepartNoteWin editDepartNote = new EditDepartNoteWin(SelectedDepartNote);
                editDepartNote.ShowDialog();
            });

            DeleteDepartNote = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить поставщика?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Task.Run(DeleteDepartNoteMethod);
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

        internal void Sort()
        {

            if (SelectedSortType == "По умолчанию")
                return;
            else if (SelectedSortType == "По алфавиту: А-Я")
                searchResult.Sort((x, y) => x.DepartDate.CompareTo(y.DepartDate));
            else if (SelectedSortType == "По алфавиту: Я-А")
                searchResult.Sort((x, y) => y.DepartDate.CompareTo(x.DepartDate));

            paginationPageIndex = 0;
            Pagination();
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            Task.Run(LoadEntities);
            searchResult = mysearch.Where(c => c.Number.ToString().Contains(search) ||
            c.DepartDate.ToString().Contains(search) ||
            c.Client.Name.ToLower().Contains(search)).ToList();

            Sort();
            InitPagination();
            Pagination();
        }

        public async Task TakeListDepartNotes()
        {
            var result = await Api.GetListAsync<DepartNoteApi[]>("DepartNote");
            departNotes = new List<DepartNoteApi>(result);
            SignalChanged("departNotes");
            searchResult = new List<DepartNoteApi>(result);

            var result1 = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result1);
            SignalChanged("clients");
            foreach (var departNote in departNotes)
            {
                departNote.Client = clients.First(s => s.Id == departNote.ClientId);
            }

        }

        public async Task TakeListClients()
        {
            var result1 = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result1);
            SignalChanged("clients");
        }

        public async Task DeleteDepartNoteMethod()
        {
            await Api.DeleteAsync<DepartNoteApi>(selectedDepartNote, "DepartNote");
        }

        public async Task LoadEntities()
        {
            var result = await Api.GetListAsync<DepartNoteApi[]>("DepartNote");
            mysearch = new List<DepartNoteApi>(result);
            departNotes = new List<DepartNoteApi>(result);
            var result1 = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result1);
            SignalChanged("clients");
            foreach (var departNote in departNotes)
            {
                departNote.Client = clients.First(s => s.Id == departNote.ClientId);
            }
        }

        private void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из ";
            paginationPageIndex = 0;
        }

        private void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                DepartNotes = searchResult;
            }
            else
            {
                DepartNotes = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }
    }
}
