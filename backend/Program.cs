using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Perfumes.Api.Data;
using Perfumes.Api.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var conexao = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(conexao))
        throw new RegraException(503, "Configure ConnectionStrings__DefaultConnection para acessar os cadastros.");
    options.UseSqlServer(conexao);
});
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<InsumoService>();
builder.Services.AddScoped<FornecedorProdutoService>();
builder.Services.AddScoped<PrecoService>();
builder.Services.AddSingleton<UnidadeService>();
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? ["http://localhost:5173"];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
