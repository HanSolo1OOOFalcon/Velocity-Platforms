using System.Text;
using UnityEngine;
using static VelocityPlatforms.Plugin;

namespace BananaOS.Pages
{
    public class BANANAOS : WatchPage
    {
        public override string Title => "Velocity Platforms";
        public override bool DisplayOnMainMenu => ciEnabled;
        public override void OnPostModSetup()
        {
            selectionHandler.maxIndex = 0;
        }

        public override string OnGetScreenContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<color=yellow>==</color> Velocity Platforms <color=yellow>==</color>");
            stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Toggled\n")); // HELO
            stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Change right platform color."));
            stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(2, "Change left platform color."));
            return stringBuilder.ToString();
        }

        public override void OnButtonPressed(WatchButtonType buttonType)
        {
            switch (buttonType)
            {
                case WatchButtonType.Up:
                    selectionHandler.MoveSelectionUp();
                    break;

                case WatchButtonType.Down:
                    selectionHandler.MoveSelectionDown();
                    break;

                case WatchButtonType.Enter:
                    if (selectionHandler.currentIndex == 0)
                    {
                        enabledPlugin = !enabledPlugin;
                        BananaNotifications.DisplayNotification(enabledPlugin ? "<align=center><size=7> Enabled" : "<align=center><size=7> Disabled", Color.yellow, enabledPlugin ? Color.green : Color.red, 1f);
                        return;
                    }
                    if (selectionHandler.currentIndex == 1)
                    {
                        colorIndexR++;
                        return;
                    }
                    if (selectionHandler.currentIndex == 2)
                    {
                        colorIndexL++;
                        return;
                    }
                    break;

                case WatchButtonType.Back:
                    ReturnToMainMenu();
                    break;
            }
        }
    }
}
