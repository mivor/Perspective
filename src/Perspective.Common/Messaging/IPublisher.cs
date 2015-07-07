namespace Perspective.Common.Messaging
{
    public interface IPublisher
    {
        void Publish<T>(T command) where T : Command;
    }
}