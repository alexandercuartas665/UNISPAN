using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using adesoft.adepos.webview.Controller;
using Radzen;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Http;
using adesoft.adepos.webview.Bussines;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using adesoft.adepos.webview.Data.Model.Simex;
using adesoft.adepos.webview.Data.Interfaces;

namespace adesoft.adepos.webview
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpClient(); // <- habilita IHttpClientFactory
            services.AddScoped<LogisticsController>();
            services.AddScoped<LogisticsService>();

            //services.AddCors(options => options.AddPolicy("Cors", builder =>
            //{
            //    builder.WithOrigins(
            //            "http://localhost:1495",
            //            "http://localhost:1495/ReportListFactura.aspx")
            //           .WithMethods("POST", "GET", "PUT")
            //           .AllowAnyHeader(); // "*" no es válido en WithHeaders, se usa AllowAnyHeader()
            //}));
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            services.AddDbContext<AdeposDBContext>(options =>
              options.UseSqlServer(
                  Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            services.AddDbContext<AdeposReportsContext>(options =>
              options.UseSqlServer(
                  Configuration.GetConnectionString("UnispanReports")), ServiceLifetime.Transient);

            // Add custom services to container
            services.AddCustomServices();

            // services.AddDbContext<AdeposDBContext>(ServiceLifetime.Scoped);
            services.AddAuthorizationCore();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))

                };
            });
            //services.AddAuthentication(x => { x.DefaultAuthenticateScheme=})
            // services.AddDefaultIdentity<UserApp>();
            ///  services.AddIdentity<UserApp, RoleApp>();
            //.AddEntityFrameworkStores<WorldContext>();
            services.AddRazorPages();

            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
            }).AddCircuitOptions(t => { t.DetailedErrors = true; });

            //services.AddServerSideBlazor();

            ///Contexto htttp
            // services.AddCaching();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "Cookie.Adesoft";
                options.IdleTimeout = TimeSpan.FromDays(1000);
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //            @using Microsoft.AspNetCore.Http
            //@inject IHttpContextAccessor httpContextAccessor
            services.AddProtectedBrowserStorage();
            //  services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<UserApp>>();
            services.AddSingleton<WeatherForecastService>();



            services.AddScoped<SecurityService>();
            services.AddScoped<SecurityController>();
            services.AddScoped<RoleController>();
            services.AddScoped<UserAppService>();
            services.AddScoped<UserAppController>();
            services.AddScoped<RoleAppService>();

            services.AddScoped<CategoryService>();
            services.AddScoped<CategoryController>();

            services.AddScoped<ItemService>();
            services.AddScoped<ItemController>();



            services.AddScoped<TransactionGenericService>();
            services.AddScoped<TransactionGenericController>();

            services.AddScoped<StateTransactionGenericService>();
            services.AddScoped<StateTransactionGenericController>();

            services.AddScoped<WarehouseService>();
            services.AddScoped<WarehouseController>();


            services.AddScoped<AlertXOrderService>();
            services.AddScoped<AlertXOrderController>();

            services.AddScoped<MovementInventoryService>();
            services.AddScoped<MovementInventoryController>();

            services.AddScoped<ControlStateTransactionController>();
            services.AddScoped<ControlStateTransactionService>();

            services.AddScoped<NotificationService>();
            services.AddScoped<DialogService>();

            services.AddScoped<CompanyService>();
            services.AddScoped<CompanyController>();

            services.AddScoped<NominaService>();
            services.AddScoped<NominaController>();

            services.AddScoped<TerceroService>();
            services.AddScoped<TerceroController>();

            services.AddScoped<AccountingAccountService>();
            services.AddScoped<AccountingAccountController>();
            // services.AddSingleton<SecurityService>(); //genera error con multiples parametros
            services.AddScoped<TokenAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
            //services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());

            services.AddScoped<LocationGenericController>();
            services.AddScoped<LocationGenericService>();


            services.AddScoped<RequestCertificateController>();
            services.AddScoped<RequestCertificateService>();

            services.AddScoped<BiableController>();
            services.AddScoped<BiableService>();

            services.AddScoped<ParameterController>();
            services.AddScoped<ParameterService>();

            services.AddScoped<SimexController>();
            services.AddScoped<SimexService>();

            services.AddScoped<ProduccionController>();
            services.AddScoped<ProduccionService>();

            services.AddScoped<QuantifyController>();
            services.AddScoped<QuantifyService>();

            services.AddScoped<OportunidadesCRMController>();
            services.AddScoped<OportunidadesCRMService>();

            services.AddScoped<LogisticsController>();
            services.AddScoped<LogisticsService>();

            services.AddScoped<IFileUpload, FileUpload>();

            //servicio de timer ...
            services.AddHostedService<TimedHostedService>();

            services.AddSingleton<NavigationManagerViewControl>();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 60000000;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseCors(options => options
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());

            //Autenticacion
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
               name: "default",
               pattern: "api/{controller}/{id?}"
               );
                endpoints.MapControllerRoute(
                  name: "ActionApi2",
                  pattern: "api/{controller}/{action}"
              );
                endpoints.MapControllerRoute(
                   name: "ActionApi",
                   pattern: "api/{controller}/{action}/{id?}"
               );

                //app.UseCors("Cors");
                // defaults: new { id = .Optional });
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });


            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
          Path.Combine(env.ContentRootPath, "wwwroot")),
                RequestPath = "/wwwroot"
            });



        }
    }
}
