using Prism.Mvvm;
using Prism.Navigation;
using System;

namespace TT2Master
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        [Obsolete]
        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        [Obsolete]
        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        [Obsolete]
        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public virtual void Destroy()
        {
            
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
        }
    }
}
