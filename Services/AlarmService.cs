using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Barca_Dyeing_Screen.Configuration;
using Barca_Dyeing_Screen.Models;
using Barca_Dyeing_Screen.Stores;

namespace Barca_Dyeing_Screen.Services;

public class AlarmService
{
    private readonly PlcService _plcService;
    private readonly AlarmStore _alarmStore;

    public AlarmService(
        PlcService plcService
        , AlarmStore alarmStore)
    {
        _plcService = plcService;
        _alarmStore = alarmStore;
    }

    public async Task UpdateAlarms(CancellationToken token)
    {
        var bits = await _plcService.ReadAlarmBits(token);
        for (int i = 0; i < bits.Length; i++)
        {
            if (i >= AlarmDefinitions.AlarmModels.Count)
                continue;

            var definition = AlarmDefinitions.AlarmModels[i];
            var existingAlarm = _alarmStore.ActiveAlarms.FirstOrDefault(a => a.AlarmId == i);

            if (bits[i])
            {
                if (existingAlarm == null)
                {
                    _alarmStore.ActiveAlarms.Add(
                        new AlarmModel
                        {
                            AlarmId = i,
                            AlarmName = definition.AlarmName,
                            AlarmDescription = definition.AlarmDescription
                        });
                }
            }else
            {
                if (existingAlarm != null)
                {
                    _alarmStore.ActiveAlarms.Remove(existingAlarm);
                }
            }
        }
    }
}