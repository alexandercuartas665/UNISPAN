namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOPlantFilter
    {
        public string FilterId { get; set; }

        public string ZoneProductId { get; set; }

        public DTOOrder OrderSelected { get; set; }

        public DTOOrderPallet OrderPalletSelected { get; set; }
    }
}
