﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DutchTreat
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //enable cors
            services.AddCors();

            //Configuramos Identity
            services.AddIdentity<StoreUser, IdentityRole>(cfg =>
           {
               cfg.User.RequireUniqueEmail = true;
           }).AddEntityFrameworkStores<DutchContext>();

            //Configuramos tokens
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidIssuer = _configuration["Tokens:Issuer"],
                        ValidAudience = _configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]))
                    };
                });

            services.AddDbContext<DutchContext>(cfg =>
            {
                cfg.UseSqlServer(_configuration.GetConnectionString("DutchConnectionString"));
            });

            services.AddAutoMapper();

            services.AddTransient<INullMailService, NullMailService>();
            services.AddTransient<DutchSeeder>();

            services.AddScoped<IDutchRepository, DutchRepository>();

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            /*AddJsonOptions configuramos que al serializar un objeto que se autoreferencia NO entre en una referencia circular:
             Order tiene una collection de ICollection<OrderItem> Items
             y OrderItem tiene su Order asociada --> Explota! le tenemos q decir al framework que no serializa las autoreferencias.
             */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }


            /*
             * Es importante el orden de piezas en el middleware
             * En este caso:
             * Llega el request localhost:8888:
             * app.UseDefaultFiles(); --> Encuentra a index.html en wwwroot entonces cambia internamente la url de startup (localhost:8888/index.html)
             * Luego pasa esa url a:
             * app.UseStaticFiles();
             * Este metodo detecta que esa url quiere servir un archivo estatico que se encuentra en wwwroot entonces lo habilita
             * y lo sirve al browser --> OK
             * 
             * Si lo tuvieramos al reves
             * Llega el request localhost:8888
             * app.UseStaticFiles();
             * Habilita que se sirvan archivos estaticos de wwwroot pero no hay nada que servir ya que por default localhost:8888 no es un recurso explicito
             * Luego avanza a
             * app.UseDefaultFiles();
             * encuentra el la pagina default index.html pero no hay nadie a quien pasarle ese recurso, entonces no hace nada
             * 
             * Si el request que llegaba seria localhost:8888/index.html seria lo mismo el orden ya que no hay que buscar el default, directamente se sirve el recurso explicito
             * 
             */

            /*
             * Busca archivos default's en el root o en las folders para que sean el startup de la app
             * Por ejemplo si tenemos el archivo index.html es uno de los que se considera default, si lo encuentra
             * entonces lo toma y lo pone como pagina startup
             */
            //Lo sacmos al momento de implementar MVC porque esté se encarga del routeo de request
            //app.UseDefaultFiles();

            //Habilitamos el webs server para que devuelva archivos estaticos al browser
            //Por default solo servira archivos estaticos alojados en wwwroot
            app.UseStaticFiles();

            //Nuget odeToCode de ScottAllen que permite buscar archivos estaticos en la carpeta node_modules
            //En nuestro project CoreFundamentals/OdeToFood en GitHub tenemos la implementación
            app.UseNodeModules(env);

            //app.Run(async (context) =>
            //{
            //    //For each request return "Hello World!" to the browser
            //    //Literalmente retorna esta phrase sin formato html, sin <html> or <body> or etc
            //    //await context.Response.WriteAsync("Hello World!");

            //    //Retornamos con formato html
            //    //Pongamos la URL que sea retorna esto
            //    await context.Response.WriteAsync("<html><body><h1>Hello World!</h1></body></html>");
            //});

            //Habilitamos Auth --> Before MVC!
            app.UseAuthentication();

            //Confiramos Cors
            app.UseCors(builder =>
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
            );

            app.UseMvc(cfg =>
            {
                cfg.MapRoute("Default",
                    "/{controller}/{action}/{id?}",
                    new { controller = "App", action = "Index" });
            });
        }
    }
}
