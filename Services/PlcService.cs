using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Barca_Dyeing_Screen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Modbus.Device;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;

namespace Barca_Dyeing_Screen.Services;

public partial class PlcService : ObservableObject
{
    private readonly ConnectionService _connectionService;
    private IModbusSerialMaster? Master => _connectionService.Master;


    public PlcService(ConnectionService connectionService)
    {
        _connectionService = connectionService;
    }


    public async Task<bool[]> ReadParameters(CancellationToken token)
    {
        if (Master == null)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                "Serial Port Error",
                "Modbus RTU device is null");
            await messageBox.ShowAsync();
            return Array.Empty<bool>();
        }

        try
        {
            var result = await Master.ReadCoilsAsync(
                (byte)SlaveId.SlaveOne,
                (ushort)PlcAddress.Status,
                (ushort)PointsToRead.Two);

            return result;
        }
        catch (TimeoutException)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentTitle = "Read TimeOut",
                    ContentMessage = "Check the physical connection or PLC status",
                    Width = 300,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
            await messageBox.ShowAsync();
            return Array.Empty<bool>();
        }
        catch (OperationCanceledException)
        {
            return Array.Empty<bool>();
        }
        catch (Exception e)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentTitle = "Read Error",
                    ContentMessage = e.Message,
                    Width = 300,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
            return Array.Empty<bool>();
        }
    }

    public async Task<bool[]> ReadTempAndMotorParameters(CancellationToken token)
    {
        try
        {
            var result = await Master!.ReadCoilsAsync(
                (byte)SlaveId.SlaveOne,
                (ushort)PlcAddress.MemoryOne,
                (ushort)PointsToRead.One);

            return result;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }
    }

    // Method for Read Alarm Bits
    public async Task<bool[]> ReadAlarmBits(CancellationToken token)
    {
        try
        {
            var result = await Master!.ReadCoilsAsync(
                (byte)SlaveId.SlaveOne,
                (ushort)PlcAddress.MemoryOne,
                (ushort)PointsToRead.One);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}