using UnityEngine;
using System.Collections;

public class SinglePlayerSceneDataRelayer : SceneDataRelayer{

    public int PlayerCount;
    public string PlayerName;

    public static void InitSceneData(int players, string name) {
        GameObject obj = new GameObject("Relayer");
        var relay = obj.AddComponent<SinglePlayerSceneDataRelayer>();
        relay.PlayerCount = players;
        relay.PlayerName = name;
        DontDestroyOnLoad(obj);
    }
}
