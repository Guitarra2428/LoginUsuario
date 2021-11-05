using LoginUsuario.Data;
using LoginUsuario.Repository;
using LoginUsuario.Repository.IRepository;
using LoginUsuario.UsuarioMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoginUsuario
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Conections")));


            services.AddAutoMapper(typeof(UsauarioMappers));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddCors();



            services.AddControllers();
            //dependencia de token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                option => option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer=false,
                    ValidateAudience=false
                }

                ) ;

            services.AddSwaggerGen(c =>
            {
            c.SwaggerDoc("v1", new OpenApiInfo { 
                Title = "Login Usuario",
                Version = "v1" ,
                Description = "Backend Login",
                Contact = new OpenApiContact
                {
                    Email = "Guitarra2428@gmail.com",
                    Name = "Luis",
                    Url = new Uri("http://quitckcode-001-site1.etempurl.com/")
                },
                License = new OpenApiLicense()
                {
                    Name = "Mit License",
                    Url = new Uri("http://quitckcode-001-site1.etempurl.com/")
                }

            });


            c.AddSecurityDefinition("Bearer",

                new OpenApiSecurityScheme
                {
                    Name = "Autorizacion Valida",
                    Description = "Autenticacion JWT (Bearer)",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                }

                );
            c.AddSecurityRequirement(new  OpenApiSecurityRequirement  {
                {
                    
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },new List<string>()
                }
              }); 
            
            //Documentacion Api

                var ducumentacion = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutadocumentacion = Path.Combine(AppContext.BaseDirectory, ducumentacion);                
                c.IncludeXmlComments(rutadocumentacion);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LoginUsuario v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //soporte para cors
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
