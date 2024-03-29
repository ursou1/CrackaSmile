﻿using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class EditDepartNoteViewModel: BaseViewModel
    {
        #region par
        private DepartNoteApi addDepartNote;
        public DepartNoteApi AddDepartNote
        {
            get => addDepartNote;
            set
            {
                addDepartNote = value;
                SignalChanged();
            }
        }
        public List<DepartNoteApi> departNotes { get; set; }
        public List<ClientApi> clients { get; set; }

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
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        #endregion

        public EditDepartNoteViewModel(DepartNoteApi departNote)
        {
            TakeListDepartNotes().ContinueWith(s=>
            {
                if (departNote == null)
                {
                    AddDepartNote = new DepartNoteApi();

                }
                else
                {
                    AddDepartNote = new DepartNoteApi
                    {
                        Id = departNote.Id,
                        Number = departNote.Number,
                        DepartDate = departNote.DepartDate,
                        ClientId = departNote.ClientId,
                    };

                    SelectedClient = clients.First(s => s.Id == departNote.ClientId);
                }
            });

            Cancel = new CustomCommand(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }
            });

            Save = new CustomCommand(() =>
            {
                if (AddDepartNote.Id == 0)
                {
                    try
                    {
                        if (AddDepartNote.Number.ToString() != null && AddDepartNote.DepartDate.ToString() != null && SelectedClient != null)
                        {
                           Task.Run(CreateNewDepartNote);
                        }
                        else
                        {
                            MessageBox.Show("Проверьте заполнение данных!");
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Проверьте заполнение данных!");
                    }
                    
                }
                else
                {
                    Task.Run(EditDepartNote);
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }

            });
        }

        public async Task CreateNewDepartNote()
        {
            AddDepartNote.ClientId = selectedClient.Id;
            await Api.PostAsync<DepartNoteApi>(AddDepartNote, "DepartNote");
        }

        public async Task EditDepartNote()
        {
            AddDepartNote.ClientId = selectedClient.Id;
            await Api.PutAsync<DepartNoteApi>(AddDepartNote, "DepartNote");
        }

        public async Task TakeListDepartNotes()
        {
            var result = await Api.GetListAsync<DepartNoteApi[]>("DepartNote");
            departNotes = new List<DepartNoteApi>(result);
            SignalChanged("departNotes");

            var result1 = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result1);
            SignalChanged("clients");

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
