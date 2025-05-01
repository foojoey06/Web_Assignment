global using Web_Assignment.Models;
global using Web_Assignment;
using Stripe;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<DB>($@"
    Data Source = (LocalDB)\MSSQLLocalDB;
    AttachDbFileName = {builder.Environment.ContentRootPath}\DB.mdf;
");
builder.Services.AddScoped<Helper>();
builder.Services.AddAuthentication().AddCookie();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
// TODO: Stripe API Key
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SK"];

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization("en-MY");
app.UseSession();
app.MapDefaultControllerRoute(); //<- first page it directs to
app.Run();
