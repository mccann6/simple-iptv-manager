using SimpleIptvManager.Components;
using SimpleIptvManager.Components.Clients;
using SimpleIptvManager.Components.Data;
using SimpleIptvManager.Components.Services;
using MudBlazor.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddHttpClient();
        builder.Services.AddMemoryCache();
        builder.Services.AddMudServices();
        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddControllers();

        //builder.Services.AddSingleton<ITvStreamsClient, TvStreamsClientMock>();
        builder.Services.AddSingleton<ITvStreamsClient, TvStreamsClient>();
        builder.Services.AddSingleton<IPlaylistService, PlaylistService>();
        builder.Services.AddSingleton<IPlaylistRepository, PlaylistDbRepository>();
        builder.Services.AddSingleton<IProgramGuideClient, ProgramGuideClient>();
        builder.Services.AddSingleton<IProgramGuideService, ProgramGuideService>();
        builder.Services.AddDbContext<SimpleIptvManagerDbContext>();

        var app = builder.Build();
        app.MapControllers();


        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<SimpleIptvManagerDbContext>();
            context.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}