using Prism.Mvvm;
using System;
using TT2Master.Loggers;
using TT2Master.Model.Clan;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// Result of a Raid Attack
    /// </summary>
    public class RaidResult : BindableBase
    {
        #region Properties

        #region Basics
        private string _version = "?";
        /// <summary>
        /// Version
        /// </summary>
        [Obsolete()]
        public string Version { get => _version; set => SetProperty(ref _version, value); }

        private double _totalDamage;
        /// <summary>
        /// Total damage dealt
        /// </summary>
        public double TotalDamage { get => _totalDamage; set => SetProperty(ref _totalDamage, value); }

        private int _taps;
        /// <summary>
        /// Amount of taps
        /// </summary>
        public int Taps { get => _taps; set => SetProperty(ref _taps, value); }

        private double _tapDamage;
        /// <summary>
        /// Amount of damage dealt by taps
        /// </summary>
        public double TapDamage { get => _tapDamage; set => SetProperty(ref _tapDamage, value); }

        private int _tier;
        /// <summary>
        /// Tier
        /// </summary>
        public int Tier { get => _tier; set => SetProperty(ref _tier, value); }

        private int _zone;
        /// <summary>
        /// Zone
        /// </summary>
        public int Zone { get => _zone; set => SetProperty(ref _zone, value); }

        private string _playerName;
        /// <summary>
        /// Player name
        /// </summary>
        public string PlayerName { get => _playerName; set => SetProperty(ref _playerName, value); }

        private int _totalCardLevel;
        /// <summary>
        /// Total card level
        /// </summary>
        public int TotalCardLevel { get => _totalCardLevel; set => SetProperty(ref _totalCardLevel, value); }
        #endregion

        #region Cards
        private string _firstCardName;
        /// <summary>
        /// Name of first card
        /// </summary>
        public string FirstCardName { get => _firstCardName; set => SetProperty(ref _firstCardName, value); }

        private double _firstCardDamage;
        /// <summary>
        /// Damage dealt by first card
        /// </summary>
        public double FirstCardDamage { get => _firstCardDamage; set => SetProperty(ref _firstCardDamage, value); }

        private int _firstCardLevel;
        /// <summary>
        /// Level of first card
        /// </summary>
        public int FirstCardLevel { get => _firstCardLevel; set => SetProperty(ref _firstCardLevel, value); }

        private string _firstCardImage;
        /// <summary>
        /// Image of first card
        /// </summary>
        public string FirstCardImage { get => _firstCardImage; set => SetProperty(ref _firstCardImage, value); }

        private string _secondCardName;
        /// <summary>
        /// Name of second card
        /// </summary>
        public string SecondCardName { get => _secondCardName; set => SetProperty(ref _secondCardName, value); }

        private double _secondCardDamage;
        /// <summary>
        /// Damage dealt by second card
        /// </summary>
        public double SecondCardDamage { get => _secondCardDamage; set => SetProperty(ref _secondCardDamage, value); }

        private int _secondCardLevel;
        /// <summary>
        /// Level of second card
        /// </summary>
        public int SecondCardLevel { get => _secondCardLevel; set => SetProperty(ref _secondCardLevel, value); }

        private string _secondCardImage;
        /// <summary>
        /// Image of second card
        /// </summary>
        public string SecondCardImage { get => _secondCardImage; set => SetProperty(ref _secondCardImage, value); }

        private string _thirdCardName;
        /// <summary>
        /// Name of third card
        /// </summary>
        public string ThirdCardName { get => _thirdCardName; set => SetProperty(ref _thirdCardName, value); }

        private double _thirdCardDamage;
        /// <summary>
        /// Damage dealt with third card
        /// </summary>
        public double ThirdCardDamage { get => _thirdCardDamage; set => SetProperty(ref _thirdCardDamage, value); }

        private int _thirdCardLevel;
        /// <summary>
        /// Level of third card
        /// </summary>
        public int ThirdCardLevel { get => _thirdCardLevel; set => SetProperty(ref _thirdCardLevel, value); }

        private string _thirdCardImage;
        /// <summary>
        /// Image of third card
        /// </summary>
        public string ThirdCardImage { get => _thirdCardImage; set => SetProperty(ref _thirdCardImage, value); }

        /// <summary>
        /// Image for tap damage
        /// </summary>
        public string TapDamageImage { get; set; } = "TapDamage";
        #endregion

        #endregion

        #region private Methods
        /// <summary>
        /// Sets the card images with help of the <see cref="RaidCardHandler"/>
        /// </summary>
        private void SetCardImages()
        {
            FirstCardImage = RaidCardHandler.GetImagePathForCardId(FirstCardName ?? "");
            SecondCardImage = RaidCardHandler.GetImagePathForCardId(SecondCardName ?? "");
            ThirdCardImage = RaidCardHandler.GetImagePathForCardId(ThirdCardName ?? "");
        }
        #endregion

        #region ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="msg"></param>
        public RaidResult(ClanMessage msg)
        {
            try
            {
                string[] content = msg.Message.Split(',');

                // Version = content[0]; // obsolete
                TotalDamage = JfTypeConverter.ForceDoubleUniversal(content[1]);
                Taps = JfTypeConverter.ForceInt(content[2]);
                TapDamage = JfTypeConverter.ForceDoubleUniversal(content[3]);
                Tier = JfTypeConverter.ForceInt(content[4]);
                Zone = JfTypeConverter.ForceInt(content[5]);
                PlayerName = content[6];
                TotalCardLevel = JfTypeConverter.ForceInt(content[7]);

                string[] cardOne = content[11].Split(':');
                FirstCardName = cardOne[0];
                FirstCardDamage = JfTypeConverter.ForceDoubleUniversal(cardOne[1]);
                FirstCardLevel = JfTypeConverter.ForceInt(cardOne[2]);

                string[] cardTwo = content[12].Split(':');
                SecondCardName = cardTwo[0];
                SecondCardDamage = JfTypeConverter.ForceDoubleUniversal(cardTwo[1]);
                SecondCardLevel = JfTypeConverter.ForceInt(cardTwo[2]);

                string[] cardThree = content[13].Split(':');
                ThirdCardName = cardThree[0];
                ThirdCardDamage = JfTypeConverter.ForceDoubleUniversal(cardThree[1]);
                ThirdCardLevel = JfTypeConverter.ForceInt(cardThree[2]);
            }
            catch (Exception)
            {
                //Logger.WriteToLogFile($"RaidResult ex: {ex.Message}");
            }

            SetCardImages();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public RaidResult()
        {
            PlayerName = "?";
            FirstCardName = "?";
            SecondCardName = "?";
            ThirdCardName = "?";
        }
        #endregion
    }
}
