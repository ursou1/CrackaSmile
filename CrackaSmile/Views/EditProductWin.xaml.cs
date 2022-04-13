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
    /// Логика взаимодействия для EditProductWin.xaml
    /// </summary>
    public partial class EditProductWin : Window
    {
        public EditProductWin()
        {
            InitializeComponent();
            DataContext = new EditProductViewModel(null);
        }
        public EditProductWin(ProductApi product)
        {
            InitializeComponent();
            DataContext = new EditProductViewModel(product);
        }
    }
}
