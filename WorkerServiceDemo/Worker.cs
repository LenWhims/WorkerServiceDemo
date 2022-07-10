using Microsoft.Extensions.Options;

namespace WorkerServiceDemo
{
    public class Worker : BackgroundService
    {
        
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _folderWatcher;
        private readonly string _origFolder;
        private readonly string _desFolder;
        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _origFolder = settings.Value.OriginFolder;
            _desFolder = settings.Value.DestinationFolder;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        
            try
            {
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError("Service failed at: {time} with error: {err}", DateTimeOffset.Now, e.Message);
            }
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Started at: {time}. Version: {version}", DateTimeOffset.Now, System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version);
            _logger.LogInformation("Service Starting");
            if (!Directory.Exists(_origFolder))
            {
                _logger.LogWarning($"Origin Folder [{_origFolder}] not found.");
                _logger.LogInformation(MicroServices.CreateFolder(_origFolder).Result);
                
            }
            _logger.LogInformation($"Binding Events from Origin Folder: {_origFolder}");
            if (!Directory.Exists(_desFolder))
            {
                _logger.LogWarning($"Destination Folder [{_desFolder}] not found.");
                _logger.LogInformation(MicroServices.CreateFolder(_desFolder).Result);

            }
            _folderWatcher = new FileSystemWatcher(_origFolder)
            {
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                  NotifyFilters.DirectoryName
            };
            _folderWatcher.Created += Input_OnChanged;
            _folderWatcher.EnableRaisingEvents = true;

            await base.StartAsync(cancellationToken);
        }
        protected async void Input_OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                _logger.LogInformation($"InBound Change Event Triggered by [{e.FullPath}]");

                // Move file from origin folder to destination folder
                _logger.LogInformation(MicroServices.MoveFile(_origFolder, _desFolder, e.Name).Result);
                await Task.CompletedTask;
                _logger.LogInformation("Done with Inbound Change Event");
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");
            _folderWatcher.EnableRaisingEvents = false;
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Service");
            _folderWatcher.Dispose();
            base.Dispose();
        }


    }
}