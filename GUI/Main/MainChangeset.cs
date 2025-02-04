using System;
using System.Linq;
using System.Collections.Generic;

// Don't warn if we use our own obsolete properties
#pragma warning disable 0618

namespace CKAN.GUI
{
    public partial class Main
    {
        private void UpdateChangesDialog(List<ModChange> changeset, Dictionary<CkanModule, string>? conflicts)
        {
            if (CurrentInstance != null)
            {
                Changeset.LoadChangeset(
                    changeset,
                    ModuleLabelList.ModuleLabels.LabelsFor(CurrentInstance.Name)
                        .Where(l => l.AlertOnInstall)
                        .ToList(),
                    conflicts);
            }
        }

        private void Changeset_OnSelectedItemsChanged(CkanModule item)
        {
            if (MainTabControl.SelectedTab == ChangesetTabPage)
            {
                ShowSelectionModInfo(item);
            }
        }

        private void Changeset_OnCancelChanges(bool reset)
        {
            if (reset)
            {
                ManageMods.ClearChangeSet();
            }
            tabController.ShowTab(ManageModsTabPage.Name);
        }

        private void Changeset_OnConfirmChanges(List<ModChange> changeset)
        {
            if (CurrentInstance != null)
            {
                DisableMainWindow();
                try
                {
                    Wait.StartWaiting(InstallMods, PostInstallMods, true,
                        new InstallArgument(
                            // Only pass along user requested mods, so auto-installed can be determined
                            changeset.Where(ch => ch.Reasons.Any(r => r is SelectionReason.UserRequested)
                                                  // Include all removes and upgrades
                                                  || ch.ChangeType != GUIModChangeType.Install)
                                     .ToList(),
                            RelationshipResolverOptions.DependsOnlyOpts(CurrentInstance.StabilityToleranceConfig)));
                }
                catch (InvalidOperationException)
                {
                    // Thrown if it's already busy, can happen if the user double-clicks the button. Ignore it.
                    // More thread-safe than checking installWorker.IsBusy beforehand.
                }
            }
        }
    }
}
