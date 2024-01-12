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
            services.AddSingleton<INavigationService>(s => CreateLineStyleNavigationService(s));

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
            services.AddTransient<LineStyleManagerViewModel>(s => CreateLineStyleManagerViewModel(s));
            services.AddTransient<LinePatternManagerViewModel>(s => CreateLinePatternManagerViewModel(s));
            services.AddTransient<FilledRegionManagerViewModel>(s => CreateFilledRegionManagerViewModel(s));
            services.AddTransient<FillPatternManagerViewModel>(s => CreateFillPatternManagerViewModel(s));

            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private Cleaner CreateCleaner(IServiceProvider serviceProvider)
        {
            return new Cleaner(serviceProvider.GetRequiredService<LineStyleModelList>(), serviceProvider.GetRequiredService<LinePatternModelList>(), serviceProvider.GetRequiredService<FilledRegionModelList>(), serviceProvider.GetRequiredService<FillPatternModelList>());
        }

        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(
                CreateLineStyleNavigationService(serviceProvider),
                CreateLinePatternNavigationService(serviceProvider),
                CreateFilledRegionNavigationService(serviceProvider),
                CreateFillPatternNavigationService(serviceProvider)
                );    
        }

        private INavigationService CreateLineStyleNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<LineStyleManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<LineStyleManagerViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private INavigationService CreateLinePatternNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<LinePatternManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<LinePatternManagerViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private INavigationService CreateFillPatternNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<FillPatternManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<FillPatternManagerViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private INavigationService CreateFilledRegionNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<FilledRegionManagerViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<FilledRegionManagerViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }


        private LineStyleManagerViewModel CreateLineStyleManagerViewModel(IServiceProvider serviceProvider)
        {
            return LineStyleManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>());
        }

        private LinePatternManagerViewModel CreateLinePatternManagerViewModel(IServiceProvider serviceProvider)
        {
            return LinePatternManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>());
        }

        private FillPatternManagerViewModel CreateFillPatternManagerViewModel(IServiceProvider serviceProvider)
        {
            return FillPatternManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>());
        }

        private FilledRegionManagerViewModel CreateFilledRegionManagerViewModel(IServiceProvider serviceProvider)
        {
            return FilledRegionManagerViewModel.LoadViewModel(serviceProvider.GetRequiredService<CleanerStore>());
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
