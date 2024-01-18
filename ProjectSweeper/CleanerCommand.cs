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
            UIDocument uidoc = uiapp.ActiveUIDocument;

            IServiceCollection services = new ServiceCollection();
            //SERVICES
            //PROVIDERS
            services.AddSingleton<IElementProvider, ElementProvider>(s => new ElementProvider(doc));
            services.AddTransient<IElementRemover, ElementRemover>(s => new ElementRemover(doc));
            //STORES
            services.AddSingleton<CleanerStore>(s => new CleanerStore(s.GetRequiredService<Cleaner>(), doc));
            services.AddSingleton<NavigationStore>();

            //NAVIGATION SERVICE
            services.AddSingleton<INavigationService>(s => CreateManagerNavigation(s, ModelTypes.LineStyle));

            //WINDOWS
            services.AddSingleton<MainWindow>(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            //MODELS
            services.AddSingleton<Cleaner>(s => new Cleaner(s.GetRequiredService<ElementModelList>()));
            services.AddTransient<ElementModelList>();

            //VIEW MODELS
            services.AddTransient<NavigationBarViewModel>(CreateNavigationBarViewModel);
            services.AddTransient<MasterManagerViewModel>(s => CreateManagerViewModel(s, ModelTypes.LineStyle));
            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(
                CreateManagerNavigation(serviceProvider, ModelTypes.LineStyle),
                CreateManagerNavigation(serviceProvider, ModelTypes.LinePattern),
                CreateManagerNavigation(serviceProvider, ModelTypes.FilledRegion),
                CreateManagerNavigation(serviceProvider, ModelTypes.FillPattern),
                CreateManagerNavigation(serviceProvider, ModelTypes.Filter),
                CreateManagerNavigation(serviceProvider, ModelTypes.ViewTemplate),
                CreateManagerNavigation(serviceProvider, ModelTypes.Viewport),
                CreateManagerNavigation(serviceProvider, ModelTypes.Text),
                CreateManagerNavigation(serviceProvider, ModelTypes.ObjectStyle),
                CreateManagerNavigation(serviceProvider, ModelTypes.Material),
                CreateManagerNavigation(serviceProvider, ModelTypes.MaterialAppearanceAsset)
                );
        }

        private INavigationService CreateManagerNavigation(IServiceProvider serviceProvider, ModelTypes modelType)
        {
            return new LayoutNavigationService<MasterManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => CreateManagerViewModel(serviceProvider, modelType),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private MasterManagerViewModel CreateManagerViewModel(IServiceProvider serviceProvider, ModelTypes modelType)
        {
            return MasterManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>(), modelType);
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
