using System;
using BepInEx;
using Cinemachine.Utility;
using UnityEngine;
using Utilla;

namespace VelocityPlatforms
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
    {
		void OnEnable()
		{
            ciEnabled = true;

			HarmonyPatches.ApplyHarmonyPatches();
		}

		void OnDisable()
		{
            ciEnabled = false;

			HarmonyPatches.RemoveHarmonyPatches();
		}

		void OnGameInitialized(object sender, EventArgs e)
		{
			/* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
		}

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(Init);
        }

        private void Init()
        {
            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinedRoom;
            NetworkSystem.Instance.OnReturnedToSinglePlayer += OnLeftRoom;
        }

        private void OnJoinedRoom()
        {
            if (NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                EnsurePlatformsExist();
            }
            else
            {
                RemovePlatforms();
            }
        }
        private void OnLeftRoom()
        {
            RemovePlatforms();
        }

		// The methods/functions running the platform logic.
        private static GameObject lPlat = null;
        private static GameObject rPlat = null;
        private static GameObject CreatePlatform()
		{
			GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.transform.localScale = new Vector3(0.03f, 0.4f, 0.4f);
			plat.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            plat.GetComponent<Renderer>().material.color = Color.yellow;
			plat.SetActive(false);
			return plat;
        }

	private static void EnsurePlatformsExist()
	{
	    if (lPlat == null)
	    {
		lPlat = CreatePlatform();
            }

            if (rPlat == null)
	    {
                rPlat = CreatePlatform();
            }
        }

		private static void RemovePlatforms()
		{
            if (lPlat != null)
            {
                Destroy(lPlat);
                lPlat = null;
            }
            if (rPlat != null)
            {
                Destroy(rPlat);
                rPlat = null;
            }
        }

        // The array of colors.
        private static Color[] colors = new Color[]
        {
            Color.yellow,
            Color.red,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.white,
            Color.black
        };

        private static int maxColorIndex = 7;

        public static int colorIndexR = 0;
        public static int colorIndexL = 0;

        // Initialize the last positions.
        private static Vector3 lastPositionL = Vector3.zero;
		private static Vector3 lastPositionR = Vector3.zero;
        private static Vector3 lastPositionHead = Vector3.zero;

        // The bools.
        private static bool lHappen = false;
        private static bool rHappen = false;
        public static bool enabledPlugin = true;
        public static bool ciEnabled = true;

        // The main method.
        private static float threshold = 0.05f;
        void Update()
		{
            // Just the checks to make sure the room thingy majig is correct.
            if (!NetworkSystem.Instance.InRoom && !ciEnabled)
            {
                RemovePlatforms();
                return;
            }
            else if (!NetworkSystem.Instance.GameModeString.Contains("MODDED") && !ciEnabled)
            {
                RemovePlatforms();
                return;
            }

            EnsurePlatformsExist();

            // Change the color of the platforms.
            if (colorIndexL >= 8)
            {
                colorIndexL = 0;
            }
            lPlat.GetComponent<Renderer>().material.color = colors[colorIndexL];

            if (colorIndexR >= 8)
            {
                colorIndexR = 0;
            }
            rPlat.GetComponent<Renderer>().material.color = colors[colorIndexR];

            // Calculate movement deltas
            Vector3 headMovementDelta = GorillaTagger.Instance.headCollider.transform.position - lastPositionHead;
            Vector3 leftHandMovementDelta = GorillaTagger.Instance.leftHandTransform.position - lastPositionL;
            Vector3 rightHandMovementDelta = GorillaTagger.Instance.rightHandTransform.position - lastPositionR;

            // Check if hand movements are similar to head movement
            bool leftHandMovingWithHead = Vector3.Dot(headMovementDelta.normalized, leftHandMovementDelta.normalized) > 0.4f;
            bool rightHandMovingWithHead = Vector3.Dot(headMovementDelta.normalized, rightHandMovementDelta.normalized) > 0.4f;

            if (!leftHandMovingWithHead)
            {
                if (GorillaTagger.Instance.leftHandTransform.position.y + threshold <= lastPositionL.y && enabledPlugin && ciEnabled)
                {
                    if (!lHappen)
                    {
                        lHappen = true;

                        lPlat.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                        lPlat.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                        lPlat.SetActive(true);
                    }
                }
                else
                {
                    if (lHappen)
                    {
                        lHappen = false;
                        lPlat.SetActive(false);
                    }
                }
            }

            if (!rightHandMovingWithHead)
            {
                if (GorillaTagger.Instance.rightHandTransform.position.y + threshold <= lastPositionR.y && enabledPlugin && ciEnabled)
                {
                    if (!rHappen)
                    {
                        rHappen = true;

                        rPlat.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                        rPlat.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                        rPlat.SetActive(true);
                    }
                }
                else
                {
                    if (rHappen)
                    {
                        rHappen = false;
                        rPlat.SetActive(false);
                    }
                }
            }

            // Last positions of tracked objects.
            lastPositionL = GorillaTagger.Instance.leftHandTransform.position;
            lastPositionR = GorillaTagger.Instance.rightHandTransform.position;
            lastPositionHead = GorillaTagger.Instance.headCollider.transform.position;
        }
    }
}
