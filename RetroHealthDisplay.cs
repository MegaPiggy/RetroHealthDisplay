using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RetroHealthDisplay
{
	[HarmonyPatch]
	public class RetroHealthDisplay : ModBehaviour
	{
		public static RetroHealthDisplay Instance;

		public static GameObject Canvas => GameObject.Find("PlayerHUD/HelmetOnUI/UICanvas/GaugeGroup");
		public static GameObject HealthSilhouette => Canvas.transform.Find("HealthSilhouette").gameObject;
		public static GameObject Health => Canvas.transform.Find("Health").gameObject;
		public static Text HealthText => Health.GetComponentInChildren<Text>();
		public static GameObject Vitals => Canvas.transform.Find("VitalsText").gameObject;
		public static Text VitalsText => Vitals.GetComponent<Text>();

		public static int PercentageSize => HealthText.fontSize / 2;

		public static AssetBundle FontBundle;
		public static Font VCR_OSD_MONO;

		public void Awake()
		{
			Instance = this;
			new Harmony("MegaPiggy.RetroHealthDisplay").PatchAll(Assembly.GetExecutingAssembly());
		}

		public void Start()
		{
			FontBundle = ModHelper.Assets.LoadBundle("vcr_osd_mono.font");
			VCR_OSD_MONO = FontBundle.LoadAsset<Font>("Assets/Resources/fonts/english - latin/VCR_OSD_MONO.ttf");
		}

		[HarmonyPostfix, HarmonyPatch(typeof(HUDCanvas), nameof(HUDCanvas.Start))]
		public static void HUDCanvas_Start(HUDCanvas __instance)
		{
			HealthSilhouette.SetActive(false);
			Health.SetActive(true);
			HealthText.font = VCR_OSD_MONO;
			Vitals.SetActive(true);
		}

		[HarmonyPostfix, HarmonyPatch(typeof(HUDCanvas), nameof(HUDCanvas.OnPlayerDeath))]
		public static void HUDCanvas_OnPlayerDeath(HUDCanvas __instance)
		{
			UpdateText(__instance._playerResources, true);
		}

		[HarmonyPostfix, HarmonyPatch(typeof(HUDCanvas), nameof(HUDCanvas.UpdateHealth))]
		public static void HUDCanvas_UpdateHealth(HUDCanvas __instance)
		{
			UpdateText(__instance._playerResources);
		}

		public static void UpdateText(PlayerResources playerResources, bool died = false)
		{
			float healthFraction = died ? 0 : playerResources.GetHealthFraction();
			float health = died ? 0 : playerResources.GetHealth();
			HealthText.text = Mathf.Max(0f, health).ToString("F1") + $"<size={PercentageSize}>%</size>";
			float t = Mathf.Clamp01((healthFraction - 0.2f) * 1.25f);
			var color = Color.Lerp(new Color(1f, 0.2f, 0.1f, 1f), new Color(1f, 1f, 1f, 1f), t);
			HealthText.color = color;
			VitalsText.color = color;
		}
	}

}
