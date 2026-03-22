using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using Barca_Dyeing_Screen.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using Modbus.Device;
using Modbus.Serial;

namespace Barca_Dyeing_Screen.Services;

public class ConnectionService(ConnectionStore connectionStore) : ObservableObject
{
    private readonly ConnectionStore _connectionStore = connectionStore;

    private SerialPort? _serialPort; // COM4
    private IModbusSerialMaster? _master;

    private const int TimeOut = 2000;

    public void ConnectModbusRtu(
        string comPort,
        string baudRate,
        string parity,
        string dataBits,
        string stopBits)
    {
        try
        {
            var baudRateInt = int.Parse(baudRate);
            var dataBitsInt = int.Parse(dataBits);


            Parity selectedParity = parity switch
            {
                "None" => Parity.None,
                "Even" => Parity.Even,
                "Odd" => Parity.Odd,
                _ => throw new ArgumentOutOfRangeException(nameof(parity), parity, null)
            };
            StopBits selectedStopBits = stopBits switch
            {
                "1" => StopBits.One,
                "2" => StopBits.Two,
                _ => throw new ArgumentOutOfRangeException(nameof(stopBits), stopBits, null)
            };

            _serialPort = new SerialPort(comPort)
            {
                Parity = selectedParity,
                BaudRate = baudRateInt,
                DataBits = dataBitsInt,
                StopBits = selectedStopBits,
                WriteTimeout = TimeOut,
                ReadTimeout = TimeOut,
            };

            // Open Port

            // Create RTU
            var adapter = new SerialPortAdapter(_serialPort);
            _master = ModbusSerialMaster.CreateRtu(adapter);


            _connectionStore.IsConnected = _serialPort.IsOpen ? true : false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // Disconnect Device
    public void DisconnectModbusRtu()
    {
        _serialPort?.Close();
        _serialPort = null;
    }

    public void GetSerialPorts(List<string> ports)
    {
        var localPorts = SerialPort.GetPortNames();
        foreach (var port in localPorts)
        {
            if (!ports.Contains(port))
                ports.Add(port);
        }
    }
}