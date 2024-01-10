using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using ProjectSweeper.Models;
using ProjectSweeper.Services;
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
            //services.AddSingleton<ILineStyleProvider, LineStyleProvider>(s => new LineStyleProvider(doc));
            services.AddSingleton<ILineStyleProvider, LineStyleProvider>();
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
            services.AddSingleton<Cleaner>(s => new Cleaner(s.GetRequiredService<LineStyleList>()));
            services.AddTransient<LineStyleList>();

            //VIEW MODELS
            services.AddTransient<NavigationBarViewModel>(CreateNavigationBarViewModel);

            services.AddTransient<LineStyleManagerViewModel>(s => CreateLineStyleManagerViewModel(s));

            services.AddTransient<LinePatternViewModel>();


            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(
                CreateLineStyleNavigationService(serviceProvider),
                CreateLinePatternNavigationService(serviceProvider)
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
            return new LayoutNavigationService<LinePatternViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<LinePatternViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }

        private LineStyleManagerViewModel CreateLineStyleManagerViewModel(IServiceProvider services)
        {
            return LineStyleManagerViewModel.LoadViewModel(services.GetRequiredService<CleanerStore>());
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            ServiceBuilder(commandData);

            INavigationService initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            return Result.Succeeded;
        }

    }
}
