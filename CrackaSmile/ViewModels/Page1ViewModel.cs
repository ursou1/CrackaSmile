using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class Page1ViewModel
    {
        //public ISeries[] Series { get; set; }
        //    = new ISeries[]
        //    {
        //        new PieSeries<double> {  Values = new double[] { zanatiemesta } },
        //        new PieSeries<double> { Values = new double[] { placecount } }
        //    };
        public Page1ViewModel()
        {
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
