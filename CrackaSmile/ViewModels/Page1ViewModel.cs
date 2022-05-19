﻿using CrackaSmile.Tools;
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
        public ISeries[] Series { get; set; }
            = new ISeries[]
            {
                new PieSeries<double> { Name = "Sava", Values = new double[] { 2 } },//zanatiemesta
                new PieSeries<double> { Name = "Dima", Values = new double[] { 1 } }//placecount
            };


        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            products = new List<ProductApi>(result);

            var result1 = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            deliveryNotes = new List<DeliveryNoteApi>(result1);

            foreach (var product in Products)
            {
                product.DeliveryNote = deliveryNotes.First( s => s.Id == product.DeliveryNoteId);
            }

        }

        //public void Some()
        //{
        //    DateTime dt = new DateTime();
        //    DateTime lt = new DateTime();
        //    dt = DateTime.Today;
        //    lt = dt.AddMonths(-1);
        //    int sorted = 0;
            
        //    //System.Windows.MessageBox.Show($"сегодня {dt} "   + $" месяц назад {lt}" );
        //    for (int i = 0; i < 32; i++)
        //    {
        //        foreach (var item in products)
        //        {
        //            if (item.DeliveryNote.DeliveryDate == lt.Date)
        //            {
        //                sorted++;
        //            }
        //        }
        //        lt.AddDays(1);
        //    }

        //    System.Windows.MessageBox.Show($"итого {sorted}");
        //}
        public CustomCommand count { get; set; }
        public Page1ViewModel()
        {
            //Task.Run(TakeListProducts);
            //count = new CustomCommand(() =>
            //{
            //    Some();
            //});
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
