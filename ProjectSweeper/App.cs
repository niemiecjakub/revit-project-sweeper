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
using ProjectSweeper.Services.ElementRemover;
using ProjectSweeper.Services.FilledRegionProvider;
using ProjectSweeper.Services.FillPatternProvider;
using ProjectSweeper.Services.LinePatternProvider;
using ProjectSweeper.Services.LineStyleProvider;
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
            services.AddSingleton<ILinePatternProvider, LinePatternProvider>(s => new LinePatternProvider(doc));
            services.AddSingleton<ILineStyleProvider, LineStyleProvider>(s => new LineStyleProvider(doc));
            services.AddSingleton<IFilledRegionProvider, FilledRegionProvider>(s => new FilledRegionProvider(doc));
            services.AddSingleton<IFillPatternProvider, FillPatternProvider>(s => new FillPatternProvider(doc));
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
            services.AddTransient<LineStyleModelList>();
            services.AddTransient<LinePatternModelList>();
            services.AddTransient<FilledRegionModelList>();
            services.AddTransient<FillPatternModelList>();

            //VIEW MODELS
            services.AddTransient<NavigationBarViewModel>(CreateNavigationBarViewModel);
            services.AddTransient<MasterManagerViewModel>(s => CreateLineStyleManagerViewModel(s));

            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private Cleaner CreateCleaner(IServiceProvider serviceProvider)
        {
            return new Cleaner(
                serviceProvider.GetRequiredService<LineStyleModelList>(),
                serviceProvider.GetRequiredService<LinePatternModelList>(),
                serviceProvider.GetRequiredService<FilledRegionModelList>(),
                serviceProvider.GetRequiredService<FillPatternModelList>()
                );
        }

        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(CreateLineStyleNavigation(serviceProvider), CreateLinePatternNavigation(serviceProvider), CreateFilledRegionNavigation(serviceProvider), CreateFillPatternNavigation(serviceProvider));
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
