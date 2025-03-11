global using Web_Assignment.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<DB>($@"
    Data Source = (LocalDB)\MSSQLLocalDB;
    AttachDbFileName = {builder.Environment.ContentRootPath}\DB.mdf;
");

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute(); //<- first page it directs to
app.Run();
