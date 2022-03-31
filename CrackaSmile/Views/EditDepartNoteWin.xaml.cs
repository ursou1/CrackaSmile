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
    /// Логика взаимодействия для EditDepartNoteWin.xaml
    /// </summary>
    public partial class EditDepartNoteWin : Window
    {
        public EditDepartNoteWin()
        {
            InitializeComponent();
            DataContext = new EditDepartNoteViewModel(null);
        }
        public EditDepartNoteWin(DepartNoteApi departNote)
        {
            InitializeComponent();
            DataContext = new EditDepartNoteViewModel(departNote);
        }
    }
}
