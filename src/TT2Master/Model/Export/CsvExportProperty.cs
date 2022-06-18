using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TT2Master
{
    [Table("CSVEXPORTPROPERTY")]
    public class CsvExportProperty : BindableBase
    {
        [PrimaryKey]
        public string Identifier { get; set; }

        private string _exportReference = "";
        public string ExportReference { get => _exportReference;
            set
            {
                SetProperty(ref _exportReference, value);
                SetIdentifier(value, ID);
            }
        }

        private string _iD = "";
        public string ID
        {
            get => _iD; set
            {
                SetProperty(ref _iD, value);
                SetIdentifier(ExportReference, value);
            }
        }

        private string _displayName = "";
        [SQLite.Ignore]
        public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value); }

        private int _sortId = 0;
        public int SortId { get => _sortId; set => SetProperty(ref _sortId, value); }

        private bool _isExportWished;
        public bool IsExportWished { get => _isExportWished; set => SetProperty(ref _isExportWished, value); }

        [Ignore]
        public object PrintValue { get; set; }

        private void SetIdentifier(string exRef, string id) => Identifier = $"{exRef}-{id}";
    }
}