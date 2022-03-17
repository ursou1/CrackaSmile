using CrackaSmile.ViewModels;
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
    /// Логика взаимодействия для DepartNotePage.xaml
    /// </summary>
    public partial class DepartNotePage : Window
    {
        public DepartNotePage()
        {
            InitializeComponent();
            DataContext = new DepartNoteViewModel();
        }
    }
}
