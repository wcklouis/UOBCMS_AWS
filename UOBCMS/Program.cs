using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UOBCMS.Data;
using UOBCMS.Models.IBO;
using UOBCMS.Services;
using UOBCMS.Interface;
using UOBCMS.Repository;
using UOBCMS.Classes;
using log4net.Config;
using log4net;


var builder = WebApplication.CreateBuilder(args);

string logLvl = builder.Configuration["Logger:LogLvl"];
Logger.SetConfiguration(logLvl, Directory.GetCurrentDirectory());

//var log4netConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");
//XmlConfigurator.Configure(new FileInfo(log4netConfigPath));

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo { Title = "Client Data Management API", Version = "v1" });
});

builder.Services.Configure<IBOConnectionStringSettings>(
    builder.Configuration.GetSection("ConnectionStrings"));

string iboConnStrKey = builder.Configuration.GetConnectionString("IBOEncryptPublicKey").ToString();
string iboConnStr = builder.Configuration.GetConnectionString("IBOConnection_UAT").ToString();
string eformConnStr = builder.Configuration.GetConnectionString("EFormConnection_UAT").ToString();


/*string uOBCMSConnStrKey = builder.Configuration.GetConnectionString("UOBCMSEncryptPublicKey").ToString();
string uOBCMSConnStr = builder.Configuration.GetConnectionString("UOBCMSConnection").ToString();
uOBCMSConnStr = EncryptDecrypt.Decrypt(uOBCMSConnStr, uOBCMSConnStrKey);*/


// IBO db pending iboConnStr = "Server=UHKS0116UAT;Database=UOBOctoCsharp_UAT;User Id=TestingDev;Password=aa00pp11;TrustServerCertificate=True;";

iboConnStr = "Server=UHKS0116UAT;Database=UOBOctoCsharp_UAT;User Id=TestingDev;Password=aa00pp11;TrustServerCertificate=True;";
//iboConnStr = "Server=WIN2022-IBO-DB;Database=UOBOctoCsharp_UAT;User Id=TestingDev;Password=aa00pp11;TrustServerCertificate=True;";

// Local
string uOBCMSConnStr = "Server=UHKS0124UAT;Database=UOBCMS;User Id=TestingDev;Password=aa00pp11;TrustServerCertificate=True;";

// EForm
eformConnStr = "Server=UHKS0124UAT;Database=UOBKHWebForm;User Id=TestingDev;Password=aa00pp11;TrustServerCertificate=True;";

// Tencent
//string uOBCMSConnStr = "Server=10.50.16.13;Database=UOBCMS_Tencent;User Id=TestingDev;Password=aa00pp11;TrustServerCertificate=True;";

// Add services to the container.
builder.Services.AddDbContext<IBOApplicationDbContext>(options => options.UseSqlServer(iboConnStr));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(uOBCMSConnStr));

builder.Services.AddDbContext<EFormApplicationDbContext>(options =>
    options.UseSqlServer(eformConnStr));

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Handle reference loops
            options.JsonSerializerOptions.MaxDepth = 64; // Optional: Increase max depth if needed
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // Ignore null values
        });

/*builder.Services.AddControllers()
 * 
    .AddJsonOptions(options =>
    {
        // Customize JSON options here
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve; // Handle reference loops
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // Ignore null values
    });*/

builder.Services.AddControllersWithViews();

// Register HttpClient
builder.Services.AddHttpClient();

// Add controllers
builder.Services.AddControllers();

// Configure CORS
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});*/
// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddScoped<InstrumentService>(); // Register your service

// Reister the repository
builder.Services.AddScoped<IInstrumentHoldingRepository, InstrumentHoldingRespository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client Data Management API V1");
    });
}

app.UseHttpsRedirection();

// CORSbody
app.UseRouting();

// Enable CORS
//app.UseCors("AllowAll");
// Use CORS policy
app.UseCors("AllowAllOrigins");


app.UseAuthorization();

app.MapControllers();

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.Run();
