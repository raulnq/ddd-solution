﻿namespace Application
{
    public interface ICommandSender
    {
        Task Send(ICommand command, TimeSpan deferTimeSpan = default);
    }
}