﻿using Autodesk.Revit.DB;
using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class LineStyleManagerViewModel : ViewModelBase
    {
        public string Name => "Line style";
        private readonly ObservableCollection<LineStyleViewModel> _lineStyles;
        private readonly CleanerStore _cleanerStore;

        public IEnumerable<LineStyleViewModel> LineStyles => _lineStyles.OrderBy(x => x.Name);

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand LoadLineStylesCommand { get; }
        public ICommand RemoveElementCommand { get; }
        public LineStyleManagerViewModel(CleanerStore cleanerStore)
        {
            Debug.WriteLine("new line style");

            _lineStyles = new ObservableCollection<LineStyleViewModel>();
            _cleanerStore = cleanerStore;
            LoadLineStylesCommand = new LoadLineStylesCommand(this, cleanerStore);

            IEnumerable<IElement> elements = (IEnumerable<IElement>)_lineStyles.Where(ls => !ls.IsUsed).ToList();
            RemoveElementCommand = new RemoveElementsCommand(cleanerStore, elements);


            _cleanerStore.LineStyleDeleted += OnLineStyleDeleted;
        }
        public override void Dispose()
        {
            _cleanerStore.LineStyleDeleted -= OnLineStyleDeleted;
            base.Dispose();
        }

        private void OnLineStyleDeleted(LineStyle style)
        {
            LineStyleViewModel lineStyleViewModel = _lineStyles.First(ls => ls.Id == style.Id);
            _lineStyles.Remove(lineStyleViewModel);
        }

        public static LineStyleManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            LineStyleManagerViewModel viewModel = new LineStyleManagerViewModel(cleanerStore);
            viewModel.LoadLineStylesCommand.Execute(null);
            return viewModel;
        }

        public void UpdateLineStyles(IEnumerable<LineStyle> lineStyles)
        {
            _lineStyles.Clear();
            ISet<LineStyle> unusedLineStyles = lineStyles.Where(l => !l.IsUsed).ToHashSet();
            Debug.WriteLine($"{lineStyles.Count()} -  LineStyles found - {unusedLineStyles.Count} Unused");

            foreach (LineStyle lineStyle in lineStyles)
            {
                LineStyleViewModel lineStyleViewModel = new LineStyleViewModel(lineStyle);
                _lineStyles.Add(lineStyleViewModel);
            }
        }
    }
}
