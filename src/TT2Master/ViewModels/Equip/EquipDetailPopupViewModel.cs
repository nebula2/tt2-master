using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master
{
    public class EquipDetailPopupViewModel : ViewModelBase
    {
        #region Member
        private Equipment _thisEquip;
        public Equipment ThisEquip { get => _thisEquip; set => SetProperty(ref _thisEquip, value); }

        private readonly INavigationService _navigationService; 
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public EquipDetailPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
        }
        #endregion

        #region CommandMethods

        #endregion

        #region Helper

        #endregion

        #region Override

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            ThisEquip = string.IsNullOrEmpty((string)parameters["item"])
                ? new Equipment()
                : EquipmentHandler.OwnedEquipment.Where(x => x.UniqueId.ToString() == (string)parameters["item"]).FirstOrDefault();

            OnPropertyChanged(new PropertyChangedEventArgs("ThisEquip"));

            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}