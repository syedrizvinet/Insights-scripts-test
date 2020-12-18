﻿using System.Threading.Tasks;
using Knapcode.ExplorePackages.Website.Models;
using Knapcode.ExplorePackages.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Knapcode.ExplorePackages.Website.Controllers
{
    [Authorize(Policy = AllowListAuthorizationHandler.PolicyName)]
    public class AdminController : Controller
    {
        private readonly CatalogScanStorageService _catalogScanStorageService;
        private readonly CatalogScanService _catalogScanService;
        private readonly IRawMessageEnqueuer _rawMessageEnqueuer;

        public AdminController(
            CatalogScanStorageService catalogScanStorageService,
            CatalogScanService catalogScanService,
            IRawMessageEnqueuer rawMessageEnqueuer)
        {
            _catalogScanStorageService = catalogScanStorageService;
            _catalogScanService = catalogScanService;
            _rawMessageEnqueuer = rawMessageEnqueuer;
        }

        public async Task<ViewResult> Index()
        {
            await _catalogScanService.InitializeAsync();
            var approximateMessageCountTask = _rawMessageEnqueuer.GetApproximateMessageCountAsync();
            var availableMessageCountLowerBoundTask = _rawMessageEnqueuer.GetAvailableMessageCountLowerBoundAsync();
            var cursorTask = _catalogScanService.GetCursorAsync(CatalogScanType.FindPackageAssets);
            var cursor = await cursorTask;
            var latestScansTask = _catalogScanStorageService.GetLatestIndexScans(cursor.Name, maxEntities: 10);
            await Task.WhenAll(approximateMessageCountTask, availableMessageCountLowerBoundTask, cursorTask, latestScansTask);

            var model = new AdminViewModel
            {
                ApproximateMessageCount = await approximateMessageCountTask,
                AvailableMessageCountLowerBound = await availableMessageCountLowerBoundTask,
                Cursor = cursor,
                LatestScans = await latestScansTask,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> UpdateFindPackageAssets()
        {
            await _catalogScanService.UpdateFindPackageAssetsAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}