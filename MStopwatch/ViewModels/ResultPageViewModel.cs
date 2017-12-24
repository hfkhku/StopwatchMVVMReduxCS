using MStopwatch.Models;
using Prism.Commands;
using Prism.Regions;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System;
using MStopwatch.States;

namespace MStopwatch.ViewModels
{
    class ResultPageViewModel : INavigationAware
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ObservableCollection<LapTime> LapTimes { get; } = new ObservableCollection<LapTime>();

        public DelegateCommand BackCommand { get; }

        IRegionManager RegionManager;

        public ResultPageViewModel(/*Stopwatch model,*/ IRegionManager regionManager)
        {
            this.RegionManager = regionManager;
            StateStore.Store.ObserveOnDispatcher().Subscribe(state =>
            {
                LapTimes.Clear();
                LapTimes.AddRange(state.GetState<ObservableCollection<LapTime>>(ApplicationStateKey.LapTimeList));
            });

                //this.LapTimes = model.Items
                //    .ToReadOnlyReactiveCollection(x => new LapTimeViewModel(x));

                BackCommand = new DelegateCommand(Back);
        }

        private void Back()
        {
            RegionManager.RequestNavigate("ContentRegion", "MainPageView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
