﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Simulation
{
    public interface IInsideWorldSimulatior
    {
        void DoSimulation();
        long GetDeltaTimeUsec();
    }
}
