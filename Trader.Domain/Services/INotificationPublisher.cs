using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Trader.Domain.Services;

public interface INotificationPublisher
{
    void Publish(Notification notification);
}

public interface INotificationStream
{
    IObservable<Notification> Observe();
}

public class NotificationService : INotificationStream, INotificationPublisher
{
    private readonly ISubject<Notification> _notification = new Subject<Notification>(); 

    public IObservable<Notification> Observe()
    {
        return _notification.AsObservable();
    }

    public void Publish(Notification notification)
    {
        _notification.OnNext(notification);
    }
}

public enum NotificatonType
{
    Information,
    Warning,
    Error
}

public class Notification
{
    private readonly NotificatonType _type;
    private readonly DateTime _timeStamp=DateTime.Now;
    private readonly string _category;
    private readonly string _message;

    public Notification(NotificatonType type, string category, string message)
    {
        _type = type;
        _category = category;
        _message = message;
    }

    public NotificatonType Type
    {
        get { return _type; }
    }

    public string Category
    {
        get { return _category; }
    }

    public string Message
    {
        get { return _message; }
    }

    public DateTime TimeStamp
    {
        get { return _timeStamp; }
    }

    #region Equality

    protected bool Equals(Notification other)
    {
        return _type == other._type && _timeStamp.Equals(other._timeStamp) && string.Equals(_category, other._category) && string.Equals(_message, other._message);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Notification) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (int) _type;
            hashCode = (hashCode*397) ^ _timeStamp.GetHashCode();
            hashCode = (hashCode*397) ^ (_category != null ? _category.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (_message != null ? _message.GetHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}