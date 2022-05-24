using CrackaSmile.Tools;
using CrackaSmile.Views;
using Enterwell.Clients.Wpf.Notifications;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class DeliveryNoteViewModel: BaseViewModel
    {

        #region properties

        public ObservableCollection<string> AutoTB { get; set; }
        public List<ProviderApi> providers { get; set; }

        public List<DeliveryNoteApi> countForSearch { get; set; }
        public List<DeliveryNoteApi> CountForSearch
        {
            get => countForSearch;
            set
            {
                countForSearch = value;
                SignalChanged();
            }
        }

        public DeliveryNoteApi selectedDeliveryNote;
        public DeliveryNoteApi SelectedDeliveryNote
        {
            get => selectedDeliveryNote;
            set
            {
                selectedDeliveryNote = value;
                SignalChanged();
            }
        }

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

        #endregion

        #region alerts
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
        public void EditDelNoteNotification()
        {
            Manager
                .CreateMessage()
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Accent("#327d0b")
                .Background("#3E63BB")
                .HasMessage("Вы изменили накладную.")
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }

        public void AddDelNoteNotification()
        {
            Manager
                .CreateMessage()
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Accent("#327d0b")
                .Background("#EEA75D")
                .HasMessage("Вы добавили накладную.")
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }

        public void DeleteDelNoteNotification()
        {
            Manager
                .CreateMessage()
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Accent("#700d04")
                .Background("#D74258")
                .HasMessage("Вы удалили накладную.")
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }

        #endregion

        #region search&sort
        public List<DeliveryNoteApi> mysearch { get; set; }

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
        public List<DeliveryNoteApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        #endregion

        #region Commands
        public CustomCommand AddDeliveryNote { get; set; }
        public CustomCommand EditProductInDelivery { get; set; }
        public CustomCommand EditDeliveryNote { get; set; }
        public CustomCommand DeleteDeliveryNote { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        #endregion

        public DeliveryNoteViewModel()
        {
            Task.Run(TakeListDeliveryNotes).ContinueWith(s =>
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
            SortTypes.AddRange(new string[] { "По умолчанию", "По дате: новые", "По дате: старые"});
            selectedSortType = SortTypes.First();

            //Task.Run(LoadEntities);

            #region команды по работе с записями

            EditProductInDelivery = new CustomCommand(() =>
            {
                if (SelectedDeliveryNote == null) return;
                EditProductInDeliveryWin editProductInDeliveryWin = new EditProductInDeliveryWin(SelectedDeliveryNote);
                editProductInDeliveryWin.ShowDialog();
                EditDelNoteNotification();
            });

            AddDeliveryNote = new CustomCommand(() =>
            {
                EditDeliveryNoteWin editDeliveryNote = new EditDeliveryNoteWin();
                editDeliveryNote.ShowDialog();
                AddDelNoteNotification();
                Thread.Sleep(200);
                Task.Run(TakeListDeliveryNotes);
            });

            EditDeliveryNote = new CustomCommand(() =>
            {
                if (SelectedDeliveryNote == null) return;
                EditDeliveryNoteWin editDeliveryNote = new EditDeliveryNoteWin(SelectedDeliveryNote);
                editDeliveryNote.ShowDialog();
                EditDelNoteNotification();
                Thread.Sleep(200);
                Task.Run(TakeListDeliveryNotes);
            });

            DeleteDeliveryNote = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить поставщика?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Task.Run(DeleteDeliveryNoteMethod);
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
            else if (SelectedSortType == "По дате: старые")
                searchResult.Sort((x, y) => x.DeliveryDate.CompareTo(y.DeliveryDate));
            else if (SelectedSortType == "По дате: новые")
                searchResult.Sort((x, y) => y.DeliveryDate.CompareTo(x.DeliveryDate));

            paginationPageIndex = 0;
            Pagination();

        }

        private void Search()
        {
            var search = SearchText.ToLower();
            //Task.Run(TakeListDeliveryNotes);
            searchResult = mysearch.Where(c => c.Number.ToString().Contains(search) ||
            c.DeliveryDate.ToString().Contains(search) ||
            c.Provider.Name.ToLower().Contains(search)).ToList();

            Sort();
            InitPagination();
            Pagination();
        }
        public async Task TakeListDeliveryNotes()
        {
            var result = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            DeliveryNotes = new List<DeliveryNoteApi>(result);
            
            var result1 = await Api.GetListAsync<ProviderApi[]>("Provider");
            providers = new List<ProviderApi>(result1);

            foreach (var deliveryNote in DeliveryNotes)
            {
                deliveryNote.Provider = providers.First(s => s.Id == deliveryNote.ProviderId);
            }
           
            searchResult = new List<DeliveryNoteApi>(DeliveryNotes);
            mysearch = new List<DeliveryNoteApi>(DeliveryNotes);

            #region AutoTB
            AutoTB = new ObservableCollection<string>();
            foreach (var item in DeliveryNotes)
            {
                AutoTB.Add(item.Number.ToString());
                AutoTB.Add(item.DeliveryDate.ToShortDateString().ToString());
            }
            SignalChanged("AutoTB");
            #endregion

            CountForSearch = new List<DeliveryNoteApi>(result);//для вывода кол-ва записей снизу
        }


        public async Task DeleteDeliveryNoteMethod()
        {
            await Api.DeleteAsync<DeliveryNoteApi>(selectedDeliveryNote, "DeliveryNote");
        }

        public async Task LoadEntities()
        {
            var result = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            mysearch = new List<DeliveryNoteApi>(result);
            DeliveryNotes = new List<DeliveryNoteApi>(result);
            //var result1 = await Api.GetListAsync<ProviderApi[]>("Provider");
            //providers = new List<ProviderApi>(result1);
            //SignalChanged("providers");
            //foreach (var deliveryNote in deliveryNotes)
            //{
            //    deliveryNote.Provider = providers.First(s => s.Id == deliveryNote.ProviderId);
            //}
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
                DeliveryNotes = searchResult;
            }
            else
            {
                DeliveryNotes = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }
    }
}
