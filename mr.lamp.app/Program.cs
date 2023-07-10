using System.Security.Cryptography.X509Certificates;
using System.Device.Gpio;


var builder = WebApplication.CreateBuilder(args);

// Configure certificate usage
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var certificateName = configuration["ServerCertName"];
var certificatePassword = configuration["ServerCertPassword"];
X509Certificate2 serverCertificate = new X509Certificate2($"{Directory.GetCurrentDirectory()}/certificates/{certificateName}", certificatePassword);
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(8443, listenOptions =>
    {
        listenOptions.UseHttps(serverCertificate);
    });
    options.ListenAnyIP(5000);
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<GpioController>();
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 8443;
});

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
