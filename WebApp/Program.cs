using ClientConfiguration.IdentityServerHandler;
using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.Token;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

/*CONFIGURE SERVICES CONTAINER*/
IdentityServerClientDefinition identityServerClientDefinition = new();
builder.Configuration.Bind("IdentityServerClientDefinition", identityServerClientDefinition);
builder.Services.AddSingleton(identityServerClientDefinition);

ApiResourceUrl apiResourceUrl = new();

builder.Configuration.Bind("ApiResourceUrl", apiResourceUrl);
builder.Services.AddSingleton(apiResourceUrl);

builder.Services.AddSingleton<IdentityServerTokenDetail>();
builder.Services.AddTransient<IdentityServerTokenHandler>();

builder.Services.AddHttpClient("ProjectTemplateApiClient", opt =>
{
    opt.Timeout = TimeSpan.FromMinutes(5);
    //TODO: Rename base URL
    opt.BaseAddress = new Uri(apiResourceUrl.ProjectTemplateApiBaseUrl);
}).AddHttpMessageHandler<IdentityServerTokenHandler>();

AzureAdClientDefinition azureAdClientDefinition = new();
builder.Configuration.Bind("AzureAdClientDefinition", azureAdClientDefinition);
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(opt =>
    {
        opt.Instance = azureAdClientDefinition.Instance;
        opt.CallbackPath = azureAdClientDefinition.CallbackPath;
        opt.ClientId = azureAdClientDefinition.ClientId;
        opt.TenantId = azureAdClientDefinition.TenantId;
    });

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
{
    opt.AccessDeniedPath = new PathString(azureAdClientDefinition.AccessDeniedPath);
});

//Security Group Policy
SecurityGroup securityGroup = new();
builder.Configuration.Bind("SecurityGroup", securityGroup);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Support",
         policy => policy.RequireClaim("groups", securityGroup.ApplicationSupport));
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllersWithViews(opt =>
{
    var policy = new AuthorizationPolicyBuilder()
       .RequireAuthenticatedUser()
       //TODO: Remove below line if all users in tenant are allowed to access the application
       .RequireClaim("groups", securityGroup.AllowedGroups)
       .Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
