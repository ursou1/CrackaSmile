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
    /// Логика взаимодействия для EditProviderWin.xaml
    /// </summary>
    public partial class EditProviderWin : Window
    {
        public EditProviderWin()
        {
            InitializeComponent();
            DataContext = new EditProviderViewModel(null);
        }
        public EditProviderWin(ProviderApi provider)
        {
            InitializeComponent();
            DataContext = new EditProviderViewModel(provider);
        }
    }
}
