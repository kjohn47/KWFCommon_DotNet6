﻿namespace KWFEventBus.Abstractions.Interfaces
{
    public interface IKwfEventConsumerHandler
    {
        void StartConsuming();
        void StopConsuming();
        bool IsStarted { get; }
    }
}
