﻿using System;
using TeslaSolarCharger.Shared.Dtos.Settings;
using Xunit;
using Xunit.Abstractions;

namespace TeslaSolarCharger.Tests.Services.Server;

public class TeslaMateApiService : TestBase
{
    public TeslaMateApiService(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
    }

    [Theory]
    [InlineData(18, null, null, false)]
    [InlineData(18, null, 19, true)]
    [InlineData(18, null, 17, false)]
    [InlineData(18, 17, null, true)]
    [InlineData(27, null, 3, false)]
    [InlineData(27, 4, 4, false)]
    public void CanDecideIfScheduledChargingIsNeeded(int currentDateHour, int? carSetHour, int? carHourToSet, bool expectedResult)
    {
        var teslamateApiService = Mock.Create<TeslaSolarCharger.Server.Services.TeslamateApiService>();

        var day = 13;
        if (currentDateHour > 24)
        {
            day++;
            currentDateHour -= 24;
        }

        var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
        var currentDate = new DateTimeOffset(2022, 2, day, currentDateHour, 0, 0, utcOffset);
        
        DateTimeOffset? setChargeStart = carSetHour == null ? null :
            new DateTimeOffset(2022, 2, 14, (int)carSetHour, 0, 0, utcOffset);
        var hourDifference = 1;
        DateTimeOffset? chargeStartToSet = carHourToSet == null ? null :
            //Minutes set to check if is rounding up to next 15 minutes
            new DateTimeOffset(2022, 2, 13, (int)carHourToSet - hourDifference, 51, 0, utcOffset);

        var car = new Car()
        {
            CarState = new CarState()
            {
                ScheduledChargingStartTime = setChargeStart,
            },
        };

        var isChangeNeeded = teslamateApiService.IsChargingScheduleChangeNeeded(chargeStartToSet, currentDate, car, out var parameters);

        Assert.Equal(expectedResult, isChangeNeeded);

        if (!isChangeNeeded)
        {
            Assert.Empty(parameters);
        }
        else
        {
            Assert.Equal(2, parameters.Count);
            if (carHourToSet == null)
            {
                Assert.Equal("false", parameters["enable"]);
                Assert.Equal("0", parameters["time"]);
            }
            else
            {
                Assert.Equal("true", parameters["enable"]);
                var localhour = chargeStartToSet!.Value.ToLocalTime().TimeOfDay.Hours + hourDifference;
                Assert.Equal((localhour * 60).ToString(), parameters["time"]);
            }
            
        }
    }

    [Theory]
    [InlineData(14, 15, 14, 15)]
    [InlineData(14, 16, 14, 30)]
    [InlineData(14, 0, 14, 0)]
    [InlineData(14, 27, 14, 30)]
    public void CanRoundToNextQuarterHour(int hour, int minute, int resultHour, int resultMinute)
    {
        var inputDateTimeOffset = new DateTimeOffset(2023, 3, 19, hour, minute, 0, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow));
        var teslamateApiService = Mock.Create<TeslaSolarCharger.Server.Services.TeslamateApiService>();
        var outputTime = teslamateApiService.RoundToNextQuarterHour(inputDateTimeOffset);

        Assert.Equal(resultHour, outputTime.Hour);
        Assert.Equal(resultMinute, outputTime.Minute);
    }
}
