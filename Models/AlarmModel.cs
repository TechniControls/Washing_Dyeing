namespace Barca_Dyeing_Screen.Models;

public class AlarmModel
{
    public string AlarmName { get; set; }
    public string AlarmDescription { get; set; }
    
    public AlarmModel(string alarmName, string alarmDescription)
    {
        AlarmName = alarmName;
        AlarmDescription = alarmDescription;
    }
}