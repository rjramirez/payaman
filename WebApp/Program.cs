using Common.Constants;
using Common.DataTransferObjects.AppSettings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using WebApp.Authorizations.Handler;
using WebApp.Authorizations.Requirements;
using WebApp.Services.Interfaces;
using WebApp.Services;
using Common.DataTransferObjects.Configurations;

var builder = WebApplication.CreateBuilder(args);

/*CONFIGURE SERVICES CONTAINER*/    
IdentityServerClientDefinition identityServerClientDefinition = new();
builder.Configuration.Bind("IdentityServerClientDefinition", identityServerClientDefinition);
builder.Services.AddSingleton(identityServerClientDefinition);

ClientSetting clientSetting = new();
builder.Configuration.Bind("ClientSetting", clientSetting);
builder.Services.AddSingleton(clientSetting);

ApiResourceUrl apiResourceUrl = new();
builder.Configuration.Bind("ApiResourceUrl", apiResourceUrl);
builder.Services.AddSingleton(apiResourceUrl);

WebAppSetting webAppSetting = new();
builder.Configuration.Bind("WebAppSetting", webAppSetting);
builder.Services.AddSingleton(webAppSetting);


builder.Services.AddSingleton<ICommonService, CommonService>();

builder.Services.AddHttpClient("RITSApiClient", opt =>
{
    opt.Timeout = TimeSpan.FromMinutes(5);
    //TODO: Rename base URL
    opt.BaseAddress = new Uri(apiResourceUrl.RITSApiBaseUrl);
});
//.AddHttpMessageHandler<LocalTokenHandler>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = new PathString(clientSetting.AccessDeniedPath); ;
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
         policy => {
			 //policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
			 policy.Requirements.Add(new RoleRequirement(new byte[] { RoleConstant.Admin }));
         });

    options.AddPolicy("Cashier",
         policy => policy.Requirements.Add(new RoleRequirement(new byte[] { RoleConstant.Cashier })));
});
builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

//Session and State Management
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RITS.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(webAppSetting.SessionExpirationMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
});

// configure automapper with all automapper profiles from this assembly
builder.Services.AddAutoMapper(typeof(Program));


/*CONFIGURE HTTP REQUEST PIPELINE*/
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use((context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Server", "Server"); //For IIS 8 Only
    return next.Invoke();
});

app.UseStatusCodePagesWithRedirects("/Error/StatusPage?code={0}");
app.UseExceptionHandler("/Error/LogError");
app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
