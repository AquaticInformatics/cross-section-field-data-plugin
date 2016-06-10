using System.Collections.Generic;
using System.Linq;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Parsers
{
    public class PanelParser : IPanelParser
    {
        public IReadOnlyCollection<PanelItem> Parse(PocketGaugerFiles pocketGaugerFiles)
        {
            var panels = pocketGaugerFiles.ParseType<Panels>();
            var verticals = pocketGaugerFiles.ParseType<Verticals>();

            panels.PanelItems = panels.PanelItems.OrderBy(item => item.PanelId).ToList();
            AssignVerticals(panels, verticals);

            return panels.PanelItems;
        }

        private static void AssignVerticals(Panels panels, Verticals verticals)
        {
            foreach (var panel in panels.PanelItems)
            {
                panel.Verticals = verticals.VerticalItems
                    .Where(vertical => vertical.PanelId == panel.PanelId)
                    .OrderBy(item => item.VerticalNo)
                    .ToList();
            }
        }
    }
}
