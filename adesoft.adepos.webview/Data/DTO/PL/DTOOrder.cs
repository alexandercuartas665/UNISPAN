using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOOrder
    {
        public DTOOrder()
        {
            Pictures = new List<DTOOrderPicture>();
            Comments = new List<DTOOrderComment>();
            Products = new List<DTOOrderProduct>();
            PickingStatusDetails = new List<DTOPickingStatusDetail>();

            this.DispatchDateTime = DateTime.MinValue;
            this.ReturnDateTime = DateTime.MinValue;

            this.OrderNum = string.Empty;
            this.CustomerAccount = string.Empty;
            this.Works = string.Empty;
        }
        
        public bool IsSelected { get; set; } //OPCION DE SELECCION

        public long OrderId { get; set; }

        public string OrderNum { get; set; }

        public string Email { get; set; }

        public OrderType OrderType { get; set; }

        public string Works { get; set; }

        public string CustomerName { get; set; }

        public string OPNum { get; set; }

        public string AccordingNo { get; set; }

        public string SalesPerson { get; set; }

        public string Module { get; set; }

        public string City { get; set; }

        public string ReponsableTrans { get; set; }

        public DateTime? DispatchDateTime { get; set; }

        public DateTime? ReturnDateTime { get; set; }

        public decimal Wight { get; set; }

        public decimal TotalPalletWeight { get; set; }

        public string VehicleType { get; set; }

        /*Private*/

        public string VendorName { get; set; }

        public string PlateNum { get; set; }

        public string DriverName { get; set; }

        public decimal InvoiceAmount { get; set; }

        public string InvoiceNum { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string Comment { get; set; }

        #region Sync

        public bool Sync { get; set; }

        public DateTime SyncDateTime { get; set; }

        public string ErrorMessage { get; set; }

        #endregion

        public ICollection<DTOOrderPicture> Pictures { get; set; }

        public ICollection<DTOOrderComment> Comments { get; set; }

        public int SalesPersonId { get; set; }        

        public string CustomerAccount { get; set; }

        public string VendorAccount { get; set; }

        public int ModuleId { get; set; }

        public int CityId { get; set; }

        public int ReponsableTransId { get; set; }

        public int VehicleTypeId { get; set; }

        public long DispatchId { get; set; }

        public long DispatchIdSelect { get; set; }

        public bool DispatchParent { get; set; }

        public DateTime TransDate { get; set; }

        public string TransDateFilter { get; set; }

        public bool Ok { get; set; }

        public int CounterPictures { get; set; }

        public double Progress { get; set; }

        public OrderStatus Status { get; set; }

        public string IsConform { get; set; }

        public string NoConform { get; set; }

        public string FVTransport { get; set; }

        public DateTime? Period { get; set; }        

        public ICollection<DTOOrderPallet> OrderPallets { get; set; }

        public ICollection<DTOOrderProduct> Products { get; set; }

        public ICollection<DTOOrderProductVersion> ProductVersions { get; set; }

        public int OrderState { get; set; }

        public long TransactionGenericId { get; set; }

        public ICollection<DTOZoneProduct> ZoneProducts { get; set; }

        public int PalletNo { get; set; }

        public DateTime ModifiedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public int Version { get; set; }

        public bool VersionChange { get; set; }

        public ICollection<DTOPickingStatusDetail> PickingStatusDetails { get; set; }

        [NotMapped]

        public byte[] AuxTest { get; set; }

        [NotMapped]

        public string NombreArchivo { get; set; }
    }    

    public class DTOSharedOrder
    {
        public long OrderId { get; set; }

        public string OrderNum { get; set; }
    }
}
