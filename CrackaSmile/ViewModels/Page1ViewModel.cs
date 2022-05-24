using CrackaSmile.Tools;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class Page1ViewModel: BaseViewModel
    {

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

        public List<ClientApi> clients { get; set; }
        public List<ClientApi> Clients
        {
            get => clients;
            set
            {
                clients = value;
                SignalChanged();
            }
        }

        public List<ProviderApi> providers { get; set; }
        public List<ProviderApi> Providers
        {
            get => providers;
            set
            {
                providers = value;
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
        public ISeries[] Series { get; set; }
        public ISeries[] Series1 { get; set; }
            
            

         int del = 0;
         int dep = 0;
        
        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            products = new List<ProductApi>(result);

            var result1 = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            deliveryNotes = new List<DeliveryNoteApi>(result1);

            var result2 = await Api.GetListAsync<DepartNoteApi[]>("DepartNote");
            departNotes = new List<DepartNoteApi>(result2);

            var result3 = await Api.GetListAsync<ClientApi[]>("Client");
            Clients = new List<ClientApi>(result3);

            var result4 = await Api.GetListAsync<ProviderApi[]>("Provider");
            Providers = new List<ProviderApi>(result4);


            

            

        }

        public void Some()
        {
            DateTime dt = new DateTime();
            DateTime lt = new DateTime();
            dt = DateTime.Today;
            lt = dt.AddMonths(-1);

            foreach (var product in Products)
            {
                if (product.DeliveryNoteId != null)
                {
                    product.DeliveryNote = deliveryNotes.First(s => s.Id == product.DeliveryNoteId);
                }
                if (product.DepartNoteId != null)
                {
                    product.DepartNote = departNotes.First(s => s.Id == product.DepartNoteId);
                }
            }

            //SignalChanged("Series");
            foreach (var item in Products)
            {
                if (item.DeliveryNoteId != null)
                {
                    //if (item.DeliveryNote.DeliveryDate < lt)
                    //{
                    int result = DateTime.Compare(item.DeliveryNote.DeliveryDate, lt);
                    string relationship;
                    if (result < 0)
                        relationship = "";
                    else if (result == 0)
                        del++;
                    else
                        del++;
                }
                if(item.DepartNoteId != null)
                {
                    int result = DateTime.Compare(item.DepartNote.DepartDate, lt);
                    string relationship1;
                    if (result < 0)
                        relationship1 = "";
                    else if (result == 0)
                        dep++;
                    else
                        dep++;
                }
            }
            SignalChanged("Series");
        }
        public CustomCommand count { get; set; }
        public Page1ViewModel()
        {
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                Some();
                Series = new ISeries[]
                {
                    new PieSeries<double> { Name = "Пришло товара за последний месяц", Values = new double[] { del } },//zanatiemesta
                    new PieSeries<double> { Name = "Ушло товара за последний месяц", Values = new double[] { dep } }//placecount
                };

                //Series1 = new ISeries[]
                //{
                //    new PieSeries<double> { Name = "Пришло товара", Values = new double[] { 2 } },
                //    new PieSeries<double> { Name = "Ушло товара", Values = new double[] { 1 } }
                //};

            });
            
            //Task.Run(TakeListProviders).ContinueWith(s =>
            //{
            //    Method();
            //});
        }
        //public async Task TakeListProviders()
        //{
        //    var result = await Api.GetListAsync<WorkPlaceApi[]>("WorkPlace");
        //    workPlaces = new List<WorkPlaceApi>(result);
        //    SignalChanged("workPlaces");
        //}

        //public List<WorkPlaceApi> workPlaces { get; set; }
        //public List<WorkPlaceApi> WorkPlaces
        //{
        //    get => workPlaces;
        //    set
        //    {
        //        workPlaces = value;
        //        SignalChanged();
        //    }
        //}

        static int placecount = 0;
        static int zanatiemesta = 0;

        //public void Method()
        //{
        //    foreach (var item in workPlaces)
        //    {
        //        placecount++;
        //        if (item.IdResponsiblePerson != 0)
        //        {
        //            zanatiemesta++;
        //        }
        //    }
        //}
    }
}
