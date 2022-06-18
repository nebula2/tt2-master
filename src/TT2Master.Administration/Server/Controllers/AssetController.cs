using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Server.Services;
using TT2MasterAdministrationApp.Shared;

namespace TT2MasterAdministrationApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly ILogger<AssetController> _logger;

        private readonly IAssetService _assetService;

        public AssetController(ILogger<AssetController> logger, IAssetService assetService)
        {
            _logger = logger;
            _assetService = assetService;
        }

        [HttpGet]
        public async Task<IEnumerable<AssetContainer>> Get()
        {
            _logger.LogInformation($"Getting asset containers with latest version from server...");
            return await _assetService.GetAssetContainersWithLatestVersionAsync();
        }

        [HttpGet("{reference}")]
        public async Task<IEnumerable<string>> GetVersions(string reference)
        {
            _logger.LogInformation("Getting versions for {reference}...", reference);
            return await _assetService.GetVersionsStoredInContainerAsync(reference);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AssetContainer container)
        {
            _logger.LogInformation($"requesting asset download from gh to blob storage...");
            var result = await _assetService.DownloadAssetsFromGameHiveAsync(container);

            return result.IsSuccessful ? Ok(container.LatestVersion) : BadRequest();
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> PostAssetUpload(List<AssetUploadItem> items)
        {
            _logger.LogInformation($"requesting asset upload...");
            var result = await _assetService.UploadAssets(items);

            return result.IsSuccessful ? Ok(result) : (IActionResult)BadRequest();
        }

        [HttpGet("[action]/{containerReference}/{version}")]
        public async Task<IActionResult> GetAssetZipFile(string containerReference, string version)
        {
            var assetLocation = await _assetService.DownloadAssetsFromContainerToTempDirAsync(containerReference, version);
            var zipDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Guid.NewGuid().ToString());
            Directory.CreateDirectory(zipDirectory);


            var zipName = $"{containerReference}_{version}.zip";
            var zipLocation = Path.Combine(zipDirectory, zipName);
            ZipFile.CreateFromDirectory(assetLocation, zipLocation);
            Directory.Delete(assetLocation, true);

            // TODO delete zip file in temp dir
            var zReader = System.IO.File.OpenRead(zipLocation);
            return File(zReader, "application/octet-stream", zipName);
        }
    }
}
