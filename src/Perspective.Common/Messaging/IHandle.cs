﻿namespace Perspective.Common.Messaging
{
    public interface IHandle<T> where T : Command
    {
        void Handle(T command);
    }
}