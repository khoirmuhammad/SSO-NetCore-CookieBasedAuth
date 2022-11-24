using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

string path = builder.Configuration.GetSection("PersistKeyValue")?.Value ?? string.Empty; // @"D:\Me\Research\SSO\SSOCookieBasedAuthentication\CookieKey";

// Add services to the container.
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(path))
    .SetApplicationName("MyAppAuthCookie");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Specify in case un-authenticated users
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            //context.Properties.IsPersistent = true; //true : cookie will be deleted after expired. Even if the browser is closed, it will be exists. false : if browser is closed then cookie will be removed
            return Task.CompletedTask;
        };

        // Specify the name of the auth cookie.
        // ASP.NET picks a dumb name by default. "AspNetCore.Cookies"
        options.Cookie.Name = "my_app_auth_cookie";
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
