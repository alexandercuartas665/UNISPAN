using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string OrderNum { get; set; }

        public OrderType OrderType { get; set; }

        public string Works { get; set; }

        public string CustomerName { get; set; }

        public string OPNum { get; set; }

        public string AccordingNo { get; set; }

        public string SalesPerson { get; set; }

        public string Module { get; set; }

        public string City { get; set; }

        public string ReponsableTrans { get; set; }

        public DateTime DispatchDateTime { get; set; }

        public DateTime ReturnDateTime { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal Wight { get; set; }

        public string VehicleType { get; set; }

        /*Private*/

        public string VendorName { get; set; }

        public string PlateNum { get; set; }

        public string DriverName { get; set; }

        public decimal InvoiceAmount { get; set; }

        public string InvoiceNum { get; set; }

        public DateTime InvoiceDate { get; set; }        

        #region Sync

        public bool Sync { get; set; }

        public DateTime SyncDateTime { get; set; }

        public string ErrorMessage { get; set; }

        #endregion

        public int SalesPersonId { get; set; }

        public string CustomerAccount { get; set; }

        public int ModuleId { get; set; }

        public int CityId { get; set; }

        public int ReponsableTransId { get; set; }

        public int VehicleTypeId { get; set; }

        public string VendorAccount { get; set; }

        public long DispatchId { get; set; }

        public bool Ok { get; set; }

        public OrderStatus Status { get; set; }

        public Decimal Progress { get; set; }

        public long TransactionGenericId { get; set; }

        public bool IsConform { get; set; }

        public string NoConform { get; set; }

        public string FVTransport { get; set; }

        public DateTime Period { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public int Version { get; set; }

        public int PalletNo { get; set; }
        public string Email { get; set; }

        public virtual ICollection<OrderNotification> Notifications { get; set; }
    }

    public enum OrderStatus
    {
        None,        
        Packaged,
        Dispatched,
        Finished,
        Paused,
        Draft, // 5 cuando se programa una order 1
        Postponed
    }

    public enum OrderType
    {
        None,
        Dispatch,
        Return
    }
}
