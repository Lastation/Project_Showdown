
using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yamadev.YamaStream.UI;

namespace Yamadev.YamaStream.Script
{
    public class YamaPlayerBuildProcess : IProcessSceneWithReport
    {
        public int callbackOrder => -1;
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            YamaPlayer[] yamaPlayers = Utils.FindComponentsInHierarthy<YamaPlayer>();
            foreach (YamaPlayer player in yamaPlayers)
            {
                LatencyManager latencyManager = player.GetComponentInChildren<LatencyManager>();
                if (latencyManager != null)
                {
                    Transform latencyRecordTemplate = latencyManager.transform.GetChild(0);
                    for (int i = 0; i < 500; i++)
                    {
                        GameObject newRecord = GameObject.Instantiate(latencyRecordTemplate.gameObject, latencyManager.transform, false);
                        GameObjectUtility.EnsureUniqueNameForSibling(newRecord);
                        newRecord.SetActive(true);
                    }
                }
                Transform internalTransform = player.InternalTransform != null ? player.InternalTransform : player.transform.Find("Internal");
                if (internalTransform != null)
                {
                    internalTransform.SetParent(null, true);
                    GameObjectUtility.EnsureUniqueNameForSibling(internalTransform.gameObject);
                }
            }

            UIController[] uiControllers = Utils.FindComponentsInHierarthy<UIController>();
            foreach (UIController uiController in uiControllers)
            {
                UIColor[] colorItems = uiController.GetComponentsInChildren<UIColor>(true);
                for (int i = 0; i < colorItems.Length; i++) colorItems[i].SetProgramVariable("_uiController", uiController);
                UIToggle[] toggleItems = uiController.GetComponentsInChildren<UIToggle>(true);
                for (int i = 0; i < toggleItems.Length; i++) toggleItems[i].SetProgramVariable("_uiController", uiController);
            }
        }
    }
}