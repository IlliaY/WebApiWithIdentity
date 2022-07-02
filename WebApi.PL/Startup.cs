using System;
using System.Text;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Models;
using WebApi.BLL.Services;
using WebApi.BLL.Validators;
using WebApi.DAL.Data;
using WebApi.DAL.Interfaces;
using WebApi.DAL.Repositories;
using WebApi.PL.Configurations;

namespace WebApi.PL
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddProblemDetailsConventions().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            // A middleware that return a problem details object when an exception is thrown.
            services.AddProblemDetails(options =>
            {
                options.MapFluentValidationException();
                options.IncludeExceptionDetails = (con, action) => false;
                options.MapAuthentificationException();
            });

            //add cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            //add identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

            //add authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
                    };
                });

            //add dbcontext
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //add swagger
            services.AddSwaggerGen();

            //add users repository
            services.AddScoped<IUserRepository, UserRepository>();


            //add users repository
            services.AddScoped<IRoleRepository, RoleRepository>();

            //add users manager
            services.AddScoped<UserManager<IdentityUser>, UserManager<IdentityUser>>();

            //add roles manager
            services.AddScoped<RoleManager<IdentityRole>, RoleManager<IdentityRole>>();

            //add unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //add AuthService
            services.AddScoped<IAuthService, AuthService>();

            //configure apibehavior
            services.Configure<ApiBehaviorOptions>(
                options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });

            //configure jwtservice
            services.AddScoped<IJwtCreationService, JwtCreationService>();

            //add validator for user login
            services.AddScoped<IValidator<UserLoginModel>, UserLoginValidator>();

            //add validator for user register
            services.AddScoped<IValidator<UserRegisterModel>, UserRegisterValidator>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseProblemDetails();

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
