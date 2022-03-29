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
        #region par
        public ClientApi AddClient { get; set; }
        public List<ClientApi> clients { get; set; }
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        #endregion


        public EditClientViewModel(ClientApi client)
        {
            Task.Run(TakeListClients);

            if (client == null)
            {
                AddClient = new ClientApi();
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
            }

            Save = new CustomCommand(() =>
            {
                if (AddClient.Id == 0)
                    Task.Run(CreateNewClient);
                else
                    Task.Run(EditClient);

                
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }
                
            });
        }

        public async Task CreateNewClient()
        {
            await Api.PostAsync<ClientApi>(AddClient, "Client");
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
