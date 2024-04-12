using System.Collections.Generic;
using UdonSharp;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase;

namespace Yamadev.YamachanWebUnit.Editor
{
    public class ClientBuildProcess : IProcessSceneWithReport
    {
        public int callbackOrder => -1;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            GenerateUrls();
        }

        void GenerateUrls()
        {
            Client[] clients = Resources.FindObjectsOfTypeAll<Client>();
            if (clients.Length == 0) return;

            foreach (Client client in clients)
            {
                UdonSharpBehaviour udon = client.GetComponent<UdonSharpBehaviour>();
                int maxLength = (int)udon.GetProgramVariable("MaxLength");
                string api = (string)udon.GetProgramVariable("ApiBase");

                List<VRCUrl> urls = new List<VRCUrl>();
                for (int i = 0; i < maxLength * 256; i++)
                {
                    VRCUrl url = new VRCUrl($"rtsp://{api}/vrchat/set?i={i/256}&b={i%256}");
                    urls.Add(url);
                }
                udon.SetVariable("Urls", urls.ToArray());
            }
        }
    }
}