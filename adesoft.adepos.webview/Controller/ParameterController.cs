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
using System.IO;
using Newtonsoft.Json;
using adesoft.adepos.webview.Util;
using adesoft.adepos.webview.Data.DTO;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParameterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        string urlappaux;
        public ParameterController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            urlappaux = configuration.GetValue<string>("UrlBaseReports");
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public Parameter Create(Parameter parameter)
        {
            Parameter find = _dbcontext.Parameters.Where(x => x.NameIdentify == parameter.NameIdentify).FirstOrDefault();
            if (find != null)
            {
                try
                {
                    if (parameter.ValueType == "Archivo")
                    {
                        CreateOrUpdateFile(parameter);
                        parameter.AuxValue = Convert.ToBase64String(parameter.FileBuffer);
                        //  string arrays = JsonConvert.SerializeObject(parameter);
                        if (parameter.NameIdentify == "SaldosMensuales")
                        {
                            string jsonparam = "";
                            string pathFile = parameter.Fullpath;
                            Task<string> res = HttpAPIClient.PostSendRequestConfigureAwait(jsonparam, urlappaux, "api/Certificate/ReadReportBalanceMonth?pathFile=" + pathFile, true);
                            res.Wait();

                            List<DetailReportDynamic> trans = JsonConvert.DeserializeObject<List<DetailReportDynamic>>(res.Result);
                            List<DetailReportDynamic> transdeta = _dbcontext.DetailReportDynamics.Where(x => x.ReportDynamicId == 1).ToList();
                            _dbcontext.DetailReportDynamics.RemoveRange(transdeta);
                            //trans.ForEach(x => _dbcontext.DetailReportDynamics.Add(x));
                            _dbcontext.DetailReportDynamics.AddRange(trans);
                            _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                        }
                        _dbcontext.Entry<Parameter>(parameter).State = EntityState.Modified;
                        _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                        // transaction.AuxTest = null;
                    }
                    else if (parameter.ValueType == "Ventana")
                    {
                        _dbcontext.Entry<Parameter>(parameter).State = EntityState.Modified;
                        _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    }
                    else if (parameter.ValueType == "Numerico")
                    {
                        _dbcontext.Entry<Parameter>(parameter).State = EntityState.Modified;
                        _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            return parameter;
        }


        public void CreateOrUpdateFile(Parameter parameter)
        {
            if (parameter.FileEntry != null)
            {
                string pathapp = Directory.GetCurrentDirectory();
                string directory = "/wwwroot/FilesApp/DocParameters/" + parameter.ParameterId;

                // Ajustado por rocampo 10/12/2021 - la validacion debe tener en cuenta si parameter.value esta null o empty
                //if (parameter.Value != null)
                if (!string.IsNullOrEmpty(parameter.Value))
                {
                    System.IO.File.Delete(pathapp + parameter.Value);
                }
                parameter.Value = directory + "/" + parameter.NameFile;
                if (!Directory.Exists(pathapp + directory))
                {
                    Directory.CreateDirectory(pathapp + directory);
                }
                //var reader = new System.IO.StreamReader(nominanov.FileEntry.Data);
                //byte[] arraybytes;
                //Task<string> task = reader.ReadToEndAsync();
                //task.Wait();
                //string t = task.Result;
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    Task task = reader.BaseStream.CopyToAsync(ms);
                //    task.wa();
                //    arraybytes = reader..ToArray();
                //}
                parameter.Fullpath = pathapp + parameter.Value;
                FileStream filestream = System.IO.File.Create(pathapp + parameter.Value);
                filestream.Write(parameter.FileBuffer, 0, parameter.FileBuffer.Count());
                filestream.Close();
            }
        }

        public Parameter Update(Parameter parameter)
        {
            Parameter find = _dbcontext.Parameters.Where(x => x.ParameterId == parameter.ParameterId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<Parameter>(parameter).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return parameter;
        }

        public Parameter SelectById(Parameter parameter)
        {
            if (parameter.TransOption == 1 && parameter.TransOption == 0)
            {
                Parameter find = _dbcontext.Parameters.Where(x => x.ParameterId == parameter.ParameterId).FirstOrDefault();
                return find;
            }
            else if (parameter.TransOption == 2)
            {
                Parameter find = _dbcontext.Parameters.Where(x => x.Module == parameter.Module).FirstOrDefault();
                return find;
            }
            else if (parameter.TransOption == 3)//nombre NameIdentify
            {
                Parameter find = _dbcontext.Parameters.Where(x => x.NameIdentify == parameter.NameIdentify).FirstOrDefault();
                return find;
            }
            return null;
        }
        public List<Parameter> selectAll(Parameter parameter)
        {
            if (parameter.TransOption == 1)
            {
                return _dbcontext.Parameters.ToList();
            }
            else if (parameter.TransOption == 2)
            {
                return _dbcontext.Parameters.Where(x => x.Module == parameter.Module).ToList();
            }
            else
            {
                return _dbcontext.Parameters.ToList();
            }
        }

        public Parameter SelectByIdentify(string nameIdentify)
        {
            return _dbcontext.Parameters.Where(x => x.NameIdentify == nameIdentify).FirstOrDefault();            
        }

        [HttpGet("SelectMedidas")]
        public List<DTOParamMedidas> SelectMedidas()
        {
            Parameter find = _dbcontext.Parameters.Where(x => x.NameIdentify == "ParametrosMedida").FirstOrDefault();
            List<DTOParamMedidas> listmedidas = JsonConvert.DeserializeObject<List<DTOParamMedidas>>(find.Value2);
            return listmedidas;
        }
    }
}