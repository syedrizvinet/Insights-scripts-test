﻿using System;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Entities;

namespace Knapcode.ExplorePackages.Logic
{
    public interface ILeaseService
    {
        Task<LeaseEntity> AcquireAsync(string name, TimeSpan leaseDuration);
        Task<LeaseEntity> GetOrNullAsync(string name);
        Task ReleaseAsync(LeaseEntity lease);
        Task RenewAsync(LeaseEntity lease, TimeSpan leaseDuration);
        Task<LeaseResult> TryAcquireAsync(string name, TimeSpan leaseDuration);
        Task<bool> TryReleaseAsync(LeaseEntity lease);
        Task<bool> TryRenewAsync(LeaseEntity lease, TimeSpan leaseDuration);
    }
}