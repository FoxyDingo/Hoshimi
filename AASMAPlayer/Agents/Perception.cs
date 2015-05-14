using System;
using PH.Common;
using PH.Map;
using System.Drawing;

using AASMAHoshimi;

namespace AASMAHoshimi
{
    public enum PerceptionType
    {
        FriendlyBot,
        EnemyBot,
        HoshimiPoint,
        AZNPoint,
        NavPoint,
        BloodStream,
        Cell,
        EmptyNeedle,
        FullNeedle,
        None
    }

    public abstract class Perception
    {
        protected double distance;
        protected Object obj;
        public Perception(Object obj, double distance)
        {
            this.obj = obj;
            this.distance = distance;
        }
        public Perception(Object obj)
        {
            this.obj = obj;
            this.distance = 0;
        }

        public double getDistance()
        {
            return distance;
        }

        public abstract PerceptionType getType();
        public bool isType(PerceptionType t)
        {
            return t == getType();
        }
    }

    public class FriendlyBotPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.FriendlyBot;
        public FriendlyBotPerception(NanoBot obj, double distance) : base(obj, distance) { }

        public NanoBot getBot()
        {
            return (NanoBot)obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }
    public class EnemyBotPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.EnemyBot;
        public EnemyBotPerception(Point obj, double distance) : base(obj, distance) { }
        public EnemyBotPerception(Point obj) : base(obj) { }

        public Point getPoint()
        {
            return (Point)obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class EmptyNeedlePerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.EmptyNeedle;
        public EmptyNeedlePerception(Point obj, double distance) : base(obj, distance) { }
        public EmptyNeedlePerception(Point obj) : base(obj) { }

        public Point getPoint()
        {
            return (Point) obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class FullNeedlePerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.FullNeedle;
        public FullNeedlePerception(Point obj, double distance) : base(obj, distance) { }
        public FullNeedlePerception(Point obj) : base(obj) { }

        public Point getPoint()
        {
            return (Point)obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class AZNPointPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.AZNPoint;
        public AZNPointPerception(Point obj, double distance) : base(obj, distance) { }
        public AZNPointPerception(Point obj) : base(obj) { }

        public Point getPoint()
        {
            return (Point) obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class HoshimiPointPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.HoshimiPoint;
        public HoshimiPointPerception(Point obj, double distance) : base(obj, distance) { }
        public HoshimiPointPerception(Point obj) : base(obj) { }

        public Point getPoint()
        {
            return (Point) obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class NavPointPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.NavPoint;
        public NavPointPerception(Point obj, double distance) : base(obj, distance) { }
        public NavPointPerception(Point obj) : base(obj) { }

        public Point getPoint()
        {
            return (Point)obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class BloodStreamPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.BloodStream;
        public BloodStreamPerception(BloodStream obj, double distance) : base(obj, distance) { }

        public BloodStream getBloodStream()
        {
            return (BloodStream)obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class CellPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.Cell;
        public CellPerception(Entity obj, double distance) : base(obj, distance) { }

        public Cell getCell()
        {
            return (Cell)obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }
}