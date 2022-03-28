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
    public class EditProviderViewModel: BaseViewModel
    {
        #region par
        public ProviderApi AddProvider { get; set; }
        public List<ProviderApi> providers { get; set; }
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        #endregion

        public EditProviderViewModel(ProviderApi provider)
        {
            Task.Run(TakeListProviders);
            if (provider == null)
            {
                AddProvider = new ProviderApi();
            }
            else
            {
                AddProvider = new ProviderApi
                {
                    Id = provider.Id,
                    Name = provider.Name,
                    Email = provider.Email,
                    Telephone = provider.Telephone
                };
            }

            Save = new CustomCommand(() =>
            {
                if (AddProvider.Id == 0)
                    Task.Run(CreateNewProvider);
                else
                    Task.Run(EditProvider);


                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }

            });
        }

        public async Task CreateNewProvider()
        {
            await Api.PostAsync<ProviderApi>(AddProvider, "Provider");
        }

        public async Task EditProvider()
        {
            await Api.PutAsync<ProviderApi>(AddProvider, "Provider");
        }

        public async Task TakeListProviders()
        {
            var result = await Api.GetListAsync<ProviderApi[]>("Provider");
            providers = new List<ProviderApi>(result);
            SignalChanged("providers");
        }
        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
    }
}
