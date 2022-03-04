using CrackaSmile.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CrackaSmile.Views;

namespace CrackaSmile.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        #region something


        #endregion

        private object _currentPage;
        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                SignalChanged("CurrentPage");
            }
        }

        public Page1 Page1 { get; set; }

        #region commands
        public CustomCommand ProductPage { get; set; }
        public CustomCommand ClientPage { get; set; }
        #endregion

        public MainViewModel()
        {

            CurrentPage = new Page1();

            ProductPage = new CustomCommand(() =>
            {
                CurrentPage = new ProductListPage();
                SignalChanged("CurrentPage");
            });

            ClientPage = new CustomCommand(() =>
            {
                CurrentPage = new ClientListPage();
                SignalChanged("CurrentPage");
            });
        }
    }
}
