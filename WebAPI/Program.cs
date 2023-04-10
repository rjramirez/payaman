using ApiConfiguration;
using Common.Constants;
using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.Version;
using DataAccess.DBContexts.ProjectTemplateDB;
using DataAccess.Services;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWorks.ProjectTemplateDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using System;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

/*SERVICES CONTAINER*/
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

/* Identity Server */
//IdentityServerApiDefinition identityServerApiDefinition = new();
//builder.Configuration.Bind("IdentityServerApiDefinition", identityServerApiDefinition);
//builder.Services.AddSingleton(identityServerApiDefinition);

//ApiServices.ConfigureServices(builder.Services, identityServerApiDefinition);

//Api Policy Authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("SystemLog", builder =>
//    {
//        builder.RequireScope("ProjectTemplateApi.SystemLog");
//    });
//});

//DBContext Registration
builder.Services.AddDbContextPool<ProjectTemplateDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("ProjectTemplateDB")));

//UoW Registration
builder.Services.AddScoped<IProjectTemplateDBUnitOfWork, ProjectTemplateDBUnitOfWork>();

//Internal Service Registration
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IDbContextChangeTrackingService, DbContextChangeTrackingService>();

// configure DI for application services
//builder.Services.AddScoped<IJwtUtils, JwtUtils>();
//builder.Services.AddScoped<IUserService, UserService>();


/*HTTP REQUEST PIPELINE*/
var app = builder.Build();

app.UseExceptionHandler(errorLogger =>
{
    errorLogger.Run(async context =>
    {
        var scoped = app.Services.CreateScope();
        IErrorLogService errorLogService = scoped.ServiceProvider.GetRequiredService<IErrorLogService>();
        context.Response.StatusCode = 500;
        ErrorMessage unhandledErrorDetail = await errorLogService.LogApiError(context);
        await context.Response.WriteAsync(JsonConvert.SerializeObject(unhandledErrorDetail));
    });
});

app.UseSwagger();
IApiVersionDescriptionProvider apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
        options.RoutePrefix = "docs";
    }
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = ApiHomePageConstant.ContentType;
        await context.Response.WriteAsync(
            string.Format(ApiHomePageConstant.ContentFormat,
            //identityServerApiDefinition.ApiName,
            app.Environment.EnvironmentName,
            context.Request.Scheme,
            context.Request.Host.Value,
            VersionDetail.DisplayVersion()));
    });
});

app.Run();

