
using GetAllDevicesStatusSingalR.Hubs;
using GetAllDevicesStatusSingalR.Interface.DBConn;
using GetAllDevicesStatusSingalR.Interface.Device;
using GetAllDevicesStatusSingalR.Service.DBConn;
using GetAllDevicesStatusSingalR.Service.Device;

namespace GetAllDevicesStatusSingalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IGetDeviceControlService, GetDeviceControlService>();
            builder.Services.AddScoped<IGetUdiBreathing, GetUdIBreathing>();
            builder.Services.AddScoped<IDBConn, DBConn>();
            builder.Services.AddScoped<IGetSiteList, GetSiteList>();

            // Cors 設定
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            builder.Services.AddSignalR();
            var app = builder.Build();
            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();
            // 設定路由
            app.MapHub<SignalRServer>("/SignalRServer");
            app.Run();
        }
    }
}
