using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class EditClientViewModel: BaseViewModel
    {
        #region properties
        private ClientApi addClient;
        public ClientApi AddClient
        {
            get => addClient;
            set
            {
                addClient = value;
                SignalChanged();
            }
        }

        //public List<ClientApi> clients { get; set; }
        //public List<ClientApi> Clients
        //{
        //    get => clients;
        //    set
        //    {
        //        clients = value;
        //        SignalChanged();
        //    }
        //}
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        #endregion


        public EditClientViewModel(ClientApi client)
        {
            //Task.Run(TakeListClients);

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

            Cancel = new CustomCommand(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }
            });


            Save = new CustomCommand(() =>
            {
                if (AddClient.Id == 0)
                {
                    try
                    {
                        if (AddClient.Name != null && AddClient.Telephone != null && AddClient.Address != null)
                        {
                            Task.Run(CreateNewClient);
                        }
                        else
                        {
                            MessageBox.Show("Проверьте заполнение данных!");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Проверьте заполнение данных!");
                    }
                }
                else
                {
                    Task.Run(EditClient);
                }
                    

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }
                
            });
        }

        public async Task CreateNewClient()
        {
            if(AddClient.Email == null)
            {
                AddClient.Email = "Отсутствует";
            }
            await Api.PostAsync<ClientApi>(AddClient, "Client");
        }

        public async Task EditClient()
        {
            await Api.PutAsync<ClientApi>(AddClient, "Client");
        }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
    }
}
