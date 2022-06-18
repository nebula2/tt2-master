﻿using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Resources;
using TT2Master.Views.Equip;
using TT2Master.Views.Raid;

namespace TT2Master.Model.Dashboard
{
    [ShortcutAttribute(19, "CraftingAdvisor", true)]
    public class CraftingAdvisorShortcut : DashboardShortcut
    {
        public override int ID { get; set; } = 19;

        public override string Header { get; set; } = AppResources.CraftingAdvisor;

        private bool _hasContent = false;
        public override bool HasContent { get => _hasContent; set => SetProperty(ref _hasContent, value); }

        public override ICommand ItemTappedAction { get; protected set; }
        public override Func<Task> LoadItem { get; set; }

        public CraftingAdvisorShortcut(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Destination = typeof(CraftingAdvisorPage).Name;
            Icon = "";

            ItemTappedAction = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<DashboardPage, CraftingAdvisorPage>());

                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });
        }
    }
}
