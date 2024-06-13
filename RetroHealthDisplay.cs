using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroHealthDisplay
{
	[HarmonyPatch]
	public class RetroHealthDisplay : ModBehaviour
	{
		public static RetroHealthDisplay Instance;

		public static GameObject Canvas => GameObject.Find("PlayerHUD/HelmetOnUI/UICanvas/GaugeGroup");
		public static GameObject HealthSilhouette => Canvas.transform.Find("HealthSilhouette").gameObject;
		public static GameObject Health => Canvas.transform.Find("Health").gameObject;
		public static GameObject VitalsText => Canvas.transform.Find("VitalsText").gameObject;

		public void Awake()
		{
			Instance = this;
			new Harmony("MegaPiggy.RetroHealthDisplay").PatchAll(Assembly.GetExecutingAssembly());
		}

		[HarmonyPostfix, HarmonyPatch(typeof(HUDCanvas), nameof(HUDCanvas.Start))]
		public static void HUDCanvas_Start(HUDCanvas __instance)
		{
			HealthSilhouette.SetActive(false);
			Health.SetActive(true);
			VitalsText.SetActive(true);
		}
	}

}
