using Hangfire;
using Flsurf.Infrastructure;
using Flsurf.Presentation.Web;
using Flsurf.Domain;
using Flsurf.Application;
using Flsurf.Infrastructure.Data;
using Flsurf.Infrastructure.EventStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddWebServices(builder.Configuration, builder.Environment, builder.Logging); 
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NameApp API");
        options.DefaultModelsExpandDepth(-1);
    });
    await app.InitialiseDatabaseAsync();
    await app.InitialiseEventStoreDatabaseAsync(); 
}
app.UseEventDispatcher(); 
app.UseStaticFiles();
app.MapDefaultControllerRoute();

app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.UseHangfireDashboard();
app.UseAuthorization();
app.UseAuthentication();
app.UseWebSockets(new WebSocketOptions() { 
    KeepAliveInterval = TimeSpan.FromMinutes(2), 
});
app.UseCors("TaskManger");
app.MapControllers();

app.Run();

// for testing! 
public partial class Program { }