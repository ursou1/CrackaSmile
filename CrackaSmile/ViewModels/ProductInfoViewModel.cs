using CrackaSmile.Tools;
using ModelsApi;
using QRCoder;
using QRCoder.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrackaSmile.ViewModels
{
    public class ProductInfoViewModel : BaseViewModel
    {

        #region properties
        private ProductApi addProduct;
        public ProductApi AddProduct
        {
            get => addProduct;
            set
            {
                addProduct = value;
                SignalChanged();
            }
        }

        public ProductTypeApi selectedProductType;
        public ProductTypeApi SelectedProductType
        {
            get => selectedProductType;
            set
            {
                selectedProductType = value;
                SignalChanged();
            }
        }

        public UnitApi selectedUnit;
        public UnitApi SelectedUnit
        {
            get => selectedUnit;
            set
            {
                selectedUnit = value;
                SignalChanged();
            }
        }
        public List<DepartNoteApi> departNotes { get; set; }
        public List<DepartNoteApi> DepartNotes
        {
            get => departNotes;
            set
            {
                departNotes = value;
                SignalChanged();
            }
        }


        public List<DeliveryNoteApi> deliveryNotes { get; set; }
        public List<DeliveryNoteApi> DeliveryNotes
        {
            get => deliveryNotes;
            set
            {
                deliveryNotes = value;
                SignalChanged();
            }
        }

        //private BitmapImage imageProduct;
        //public BitmapImage ImageProduct
        //{
        //    get => imageProduct;
        //    set
        //    {
        //        imageProduct = value;
        //        SignalChanged();
        //    }
        //}
        public DrawingImage qR;
        public DrawingImage QR
        {
            get => qR;
            set
            {
                qR = value;
                SignalChanged();
            }
        }

        public DepartNoteApi selectedDepartNote;
        public DepartNoteApi SelectedDepartNote
        {
            get => selectedDepartNote;
            set
            {
                selectedDepartNote = value;
                SignalChanged();
            }
        }

        public DeliveryNoteApi selectedDeliveryNote;
        public DeliveryNoteApi SelectedDeliveryNote
        {
            get => selectedDeliveryNote;
            set
            {
                selectedDeliveryNote = value;
                SignalChanged();
            }
        }


        public List<ProductApi> products { get; set; }
        public List<ProductTypeApi> productTypes { get; set; }
        public List<UnitApi> units { get; set; }
        #endregion
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        #region commands
        public CustomCommand Cancel { get; set; }
        #endregion

        public ProductInfoViewModel(ProductApi product)
        {
            
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                AddProduct = new ProductApi
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Image = product.Image,
                    Count = product.Count,
                    Price = product.Price,
                    Description = product.Description,
                    UnitId = product.UnitId,
                    ProductTypeId = product.ProductTypeId,
                };
                SelectedProductType = productTypes.First(s => s.Id == product.ProductTypeId);
                SelectedUnit = units.First(s => s.Id == product.UnitId);
                if (product.DeliveryNoteId != null)
                {
                    SelectedDeliveryNote = DeliveryNotes.First(s => s.Id == product.DeliveryNoteId);
                }
                if (product.DepartNoteId != null)
                {
                    SelectedDepartNote = DepartNotes.First(s => s.Id == product.DepartNoteId);
                }

            });
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("Код:" +  product.Code + "\n" + "Наименование:"  + product.Name, QRCodeGenerator.ECCLevel.Q);
            XamlQRCode qrCode = new XamlQRCode(qrCodeData);
            QR = qrCode.GetGraphic(20);
        }

        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            products = new List<ProductApi>(result);
            SignalChanged("products");

            var result1 = await Api.GetListAsync<ProductTypeApi[]>("ProductType");
            productTypes = new List<ProductTypeApi>(result1);
            SignalChanged("productTypes");

            var result2 = await Api.GetListAsync<UnitApi[]>("Unit");
            units = new List<UnitApi>(result2);
            SignalChanged("units");

            var result3 = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            DeliveryNotes = new List<DeliveryNoteApi>(result3);

            var result4 = await Api.GetListAsync<DepartNoteApi[]>("DepartNote");
            DepartNotes = new List<DepartNoteApi>(result4);
            
            foreach (var product in products)
            {
                
            }
        }
    }
}
