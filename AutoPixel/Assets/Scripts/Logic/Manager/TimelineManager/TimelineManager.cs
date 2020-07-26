using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic.Map.LevelMap.MapItem.MapItem;
using Logic.Map.LevelMap.MapItemCommon.Component;
using TimeLineSupport;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Logic.Manager.TimelineMangager
{
    [Serializable]
    public class TimelineBinding
    {
        public string streamName;
        public int hashCode;

        public TimelineBinding(string inStreamName, int inHashCode)
        {
            streamName = inStreamName;
            hashCode = inHashCode;
        }
    };

    [Serializable]
    public class TimelineBindingsWrapper
    {
        public TimelineBinding[] bindings;

        public TimelineBindingsWrapper(TimelineBinding[] inBindings)
        {
            bindings = inBindings;
        }
    };

    [ManagerDefine(100, false)]
    public sealed class TimelineManager : Manager<TimelineManager>, IManager
    {
        private List<GameObject> mapItemsWithAnimator;

        public override void OnAwake()
        {
            mapItemsWithAnimator = new List<GameObject>();
        }

        // Load playable asset specified in $timelineName into $playableDirectorComp.playableAsset.
        public bool LoadTimeline(PlayableDirector playableDirectorComp, string timelineName)
        {
            if (playableDirectorComp.state == PlayState.Playing)
            {
                Debug.LogWarning("There is a timeline asset playing in PlayableDirectorComponent. Loading cancelled.");
                return false;
            }

            this.mapItemsWithAnimator.Clear();

            PlayableAsset playableAsset = Resources.Load<PlayableAsset>("Timelines/" + timelineName);
            if (playableAsset is null)
            {
                Debug.LogWarning("Specified timeline asset [" + timelineName + ".playable] not found.");
                return false;
            }

            playableDirectorComp.playableAsset = playableAsset;

            // Read from specified path.
            string path =  "Timelines/" + timelineName;
            string serializedJsonString = "";
            var json = Resources.Load<TextAsset>(path);
            if (json)
            {
                serializedJsonString = json.text;
                if (string.IsNullOrEmpty(serializedJsonString))
                {
                    return false;
                }
            }

            // Reset all timeline asset bindings.

            TimelineBindingsWrapper bindingsWrapper = JsonUtility.FromJson<TimelineBindingsWrapper>(serializedJsonString);

            // Construct streamName to hashCode dictionary.
            Dictionary<string, int> streamNameToHashCode = new Dictionary<string, int>();
            foreach (TimelineBinding binding in bindingsWrapper.bindings)
            {
                streamNameToHashCode.Add(binding.streamName, binding.hashCode);
            }

            foreach (PlayableBinding playableBinding in playableDirectorComp.playableAsset.outputs)
            {
                if (playableBinding.streamName != "Markers")
                {
                    int hashCode;
                    if (streamNameToHashCode.TryGetValue(playableBinding.streamName, out hashCode))
                    {
                        if (SceneManager.GetActiveScene().name == "GameScene")
                        {
                            MapItemComponent mapItemComponent = MapItemBase.GetItemComponent(hashCode);
                            if (mapItemComponent != null)
                            {
                                GameObject mapItem = mapItemComponent.gameObject;

                                this.mapItemsWithAnimator.Add(mapItem);

                                Animator mapItemAnimator = mapItem.AddComponent<Animator>();

                                playableDirectorComp.SetGenericBinding(playableBinding.sourceObject, mapItemAnimator);
                            }
                        }
                        else
                        {
                            MapItemComponent mapItemComp;
                            if (TimeLineSupportScript.m_mapItemComponents.TryGetValue(hashCode, out mapItemComp))
                            {
                                playableDirectorComp.SetGenericBinding(playableBinding.sourceObject, mapItemComp.gameObject.GetComponent<Animator>());
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool LoadAndPlayTimeline(PlayableDirector playableDirector, string timelineName)
        {
            if (this.LoadTimeline(playableDirector, timelineName))
            {
                playableDirector.stopped += OnTimelineStopped;

                playableDirector.Play();
                return true;
            }
            return false;
        }

        // Save playable asset specified in $playableDirectorComp.playableAsset as $timelineName.json
        public bool SaveTimeline(PlayableDirector playableDirectorComp, string timelineName)
        {
            string playableAssetName = playableDirectorComp.playableAsset.name;
            if (string.IsNullOrEmpty(playableAssetName))
            {
                Debug.LogWarning("No timeline asset attached to PlayableDirectorComponent.");
                return false;
            }

            // Get all timeline asset bindings.
            List<TimelineBinding> bindings = new List<TimelineBinding>();
            foreach (PlayableBinding playableBinding in playableDirectorComp.playableAsset.outputs)
            {
                if (playableBinding.streamName != "Markers")
                {
                    bindings.Add(new TimelineBinding(
                        playableBinding.streamName,
                        (playableDirectorComp.GetGenericBinding(playableBinding.sourceObject) as Animator).gameObject.GetComponent<MapItemComponent>().HostedItem.HashCode
                    ));
                }
            }

            // Write to specified path.
            var x = new TimelineBindingsWrapper(bindings.ToArray());
            string serializedJsonString = JsonUtility.ToJson(x);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(serializedJsonString);

            string path = Application.dataPath + "/Resources/Timelines/" + timelineName + ".json";
            FileStream stream = File.Open(path, FileMode.Create);
            stream.Write(jsonBytes, 0, jsonBytes.Length);
            stream.Dispose();
            stream.Close();
            
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            return true;
        }

        public IEnumerator PreInit()
        {
            yield return null;
        }

        private void OnTimelineStopped(PlayableDirector playableDirector)
        {
            playableDirector.stopped -= OnTimelineStopped;

            foreach (GameObject mapItem in mapItemsWithAnimator)
            {
                UnityEngine.Object.Destroy(mapItem.GetComponent<Animator>());
            }
        }
    }
}