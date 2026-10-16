using System;
using System.Threading;
using DShotgun.RichText;
using HierarchySequences;
using UnityEngine;

namespace DShotgun.Dialogs
{
    public class CommentFrame : ASequencable
    {
        [SerializeField] private string _name;
        [SerializeField] private RichTextData _text;
        [SerializeField] private SOCharacterDialogProfile _dialogProfile;
        
        
        protected override async Awaitable OnPlay() {
            TransferData();
        }
        
        private void TransferData() {
            string text = RichTextCompiler.Correct(_text.Text, RichTextTagRegistry.Default);
            
            CharacterDialogData dialogData = new CharacterDialogData()
            {
                DialogProfile = _dialogProfile,
                Name = _name,
                Text = text,
            };
            
            //TODO Remettre quand c'est bon TITOUAN
            DialogsManager.instance.CaptureCommentInfo(dialogData); 
        }
    }
}
