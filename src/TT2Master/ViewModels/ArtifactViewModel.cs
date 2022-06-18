using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TT2Master
{
    public class ArtifactViewModel : ViewModelBase
    {
        public List<Artifact> ArtifactList { get; set; }

        public ArtifactViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Artifacts";
            LoadArtifactData();
        }

        private void LoadArtifactData()
        {
            //process data from save
            ArtifactList = new List<Artifact>();

            foreach (Artifact item in ArtifactConstants.Artifacts)
            {
                ArtifactList.Add(item);
            }
        }


    }
}

