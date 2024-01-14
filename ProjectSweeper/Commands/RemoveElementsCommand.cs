using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSweeper.Commands
{
    public class RemoveElementsCommand : CommandBase
    {
        private readonly CleanerStore _cleanerStore;
        private readonly ModelTypes _modelType;

        public RemoveElementsCommand(CleanerStore cleanerStore, ModelTypes modelType)
        {
            _cleanerStore = cleanerStore;
            _modelType = modelType;
        }

        public override void Execute(object parameter)
        {
            try
            {
                _cleanerStore.DeleteElements(_modelType);
                OnCanExecuteChanged();
            } catch (Exception ex)
            {
                MessageBox.Show($"Cant delete elements, {ex}");
            }
        }
    }
}
