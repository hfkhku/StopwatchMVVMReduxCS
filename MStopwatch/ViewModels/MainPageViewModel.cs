using MStopwatch.Actions;
using MStopwatch.Models;
using MStopwatch.States;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace MStopwatch.ViewModels
{
    class MainPageViewModel:BindableBase, INavigationAware
    {
        private string _startButtonLabel;
        public string StartButtonLabel
        {
            get => _startButtonLabel;
            set => SetProperty(ref _startButtonLabel, value);
        }

        private string nowSpan;
        public string NowSpan
        {
            get => nowSpan;
            set => SetProperty(ref nowSpan, value);
        }

        private bool _isShowed;
        public bool IsShowed
        {
            get => _isShowed;
            set
            {
                if (!SetProperty(ref _isShowed, value)) return;
                StateStore.Store.Dispatch(value
                    ? new TimeFormatAction() {Format = Constants.TimeSpanFormat}
                    : new TimeFormatAction() {Format = Constants.TimeSpanFormatNoMillsecond});
            }
        }

        private DelegateCommand lapCommand;
        public DelegateCommand LapCommand =>
            lapCommand ?? (lapCommand = new DelegateCommand(Lap, CanLap));

        private static bool CanLap()
        {
            return StateStore.Store.GetState().GetState<StopwatchMode>(ApplicationStateKey.Mode) == StopwatchMode.Start;
        }

        private static void Lap()
        {
            StateStore.Store.Dispatch(new LapAction());
        }

        public ObservableCollection<LapTime> Items { get; } = new ObservableCollection<LapTime>();

        private IDisposable TimerSubscription { get; set; }

        private DelegateCommand startCommand;
        public DelegateCommand StartCommand =>
            startCommand ?? (startCommand = new DelegateCommand(Start));

        private void Start()
        {
            var mode = StateStore.Store.GetState().GetState<StopwatchMode>(ApplicationStateKey.Mode);
            var scheduler = StateStore.Store.GetState().TimerScheduler;
            switch (mode)
            {
                case StopwatchMode.Init:
                    TimerSubscription = Observable.Interval(TimeSpan.FromMilliseconds(10), scheduler)
                        .Subscribe(_ =>
                        {
                            StateStore.Store.Dispatch(new TimerAction() { Now = scheduler.Now.DateTime.ToLocalTime() });
                        });
                    break;
                case StopwatchMode.Start:
                    TimerSubscription.Dispose();
                    TimerSubscription = null;
                    break;
                case StopwatchMode.Stop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StateStore.Store.Dispatch(new ChangeModeAction());
        }

        IRegionManager RegionManager;

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        public MainPageViewModel(IRegionManager regionManager)
        {
            this.RegionManager = regionManager;

            ConfirmationRequest = new InteractionRequest<IConfirmation>();

            StateStore.Store.ObserveOnDispatcher().Subscribe(state =>
            {
                StartButtonLabel = state.GetState<string>(ApplicationStateKey.ButtonLabel);
                NowSpan = state.GetState<TimeSpan>(ApplicationStateKey.NowSpan).ToString(state.GetState<string>(ApplicationStateKey.DisplayFormat));
                LapCommand.RaiseCanExecuteChanged();
                Items.Clear();
                Items.AddRange(state.GetState<ObservableCollection<LapTime>>(ApplicationStateKey.LapTimeList));
                if (state.GetState<StopwatchMode>(ApplicationStateKey.Mode) != StopwatchMode.Stop) return;
                var nowSpan = state.GetState<TimeSpan>(ApplicationStateKey.NowSpan);
                var maxLapTime = state.GetState<TimeSpan>(ApplicationStateKey.MaxLapTime);
                var minLapTime = state.GetState<TimeSpan>(ApplicationStateKey.MinLapTime);

                ConfirmationRequest.Raise(new Confirmation
                {
                    Title = "Confirmation",
                    Content = $"All time: {nowSpan.ToString(state.GetState<string>(ApplicationStateKey.DisplayFormat))}\r\nMax laptime: {maxLapTime.TotalMilliseconds} ms\nMin laptime: { minLapTime.TotalMilliseconds}ms\n\nShow all lap result?"
                }, r => {
                    if (r.Confirmed)
                    {
                        RegionManager.RequestNavigate("ContentRegion", "ResultPageView");
                    }
                });
            });
            
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

        void RaiseConfirmation(string content)
        {
            
        }
    }
}
