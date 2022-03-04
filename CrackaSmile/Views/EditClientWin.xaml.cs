using CrackaSmile.ViewModels;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CrackaSmile.Views
{
    /// <summary>
    /// Логика взаимодействия для EditClientWin.xaml
    /// </summary>
    public partial class EditClientWin : Window
    {
        public EditClientWin()
        {
            InitializeComponent();
            DataContext = new EditClientViewModel(null);
        }
        public EditClientWin(ClientApi client)
        {
            InitializeComponent();
            DataContext = new EditClientViewModel(client);
        }
    }
}
