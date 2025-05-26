using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data.DTO;
using Newtonsoft.Json;
using adesoft.adepos.webview.Util;
using System.Net.Mail;
using System.Net;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestCertificateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        string urlappaux;
        string EmailAccount, EmailPass, ServerSMTP, portSMTP;
        public RequestCertificateController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
            urlappaux = configuration.GetValue<string>("UrlBaseReports");
            EmailAccount = configuration.GetValue<string>("AccountSendEmail:EmailAccount");
            EmailPass = configuration.GetValue<string>("AccountSendEmail:EmailPass");
            ServerSMTP = configuration.GetValue<string>("AccountSendEmail:ServerSMTP");
            portSMTP = configuration.GetValue<string>("AccountSendEmail:portSMTP");
        }

        public RequestCertificate Create(RequestCertificate requestCertificate)
        {
            _dbcontext.RequestCertificates.Add(requestCertificate);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();

            return requestCertificate;
        }


        public RequestCertificate Update(RequestCertificate requestCertificate)
        {
            if (requestCertificate.TransOption == 2)//actualizar estado y enviar correo
            {
                if (requestCertificate.StateRequestCertificateId == 2)
                {
                    try
                    {
                        DTOTercero trans = new DTOTercero();
                        Tercero tercer = _dbcontext.Terceros.Where(x => x.TerceroId == requestCertificate.TerceroId).First();
                        if(tercer.CargoIdHomologate != 0)
                        {
                            trans.FirstName = tercer.FirstName;
                            trans.LastName = tercer.LastName;
                            trans.NumDocument = tercer.NumDocument;
                            trans.DateContractStart = tercer.DateContractStart;
                            trans.DateContractEnd = tercer.DateContractEnd;
                            trans.DateRetirement = tercer.DateRetirement;
                            trans.IsActive = tercer.IsActive;
                            trans.Salary = tercer.Salary;
                            LocationGeneric locationcargo = _dbcontext.LocationGenerics.Where(x => x.LocationGenericId == tercer.CargoIdHomologate).First();
                            trans.CargoName = string.IsNullOrEmpty(locationcargo.LongDescription) ? locationcargo.Description : locationcargo.LongDescription;
                            string jsonobj = JsonConvert.SerializeObject(trans);
                            Task<string> res = HttpAPIClient.PostSendRequestConfigureAwait(jsonobj, urlappaux, "api/Certificate/GenerateCertificate", false);
                            res.Wait();
                            trans = JsonConvert.DeserializeObject<DTOTercero>(res.Result);
                            if (!string.IsNullOrEmpty(trans.FirstName))
                            {
                                requestCertificate.PathDocumentoAdjunto = trans.FirstName;
                                _dbcontext.Entry<RequestCertificate>(requestCertificate).State = EntityState.Modified;
                                _dbcontext.SaveChanges();
                                _dbcontext.DetachAll();
                                requestCertificate.UrlPathDocumentoAdjunto = urlappaux + requestCertificate.PathDocumentoAdjunto;
                                requestCertificate.TransactionIsOk = true;
                                requestCertificate.MessageResponse = "Se genero correctamente el certificado.";
                            }
                            else
                            {
                                requestCertificate.TransactionIsOk = false;
                                requestCertificate.MessageResponse = "Problemas al generar el certificado.";
                            }
                        }    
                        else
                        {
                            requestCertificate.TransactionIsOk = false;
                            requestCertificate.MessageResponse = "El empleado no tiene un cargo homologado configurado.";
                        }
                    }
                    catch (Exception ex)
                    {
                        requestCertificate.TransactionIsOk = false;
                        requestCertificate.MessageResponse = "Problemas al generar el certificado.";
                    }
                }
                else
                {
                    _dbcontext.Entry<RequestCertificate>(requestCertificate).State = EntityState.Modified;
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                    requestCertificate.TransactionIsOk = true;
                    requestCertificate.MessageResponse = "Se actualizo el estado correctamente.";
                }
            }
            else
            {
                _dbcontext.Entry<RequestCertificate>(requestCertificate).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                requestCertificate.TransactionIsOk = true;
                requestCertificate.MessageResponse = "Se actualizo el estado correctamente.";
            }
            return requestCertificate;
        }

        public RequestCertificate SelectById(RequestCertificate requestCertificate)
        {
            if (requestCertificate.TransOption == 0 || requestCertificate.TransOption == 1)
            {
                RequestCertificate find = _dbcontext.RequestCertificates.Where(x => x.RequestCertificateId == requestCertificate.RequestCertificateId).FirstOrDefault();
                return find;
            }
            else if (requestCertificate.TransOption == 2)
            {//certificados en tramite
                RequestCertificate find = _dbcontext.RequestCertificates.Where(x => x.TerceroId == requestCertificate.TerceroId
                && x.TypeCertificate == requestCertificate.TypeCertificate && x.StateRequestCertificateId == 1).FirstOrDefault();
                return find;
            }
            else
            {
                RequestCertificate find = _dbcontext.RequestCertificates.Where(x => x.RequestCertificateId == requestCertificate.RequestCertificateId).FirstOrDefault();
                return find;
            }
        }

        public List<RequestCertificate> selectAll(RequestCertificate RequestCertificate)
        {
            if (RequestCertificate.TransOption == 2)
            {
                List<RequestCertificate> listreturn = _dbcontext.RequestCertificates.Where(x => x.StateRequestCertificateId == RequestCertificate.StateRequestCertificateId).ToList();
                foreach (RequestCertificate nom in listreturn)
                {
                    if (!string.IsNullOrEmpty(nom.PathDocumentoAdjunto))
                        nom.UrlPathDocumentoAdjunto = urlappaux + nom.PathDocumentoAdjunto;
                    nom.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == nom.TerceroId).First();
                }
                return listreturn;
            }
            else
            {
                List<RequestCertificate> listreturn = _dbcontext.RequestCertificates.ToList();
                foreach (RequestCertificate nom in listreturn)
                {
                    if (!string.IsNullOrEmpty(nom.PathDocumentoAdjunto))
                        nom.UrlPathDocumentoAdjunto = urlappaux + nom.PathDocumentoAdjunto;
                    nom.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == nom.TerceroId).First();
                }
                return listreturn;
            }
        }

        public RequestCertificate SendEmailToWorker(RequestCertificate request)
        {
            try
            {
                if (!string.IsNullOrEmpty(ServerSMTP))
                {
                    if (!string.IsNullOrEmpty(request.Tercero.Email))
                    {
                        //SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                        SmtpClient client = new SmtpClient(ServerSMTP, int.Parse(portSMTP));
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true;
                        //client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(EmailAccount, EmailPass);
                        //client.Credentials = new NetworkCredential("glhl86@gmail.com", "ninguna");
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(EmailAccount);
                        //mailMessage.From = new MailAddress("glhl86@gmail.com");
                        //string[] arrayemails = EmailTo.Split(',');
                        //foreach (string e in arrayemails)
                        //{
                        //    mailMessage.To.Add(e);//email destino
                        //}
                        mailMessage.To.Add(request.Tercero.Email);//email destino
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = "El certificado que solicito ya esta diponible para descargarlo en el siguiente enlace : "
                            + "<a href=\"" + request.UrlPathDocumentoAdjunto + "\">" + request.UrlPathDocumentoAdjunto + "</a>";
                        //mailMessage.Attachments.Add(new Attachment() {  })
                        mailMessage.Subject = "UNISPAN - " + request.TypeCertificate;
                        client.Send(mailMessage);
                        request.TransactionIsOk = true;
                        request.MessageResponse = "Se envio el correo correctamente.";
                    }
                    else
                    {
                        request.TransactionIsOk = false;
                        request.MessageResponse = "El empleado no tiene un correo asignado.";
                    }

                }
                else
                {
                    request.TransactionIsOk = false;
                    request.MessageResponse = "No se ha configurado el servidor SMTP.";
                }
            }
            catch (Exception ex)
            {
                request.TransactionIsOk = false;
                request.MessageResponse = "Error al enviar el correo: " + ex.ToString();
            }
            return request;
        }


    }
}