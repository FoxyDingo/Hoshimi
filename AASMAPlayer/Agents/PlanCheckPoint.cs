﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AASMAHoshimi
{
    class PlanCheckPoint
    {
        public enum Actions
        {
            Move,
            MoveRandom,
            Collect,
            Unload,
            Attack,
            Run,
            Defend,
            BuildNeedle,
            BuildContainer,
            BuildProtector,
            BuildExplorer
        };
        public Point location;
        public Actions action;
        public Desires intention;

        public PlanCheckPoint(Point p, Actions a)
        {
            location = p;
            action = a;
            intention = Desires.None;
        }

        public PlanCheckPoint(Point p, Actions a, Desires d)
        {
            location = p;
            action = a;
            intention = d;
        }
    }
}
