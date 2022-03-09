using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class EditClientViewModel: BaseViewModel
    {
        public ClientApi AddClient { get; set; }
        public EditClientViewModel(ClientApi client)
        {

            TakeListClients();

            if (client == null)
            {
                //CreateNewClient();
            }
            else
            {
                AddClient = new ClientApi
                {
                    Name = client.Name,
                    FatherName = client.FatherName,
                    LastName = client.LastName,
                    Address = client.Address,
                    Email = client.Email,
                    Telephone = client.Telephone
                };


            }
        }


        public async Task CreateNewClient()
        {

            ClientApi editClient = new ClientApi
            {
                Name = "test",
                FatherName = "test",
                LastName = "test",
                Address = "test",
                Email = "test",
                Telephone = "test"

            };
            await Api.PostAsync<ClientApi>(editClient,"Client");
        }

        public async Task EditClient()
        {

            ClientApi editClient = new ClientApi
            {
                Name = "test",
                FatherName = "test",
                LastName = "test",
                Address = "test",
                Email = "test",
                Telephone = "test"

            };
            await Api.PutAsync<ClientApi>(editClient, "Client");
        }

        public async Task TakeListClients()//вызов клиентов
        {
            var result = await Api.GetListAsync<ClientApi[]>("Client");
            SignalChanged("clients");
        }
    }
}
