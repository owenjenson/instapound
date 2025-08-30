using Instapound.Data;
using Instapound.Data.Entities;
using Instapound.Web;
using Instapound.Web.Hubs;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "instapound.db");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSqlite(connectionString)
        .LogTo(text => System.Diagnostics.Debug.WriteLine(text), LogLevel.Information));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddScoped<PostsService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<CommentsService>();
builder.Services.AddScoped<MessagesService>();
builder.Services.AddScoped<ImagesService>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<ChatHub>("/chatHub");

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    
    TestData.SeedData(dbContext, userManager);
}

app.Run();