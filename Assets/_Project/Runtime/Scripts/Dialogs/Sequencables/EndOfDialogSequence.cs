using HierarchySequences;
using UnityEngine;

namespace DShotgun.Dialogs
{
    public class EndOfDialogSequence : ASequencable
    {
        protected override async Awaitable OnPlay()
        {
            DialogsManager.instance.ShowDialogUI(false);
        }
    }
}
