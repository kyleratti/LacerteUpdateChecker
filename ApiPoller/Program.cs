using ApiPoller;
using ApiPoller.Config;
using ApiPoller.Services;

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(ConfigureServices)
	.Build();

await host.RunAsync();

void ConfigureServices(IServiceCollection services)
{
	services.AddHostedService<PollWorker>();

	services.AddSingleton<GetUpdateManifestActionSettings>();
	services.AddSingleton<LacerteApiSettings>();
	services.AddSingleton<SendGridSettings>();
	services.AddSingleton<NotificationSettings>();
	services.AddSingleton<Emailer>();
}