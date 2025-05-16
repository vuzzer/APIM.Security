using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using Sec.Market.MVC.Services;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(builder.Configuration.GetValue<string>("API:Scopes")?.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>

{
    options.FallbackPolicy = options.DefaultPolicy;
});


builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages()
.AddMicrosoftIdentityUI();

builder.Services.AddHttpClient<IProductService, ProductServiceProxy>(client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApi")));
builder.Services.AddHttpClient<IUserService, UserServiceProxy>(client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApi")));
builder.Services.AddHttpClient<IOrderService, OrderServiceProxy>(client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApi")));
builder.Services.AddHttpClient<ICustomerReviewService, CustomerReviewServiceProxy>(client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApi")));
builder.Services.Configure<Subscription>(builder.Configuration.GetRequiredSection("Subscription"));


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Auth";
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();

app.Run();
