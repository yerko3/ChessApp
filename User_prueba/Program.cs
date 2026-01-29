using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace User_prueba
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSingleton<IRepository, SupabaseService>();
            builder.Services.AddSingleton<ICacheService, RedisCacheService>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(
                    builder.Configuration.GetConnectionString("redis")!
                )
            );

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebApp", policy =>
                {
                    policy.WithOrigins(
                        "https://localhost:5051", 
                        "http://127.0.0.1:5500",  
                        "https://chessapp20250707173301-fvdbhuhwgpb7b4dq.canadacentral-01.azurewebsites.net", 
                        "https://chessapp3-a4chcke6bvhkgwbw.eastus-01.azurewebsites.net" 
                    )
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        ),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("jwt"))
                            {
                                context.Token = context.Request.Cookies["jwt"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });


            var app = builder.Build();

            app.UseCors("AllowWebApp");           
            app.UseStaticFiles();                   
            app.UseAuthentication();               
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/Main.html");
                    return;
                }

                await next();
            });

            app.MapControllers();

            app.Run();
        }
    }
}
