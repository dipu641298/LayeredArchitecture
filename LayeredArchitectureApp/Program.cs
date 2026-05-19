using LayeredArchitecture.Application;
using LayeredArchitecture.Business.Services;
using LayeredArchitecture.DataAccess.Clients;
using LayeredArchitecture.DataAccess.Repositories;
using LayeredArchitecture.Domain.Clients;
using LayeredArchitecture.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURE PRESENTATION LAYER (Framework Services)
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================
// 2. CONFIGURE DATA ACCESS LAYER (I/O & Infrastructure)
// ============================================================
// Register the database repository. In a real application, you would 
// also register your Entity Framework DbContext right above this.
builder.Services.AddScoped<ILoanRepository, SqlLoanRepository>();

// Register the HTTP Client for external API communication.
// We configure the base address here (usually pulled from appsettings.json) 
// so the HttpCreditBureauClient doesn't have to hardcode URLs.
builder.Services.AddHttpClient<ICreditBureauClient, HttpCreditBureauClient>(client =>
{
    var baseUrl = builder.Configuration["CreditBureau:BaseUrl"] ?? "https://api.fakecreditbureau.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddHttpClient<IBankTransferClient, HttpBankTransferClient>(client =>
{
    client.BaseAddress = new Uri("https://api.fakebank.com/");
});

// ============================================================
// 3. CONFIGURE BUSINESS LAYER (Pure Logic)
// ============================================================
// Business services are stateless pure logic, so Transient is usually 
// the safest and most efficient lifetime.
builder.Services.AddTransient<RiskAssessmentService>();
builder.Services.AddTransient<DisbursementCalculatorService>();


// ============================================================
// 4. CONFIGURE APPLICATION LAYER (Orchestrators)
// ============================================================
// Use cases are scoped to the HTTP request. If they trigger multiple 
// repository updates, they will share the same DbContext instance.
builder.Services.AddScoped<ProcessLoanUseCase>();
builder.Services.AddScoped<DisburseLoanUseCase>();


var app = builder.Build();

// ============================================================
// 5. CONFIGURE HTTP PIPELINE (Middleware)
// ============================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// This maps the incoming HTTP requests to your Presentation layer controllers
app.MapControllers();

app.Run();