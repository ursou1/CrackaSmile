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
    /// Логика взаимодействия для EditDeliveryNoteWin.xaml
    /// </summary>
    public partial class EditDeliveryNoteWin : Window
    {
        public EditDeliveryNoteWin()
        {
            InitializeComponent();
            DataContext = new EditDeliveryNoteViewModel(null);
        }
        public EditDeliveryNoteWin(DeliveryNoteApi deliveryNote)
        {
            InitializeComponent();
            DataContext = new EditDeliveryNoteViewModel(deliveryNote);
        }
    }
}
