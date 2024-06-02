using AccesoDatos.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using AccesoDatos.Repositorio;
using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Utilities;
using ModelosApp.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LibreriaContexto>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("ConexionLibreria")));

builder.Services.Configure<ConfiguracionStripe>(builder.Configuration.GetSection("ConfiguracionPago"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<LibreriaContexto>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("ConfiguracionPago:SecretKey").Get<string>();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Usuario}/{controller=Home}/{action=Index}/{id?}");

app.Run();
