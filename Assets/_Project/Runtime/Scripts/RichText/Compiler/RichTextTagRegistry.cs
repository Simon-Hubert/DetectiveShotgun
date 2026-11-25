using System;
using System.Collections.Generic;

namespace DShotgun.RichText
{
    public class RichTextTagRegistry
    {
        private readonly Dictionary<string, RichTextTag> _tags = new(StringComparer.OrdinalIgnoreCase);

        public static RichTextTagRegistry Default { get; } = CreateDefault();

        public IEnumerable<RichTextTag> Tags => _tags.Values;

        // Ajouter ici les balises du projet : elles seront connues de l'éditeur comme du jeu.
        public static RichTextTagRegistry CreateDefault() {
            RichTextTagRegistry registry = new();

            // --- TextMeshPro
            TmpTag bold = new("b", preview: TmpPreview.Raw);
            TmpTag italic = new("i", preview: TmpPreview.Raw);
            TmpTag color = new("color", TmpValueKind.Color, TmpPreview.Color);
            TmpTag size = new("size", TmpValueKind.Percentage, TmpPreview.Size);
            registry.Register(bold);
            registry.Register(italic);
            registry.Register(color);
            registry.Register(size);
            registry.Register(new TmpTag("u"));
            registry.Register(new TmpTag("s"));
            registry.Register(new TmpTag("strikethrough"));
            registry.Register(new TmpTag("sub"));
            registry.Register(new TmpTag("sup"));
            registry.Register(new TmpTag("mark", TmpValueKind.Color));
            registry.Register(new TmpTag("cspace", TmpValueKind.Length));
            registry.Register(new TmpTag("font", TmpValueKind.Text));
            registry.Register(new TmpTag("font-weight", TmpValueKind.Text));
            registry.Register(new TmpTag("allcaps", preview: TmpPreview.Uppercase));
            registry.Register(new TmpTag("uppercase", preview: TmpPreview.Uppercase));
            registry.Register(new TmpTag("smallcaps", preview: TmpPreview.Uppercase));
            registry.Register(new TmpTag("lowercase", preview: TmpPreview.Lowercase));
            registry.Register(new TmpTag("br", preview: TmpPreview.LineBreak, closing: TagClosing.None));
            
            registry.Register(new DialogueTag("pass", TagClosing.None, DialogueValueKind.None, previewOpen: DialogueTag.Marker("passe au suivant"), endsDialogue: true));
            registry.Register(new DialogueTag("wait", TagClosing.None, DialogueValueKind.PositiveNumber, previewOpen: DialogueTag.Marker("pause {0}s")));
            registry.Register(new DialogueTag("speed", TagClosing.Optional, DialogueValueKind.PositiveNumber));
            registry.Register(new DialogueTag("pitch", TagClosing.Optional, DialogueValueKind.PositiveNumber, runtimeSupported: false));
            registry.Register(new DialogueTag("sound", TagClosing.Optional, DialogueValueKind.Text, runtimeSupported: false));
            registry.Register(new DialogueTag("readmod", TagClosing.Optional, DialogueValueKind.Choice, runtimeSupported: false, choices: new[] { "char", "word" }));
            registry.Register(new DialogueTag("nrpunct", TagClosing.Optional, DialogueValueKind.None, runtimeSupported: false));
            registry.Register(new DialogueTag("charemo", TagClosing.None, DialogueValueKind.Text, previewOpen: DialogueTag.Marker("émotion {0}"), runtimeSupported: false));
            registry.Register(new DialogueTag("charanim", TagClosing.None, DialogueValueKind.Text, previewOpen: DialogueTag.Marker("animation {0}"), runtimeSupported: false));
            registry.Register(new DialogueTag("burst", TagClosing.Optional, DialogueValueKind.None, runtimeSupported: false));
            registry.Register(new DialogueTag("gold", TagClosing.Optional, DialogueValueKind.None, "<color=#FFD700>", "</color>", runtimeSupported: false));
            registry.Register(new DialogueTag("bounce", TagClosing.Optional, DialogueValueKind.None, runtimeSupported: false));
            registry.Register(new DialogueTag("fear", TagClosing.Optional, DialogueValueKind.None, runtimeSupported: false));
            registry.Register(new DialogueTag("shake", TagClosing.None, DialogueValueKind.None, previewOpen: DialogueTag.Marker("tremblement d'écran"), runtimeSupported: false));

            // --- Raccourcis : écrits avec des balises TMP, donc déjà fonctionnels
            registry.Register(new ExpandedTag("big", (size, "200%")));
            registry.Register(new ExpandedTag("huge", (size, "300%")));
            registry.Register(new ExpandedTag("whisper", (size, "80%"), (color, "grey"), (italic, null)));

            return registry;
        }

        public void Register(RichTextTag tag) {
            _tags[tag.Name] = tag;
        }

        public bool TryGet(string name, out RichTextTag tag) => _tags.TryGetValue(name, out tag);
    }
}
