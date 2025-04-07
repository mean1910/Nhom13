using MongoDB.Driver;
using Nhom13.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
    return new MongoClient(settings?.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
    var database = client.GetDatabase(settings?.DatabaseName);
    return database;
});

// Add data migration service
builder.Services.AddScoped<DataMigration>(sp =>
{
    var mongoDb = sp.GetRequiredService<IMongoDatabase>();
    var mysqlConnectionString = "Server=127.0.0.1;Port=3306;Database=restaurantsupplydb;User=root;Password=;Allow User Variables=True";
    return new DataMigration(mysqlConnectionString, mongoDb);
});

var app = builder.Build();

// Run data migration and seed sample data
using (var scope = app.Services.CreateScope())
{
    var migration = scope.ServiceProvider.GetRequiredService<DataMigration>();
    await migration.SeedSampleData();
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
