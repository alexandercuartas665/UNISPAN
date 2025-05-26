using adesoft.adepos.webview.Data.DTO.PL;
using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface IFileUpload
    {
        Task UploadFile(IFileListEntry file, DTOOrder dtoOrder);
    }
}
