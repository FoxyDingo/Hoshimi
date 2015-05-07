using System;
using PH.Common;
using PH.Map;
using System.Drawing;

using AASMAHoshimi;

namespace AASMAHoshimi.Examples
{
    public enum PerceptionType
    {
        FriendlyBot,
        EnemyBot,
        HoshimiPoint,
        AZNPoint,
        BloodStream,
        Cell,
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
        public EnemyBotPerception(NanoBotInfo obj, double distance) : base(obj, distance) { }

        public NanoBotInfo getBot()
        {
            return (NanoBotInfo)obj;
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

        public Point getAZNPoint()
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

        public Point getHoshimiPoint()
        {
            return (Point) obj;
        }
        public override PerceptionType getType()
        {
            return TYPE;
        }
    }

    public class BloodStreamPerception : Perception
    {
        public const PerceptionType TYPE = PerceptionType.AZNPoint;
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