using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PixelCelebrateBackend.Entities;
using PixelCelebrateBackend.Scheduler;
using PixelCelebrateBackend.Service;
using PixelCelebrateBackend.Service.Model;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PixelCelebrateDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton(serviceProvider => new NotificationsService(
    configuration: builder.Configuration,
    mapper: serviceProvider.GetRequiredService<IMapper>()
    ));

builder.Services.AddSingleton(serviceProvider => new NotificationsScheduler(
    timeSpan: TimeSpan.FromMilliseconds(50000), //for testing-sending emails once every 50 seconds;
                                                // timeSpan: TimeSpan.FromMilliseconds(1, 0, 0, 0), //for sending emails once a day;
    emailService: serviceProvider.GetRequiredService<IEmailService>(),
    notificationsService: serviceProvider.GetRequiredService<NotificationsService>()
    ));

//var task = builder.Services.BuildServiceProvider().GetRequiredService<NotificationsScheduler>();
//task.Start();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

var task = app.Services.GetRequiredService<NotificationsScheduler>();
task.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();