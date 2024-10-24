using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartSchool.API.Context;
using SmartSchool.API.Repository;
using System;
using System.IO;
using System.Reflection;

namespace SmartSchool.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            #region CONNECTION STRING
            services.AddDbContext<SmartContext>(
                context => context.UseSqlite(Configuration.GetConnectionString("Default"))
            );
            #endregion

            #region CONTROLLES
            services.AddControllers()
                    .AddNewtonsoftJson(
                        opt => opt.SerializerSettings.ReferenceLoopHandling =
                            Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            #endregion

            #region AUTO MAPPER
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            #region DEPENDENCY INJECTION
            services.AddScoped<IBaseRepository, BaseRepository>();
            #endregion

            #region SWAGGER
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;

            }).AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            var apiProviderDescription = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(options =>
            {
                foreach (var description in apiProviderDescription.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,

                        new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = "SmartSchool API",
                            Version = description.ApiVersion.ToString(),
                            TermsOfService = new Uri("http://SeusTermosDeUso.com"),
                            Description = "A descrição da WebAPI do SmartSchool",
                            License = new Microsoft.OpenApi.Models.OpenApiLicense
                            {
                                Name = "SmartSchool License",
                                Url = new Uri("http://mit.com")
                            },

                            Contact = new Microsoft.OpenApi.Models.OpenApiContact
                            {
                                Name = "Cristiano Souza",
                                Email = "cristiano.souzac1@gmail.com",
                                Url = new Uri("https://www.linkedin.com/in/cristianocsouza/")

                            }
                        });

                }

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                options.IncludeXmlComments(xmlCommentsFullPath);
            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiProviderDescription)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                foreach (var description in apiProviderDescription.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToLowerInvariant());
                }

                options.RoutePrefix = "";
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
