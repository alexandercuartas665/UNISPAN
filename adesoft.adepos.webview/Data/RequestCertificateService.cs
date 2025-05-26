using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class RequestCertificateService
    {
        private readonly IConfiguration _configuration;
        private readonly RequestCertificateController _requestCertificateController;

        public RequestCertificateService(IConfiguration configuration, RequestCertificateController requestCertificateController)
        {
            _configuration = configuration;
            _requestCertificateController = requestCertificateController;
        }

        public async Task<List<RequestCertificate>> selectAll(RequestCertificate requestCertificate)
        {
            return await Task.FromResult(_requestCertificateController.selectAll(requestCertificate));
        }
        public async Task<RequestCertificate> SendEmailToWorker(RequestCertificate model)
        {
            return await Task.FromResult(_requestCertificateController.SendEmailToWorker(model));
        }
      
        public async Task<RequestCertificate> Create(RequestCertificate model)
        {
            return await Task.FromResult(_requestCertificateController.Create(model));
        }

        public async Task<RequestCertificate> Update(RequestCertificate model)
        {
            return await Task.FromResult(_requestCertificateController.Update(model));
        }
        public async Task<RequestCertificate> SelectById(RequestCertificate model)
        {
            return await Task.FromResult(_requestCertificateController.SelectById(model));
        }
    }
}
