﻿using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Knapcode.ExplorePackages.Tool.Commands
{
    public interface ICommand
    {
        void Configure(CommandLineApplication app);
        Task ExecuteAsync(CancellationToken token);
        bool IsInitializationRequired();
        bool IsDatabaseRequired();
        bool IsReadOnly();
    }
}
