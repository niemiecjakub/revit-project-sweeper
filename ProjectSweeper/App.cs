using System;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using ProjectSweeper.Services;
using ProjectSweeper.Services.ElementProvider;
using ProjectSweeper.Services.ElementRemover;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using ProjectSweeper.Views;

namespace ProjectSweeper
{
    [Transaction(TransactionMode.Manual)]
    public class CleanerComamnd : IExternalCommand
    {
        private IServiceProvider _serviceProvider;

        public void ServiceBuilder(ExternalCommandData commandData)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            IServiceCollection services = new ServiceCollection();
            //SERVICES
            //PROVIDERS
            services.AddSingleton<IElementProvider, ElementProvider>(s => new ElementProvider(doc));
            services.AddTransient<IElementRemover, ElementRemover>(s => new ElementRemover(doc));
            //STORES
            services.AddSingleton<CleanerStore>();
            services.AddSingleton<NavigationStore>();

            //NAVIGATION SERVICE
            services.AddSingleton<INavigationService>(s => CreateLineStyleNavigation(s));

            //WINDOWS
            services.AddSingleton<MainWindow>(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            //MODELS
            services.AddSingleton<Cleaner>(s => CreateCleaner(s));
            services.AddTransient<ElementModelList>();

            //VIEW MODELS
            services.AddTransient<NavigationBarViewModel>(CreateNavigationBarViewModel);
            services.AddTransient<MasterManagerViewModel>(s => CreateLineStyleManagerViewModel(s));

            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private Cleaner CreateCleaner(IServiceProvider serviceProvider)
        {
            return new Cleaner(serviceProvider.GetRequiredService<ElementModelList>());
        }

        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(
                CreateLineStyleNavigation(serviceProvider),
                CreateLinePatternNavigation(serviceProvider),
                CreateFilledRegionNavigation(serviceProvider),
                CreateFillPatternNavigation(serviceProvider),
                CreateFilterNavigation(serviceProvider),
                CreateViewTemplateNavigation(serviceProvider),
                CreateViewportNavigation(serviceProvider),
                CreateTextNavigation(serviceProvider),
                CreateObjectStyleNavigation(serviceProvider)
                );
        }

        private INavigationService CreateLineStyleNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateLineStyleManagerViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }
        private INavigationService CreateLinePatternNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateLinePatterManagerViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }
        private INavigationService CreateFilledRegionNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateFilledRegionManagerViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }
        private INavigationService CreateFillPatternNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateFillPatternViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private INavigationService CreateFilterNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateFilterViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }
        private INavigationService CreateViewTemplateNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateViewTemplateViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }
        private INavigationService CreateViewportNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateViewportViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private INavigationService CreateTextNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateTextViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private INavigationService CreateObjectStyleNavigation(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateObjectStyleManagerViewModel(serviceProvider),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private MasterManagerViewModel CreateLineStyleManagerViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.LineStyle);
        }
        private MasterManagerViewModel CreateLinePatterManagerViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.LinePattern);
        }
        private MasterManagerViewModel CreateFilledRegionManagerViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.FilledRegion);
        }
        private MasterManagerViewModel CreateFillPatternViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.FillPattern);
        }
        private MasterManagerViewModel CreateFilterViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.Filter);
        }
        private MasterManagerViewModel CreateViewTemplateViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.ViewTemplate);
        }
        private MasterManagerViewModel CreateViewportViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.Viewport);
        }
        private MasterManagerViewModel CreateTextViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.Text);
        }
        private MasterManagerViewModel CreateObjectStyleManagerViewModel(IServiceProvider serviceProvider)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), ModelTypes.ObjectStyle);
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ServiceBuilder(commandData);

            INavigationService initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.ShowDialog();

            return Result.Succeeded;
        }

    }
}
