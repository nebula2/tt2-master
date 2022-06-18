using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Equip
{
    /// <summary>
    /// Optimizes Equipment
    /// </summary>
    public class EquipOptimizer
    {
        #region Members and Properties
        public EquipAdvSettings CurrentSettings { get; set; }

        /// <summary>
        /// All Equipment I own
        /// </summary>
        public List<Equipment> MyEquipment { get; set; } = new List<Equipment>();

        /// <summary>
        /// Sword-Equip
        /// </summary>
        public List<Equipment> MySwords { get; set; } = new List<Equipment>();

        /// <summary>
        /// Chest-Equip
        /// </summary>
        public List<Equipment> MyChests { get; set; } = new List<Equipment>();

        /// <summary>
        /// Hat-Equip
        /// </summary>
        public List<Equipment> MyHats { get; set; } = new List<Equipment>();

        /// <summary>
        /// Aura-Equip
        /// </summary>
        public List<Equipment> MyAuras { get; set; } = new List<Equipment>();

        /// <summary>
        /// Aura-Equip
        /// </summary>
        public List<Equipment> MySlashs { get; set; } = new List<Equipment>();

        private readonly DBRepository _dbRepo;
        private readonly SaveFile _save;
        #endregion

        #region Private methods
        /// <summary>
        /// Sets efficiency values for my equipment
        /// </summary>
        private void SetEfficiencyValues()
        {
            //make this real
            foreach (var item in MyEquipment)
            {
                item.SetEfficiency(CurrentSettings);
                item.SetVisualProperties();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EquipOptimizer(DBRepository dbRepo, SaveFile save)
        {
            _dbRepo = dbRepo;
            _save = save;
        }

        /// <summary>
        /// Populates lists in each equipment category ordered by efficiency
        /// </summary>
        private void BuildLists()
        {
            MySwords = MyEquipment.Where(x => x.EquipmentCategory == "Weapon" && !x.Hidden).OrderByDescending(n => n.EfficiencyValue).ToList();
            MyChests = MyEquipment.Where(x => x.EquipmentCategory == "Suit" && !x.Hidden).OrderByDescending(n => n.EfficiencyValue).ToList();
            MyHats = MyEquipment.Where(x => x.EquipmentCategory == "Hat" && !x.Hidden).OrderByDescending(n => n.EfficiencyValue).ToList();
            MyAuras = MyEquipment.Where(x => x.EquipmentCategory == "Aura" && !x.Hidden).OrderByDescending(n => n.EfficiencyValue).ToList();
            MySlashs = MyEquipment.Where(x => x.EquipmentCategory == "Slash" && !x.Hidden).OrderByDescending(n => n.EfficiencyValue).ToList();
        }

        public async Task<bool> ReloadList(string id = "1")
        {
            #region Load Settings

            CurrentSettings = await _dbRepo.GetEquipAdvSettingsByID(string.IsNullOrWhiteSpace(id) ? "1" : id);

            if (CurrentSettings == null)
            {
                OnLogMePlease?.Invoke("EquipOptimizer", new InformationEventArgs("EquipAdvisorViewModel.ReloadList: settings not found. Creating new"));
                CurrentSettings = new EquipAdvSettings();
                int setSaved = await _dbRepo.AddEquipAdvSettingsAsync(CurrentSettings);
            }
            else
            {
                OnLogMePlease?.Invoke("EquipOptimizer", new InformationEventArgs($"EquipAdvisorViewModel.ReloadList: found current settings -> {CurrentSettings.ToString()}"));
            }

            #endregion

            EquipmentHandler.OnLogMePlease += EquipmentHandler_OnLogMePlease;

            bool saveReloaded = await _save.Initialize(loadPlayer: false, loadAccountModel: false, loadArtifacts: false, loadSkills: false, loadClan: false, loadTournament: false);

            EquipmentHandler.FillEquipment(_save);

            MyEquipment = EquipmentHandler.OwnedEquipment;

            bool setsLoaded = EquipmentHandler.LoadSetInformation(_save);

            SetEfficiencyValues();

            BuildLists();

            return true;
        }
        #endregion

        #region E + D
        private void EquipmentHandler_OnLogMePlease(string message) 
        {
            OnLogMePlease?.Invoke("EquipOptimizer", new InformationEventArgs(message));
        }

        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void LogCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> has something to log
        /// </summary>
        public static event LogCarrier OnLogMePlease;
        #endregion
    }
}