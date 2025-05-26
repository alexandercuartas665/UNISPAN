namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOOrderProductLog
    {
        public string OrderProductLogId { get; set; }

        public long OrderId { get; set; }

        public long ItemId { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPending { get; set; }

        public string Reference { get; set; }

        public string? ZoneProductId { get; set; }

        public DTOZoneProduct? ZoneProduct { get; set; }
    }
}
