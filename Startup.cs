using API.Mowizz2.EHH.Configs;
using API.Mowizz2.EHH.Models;
using API.Mowizz2.EHH.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Mowizz2.EHH
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string SecretKey = "needtogetthisfromenvironment";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        #region Public methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DataBaseSettings>(
                Configuration.GetSection(nameof(DataBaseSettings)));

            services.AddSingleton<IDataBaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DataBaseSettings>>().Value);

            services.AddSingleton<AccountsService>();
            services.AddSingleton<UsersService>();
            services.AddSingleton<CompaniesService>();
            services.AddSingleton<ConceptsService>();
            services.AddSingleton<CostCentresService>();
            services.AddSingleton<TransactionsService>();

            // Configure JwtIssuerOptions
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                    IssuerSigningKey = _signingKey
                };
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();

            services.AddCors();            
        }        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x                  
                  .AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();  

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #endregion

        #region Private methods

        private static void AddServices(IServiceCollection services)
        {
            
        }

        #endregion
    }
}
