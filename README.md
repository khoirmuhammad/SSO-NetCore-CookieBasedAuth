# SSO With Cookie Based Auth ASP NET CORE
Here we will show about how to create single sign on using cookie sharing.

### Scenario
- Creating 2 or more application but running in same machine / server
- Creating folder to store "key". This key will be shared to all application that need single sign on. So that we host 2 or more application into same machine. But the main concept is all of application able to use shared key. Even if we try to host application in different machine

1. Add this code in service configuration. Keep in mind that cookie name has to same for each application that using SSO
```
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
```

2. Don't forget to register authentication in middleware for all applications
```
app.UseAuthentication();
```

3. Add Persist Key to share required key in configure service. Remember all application should use same directory path and application name. We able to store path string in appsettings.json
```
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"D:\Me\Research\SSO\SSOCookieBasedAuthentication\CookieKey"))
    .SetApplicationName("MyAppAuthCookie");
```

4. Finally we just need create auth controller with login logout method in one of application. Don't forget to set authorize attribute in controller/method we want
