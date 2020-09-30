using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parse;

public class HelloWorld : MonoBehaviour {
    async void Start() {
        ParseClient.Initialize("hello", "http://localhost:1337/parse");
        ParseLogger.LogDelegate += Log;

        try {
            ParseObject gameScore = new ParseObject("GameScore");
            gameScore["score"] = 1337;
            gameScore["playerName"] = "Sean Plott";
            gameScore["cheatMode"] = false;
            await gameScore.Save();
            Debug.Log(gameScore.ObjectId);
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    private static void Log(ParseLogLevel level, string info) {
        switch (level) {
            case ParseLogLevel.Debug:
                Debug.Log($"[DEBUG] {info}");
                break;
            case ParseLogLevel.Warn:
                Debug.LogWarning($"[WARNING] {info}");
                break;
            case ParseLogLevel.Error:
                Debug.LogError($"[ERROR] {info}");
                break;
            default:
                Debug.Log(info);
                break;
        }
    }
}
