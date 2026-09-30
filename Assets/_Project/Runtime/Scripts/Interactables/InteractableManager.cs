using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableManager : MonoBehaviour
{
    [SerializeField] private Blackboard _blackboard;
    [SerializeField] private List<InteractionCase> _interactionCases = new List<InteractionCase>();
    
    private void Start()
    {
        if(_blackboard == null) _blackboard = FindObjectOfType<Blackboard>();   
    }

    public bool Interact()
    {
        foreach (var interactionCase in _interactionCases)
        {
            if(interactionCase.Evaluate(_blackboard))
            {
                return true;
            }
        }

        return false;
    }
    
}
