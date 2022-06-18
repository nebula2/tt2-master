using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    public class PrivacyPolicyViewModel : ViewModelBase
    {
        #region Properties
        private string _policyText;
        public string PolicyText
        {
            get => _policyText;
            set => SetProperty(ref _policyText, value);
        }
        #endregion

        #region Ctor
        public PrivacyPolicyViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyHeader;

            PolicyText = "Personal data (usually referred to just as \"data\" below) will only be processed by us to the extent necessary and for the purpose of providing a functional and user-friendly app, including its contents, and the services offered there.";
            PolicyText += "\n\nPer Art. 4 No. 1 of Regulation (EU) 2016/679, i.e. the General Data Protection Regulation (hereinafter referred to as the \"GDPR\"), \"processing\" refers to any operation or set of operations such as collection, recording, organization, structuring, storage, adaptation, alteration, retrieval, consultation, use, disclosure by transmission, dissemination, or otherwise making available, alignment, or combination, restriction, erasure, or destruction performed on personal data, whether by automated means or not.";
            PolicyText += "\n\nThe following privacy policy is intended to inform you in particular about the type, scope, purpose, duration, and legal basis for the processing of such data either under our own control or in conjunction with others. We also inform you below about the third-party components we use to optimize our website and improve the user experience which may result in said third parties also processing data they collect and control.";
            PolicyText += "\n\nOur privacy policy is structured as follows:";
            PolicyText += "\n\nI. Information about us as controllers of your data";
            PolicyText += "\n\nII. The rights of users and data subjects";
            PolicyText += "\n\nIII. Information about the data processing";
            PolicyText += "\n\nI. Information about us as controllers of your data";
            PolicyText += "\n\nThe party responsible for this app (the \"controller\") for purposes of data protection law is:";
            PolicyText += "\n\n????";
            PolicyText += "\n\n????";
            PolicyText += "\n\n???? ?????";
            PolicyText += "\n\n????";
            PolicyText += "\n\nEmail: ???@???.de";
            PolicyText += "\n\nII. The rights of users and data subjects";
            PolicyText += "\n\nWith regard to the data processing to be described in more detail below, users and data subjects have the right";
            PolicyText += "\n\n•	to confirmation of whether data concerning them is being processed, information about the data being processed, further information about the nature of the data processing, and copies of the data (cf. also Art. 15 GDPR);";
            PolicyText += "\n\n•	to correct or complete incorrect or incomplete data (cf. also Art. 16 GDPR);";
            PolicyText += "\n\n•	to the immediate deletion of data concerning them (cf. also Art. 17 DSGVO), or, alternatively, if further processing is necessary as stipulated in Art. 17 Para. 3 GDPR, to restrict said processing per Art. 18 GDPR;";
            PolicyText += "\n\n•	to receive copies of the data concerning them and/or provided by them and to have the same transmitted to other providers/controllers (cf. also Art. 20 GDPR);";
            PolicyText += "\n\n•	to file complaints with the supervisory authority if they believe that data concerning them is being processed by the controller in breach of data protection provisions (see also Art. 77 GDPR).";
            PolicyText += "\n\nIn addition, the controller is obliged to inform all recipients to whom it discloses data of any such corrections, deletions, or restrictions placed on processing the same per Art. 16, 17 Para. 1, 18 GDPR. However, this obligation does not apply if such notification is impossible or involves a disproportionate effort. Nevertheless, users have a right to information about these recipients.";
            PolicyText += "\n\nLikewise, under Art. 21 GDPR, users and data subjects have the right to object to the controller's future processing of their data pursuant to Art. 6 Para. 1 lit. f) GDPR. In particular, an objection to data processing for the purpose of direct advertising is permissible.";
            PolicyText += "\n\nIII. Information about the data processing";
            PolicyText += "\n\nYour data processed when using our app will be deleted or blocked as soon as the purpose for its storage ceases to apply, provided the deletion of the same is not in breach of any statutory storage obligations or unless otherwise stipulated below.";
            PolicyText += "\n\nContact";
            PolicyText += "\n\nIf you contact us via email or the contact form, the data you provide will be used for the purpose of processing your request. We must have this data in order to process and answer your inquiry; otherwise we will not be able to answer it in full or at all.";
            PolicyText += "\n\nThe legal basis for this data processing is Art. 6 Para. 1 lit. b) GDPR.";
            PolicyText += "\n\nYour data will be deleted once we have fully answered your inquiry and there is no further legal obligation to store your data, such as if an order or contract resulted therefrom.";
        }
        #endregion
    }
}

