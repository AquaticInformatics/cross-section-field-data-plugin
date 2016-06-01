using System.Collections.Generic;
using System.Linq;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PanelParser
    {
        public static IReadOnlyDictionary<string, PanelItem> Parse(PocketGaugerFiles pocketGaugerFiles)
        {
            var panels = pocketGaugerFiles.ParseType<Panels>();
            var verticals = pocketGaugerFiles.ParseType<Verticals>();

            AssignVerticals(panels, verticals);

            return panels.PanelItems.ToDictionary(panel => panel.PanelId, panel => panel);
        }

        private static void AssignVerticals(Panels panels, Verticals verticals)
        {
            foreach (var panel in panels.PanelItems)
            {
                panel.Verticals =
                    verticals.VerticalItems.Where(vertical => vertical.PanelId == panel.PanelId).ToList();
            }
        }
    }
}
