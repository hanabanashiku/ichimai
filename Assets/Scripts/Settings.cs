using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Settings : object {
    static string path = (System.Environment.OSVersion.Platform == System.PlatformID.Unix) ?
        System.Environment.GetEnvironmentVariable("HOME") + "/.config/Ichimai/Settings.bin" :
        System.Environment.ExpandEnvironmentVariables("%APPDATA%") + @"\Ichimai\Settings.bin";

    public string LastPlayerName;
    public int LastNumberOfPlayers;

    private Settings() {
        LastPlayerName = "Player";
        LastNumberOfPlayers = 4;
    }

    public static Settings Load() {

        if (File.Exists(path)) {
            Debug.Log("Exists!!");
            Settings s;
            IFormatter f = new BinaryFormatter();
            Stream io = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try { s = (Settings)f.Deserialize(io); }
            // the class has likely been changed. settings will be reset
            catch (SerializationException) { return new Settings(); }
            io.Close();
            return s;
        }
        //nothing serialized, create a new instance, calling the constructor
        else return new Settings();
    }

    // Serialize object to filesystem
    ~Settings() {
        IFormatter f = new BinaryFormatter();
        if (!Directory.Exists(path.Substring(0, path.Length - 12))) Directory.CreateDirectory(path.Substring(0, path.Length - 12));
        FileStream io = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        f.Serialize(io, this);
        io.Close();
    }
}
