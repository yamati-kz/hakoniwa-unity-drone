using System;

public interface IDroneBatteryStatus
{
    public double get_full_voltage();
    public double get_curr_voltage();
    public uint get_status();
    public uint get_cycles();
    public double get_temperature();
}
