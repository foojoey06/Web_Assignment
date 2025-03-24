global using Web_Assignment.Models;
global using Web_Assignment;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<DB>($@"
    Data Source = (LocalDB)\MSSQLLocalDB;
    AttachDbFileName = {builder.Environment.ContentRootPath}\DB.mdf;
");
builder.Services.AddScoped<Helper>();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute(); //<- first page it directs to
app.Run();
