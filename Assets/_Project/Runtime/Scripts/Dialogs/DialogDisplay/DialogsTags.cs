using UnityEngine;

namespace DShotgun.Dialogs
{
    public class DialogsTags : MonoBehaviour
    {

    }
    
    public struct CustomTag
    {
        private DSTags _tag;
            
        private string _tagIdEntry;
        private string _tagIdExit;

        public CustomTag(DSTags a_tag, string a_idEntry, string a_idExit) {
            _tag = a_tag;
                
            _tagIdEntry =  a_idEntry;
            _tagIdExit = a_idExit;
        }

        public bool IsEntryTag(string a_text, out DSTags a_tag) {
            a_tag = _tag;

            TrimTagSymbols(ref a_text);
            if (a_text.StartsWith(_tagIdEntry))
                return true;
            else
                return false;
        }
        public bool IsExitTag(string a_text, out DSTags a_tag) {
            a_tag = _tag;
            
            TrimTagSymbols(ref a_text);
            if (a_text.StartsWith(_tagIdEntry))
                return true;
            else
                return false;
        }

        public string GetValue(string a_text) {
            TrimTagSymbols(ref a_text);
            return a_text.Substring(_tagIdEntry.Length);
        }

        private void TrimTagSymbols(ref string a_text) {
            a_text = a_text.Trim().Trim('<', '>').Trim('/');
        }
    }

    public enum DSTags 
    {
        Test1,
        Test2
    }
}
