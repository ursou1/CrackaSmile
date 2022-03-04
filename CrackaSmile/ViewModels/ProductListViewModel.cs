using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class ProductListViewModel: BaseViewModel
    {

        #region
        public List<ClientApi> clients { get; set; }
        #endregion

        #region

        #endregion

        #region ctor
        public ProductListViewModel()
        {
            TakeListClients();

        }
        #endregion

        public async Task TakeListClients()
        {
            var result = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result);
            SignalChanged("clients");
        }

    }
}
