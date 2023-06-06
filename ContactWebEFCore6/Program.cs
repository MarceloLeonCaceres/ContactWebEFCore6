using ContactWebEFCore6.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyContactManagerData;
using MyContactManagerRepositories;
using MyContactManagerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
var mcmdContext = builder.Configuration.GetConnectionString("MyContactManager");
builder.Services.AddDbContext<MyContactManagerDbContext>(options =>
    options.UseSqlServer(mcmdContext));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// For use in the Azure environment
var azureIdentityDbConnString = builder.Configuration.GetConnectionString("AzureIdentityDb");
var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(azureIdentityDbConnString)
    .Options;
using (var context = new ApplicationDbContext(contextOptions))
{
    context.Database.Migrate();
}

var azureContactDbConnString = builder.Configuration.GetConnectionString("AzureContactDb");
var contextOptions2 = new DbContextOptionsBuilder<MyContactManagerDbContext>()
    .UseSqlServer(azureContactDbConnString)
    .Options;
using (var context = new MyContactManagerDbContext(contextOptions2))
{
    context.Database.Migrate();
}
//

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<IStatesService, StatesService>();
builder.Services.AddScoped<IContactsRepository, ContactsRepository>();
builder.Services.AddScoped<IContactsService, ContactsService>();

builder.Services.AddScoped<IUserRolesService, UserRoleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
