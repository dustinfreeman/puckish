using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct VoiceDefn {
  public string archetype;
  public AudioClip clip;
}

public enum BarkType {
  turn_start,
  hits,
  hit_by,
  gaffe,
  laughing
};

public struct BarksDefn {
  public VoiceDefn voice;
  public Dictionary<BarkType, float[]> barkOptionTimes;
}

public class Voices : Singleton<Voices> {
  [SerializeField]
  VoiceDefn[] VoiceArchetypes;

  Dictionary<string, Dictionary<BarkType, float[]>> barkTimings = new Dictionary<string, Dictionary<BarkType, float[]>>()
  {
    { "butler",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] {0.6f, 14.35f, 18.2f} },
        { BarkType.hits, new float[] {30.6f, 33.3f, 46.7f } },
        { BarkType.hit_by, new float[] {67.9f, 99.2f} },
        { BarkType.gaffe, new float[] {113.1f, 126.9f} },
        { BarkType.laughing, new float[] {164.6f, 159.7f } },
      }
    }
  };

  public BarksDefn GetBarks(string archetype) {
    var possibleVoices = from v in VoiceArchetypes where v.archetype == archetype select v;
    if (possibleVoices.Count() == 0) {
      Debug.LogErrorFormat("Missing Voice Info for {0}", archetype);
    }
    var voice = VoiceArchetypes.First();

    return new BarksDefn()
    {
      voice = voice,
      barkOptionTimes = barkTimings[voice.archetype],
    };
  }
}

