using AzureStorageLibrary;
using AzureStorageLibrary.Service;

var builder = WebApplication.CreateBuilder(args);

ConnectionString.AzureStorageConnectionString = builder.Configuration.GetSection("ConnectionStrings")["StorageConStr"];
builder.Services.AddScoped(typeof(INoSqlStorage<>), typeof(TableStorage<>));
// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TableStorage}/{action=Index}/{id?}");

app.Run();
