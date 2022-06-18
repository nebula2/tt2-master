using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Shared;
using TT2Master.Shared.Assets;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;
using TT2Master.Shared.Extensions;
using TT2MasterAdministrationApp.Server.Models;
using TT2MasterAdministrationApp.Shared;
using static TT2MasterAdministrationApp.Server.Services.IAssetService;

namespace TT2MasterAdministrationApp.Server.Services
{
    public class AssetService : IAssetService
    {
        private readonly ILogger<AssetService> _logger;
        private readonly AppSetting _settings;
        private readonly object _objectLock = new();

        public AssetService(ILogger<AssetService> logger, AppSetting settings)
        {
            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        /// Raised when progress made
        /// </summary>
        public event ProgressCarrier OnProgressMade;
        event ProgressCarrier IAssetService.OnProgressMade
        {
            add
            {
                lock (_objectLock)
                {
                    OnProgressMade += value;
                }
            }

            remove
            {
                lock (_objectLock)
                {
                    OnProgressMade -= value;
                }
            }
        }

        #region public functions
        public async Task<InfofileAssetDownloadResult> DownloadAssetsFromGameHiveAsync(AssetContainer container)
        {
            #region check input
            if (!_settings.Containers.Where(x => x.ContainerType == AssetContainerType.Staging && x.Provider == AssetProvider.GameHive)
                    .Select(x => x.ContainerReference).ToList().Contains(container.ContainerReference))
            {
                return new InfofileAssetDownloadResult
                {
                    IsSuccessful = false,
                    Message = "could not find staging container for game hive assets matching given specification",
                };
            }

            if (!Version.TryParse(container.LatestVersion, out var version))
            {
                return new InfofileAssetDownloadResult
                {
                    IsSuccessful = false,
                    Message = "could not parse requested version",
                };
            } 
            #endregion

            if (!await IsVersionExistingOnServerAsync(version, container.ContainerReference))
            {
                _logger.LogInformation($"Getting data for version: {version}");

                var progress = await CreateStagingAssets(container.LatestVersion, container.ContainerReference);
                progress.IsFinished = true;
                progress.Message = "Done";

                return new InfofileAssetDownloadResult
                {
                    DownloadedInfofilesCount = progress.FinishedWorkItemCount,
                    IsSuccessful = true,
                    IsVersionAlreadyExisting = false,
                    Message = "Downloaded new asset version"
                };
            }
            else
            {
                _logger.LogInformation($"Info files are up to date");
                return new InfofileAssetDownloadResult
                {
                    IsSuccessful = true,
                    IsVersionAlreadyExisting = true,
                    Message = "given version already exists in this container",
                };
            }
        }

        public async Task<IEnumerable<string>> GetVersionsStoredInContainerAsync(string containerReference)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(_settings.AzureBlobConString);

            CloudBlobClient client;
            client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container;
            container = client.GetContainerReference(containerReference);

            var dir = await container.ListBlobsSegmentedAsync(null);

            var folders = dir.Results.Where(x => x as CloudBlobDirectory != null).ToList();

            var versions = new List<string>();

            foreach (var item in folders)
            {
                if (Version.TryParse(item.Uri.Segments.Last().Replace("/", ""), out var v))
                {
                    versions.Add(v.ToString());
                }
            }

            return versions.ToArray();
        }

        public async Task<IEnumerable<AssetContainer>> GetAssetContainersWithLatestVersionAsync()
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(_settings.AzureBlobConString);

            CloudBlobClient client;
            client = storageAccount.CreateCloudBlobClient();

            var items = new List<AssetContainer>();

            foreach (var item in _settings.Containers)
            {
                var ac = new AssetContainer()
                {
                    ContainerReference = item.ContainerReference,
                    ContainerType = item.ContainerType,
                    ProductionContainerReferences = item.ProductionContainerReferences,
                    Provider = item.Provider,
                };

                CloudBlobContainer container;
                container = client.GetContainerReference(ac.ContainerReference);

                var dir = await container.ListBlobsSegmentedAsync(null);

                var folders = dir.Results.Where(x => x as CloudBlobDirectory != null).ToList();

                var versions = new List<Version>();

                foreach (var f in folders)
                {
                    if (Version.TryParse(f.Uri.Segments.Last().Replace("/", ""), out var v))
                    {
                        versions.Add(v);
                    }
                }

                ac.LatestVersion = versions.Count > 0 ? versions.Max(x => x).ToString() : null;
                ac.ItemsCount = versions.Count;
                items.Add(ac);
            }

