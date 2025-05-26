using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Util;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class OportunidadesCRMBussines
    {

        public AdeposDBContext context { get; set; }
        DTOParamCRM param { get; set; }
        public OportunidadesCRMBussines(AdeposDBContext context)
        {
            this.context = context;
        }

        public async Task<OportunidadesCRM> Sync()
        {
            try
            {
                //{"IsEnable":1,"Horas":"23:00","CRMUrlIntegration":"https://desarrollo.maxgp.com.co/phrame.php?action=tarea_programada&metodo=clienteTarea&clase=webServices&metodo_ejec=getOportunidades&empresa=unispan" , "Authorization": "Hmac Y3JtLnVuaXNwYW46VU4xU1A0Tg" , "Cookie": "PHPSESSID=5i8b6hdn35pphfi4ubghhssa66"}
                string parameter = context.Parameters.Where(x => x.NameIdentify == "ParamCRMIntegration").First().Value2;
                param = JsonConvert.DeserializeObject<DTOParamCRM>(parameter);
                param.ReadTimes();
                context.DetachAll();

                string Response = await HttpAPIClient.PostSendRequestCRM("", param, "");
                List<OportunidadesCRM> oportunidadesCRM = JsonConvert.DeserializeObject<List<OportunidadesCRM>>(Response);
                OportunidadesCRM response = new OportunidadesCRM();
                List<OportunidadesCRM> OportuBD = context.OportunidadesCRM.ToList();
                foreach (OportunidadesCRM sync in oportunidadesCRM.AsParallel())
                {
                    OportunidadesCRM oport = OportuBD.Where(x => x.OPRT_NUMERO == sync.OPRT_NUMERO).FirstOrDefault();
                    if (oport != null
                        && (oport.FECHA_APERTURA != sync.FECHA_APERTURA
                        || oport.CONSECUTIVO != sync.CONSECUTIVO
                        || oport.TIPO_NEGOCIO != sync.TIPO_NEGOCIO
                         || oport.TIPO_OPRT != sync.TIPO_OPRT
                          || oport.NIT != sync.NIT || oport.CLIENTE != sync.CLIENTE || oport.OBRA != sync.OBRA
                          || oport.COMERCIAL != sync.COMERCIAL || oport.VR_VENTA != sync.VR_VENTA || oport.VR_RENTA_MENSUAL != sync.VR_RENTA_MENSUAL
                           || oport.DURACION != sync.DURACION
                           || oport.COD_ETAPA != sync.COD_ETAPA
                           || oport.ETAPA != sync.ETAPA))
                    {
                        try
                        {
                            // || oport.APROBACION != sync.APROBACION  --- se puede editar lo mismo la TONELADA
                            oport.FECHA_APERTURA = sync.FECHA_APERTURA;
                            oport.CONSECUTIVO = sync.CONSECUTIVO;
                            oport.FECHA_ULT_MOD = sync.FECHA_ULT_MOD;
                            oport.FECHA_APERTURA_ = DateTime.ParseExact(sync.FECHA_APERTURA, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("Es-co"));
                            oport.FECHA_ULT_MOD_ = DateTime.ParseExact(sync.FECHA_ULT_MOD, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("Es-co"));
                            oport.TIPO_NEGOCIO = sync.TIPO_NEGOCIO; oport.TIPO_OPRT = sync.TIPO_OPRT;
                            oport.NIT = sync.NIT; oport.CLIENTE = sync.CLIENTE; oport.OBRA = sync.OBRA;
                            oport.COMERCIAL = sync.COMERCIAL;
                            oport.VR_VENTA = sync.VR_VENTA; oport.VR_RENTA_MENSUAL = sync.VR_RENTA_MENSUAL;
                            oport.DURACION = sync.DURACION;
                            oport.COD_ETAPA = (sync.COD_ETAPA != null) ? sync.COD_ETAPA.Trim() : "";
                            oport.ETAPA = (sync.ETAPA != null) ? sync.ETAPA.Trim() : "";
                            context.Entry<OportunidadesCRM>(oport).State = EntityState.Modified;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else if (oport == null)
                    {
                        try
                        {
                            sync.FECHA_ULT_MOD_ = DateTime.ParseExact(sync.FECHA_ULT_MOD, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("Es-co"));
                            sync.FECHA_APERTURA_ = DateTime.ParseExact(sync.FECHA_APERTURA, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("Es-co"));
                            context.OportunidadesCRM.Add(sync);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {

                    }
                }
                context.SaveChanges();
                context.DetachAll();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public OportunidadesCRM Update(OportunidadesCRM oportun)
        {
            OportunidadesCRM find = context.OportunidadesCRM.Where(x => x.OportunidadID == oportun.OportunidadID).FirstOrDefault();
            if (find != null)
            {
                context.Entry<OportunidadesCRM>(oportun).State = EntityState.Modified;
                context.SaveChanges();
                context.DetachAll();
            }
            else
            {

            }
            return oportun;
        }


    }
}
