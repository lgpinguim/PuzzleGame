using Godot;
using Newtonsoft.Json;

namespace Game.Autoload;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }

    public override void _Ready()
    {
        var saveData = new SaveData();
        saveData.SaveLevelCompletionStatus("test", true);
        var dataString = JsonConvert.SerializeObject(saveData);
        
        // var SavedData = JsonConvert.DeserializeObject<SaveData>(dataString);
        //
        // GD.Print(dataString);
    }
}