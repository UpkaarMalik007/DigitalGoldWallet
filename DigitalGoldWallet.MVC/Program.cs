using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

namespace DigitalGoldWallet.MVC;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();

        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IGoldApiService, GoldApiService>();

        builder.Services.AddHttpClient<ApiService>(client =>
        builder.Services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
        });

        builder.Services.AddHttpClient("DigitalGoldWalletApi", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]! + "api/");
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ApiService>();
        builder.Services.AddScoped<IVendorApiService, VendorApiService>();
        builder.Services.AddScoped<ITransactionApiService, TransactionApiService>();
        builder.Services.AddScoped<IUserApiService, UserApiService>();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        WebApplication app = builder.Build();
builder.Services.AddHttpClient<ApiService>(
    client =>
    {
        client.BaseAddress =
            new Uri("http://localhost:5103/");
    });

var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

        app.UseHttpsRedirection();
        app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseStaticFiles();

        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Auth}/{action=Login}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern:
    "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
app.Run();