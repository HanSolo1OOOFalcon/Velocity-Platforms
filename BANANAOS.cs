using System.Text;
using UnityEngine;
using static VelocityPlatforms.Plugin;

namespace BananaOS.Pages
{
    public class ExamplePage : WatchPage
    {
        public override string Title => "Velocity Platforms";
        public override bool DisplayOnMainMenu => true;
        public override void OnPostModSetup()
        {
            selectionHandler.maxIndex = 0;
        }

        public override string OnGetScreenContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<color=yellow>==</color> Velocity Platforms <color=yellow>==</color>");
            stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Toggled"));
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
                        BananaNotifications.DisplayNotification(enabledPlugin ? "<align=center><size=5> Enabled" : "<align=center><size=5> Disabled", Color.yellow, enabledPlugin ? Color.green : Color.red, 1f);
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