            return items.ToArray();
        }

        /// <summary>
        /// Until we got this to use the unit test projects read assets manually
        /// </summary>
        /// <param name="newContainer"></param>
        /// <returns></returns>
        public async Task<AssetValidationResult> ValidateAssetVersion(AssetContainer newContainer)
        {
            var result = new AssetValidationResult();
            var dirsToDrop = new List<string>();

            OnProgressMade?.Invoke(this, new JfProgressEventArgs
            {
                Message = $"Starting asset validation for {newContainer.ContainerReference}",
            });

            var at = await StoreAssetsFromContainer(newContainer);

            if (at == (null, null))
            {
                return result;
            }

            dirsToDrop.Add(at.assetPath);

            newContainer.Provider = _settings.Containers.Where(x => x.ContainerReference == newContainer.ContainerReference).FirstOrDefault().Provider;

            if (newContainer.Provider == AssetProvider.GameHive)
            {
                var (divAssetPath, _) = await StoreAssetsFromContainer("div");
                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"Loaded div assets from prod for header mapping",
                });

                dirsToDrop.Add(divAssetPath);

                result.IsSuccessful = TestIfCanReadInfoAssets(at.assetPath, divAssetPath);
            }
            else
            {
                result.IsSuccessful = TestIfCanReadDivAssets(at.assetPath);
            }

            // cleanup
            foreach (var item in dirsToDrop)
            {
                if (Directory.Exists(item))
                {
                    Directory.Delete(item, true);
                }
            }

            OnProgressMade?.Invoke(this, new JfProgressEventArgs
            {
                Message = $"Deleted local asset copies",
                WorkloadCount = 1,
                FinishedWorkItemCount = 1,
                IsFinished = true,
            });

            result.IsSuccessful = true;
            return result;
        }


        public async Task<bool> PushAssetsToProduction(AssetContainer source, string targetContainerReference)
        {
            var progress = new JfProgressEventArgs();

            try
            {
                progress.Message = "connecting to azure storage";
                OnProgressMade?.Invoke(this, progress);
                CloudStorageAccount storageAccount;
                storageAccount = CloudStorageAccount.Parse(_settings.AzureBlobConString);

                CloudBlobClient client;
                client = storageAccount.CreateCloudBlobClient();

                progress.Message = "getting containers";
                OnProgressMade?.Invoke(this, progress);
                var sourceContainer = client.GetContainerReference(source.ContainerReference);
                var targetContainer = client.GetContainerReference(targetContainerReference);

                progress.Message = "getting directories";
                OnProgressMade?.Invoke(this, progress);
                var sourceDirectory = sourceContainer.GetDirectoryReference(source.LatestVersion);
                var sourceItems = (await sourceDirectory.ListBlobsSegmentedAsync(null)).Results;

                progress.WorkloadCount = sourceItems.Count();
                progress.Message = $"Received workload ({progress.WorkloadCount})";
                OnProgressMade?.Invoke(this, progress);

                foreach (var item in sourceItems)
                {
                    string filename = $"{source.LatestVersion}\\{item.Uri.Segments.Last()}";
                    var sourceBlob = sourceContainer.GetBlockBlobReference(filename);
                    var targetBlob = targetContainer.GetBlockBlobReference(filename);
                    await targetBlob.StartCopyAsync(sourceBlob);
                    
                    progress.FinishedWorkItemCount++;
                    progress.Message = $"StartCopyAsync for {filename}";
                    OnProgressMade?.Invoke(this, progress);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PushAssetsToProduction ERROR");
                progress.Message = $"Err: {ex.Message}";
                OnProgressMade?.Invoke(this, progress);
                return false;
            }
        }

        public async Task<string> DownloadAssetsFromContainerToTempDirAsync(string containerReference, string version)
        {
            var at = await DownloadVersionContentFromContainerAsync(new AssetContainer
            {
                ContainerReference = containerReference,
                LatestVersion = version,
            });

            if (at == null)
            {
                return null;
            }

            string destinationPath = SaveAssetListLocally(at);

            if (string.IsNullOrEmpty(destinationPath))
            {
                return null;
            }

            DownloadAssets(at, destinationPath);

            return destinationPath;
        }

        public async Task<InfofileAssetDownloadResult> UploadAssets(List<AssetUploadItem> items)
        {
            var progress = new JfProgressEventArgs
            {
                WorkloadCount = items.Count,
                Message = "processing files"
            };
            OnProgressMade?.Invoke(this, progress);

            foreach (var item in items)
            {
                string filename = $"{item.Version}\\{item.FileName}";
                var stream = new MemoryStream(item.FileContent);
                await CreateBlobAsync(filename, stream, item.ContainerReference);

                progress.FinishedWorkItemCount++;
                progress.Message = $"Uploaded {item.FileName}";
                OnProgressMade?.Invoke(this, progress);
            }

            return new InfofileAssetDownloadResult
            {
                DownloadedInfofilesCount = progress.FinishedWorkItemCount,
                IsSuccessful = true,
                Message = "Uploaded",
            };
        }
        #endregion

        #region private methods
        #region Asset testing
        private bool TestIfCanReadInfoAssets(string assetPath, string divAssetPath)
        {
            try
            {
                var progress = new JfProgressEventArgs { WorkloadCount = 16 };

                var result = AssetHandler.GetMappedEntitiesFromCsvFile<AssetNameMapping, AssetNameMappingMap>(Path.Combine(divAssetPath, "assetNameMapping.csv"));
                AssetMapNameProvider.AddAdditionalMappings(result);
                if (result.Count == 0)
                {
                    progress.Message = "got no AssetNameMappings";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "AssetNameMappings OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<RaidLevelInfo, RaidLevelInfoMap>(Path.Combine(assetPath, "RaidLevelInfo.csv")).Count == 0)
                {
                    progress.Message = "got no RaidLevelInfo";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "RaidLevelInfo OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<RaidEnemyInfo, RaidEnemyInfoMap>(Path.Combine(assetPath, "RaidEnemyInfo.csv")).Count == 0)
                {
                    progress.Message = "got no RaidEnemyInfo";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "RaidEnemyInfo OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<RaidAreaInfo, RaidAreaInfoMap>(Path.Combine(assetPath, "RaidAreaInfo.csv")).Count == 0)
                {
                    progress.Message = "got no RaidAreaInfo";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "RaidAreaInfo OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<Artifact, ArtifactMap>(Path.Combine(assetPath, "ArtifactInfo.csv")).Count == 0)
                {
                    progress.Message = "got no Artifact";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "Artifact OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<Equipment, EquipmentMap>(Path.Combine(assetPath, "EquipmentInfo.csv")).Count == 0)
                {
                    progress.Message = "got no Equipment";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "Equipment OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<ActiveSkill, ActiveSkillMap>(Path.Combine(assetPath, "ActiveSkillInfo.csv")).Count == 0)
                {
                    progress.Message = "got no ActiveSkill";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "ActiveSkill OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<ArtifactCost, ArtifactCostMap>(Path.Combine(assetPath, "ArtifactCostInfo.csv")).Count == 0)
                {
                    progress.Message = "got no ArtifactCost";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "ArtifactCost OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<ClanTrait, ClanTraitMap>(Path.Combine(assetPath, "RaidClanInfo.csv"), ClanTraitMap.GetSkipExpression(), true).Count == 0)
                {
                    progress.Message = "got no ClanTrait";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "ClanTrait OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<PassiveSkill, PassiveSkillMap>(Path.Combine(assetPath, "PassiveSkillInfo.csv")).Count == 0)
                {
                    progress.Message = "got no PassiveSkill";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "PassiveSkill OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<Pet, PetMap>(Path.Combine(assetPath, "PetInfo.csv")).Count == 0)
                {
                    progress.Message = "got no Pet";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "Pet OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<RaidCard, RaidCardMap>(Path.Combine(assetPath, "RaidSkillInfo.csv")).Count == 0)
                {
                    progress.Message = "got no RaidCard";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "RaidCard OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<Skill, SkillMap>(Path.Combine(assetPath, "SkillTreeInfo2.0.csv")).Count == 0)
                {
                    progress.Message = "got no Skill";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "Skill OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<EquipEnhanceCombo, EquipEnhanceComboMap>(Path.Combine(assetPath, "EquipmentEnhancementComboInfo.csv")).Count == 0)
                {
                    progress.Message = "got no EquipEnhanceCombo";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "EquipEnhanceCombo OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<EquipEnhScalingInfo, EquipEnhScalingInfoMap>(Path.Combine(assetPath, "EquipmentEnhancementScalingInfo.csv")).Count == 0)
                {
                    progress.Message = "got no EquipEnhScalingInfo";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "EquipEnhScalingInfo OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                if (AssetHandler.GetMappedEntitiesFromCsvFile<EquipmentSet, EquipmentSetMap>(Path.Combine(assetPath, "EquipmentSetInfo.csv")).Count == 0)
                {
                    progress.Message = "got no EquipmentSet";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "EquipmentSet OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                return true;
            }
            catch (Exception ex)
            {
                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"TestIfCanReadAssets ERROR {ex.Message}",
                    IsFinished = true,
                });
                return false;
            }
        }

        private bool TestIfCanReadDivAssets(string divAssetPath)
        {
            try
            {
                var progress = new JfProgressEventArgs { WorkloadCount = 6 };

                var result = AssetHandler.GetMappedEntitiesFromCsvFile<AssetNameMapping, AssetNameMappingMap>(Path.Combine(divAssetPath, "assetNameMapping.csv"));
                AssetMapNameProvider.AddAdditionalMappings(result);
                if (result.Count == 0)
                {
                    progress.Message = "got no AssetNameMappings";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "AssetNameMappings OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                if (AssetHandler.GetMappedEntitiesFromCsvFile<DefaultArtifactBuildEntry, DefaultArtifactBuildEntryMap>(Path.Combine(divAssetPath, "defaultBuilds.csv")).Count == 0)
                {
                    progress.Message = "got no DefaultArtifactBuildEntry";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "DefaultArtifactBuildEntry OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                if (AssetHandler.GetMappedEntitiesFromCsvFile<EquipmentReduction, EquipmentReductionMap>(Path.Combine(divAssetPath, "equipmentReductions.csv")).Count == 0)
                {
                    progress.Message = "got no EquipmentReduction";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "EquipmentReduction OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                if (AssetHandler.GetMappedEntitiesFromCsvFile<EquipmentSetMapping, EquipmentSetMappingMap>(Path.Combine(divAssetPath, "equipSetMapping.csv")).Count == 0)
                {
                    progress.Message = "got no EquipmentSetMapping";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "EquipmentSetMapping OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                if (AssetHandler.GetMappedEntitiesFromCsvFile<OfficialStore, OfficialStoreMap>(Path.Combine(divAssetPath, "officialStores.csv")).Count == 0)
                {
                    progress.Message = "got no OfficialStore";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "OfficialStore OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                if (AssetHandler.GetMappedEntitiesFromCsvFile<SPSkillReduction, SPSkillReductionMap>(Path.Combine(divAssetPath, "skillReductions.csv")).Count == 0)
                {
                    progress.Message = "got no SPSkillReduction";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }
                else
                {
                    progress.Message = "SPSkillReduction OK";
                    progress.FinishedWorkItemCount++;
                    OnProgressMade?.Invoke(this, progress);
                }

                return true;
            }
            catch (Exception ex)
            {
                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"TestIfCanReadDivAssets ERROR {ex.Message}",
                    IsFinished = true,
                });
                return false;
            }
        }
        #endregion

        #region asset download
        private async Task<(string assetPath, JfProgressEventArgs progress)> StoreAssetsFromContainer(string container)
        {
            return await StoreAssetsFromContainer(new AssetContainer
            {
                ContainerReference = container,
                LatestVersion = (await GetLatestAssetVersionAsync(container)).ToString(),
            });
        }

        private async Task<Version> GetLatestAssetVersionAsync(string container)
        {
            var result = new Version();

            try
            {
                var at = new AssetType
                {
                    AzureContainer = container,
                    Identifier = "InfoFiles",
                };

                var content = new StringContent(JsonConvert.SerializeObject(at), Encoding.UTF8, "application/json");

                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };
                using var resonse = await client.PostAsync(_settings.PostLatestAssetVersionUrl, content);
                using var respContent = resonse.Content;
                string tr = await respContent.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Version>(tr);
            }
            catch (Exception ex)
            {
                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"Something went wrong getting latest asset version for container {container} ({ex.Message})",
                    IsFinished = true,
                });
                return null;
            }

            return result;
        }

        private async Task<(string assetPath, JfProgressEventArgs progress)> StoreAssetsFromContainer(AssetContainer container)
        {
            var at = await DownloadVersionContentFromContainerAsync(container);

            if (at == null)
            {
                return (null, null);
            }

            OnProgressMade?.Invoke(this, new JfProgressEventArgs
            {
                Message = $"received workload size",
                WorkloadCount = at.Assets.Count,
                FinishedWorkItemCount = 0,
            });

            string destinationPath = SaveAssetListLocally(at);

            if (string.IsNullOrEmpty(destinationPath))
            {
                return (null, null);
            }

            var (_, progress) = DownloadAssets(at, destinationPath);

            return (destinationPath, progress);
        }

        private async Task<AssetType> DownloadVersionContentFromContainerAsync(AssetContainer container)
        {
            try
            {
                var at = new AssetType
                {
                    Assets = new List<Uri>(),
                    AzureContainer = container.ContainerReference,
                    CurrentVersion = container.LatestVersion,
                    //StoredVersion = container.LatestVersion,
                    Identifier = "InfoFiles",
                };

                var content = new StringContent(JsonConvert.SerializeObject(at), Encoding.UTF8, "application/json");

                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };
                using var resonse = await client.PostAsync(_settings.PostAssetVersionCheckUrl, content);
                using var respContent = resonse.Content;
                string tr = await respContent.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AssetType>(tr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DownloadAssetsFromContainer Exception");

                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"Something went wrong getting version details :/ ({ex.Message})",
                    IsFinished = true,
                });

                return null;
            }
        }

        private string SaveAssetListLocally(AssetType at)
        {
            string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Guid.NewGuid().ToString());

            OnProgressMade?.Invoke(this, new JfProgressEventArgs
            {
                Message = $"Asset stuff for {at.AzureContainer} will be saved under: ({parentDir})",
            });

            try
            {
                if (!Directory.Exists(parentDir))
                {
                    Directory.CreateDirectory(parentDir);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveAssetListLocally Directory Exception");
                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"Something went wrong creating the parent directory ({ex.Message})",
                    IsFinished = true,
                });
                return null;
            }

            string assetsListString = "";

            foreach (var item in at.Assets)
            {
                assetsListString += $"{item.Segments.Last()}\n";
            }

            try
            {
                string destinationFilename = Path.Combine(parentDir, $"{at.AzureContainer}.txt");

                if (File.Exists(destinationFilename))
                {
                    File.Delete(destinationFilename);
                }

                File.WriteAllText(destinationFilename, assetsListString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveAssetListLocally File Exception");
                OnProgressMade?.Invoke(this, new JfProgressEventArgs
                {
                    Message = $"Something went wrong creating the asset list ({ex.Message})",
                    IsFinished = true,
                });
                return null;
            }

            return parentDir;
        }

        private (AssetDownloadResult dResult, JfProgressEventArgs progress) DownloadAssets(AssetType at, string location)
        {
            var progressItem = new JfProgressEventArgs
            {
                WorkloadCount = at.Assets.Count,
                FinishedWorkItemCount = 0,
            };

            foreach (var item in at.Assets)
            {
                string nameLocal = item.Segments.Last();

                string filename = Path.Combine(location, nameLocal);

                ServicePointManager.ServerCertificateValidationCallback = 
                    new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

                using var client = new WebClient();
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }

                    client.DownloadFile(item, filename);

                    progressItem.FinishedWorkItemCount++;
                    progressItem.Message = $"Downloaded {item}";
                    OnProgressMade?.Invoke(this, progressItem);
                }
                catch (Exception ex)
                {
                    progressItem.Message = $"Error downloading asset file {item} ({ex.Message})";
                    OnProgressMade?.Invoke(this, progressItem);
                    return (AssetDownloadResult.TotalFuckUp, progressItem);
                }
            }

            return (AssetDownloadResult.SuccessfulAssetDownload, progressItem);
        } 
        #endregion

        /// <summary>
        /// Checks if version exists on server
        /// </summary>
        /// <param name="version">version to check for</param>
        /// <param name="containerName">name of container</param>
        /// <returns></returns>
        private async Task<bool> IsVersionExistingOnServerAsync(Version version, string containerName)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(_settings.AzureBlobConString);

            CloudBlobClient client;
            client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container;
            container = client.GetContainerReference(containerName);

            var dir = container.GetDirectoryReference(version.ToString());

            var items = await dir.ListBlobsSegmentedAsync(null);

            return items.Results.Any();
        }

        /// <summary>
        /// Gets all info files from GameHive servers and stores them in staging blob storage
        /// </summary>
        /// <param name="version">version to get infofiles for</param>
        /// <returns>true if successful else false</returns>
        private async Task<JfProgressEventArgs> CreateStagingAssets(string version, string container)
        {
            var ttApi = GetFilledServerApi();
            ttApi.Initialize();

            var workload = (InfoFileEnum[])Enum.GetValues(typeof(InfoFileEnum));

            var progress = new JfProgressEventArgs
            {
                IsFinished = false,
                WorkloadCount = workload.Length,
                FinishedWorkItemCount = 0,
                Message = "Initialized workload",
            };

            OnProgressMade?.Invoke(this, progress);

            foreach (var item in workload)
            {
                try
                {
                    //Get content
                    string content = await ttApi.GetInfoFile(item);

                    //write content
                    string itemStr = item.GetDescription();
                    string filename = $"{version}\\{itemStr}.csv";

                    // convert string to stream
                    byte[] byteArray = Encoding.UTF8.GetBytes(content);
                    var stream = new MemoryStream(byteArray);
                    await CreateBlobAsync(filename, stream, container);

                    progress.FinishedWorkItemCount++;
                    progress.Message = $"Downloaded {itemStr}";
                    OnProgressMade?.Invoke(this, progress);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error {DateTime.Now}: {ex.Message}");
                    throw new InfoUpdateFailureExeption($" Error {DateTime.Now}: {ex.Message}");
                }
            }

            return progress;
        }

        /// <summary>
        /// Creates and uploads a blob
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="name">filename including directory</param>
        /// <param name="data">content to write</param>
        /// <param name="containerName">name of container</param>
        /// <returns></returns>
        private async Task CreateBlobAsync(string name, Stream data, string containerName)
        {
            CloudStorageAccount storageAccount;
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            storageAccount = CloudStorageAccount.Parse(_settings.AzureBlobConString);

            client = storageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                await container.CreateAsync();
            }

            blob = container.GetBlockBlobReference(name);

            await blob.UploadFromStreamAsync(data);
        }

        /// <summary>
        /// Returns a filled ServerAPI object. you need to init this afterwards
        /// </summary>
        /// <returns></returns>
        private TT2ServerAPI GetFilledServerApi()
        {
            string adId = _settings.AdId;
            string authToken = _settings.AuthToken;
            string playerId = _settings.PlayerId;

            return new TT2ServerAPI(adId, authToken, playerId);
        } 
        #endregion
    }
}
