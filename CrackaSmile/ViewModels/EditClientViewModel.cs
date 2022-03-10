using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class EditClientViewModel: BaseViewModel
    {
        public ClientApi AddClient { get; set; }

        #region par
        
        public List<ClientApi> clients { get; set; }
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        #endregion


        public EditClientViewModel(ClientApi client)
        {
            TakeListClients();

            if (client == null)
            {
                AddClient = new ClientApi
                {
                    Name = "Name",
                    FatherName = "FatherName",
                    LastName = "NaLastNameme",
                    Address = "Address",
                    Email = "Email",
                    Telephone = "Telephone"
                };
            }
            else
            {
                AddClient = new ClientApi
                {
                    Id = client.Id,
                    Name = client.Name,
                    FatherName = client.FatherName,
                    LastName = client.LastName,
                    Address = client.Address,
                    Email = client.Email,
                    Telephone = client.Telephone
                };

                Save = new CustomCommand(() =>
                {
                    if (AddClient.Id != 0)
                    {
                        EditClient();
                    }
                    else
                    {
                        CreateNewClient();
                    }
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this) CloseWin(window);
                    }
                });
            }
        }


        public async Task CreateNewClient()
        {
            ClientApi editClient = new ClientApi
            {
                Name = AddClient.Name,
                FatherName = AddClient.FatherName,
                LastName = AddClient.LastName,
                Address = AddClient.Address,
                Email = AddClient.Email,
                Telephone = AddClient.Telephone

            };
            await Api.PostAsync<ClientApi>(editClient, "Client");
        }

        public async Task EditClient()
        {
            await Api.PutAsync<ClientApi>(AddClient, "Client");
        }

        public async Task TakeListClients()//вызов клиентов
        {
            var result = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result);
            SignalChanged("clients");
        }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
    }
}
