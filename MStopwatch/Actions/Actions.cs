﻿using Redux;
using MStopwatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStopwatch.Actions
{
    public class TimeFormatAction : IAction
    {
        public string Format { get; set; }
    }

    public class ChangeModeAction : IAction
    {
    }

    public class TimerAction : IAction
    {
        public DateTime Now { get; set; }
    }

    public class LapAction : IAction
    {
    }
}
