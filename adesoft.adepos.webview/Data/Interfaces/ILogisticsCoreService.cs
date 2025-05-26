using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Model.PL;
using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface ILogisticsCoreService
    {
        public void AddOrderReportFilter(DTOOrderReportFilter filter);

        public DTOOrderReportFilter GetReportFilter(string reportFilterId);

        public bool DeletePictures(DTOOrder dtoOrder);

        public bool DeletePicture(DTOOrder dtoOrder, DTOOrderPicture dtoOrderPicture);

        public Task UploadPicturesAsync(DTOOrder dtoOrder, IFileListEntry[] files);

        public List<DTOOrderPicture> GetPictures(OrderType orderType, long orderId, int page, int pageSize);

        public void CreateComment(DTOOrder dtoOrder, string comment);

        public DTOOrder ChangeOrder(DTOOrder dtoOrdenOrig, DTOOrder dtoOrderNew);

        public List<DTOOrder> GetOrdersForChange(DateTime fromDate, DateTime toDate, DTOOrder dtoOrderDiff);

        public DTOOrder GetOrderForChange(long orderId);

    }
}
