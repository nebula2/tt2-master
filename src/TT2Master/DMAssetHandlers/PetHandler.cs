using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Pets
{
    /// <summary>
    /// Handles pet related stuff
    /// </summary>
    public class PetHandler
    {
        /// <summary>
        /// List of pets
        /// </summary>
        public static List<Pet> Pets { get; set; } = new List<Pet>();

        /// <summary>
        /// List of available pet images in this app
        /// </summary>
        public static List<string> Images { get; private set; } = new List<string>()
        {
            "Pet1",
            "Pet2",
            "Pet3",
            "Pet4",
            "Pet5",
            "Pet6",
            "Pet7",
            "Pet8",
            "Pet9",
            "Pet10",
            "Pet11",
            "Pet12",
            "Pet13",
            "Pet14",
            "Pet15",
            "Pet16",
            "Pet17",
            "Pet18",
            "Pet19",
            "Pet20",
            "Pet21",
            "Pet22",
            "Pet23",
            "Pet24",
            "Pet25",
            "Pet26",
            "Pet27",
            "Pet28",
            "Pet29",
            "Pet30",
        };

        /// <summary>
        /// Loads pet information from info file
        /// </summary>
        /// <returns></returns>
        public static bool LoadPetsFromInfofile()
        {
            try
            {
                OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs("PetHandler.LoadPetsFromInfofile"));

                Pets = AssetReader.GetInfoFile<Pet, PetMap>(InfoFileEnum.PetInfo);

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: Exception at LoadPetsFromInfofile: {ex.Message}\n\n {ex.Data}\n\n"));
                OnProblemHaving?.Invoke("PetHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }

        public static bool FillPets() => LocalSettingsORM.IsReadingDataFromSavefile
            ? FillPetsFromSavefile()
            : FillPetsFromClipboard();

        /// <summary>
        /// Fill pet information from clipboard export
        /// </summary>
        /// <returns></returns>
        private static bool FillPetsFromClipboard() 
        { 
            OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: FillPetsFromClipboard()"));

            if (App.Save.PetModel == null)
            {
                OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: FillPetsFromClipboard() PetModel is null"));
                return false;
            }

            foreach (var item in Pets)
            {
                try
                {
                    item.Level = JfTypeConverter.ForceInt(App.Save.PetModel[item.PetName].ToString());
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: FillPetsFromClipboard() -> Error {ex.Message}\n\n{ex.Data}"));
                    OnProblemHaving?.Invoke("PetHandler", new CustErrorEventArgs(ex));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Fills pet information from save file
        /// </summary>
        /// <returns></returns>
        private static bool FillPetsFromSavefile()
        {
            OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: FillPetsFromSavefile()"));

            var allPets = (JObject)App.Save.PetModel["allPetLevels"];

            try
            {
                foreach (var token in allPets)
                {
                    if (token.Key == "$type")
                    {
                        continue;
                    }

                    var pet = Pets.Where(x => x.PetId == token.Key).FirstOrDefault();

                    if (pet == null)
                    {
                        OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: pet is null"));
                        return false;
                    }

                    pet.Level = JfTypeConverter.ForceInt(token.Value);
                    OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: filled level for pet {pet.PetName} with {pet.Level}"));

                    pet.IsEquipped = pet.PetId == App.Save.PetModel["currentPet"]["$content"].ToString();
                    OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: is equipped: {pet.IsEquipped}"));
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("PetHandler", new InformationEventArgs($"PetHandler: FillPetsFromSavefile() -> Error {ex.Message}\n\n{ex.Data}"));
                OnProblemHaving?.Invoke("PetHandler", new CustErrorEventArgs(ex));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns image path for given pet id ready to use in Xamarin.Forms UI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetImagePathForCardId(string id) => Images.Contains(id) ? $"{id}" : "notfound";
        /// <summary>
        /// Returns image path for given pet id ready to use in SkiaSharp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetImagePathForDrawerId(string id) => Images.Contains(id) ? id : "notfound";

        #region events and delegates
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}