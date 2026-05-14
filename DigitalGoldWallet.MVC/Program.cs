using DigitalGoldWallet.MVC.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

string apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7269/";
if (!apiBaseUrl.EndsWith('/'))
{
    apiBaseUrl += "/";
}

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient("DigitalGoldWalletApi", client =>
{
    client.BaseAddress = new Uri(new Uri(apiBaseUrl), "api/");
});

builder.Services.AddHttpClient<IGoldApiService, GoldApiService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<IVendorApiService, VendorApiService>();
builder.Services.AddScoped<ITransactionApiService, TransactionApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IAdminApiService, AdminApiService>();

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
