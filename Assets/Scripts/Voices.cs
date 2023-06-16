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
    { "template",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] {} },
        { BarkType.hits, new float[] {} },
        { BarkType.hit_by, new float[] {} },
        { BarkType.gaffe, new float[] {} },
        { BarkType.laughing, new float[] {} },
      }
    },
    { "butler",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] {0.6f, 14.35f, 18.2f} },
        { BarkType.hits, new float[] {30.6f, 33.3f, 46.7f } },
        { BarkType.hit_by, new float[] {67.9f, 99.2f} },
        { BarkType.gaffe, new float[] {113.1f, 126.9f} },
        { BarkType.laughing, new float[] {164.6f, 159.7f } },
      }
    },
    { "stranger",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] { 34.4f, 40.4f} },
        { BarkType.hits, new float[] { 47.9f, 52.3f } },
        { BarkType.hit_by, new float[] { 63.5f, 66.4f, 117.9f} },
        { BarkType.gaffe, new float[] {142.0f, 176.1f, 181.9f} },
        { BarkType.laughing, new float[] {204.1f} },
      }
    },
    { "refined_man",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] { 6.7f, 9.4f} },
        { BarkType.hits, new float[] { 19.4f, 37.0f, 41.4f, } },
        { BarkType.hit_by, new float[] { 45.5f, 47.3f, 51.1f, 58.7f} },
        { BarkType.gaffe, new float[] { /*75.4f, */ 79.3f, 82.9f, 91.5f, 99.8f} },
        { BarkType.laughing, new float[] { 139.8f, 143.8f , 148.3f} },
      }
    },
    { "youth",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] { 29.6f } },
        { BarkType.hits, new float[] { 45.5f, 50.4f, 65.0f, 66.9f, 71.8f } },
        { BarkType.hit_by, new float[] { 87.1f, 104.2f, 111.6f, 114.1f, 116.6f } },
        { BarkType.gaffe, new float[] {144.3f, 149.6f, 155.5f} },
        { BarkType.laughing, new float[] { 164.3f} },
      }
    },
    { "refined_woman",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] {8.3f, 13.0f } },
        { BarkType.hits, new float[] { 20.5f, 26.1f,} },
        { BarkType.hit_by, new float[] { 40.5f, 47.6f, 65.9f} },
        { BarkType.gaffe, new float[] { 73.8f, 77.2f} },
        { BarkType.laughing, new float[] { 85.7f, 91.0f} },
      }
    },
    { "gruff_man",
      new Dictionary<BarkType, float[]>()
      {
        { BarkType.turn_start, new float[] { 15.6f, 17.3f } },
        { BarkType.hits, new float[] { 28.4f, 30.2f, 32.5f, 40.0f} },
        { BarkType.hit_by, new float[] { 51.3f, 53.4f, 55.5f } },
        { BarkType.gaffe, new float[] { 72.9f, 76.6f, 86.7f} },
        { BarkType.laughing, new float[] { 100.7f, 105.1f} },
      }
    },
  };

  public BarksDefn GetBarks(string archetype) {
    var possibleVoices = from v in VoiceArchetypes where v.archetype == archetype select v;
    if (possibleVoices.Count() == 0) {
      Debug.LogErrorFormat("Missing Voice Info for {0}", archetype);
    }
    var voice = possibleVoices.First();

    return new BarksDefn()
    {
      voice = voice,
      barkOptionTimes = barkTimings[voice.archetype],
    };
  }
}

