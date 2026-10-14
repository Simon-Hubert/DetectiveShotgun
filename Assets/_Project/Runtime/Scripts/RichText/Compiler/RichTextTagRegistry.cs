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
            registry.Register(new BoldTag());
            registry.Register(new ItalicTag());
            registry.Register(new ColorTag());
            return registry;
        }

        public void Register(RichTextTag tag) {
            _tags[tag.Name] = tag;   // une balise du même nom est remplacée
        }

        public bool TryGet(string name, out RichTextTag tag) => _tags.TryGetValue(name, out tag);
    }
}
