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
    public class EditDeliveryNoteViewModel: BaseViewModel
    {
        #region par
        private DeliveryNoteApi addDeliveryNote;
        public DeliveryNoteApi AddDeliveryNote
        {
            get => addDeliveryNote;
            set
            {
                addDeliveryNote = value;
                SignalChanged();
            }
        }

        public List<DeliveryNoteApi> deliveryNotes { get; set; }
        public List<ProviderApi> providers { get; set; }

        public ProviderApi selectedProvider;
        public ProviderApi SelectedProvider
        {
            get => selectedProvider;
            set
            {
                selectedProvider = value;
                SignalChanged();
            }
        }
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        #endregion

        public EditDeliveryNoteViewModel(DeliveryNoteApi deliveryNote)
        {
            Task.Run(TakeListDeliveryNotes).ContinueWith(s=>
            {

                if (deliveryNote == null)
                {
                    AddDeliveryNote = new DeliveryNoteApi();
                }
                else
                {
                    AddDeliveryNote = new DeliveryNoteApi
                    {
                        Id = deliveryNote.Id,
                        Number = deliveryNote.Number,
                        DeliveryDate = deliveryNote.DeliveryDate,
                        ProviderId = deliveryNote.ProviderId,
                    };

                    SelectedProvider = providers.First(s => s.Id == deliveryNote.ProviderId);
                
                }

            });

            Save = new CustomCommand(() =>
            {
                if (AddDeliveryNote.Id == 0)
                    Task.Run(CreateNewDeliveryNote);
                else
                    Task.Run(EditDeliveryNote);


                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }

            });
        }
        public async Task CreateNewDeliveryNote()
        {
            AddDeliveryNote.ProviderId = selectedProvider.Id;
            await Api.PostAsync<DeliveryNoteApi>(AddDeliveryNote, "DeliveryNote");
        }

        public async Task EditDeliveryNote()
        {
            AddDeliveryNote.ProviderId = selectedProvider.Id;
            await Api.PutAsync<DeliveryNoteApi>(AddDeliveryNote, "DeliveryNote");
        }

        public async Task TakeListDeliveryNotes()
        {
            var result = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            deliveryNotes = new List<DeliveryNoteApi>(result);
            SignalChanged("deliveryNotes");

            var result1 = await Api.GetListAsync<ProviderApi[]>("Provider");
            providers = new List<ProviderApi>(result1);
            SignalChanged("providers");

            //foreach (var deliveryNote1 in deliveryNotes)
            //{
            //    deliveryNote1.Provider = providers.First(s => s.Id == deliveryNote1.ProviderId);
            //    SelectedProvider = deliveryNote1.Provider;
            //}
        }
            
        
        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
    }
}
