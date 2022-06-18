using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using SQLite;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Information
{
    [Table("DBANNOUNCEMENT")]
    public class DbAnnouncement : BindableBase
    {
        private int _id;
        [PrimaryKey]
        public int ID { get => _id; set => SetProperty(ref _id, value); }

        private ClientOs _os;
        public ClientOs Os { get => _os; set => SetProperty(ref _os, value); }

        private int _appVersionMin;
        public int AppVersionMin { get => _appVersionMin; set => SetProperty(ref _appVersionMin, value); }

        private int _appVersionMax;
        public int AppVersionMax { get => _appVersionMax; set => SetProperty(ref _appVersionMax, value); }

        private string _header;
        public string Header { get => _header; set => SetProperty(ref _header, value); }

        private string _body;
        public string Body { get => _body; set => SetProperty(ref _body, value); }

        private bool _isUpdateRequired;
        public bool IsUpdateRequired { get => _isUpdateRequired; set => SetProperty(ref _isUpdateRequired, value); }
        
        private bool _isSeen;
        public bool IsSeen { get => _isSeen; set => SetProperty(ref _isSeen, value); }
    }
}
