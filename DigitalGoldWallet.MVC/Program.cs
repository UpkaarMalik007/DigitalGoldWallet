using DigitalGoldWallet.MVC.Services;

var builder = WebApplication.CreateBuilder(args);
using DigitalGoldWallet.MVC.Services;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IGoldApiService, GoldApiService>();
namespace DigitalGoldWallet.MVC;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
        });

        builder.Services.AddHttpClient("DigitalGoldWalletApi", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]! + "api/");
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IVendorApiService, VendorApiService>();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        WebApplication app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Auth}/{action=Login}/{id?}");

        app.Run();
    }
}
