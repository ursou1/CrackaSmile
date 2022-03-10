using CrackaSmile.Tools;
using CrackaSmile.Views;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class ClientListViewModel: BaseViewModel
    {
        #region properties
        public ClientApi selectedClient;
        public ClientApi SelectedClient
        {
            get => selectedClient;
            set
            {
                selectedClient = value;
                SignalChanged();
            }
        }
        public List<ClientApi> clients { get; set; }
        #endregion



        #region Commands
        public CustomCommand AddClient { get; set; }
        public CustomCommand EditClient { get; set; }
        #endregion

        #region ctor
        public ClientListViewModel()
        {
            TakeListClients();

            AddClient = new CustomCommand(()=>
            {
                EditClientWin editClient = new EditClientWin();
                editClient.ShowDialog();
            });

            EditClient = new CustomCommand(() =>
            {
                if (SelectedClient == null) return;
                EditClientWin editClient = new EditClientWin(SelectedClient);
                editClient.ShowDialog();
            });
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
