
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AppointmentService.Data;
using AppointmentService.Repositories;
using AppointmentService.Services;
using System.Text;

namespace AppointmentService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Angular", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            builder.Services.AddDbContext<AppointmentDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService.Services.AppointmentService>();
            builder.Services.AddHttpClient("DoctorService", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:DoctorService"] ?? "http://localhost:5003");
            });

            var jwt = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"] ?? ""))
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("Angular");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // seed data
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
                db.Database.Migrate();
                if (!db.Appointments.Any())
                {
                    db.Appointments.AddRange(new Models.Appointment { PatientId = 2, DoctorId = 3, AppointmentDate = DateTime.UtcNow.AddDays(1) });
                    db.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
