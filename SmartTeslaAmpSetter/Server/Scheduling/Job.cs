﻿using Quartz;
using SmartTeslaAmpSetter.Server.Contracts;

namespace SmartTeslaAmpSetter.Server.Scheduling;

[DisallowConcurrentExecution]
public class Job : IJob
{
    private readonly ILogger<Job> _logger;
    private readonly IChargingService _chargingService;

    public Job(ILogger<Job> logger, IChargingService chargingService)
    {
        _logger = logger;
        _chargingService = chargingService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace("Executing Job to set ChargerValues");
        await _chargingService.SetNewChargingValues().ConfigureAwait(false);
    }
}