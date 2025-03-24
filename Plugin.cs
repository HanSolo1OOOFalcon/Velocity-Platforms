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
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();
		}

		void OnDisable()
		{
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

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

		private static Vector3 lastPositionL = Vector3.zero;
		private static Vector3 lastPositionR = Vector3.zero;
        private static Vector3 lastHeadPosition = Vector3.zero;

        private static bool lHappen = false;
        private static bool rHappen = false;
        public static bool enabled = true;

        private static float threshold = 0.05f;
        void Update()
		{
            if (!NetworkSystem.Instance.InRoom)
            {
                RemovePlatforms();
                return;
            }
            else if (!NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                RemovePlatforms();
                return;
            }

            EnsurePlatformsExist();

            threshold += GorillaLocomotion.GTPlayer.Instance.headCollider.transform.position.y - lastHeadPosition.y;

            if (GorillaTagger.Instance.leftHandTransform.position.y + threshold <= lastPositionL.y && enabled)
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

            if (GorillaTagger.Instance.rightHandTransform.position.y + threshold <= lastPositionR.y && enabled)
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

            lastPositionL = GorillaTagger.Instance.leftHandTransform.position;
            lastPositionR = GorillaTagger.Instance.rightHandTransform.position;
            lastHeadPosition = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.position;
            threshold = 0.05f;
        }
    }
}
