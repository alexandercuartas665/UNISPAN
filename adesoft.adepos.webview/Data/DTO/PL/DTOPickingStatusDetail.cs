namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOPickingStatusDetail
    {
        public string OrderProductId { get; set; }

        public long OrderId { get; set; }

        public long ItemId { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }

        public decimal Qty { get; set; }

        public decimal LastQty { get; set; }

        public decimal PickQty { get; set; }

        public string Reference { get; set; }

        public string ZoneProduct { get; set; }

        public int Version { get; set; }

        public string VersionCode { get; set; }
    }
}
