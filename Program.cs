using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Data.Repositories;
using VIANDRIVEFULL.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE LÍMITES PARA IMÁGENES EN BASE64 (50 MB)
// ==========================================
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
});

// ==========================================
// 2. CONFIGURACIÓN DE BASE DE DATOS Y SERVICIOS
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<SolicitudServicioService>();
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RegistroPagoService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICorreoService, CorreoService>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();

// ==========================================
// 3. AUTENTICACIÓN JWT
// ==========================================
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// 4. CONFIGURACIÓN DE MIDDLEWARES Y RUTAS
// ==========================================

// Habilitar Swagger en la ruta /swagger para pruebas limpias
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VIANDRIVE API v1");
    // Al quitar RoutePrefix = string.Empty, Swagger estará disponible en: https://tu-dominio/swagger
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de Controladores de API y Rutas MVC tradicionales
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
