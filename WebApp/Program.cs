using ClientConfiguration.IdentityServerHandler;
using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.Configurations;
using Common.DataTransferObjects.Token;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

/*CONFIGURE SERVICES CONTAINER*/
IdentityServerClientDefinition identityServerClientDefinition = new();
builder.Configuration.Bind("IdentityServerClientDefinition", identityServerClientDefinition);
builder.Services.AddSingleton(identityServerClientDefinition);

WebAppSetting webAppSetting = new();
builder.Configuration.Bind("WebAppSetting", webAppSetting);
builder.Services.AddSingleton(webAppSetting);


ApiResourceUrl apiResourceUrl = new();

builder.Configuration.Bind("ApiResourceUrl", apiResourceUrl);
builder.Services.AddSingleton(apiResourceUrl);

builder.Services.AddSingleton<IdentityServerTokenDetail>();
builder.Services.AddTransient<IdentityServerTokenHandler>();

builder.Services.AddHttpClient("RITSApiClient", opt =>
{
    opt.Timeout = TimeSpan.FromMinutes(5);
    //TODO: Rename base URL
    opt.BaseAddress = new Uri(apiResourceUrl.RITSApiBaseUrl);
}).AddHttpMessageHandler<IdentityServerTokenHandler>();

AzureAdClientDefinition azureAdClientDefinition = new();
builder.Configuration.Bind("AzureAdClientDefinition", azureAdClientDefinition);
builder.Services.AddSingleton(azureAdClientDefinition);
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
    //.AddMicrosoftIdentityWebApp(opt =>
    //{
    //    opt.Instance = azureAdClientDefinition.Instance;
    //    opt.CallbackPath = azureAdClientDefinition.CallbackPath;
    //    opt.ClientId = azureAdClientDefinition.ClientId;
    //    opt.TenantId = azureAdClientDefinition.TenantId;
    //    opt.ClientSecret = azureAdClientDefinition.ClientSecret;
    //})
    //.EnableTokenAcquisitionToCallDownstreamApi(azureAdClientDefinition.Scopes)
    //.AddInMemoryTokenCaches();

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
{
    opt.AccessDeniedPath = new PathString(azureAdClientDefinition.AccessDeniedPath);
});


//Security Group Policy
//SecurityGroup securityGroup = new();
//builder.Configuration.Bind("SecurityGroup", securityGroup);
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Support",
//         policy => policy.RequireClaim("groups", securityGroup.ApplicationSupport));
//});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


builder.Services.AddControllersWithViews(opt =>
{
    var policy = new AuthorizationPolicyBuilder()
       .RequireAuthenticatedUser()
       //TODO: Remove below line if all users in tenant are allowed to access the application
       //.RequireClaim("groups", securityGroup.AllowedGroups)
       .Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
});

// configure automapper with all automapper profiles from this assembly
builder.Services.AddAutoMapper(typeof(Program));


//Session and State Management
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RITS.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(webAppSetting.SessionExpirationMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

/*CONFIGURE HTTP REQUEST PIPELINE*/
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

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
