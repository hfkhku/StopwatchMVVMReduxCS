using MStopwatch.Models;
using MStopwatch.States;
using Redux;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using static MStopwatch.States.ApplicationStateKey;

namespace MStopwatch
{
    static class StateStore
    {
        public static IStore<ApplicationState> Store { get; }

        private static readonly IScheduler Scheduler = System.Reactive.Concurrency.Scheduler.Default;

        static StateStore()
        {
            var states = ImmutableDictionary.CreateRange(
                new Dictionary<string, object>()
                {
                    { DisplayFormat, Constants.TimeSpanFormatNoMillsecond },
                    { NowSpan, TimeSpan.Zero },
                    { Mode, StopwatchMode.Init },
                    { ButtonLabel, Constants.StartLabel },
                    { StartTime, new DateTime() },
                    { Now, new DateTime() },
                    { LapTimeList, new ObservableCollection<LapTime>() },
                    { MaxLapTime, TimeSpan.Zero },
                    { MinLapTime, TimeSpan.Zero },
                });
            var initialState = new ApplicationState(states, Scheduler);

            Store = new Store<ApplicationState>(Reducers.ReduceApplication, initialState);
        }
    }
}
