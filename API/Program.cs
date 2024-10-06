using API.Data;
using API.Services;
using EmailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt=>{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddHttpClient();

builder.Services.AddHostedService<ApiDataService>();

builder.Services.AddCors();


builder.Services.AddSingleton<IEmailSender, EmailSender>();


EmailConfiguration emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseStaticFiles();


app.MapControllers();

app.Run();

