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
    /// Логика взаимодействия для EditProductInDepartWin.xaml
    /// </summary>
    public partial class EditProductInDepartWin : Window
    {
        public EditProductInDepartWin()
        {
            InitializeComponent();
            DataContext = new EditProductInDepartVM(null);
        }
        public EditProductInDepartWin(DepartNoteApi departNote)
        {
            InitializeComponent();
            DataContext = new EditProductInDepartVM(departNote);
        }
    }
}
