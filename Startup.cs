using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTToken.Extensions;
using JWTToken.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace JWTToken
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

			services.AddControllers();

			services.AddJWTTokenServices(Configuration);

			services.AddSwaggerGen(opt =>
			{
					opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
					{
						Name = "Authorization",
						Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
						Scheme = "Bearer",
						BearerFormat = "JWT",
						In = Microsoft.OpenApi.Models.ParameterLocation.Header,
						Description = "JWT Authorization header using the Bearer scheme."
					});
					opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
					{
						new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
								Reference = new Microsoft.OpenApi.Models.OpenApiReference {
									Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
										Id = "Bearer"
								}
							},
							new string[] {}
					}
				});
			});

			services.Configure<JsonWebTokenOptions>(Configuration.GetSection(JsonWebTokenOptions.JsonWebTokenKeys));

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTToken", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTToken v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
