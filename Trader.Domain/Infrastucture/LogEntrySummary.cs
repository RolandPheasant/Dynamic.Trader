using System;

namespace Trader.Domain.Infrastucture;

public class LogEntrySummary : IEquatable<LogEntrySummary>
{
    public static readonly LogEntrySummary Empty = new LogEntrySummary();

    private LogEntrySummary()
    {
    }

    public LogEntrySummary(int debug, int info, int warning, int error)
    {
        Debug = debug;
        Info = info;
        Warning = warning;
        Error = error;
    }

    public int Debug { get; }
    public int Info { get; }
    public int Warning { get; }
    public int Error { get; }

    #region Equality members

    public bool Equals(LogEntrySummary other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Error == other.Error && Warning == other.Warning && Info == other.Info && Debug == other.Debug;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LogEntrySummary)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Error;
            hashCode = (hashCode * 397) ^ Warning;
            hashCode = (hashCode * 397) ^ Info;
            hashCode = (hashCode * 397) ^ Debug;
            return hashCode;
        }
    }

    public static bool operator ==(LogEntrySummary left, LogEntrySummary right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LogEntrySummary left, LogEntrySummary right)
    {
        return !Equals(left, right);
    }

    #endregion
}