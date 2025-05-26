using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using adesoft.adepos.Extensions;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace adesoft.adepos.webview.Controller
{
    public class CompanyController
    {
        private readonly IConfiguration _configuration;
        private AdeposDBContext _dbcontext;
        ConnectionDB connectionDB;
        private readonly IHttpContextAccessor httpContextAccessor;
        public CompanyController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            _dbcontext = new AdeposDBContext(connectionDB.Connection);

        }


        public Company Create(Company company)
        {
            Company find = _dbcontext.Companys.Where(x => x.Name == company.Name || x.Nit == company.Nit).FirstOrDefault();
            if (find == null)
            {
                company.MenuId = 1;
                _dbcontext.Companys.Add(company);


                _dbcontext.SaveChanges();
                RoleApp role = new RoleApp();
                role.Name = "Administrador";
                role.CompanyId = company.CompanyId;
                _dbcontext.RoleApps.Add(role);
                _dbcontext.SaveChanges();

                UserApp userap = new UserApp();
                userap.PassworNotCry = company.Password;
                var passwordHasher = new PasswordHasher<string>();
                userap.Password = passwordHasher.HashPassword(null, company.Password);
                userap.Username = company.Usuario;
                userap.RoleAppId = role.RoleAppId;//cambiar
                userap.CompanyId = company.CompanyId;
                _dbcontext.UserApps.Add(userap);
                _dbcontext.SaveChanges();

                List<ActionApp> listac = _dbcontext.ActionApps.Where(x => x.MenuId == company.MenuId && x.IsActive).ToList();
                List<Permission> newperm = new List<Permission>();
                foreach (ActionApp a in listac)
                {
                    Permission perm = new Permission();
                    perm.ActionAppId = a.ActionAppId;
                    perm.RoleAppId = role.RoleAppId;
                    newperm.Add(perm);
                }

                _dbcontext.Permissions.AddRange(newperm);
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();

                string msg = string.Empty;
                if (!string.IsNullOrEmpty(company.Email))
                {
                    msg = "Se ha enviado un correo de confirmacion a la direccion de correo.";
                    try
                    {
                        Task.Run(() =>
                        {
                            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

                            client.UseDefaultCredentials = false;
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential("credimax7@gmail.com", "credimax5326");

                            MailMessage mailMessage = new MailMessage();
                            mailMessage.From = new MailAddress("credimax7@gmail.com");
                            mailMessage.To.Add(company.Email);
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Body = "<p>Le damos la bienvenida a credimax , el registro de su compania " + company.Name + ". Fue exitoso.</p>"
                                + "<p>Entra a el siguiente link para acceder a nuestros servicios: </p> <a href=\"http://18.223.15.205:8010/\">CrediMax.com<a>" +
                                "<p>Usuario: " + userap.Username + " Password: " + userap.PassworNotCry;
                            mailMessage.Subject = "Registro exitoso en credimax";
                            client.Send(mailMessage);
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                }
                company.TransactionIsOk = true;
                company.MessageResponse = "Sede registrada correctamente. " + msg;
            }
            else
            {
                company.TransactionIsOk = false;
                company.MessageResponse = "La compania que intenta registrar ya existe. ";
            }
            return company;
        }


        public Company Update(Company company)
        {
            Company find = _dbcontext.Companys.Where(x => x.CompanyId == company.CompanyId).FirstOrDefault();
            if (find != null)
            {
                if (company.TransOption == 2)
                {
                    // find.Name = company.Name;
                    _dbcontext.Entry<Company>(company).State = EntityState.Modified;
                    UserApp userapp = _dbcontext.UserApps.Where(x => x.UserAppId == company.userApp.UserAppId).FirstOrDefault();
                    var passwordHasher = new PasswordHasher<string>();
                    userapp.Password = passwordHasher.HashPassword(null, company.Password);
                    userapp.PassworNotCry = company.Password;
                    _dbcontext.Entry<UserApp>(userapp).State = EntityState.Modified;
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    company.TransactionIsOk = true;
                    company.MessageResponse = "Sede actualizada correctamente. ";
                }
                else
                {
                    _dbcontext.Entry<Company>(company).State = EntityState.Modified;
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    company.TransactionIsOk = true;
                    company.MessageResponse = "Sede actualizada correctamente. ";
                }
       
            }
            else
            {

            }
            return company;
        }

        public Company SelectById(Company company)
        {
            Company find;
            if (company.TransOption == 0 || company.TransOption == 1)
            {
                find = _dbcontext.Companys.Where(x => x.CompanyId == company.CompanyId).FirstOrDefault();
            }
            else if (company.TransOption == 2)
            {
                find = _dbcontext.Companys.Where(x => x.CompanyId == company.CompanyId).FirstOrDefault();
                UserApp userapp = _dbcontext.UserApps.Where(x => x.CompanyId == company.CompanyId).OrderByDescending(x => x.UserAppId).FirstOrDefault();
                find.userApp = userapp;
                find.Usuario = userapp.Username;
                find.Password = userapp.PassworNotCry;
            }
            else
            {
                find = _dbcontext.Companys.Where(x => x.CompanyId == company.CompanyId).FirstOrDefault();
            }
            return find;
        }


        public Company SelectCurrent(Company company)
        {
            Company find;
            if (company.TransOption == 0 || company.TransOption == 1)
            {
                find = _dbcontext.Companys.Where(x => x.CompanyId == connectionDB.SedeId).FirstOrDefault();
            }
            else
            {
                return null;
            }
            return find;
        }
        public List<Company> selectAll(Company company)
        {
            if (company.TransOption == 0 || company.TransOption == 1)//seleccionar cliente y generico
            {
                return _dbcontext.Companys.ToList();
            }
            else
            {
                return _dbcontext.Companys.ToList();
            }

        }



    }
}
